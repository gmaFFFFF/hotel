using FluentAssertions.Execution;
using LanguageExt.Common;
using Validot;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.starterKit.Messaging;
using gmafffff.starterKit.tests.BusinessLogic.Fixtures;

namespace gmafffff.starterKit.tests.BusinessLogic;

[TestSubject(typeof(BusinessActionRunner<>))]
public partial class BusinessActionRunnerTests {
    public class BusinessActionRunnerWithDomainEvent {
        private const string ErrorMessage = "Обработка прервана";
        private readonly Dictionary<CommandWithTrigger, IList<BusinessEvent>> _cmd2businessEvents = [];

        private readonly List<CommandWithTrigger> _commands;

        private readonly Dictionary<TriggerEvent, CommandWithTrigger> _trigger2cmd = [];

        private readonly IBusinessCommandHandler<CommandWithTrigger> _cmdHandler =
            Substitute.For<IBusinessCommandHandler<CommandWithTrigger>>();

        private readonly IServiceProvider _provider = Substitute.For<IServiceProvider>();

        private readonly BusinessActionRunner<CommandWithTrigger> _runner;

        private readonly ITriggerEventToCommandTranslator<MyTrigger> _triggerTranslator =
            Substitute.For<ITriggerEventToCommandTranslator<MyTrigger>>();
        
        private readonly DomainEventProcessor _domainEventProcessor;
        private readonly Dictionary<CommandWithTrigger, TestDomainEvent> _cmd2domainEvents = []; 
        private readonly IDomainEventHandler<TestDomainEvent> _domainEventHandler = 
            Substitute.For<IDomainEventHandler<TestDomainEvent>>();

        public BusinessActionRunnerWithDomainEvent() {
            _runner = new BusinessActionRunner<CommandWithTrigger>(_provider);

            // Обработчик событий домена
            _domainEventProcessor = new (_provider);
            _domainEventHandler.HandleAsync(Arg.Any<IDomainEvent>(),
                                            Arg.Any<DomainEventDispatcherContext>(), 
                                            Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(Fin<Unit>.Succ(Unit.Default));

            // Контейнер служб
            _provider.GetService(typeof(IBusinessActionRunner<CommandWithTrigger>))
                .Returns(_ => new BusinessActionRunner<CommandWithTrigger>(_provider));
            _provider.GetService(typeof(IValidator<CommandWithTrigger>))
                .Returns(null);
            _provider.GetService(typeof(IEnumerable<IBusinessConstraintCheck<CommandWithTrigger>>))
                .Returns(Array.Empty<IBusinessConstraintCheck<CommandWithTrigger>>());
            _provider.GetService(typeof(IBusinessCommandHandler<CommandWithTrigger>))
                .Returns(_cmdHandler);
            _provider.GetService(typeof(IEnumerable<ITriggerEventToCommandTranslator<MyTrigger>>))
                .Returns(Enumerable.Repeat(_triggerTranslator, count: 1));
            _provider.GetService(typeof(IDomainEventDispatcher))
                .Returns(_domainEventProcessor);
            _provider.GetService(typeof(IEnumerable<IDomainEventHandler<TestDomainEvent>>))
                .Returns(Enumerable.Repeat(_domainEventHandler, count: 1));

            // Образцы данных
            _commands = Enumerable.Range(start: 0, count: 2)
                .Select(i => new CommandWithTrigger(Number: i)).ToList();

            _cmd2domainEvents = _commands
                .Select(cmd => (cmd, new TestDomainEvent(cmd.Number)))
                .ToDictionary();

            IList<BusinessEvent> step0 = [
                new EndEventForTrigger(Number: 1, _commands[0]),
                new MyTrigger(Number: 100, _commands[0])
            ];
            IList<BusinessEvent> step1 = [
                new EndEventForTrigger(Number: 5, _commands[1])
            ];

            // Транслятор команд
            _trigger2cmd[(MyTrigger)step0[1]] = _commands[1];

            _triggerTranslator.Translate(Arg.Any<TriggerEvent>())
                .ReturnsForAnyArgs(x => [_trigger2cmd[x.Arg<TriggerEvent>()]]);

            // Обработчик команды
            _cmd2businessEvents[_commands[0]] = step0;
            _cmd2businessEvents[_commands[1]] = step1;


            _cmdHandler.ExecuteAsync(default!)
                .ReturnsForAnyArgs(x => {
                    _domainEventProcessor.AddEvent(_cmd2domainEvents[x.Arg<CommandWithTrigger>()]);
                    return Fin<IList<BusinessEvent>>.Succ(_cmd2businessEvents[x.Arg<CommandWithTrigger>()]);
                });

        }

        /// <summary>
        ///     Обрабатывает события домена <see cref="DomainEvent{TEntity}" />
        /// </summary>
        [Fact]
        public async Task ProcessDomainEvent() {
            // Arrange
            // Act
            var result = await _runner.Execute(_commands[0]);

            // Assert
            using var _ = new AssertionScope();
            result.IsSucc.Should().BeTrue();
            result.SuccSpan().ToArray().SelectMany(x => x).Should()
                .BeEquivalentTo(_cmd2businessEvents.Values.SelectMany(x => x).OfType<EndEventForTrigger>().ToArray());
            Received.InOrder(async () => {
                await _cmdHandler.ExecuteAsync(_commands[0], Arg.Any<CancellationToken>());
                await _domainEventHandler.HandleAsync((IDomainEvent)_cmd2domainEvents[_commands[0]], 
                                                        Arg.Any<DomainEventDispatcherContext>(), 
                                                        Arg.Any<CancellationToken>());
                await _cmdHandler.ExecuteAsync(_commands[1], Arg.Any<CancellationToken>());
                await _domainEventHandler.HandleAsync((IDomainEvent)_cmd2domainEvents[_commands[1]], 
                                                        Arg.Any<DomainEventDispatcherContext>(), 
                                                        Arg.Any<CancellationToken>());
            });
        }

        /// <summary>
        ///     Обработка событий домена может завершиться ошибкой
        /// </summary>
        [Fact]
        public async Task ProcessDomainEventCanEndWithError() {
            // Arrange
            _domainEventHandler.HandleAsync(Arg.Any<IDomainEvent>(),
                                            Arg.Any<DomainEventDispatcherContext>(), 
                                            Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(Fin<Unit>.Fail(ErrorMessage));

            // Act
            var result = await _runner.Execute(_commands[0]);

            // Assert
            using var _ = new AssertionScope();
            result.IsFail.Should().BeTrue();
            result.FailSpan()[0].Message.Should().Be(ErrorMessage);
            Received.InOrder(async () => {
                await _cmdHandler.ExecuteAsync(_commands[0], Arg.Any<CancellationToken>());
                await _domainEventHandler.HandleAsync((IDomainEvent)_cmd2domainEvents[_commands[0]], 
                                                        Arg.Any<DomainEventDispatcherContext>(), 
                                                        Arg.Any<CancellationToken>());
            });

            await _cmdHandler.DidNotReceive().ExecuteAsync(_commands[1], Arg.Any<CancellationToken>());
            await _domainEventHandler.DidNotReceive().HandleAsync((IDomainEvent)_cmd2domainEvents[_commands[1]], 
                                                    Arg.Any<DomainEventDispatcherContext>(), 
                                                    Arg.Any<CancellationToken>());
        }
    }
}