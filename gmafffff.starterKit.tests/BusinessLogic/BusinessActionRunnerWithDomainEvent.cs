using FluentAssertions.Execution;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.starterKit.tests.BusinessLogic.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Validot;

namespace gmafffff.starterKit.tests.BusinessLogic;

[TestSubject(typeof(BusinessActionRunner<>))]
public partial class BusinessActionRunnerTests {
    public class BusinessActionRunnerWithDomainEvent {
        private const string ErrorMessage = "Обработка прервана";
        private readonly Dictionary<CommandWithTrigger, TestDomainEvent> _cmd2domainEvents;

        private readonly DbHandler _cmdHandler;

        private readonly List<CommandWithTrigger> _commands;

        private readonly IDomainEventHandler<TestDomainEvent> _domainEventHandler =
            Substitute.For<IDomainEventHandler<TestDomainEvent>>();

        private readonly IServiceProvider _provider = Substitute.For<IServiceProvider>();

        private readonly ITriggerEventToCommandTranslator<MyTrigger> _triggerTranslator =
            Substitute.For<ITriggerEventToCommandTranslator<MyTrigger>>();

        private BusinessActionRunner<CommandWithTrigger> _runner;

        public BusinessActionRunnerWithDomainEvent() {
            // Образцы данных
            _commands = Enumerable.Range(start: 0, count: 2)
                .Select(i => new CommandWithTrigger(Number: i)).ToList();

            _cmd2domainEvents = _commands
                .Select(cmd => (cmd, new TestDomainEvent(cmd.Number)))
                .ToDictionary();

            // Обработчик событий домена
            var domainEventProcessor = new DomainEventProcessor(new DomainEventHandlerFabric(_provider));
            _domainEventHandler.HandleAsync(Arg.Any<IDomainEvent>(),
                    Arg.Any<DomainEventDispatcherContext>(),
                    Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(Fin<Unit>.Succ(Unit.Default));

            // Контейнер служб
            _provider.GetService(typeof(IValidator<CommandWithTrigger>))
                .Returns(null);
            _provider.GetService(typeof(IEnumerable<IBusinessConstraintCheck<CommandWithTrigger>>))
                .Returns(Array.Empty<IBusinessConstraintCheck<CommandWithTrigger>>());
            _provider.GetService(typeof(IEnumerable<ITriggerEventToCommandTranslator<MyTrigger>>))
                .Returns(Enumerable.Repeat(_triggerTranslator, count: 1));
            _provider.GetService(typeof(IDomainEventDispatcher))
                .Returns(domainEventProcessor);
            _provider.GetService(typeof(IEnumerable<IDomainEventHandler<TestDomainEvent>>))
                .Returns(Enumerable.Repeat(_domainEventHandler, count: 1));

            // Обработчик команды
            _cmdHandler = Substitute.ForPartsOf<DbHandler>(true, _provider);
            _cmdHandler
                .WhenForAnyArgs(x => x.RunAction(Arg.Any<CommandWithTrigger>()))
                .Do(x => domainEventProcessor.AddEvent(_cmd2domainEvents[x.Arg<CommandWithTrigger>()]));
            _provider.GetService(typeof(IBusinessCommandHandler<CommandWithTrigger>))
                .Returns(_cmdHandler);

            _provider.GetService(typeof(IBusinessActionRunner<CommandWithTrigger>))
                .Returns(_ => new BusinessActionRunner<CommandWithTrigger>(
                    _cmdHandler,
                    new BusinessActionRunnerFabric(_provider),
                    [],
                    new TriggerEventToCommandTranslatorFabric(_provider),
                    validator: null,
                    logger: null
                ));

            // Тестируемый объект
            _runner = NewRunner(_provider);
        }

        private static BusinessActionRunner<CommandWithTrigger> NewRunner(IServiceProvider provider) {
            return new BusinessActionRunner<CommandWithTrigger>(
                provider.GetRequiredService<IBusinessCommandHandler<CommandWithTrigger>>(),
                new BusinessActionRunnerFabric(provider),
                provider.GetServices<IBusinessConstraintCheck<CommandWithTrigger>>(),
                new TriggerEventToCommandTranslatorFabric(provider),
                provider.GetService<IValidator<CommandWithTrigger>>(),
                logger: null
            );
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
            Received.InOrder(async () => {
                await _cmdHandler.ExecuteAsync(_commands[0], Arg.Any<CancellationToken>());
                _cmdHandler.RunAction(Arg.Any<DbHandlerCommand>());
                await _domainEventHandler.HandleAsync((IDomainEvent)_cmd2domainEvents[_commands[0]],
                    Arg.Any<DomainEventDispatcherContext>(),
                    Arg.Any<CancellationToken>());
                _cmdHandler.Save();
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
            _runner = NewRunner(_provider);

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