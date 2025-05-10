using AutoFixture;
using FluentAssertions.Execution;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging;
using gmafffff.starterKit.Tests.BusinessLogic.Fixtures;
using NSubstitute.ExceptionExtensions;
using Validot;

namespace gmafffff.starterKit.tests.BusinessLogic;

[TestSubject(typeof(BusinessActionRunner<>))]
public partial class BusinessActionRunnerTests {
    public class BusinessActionRunnerTrigger {
        private const string ExceptionMessage = "Обработка прервана";
        private readonly Dictionary<CommandWithTrigger, IList<BusinessEvent>> _cmdToEvents = [];

        private readonly List<CommandWithTrigger> _commands;
        private readonly MyTrigger _errorTrigger;
        private readonly Dictionary<TriggerEvent, CommandWithTrigger> _eventToCmd = [];
        private readonly Exception _exception = new(ExceptionMessage);
        private readonly Fixture _fixture = new();

        private readonly IBusinessCommandHandler<CommandWithTrigger> _handler =
            Substitute.For<IBusinessCommandHandler<CommandWithTrigger>>();

        private readonly IServiceProvider _provider = Substitute.For<IServiceProvider>();

        private readonly BusinessActionRunner<CommandWithTrigger> _runner;

        private readonly ITriggerEventToCommandTranslator<MyTrigger> _translator =
            Substitute.For<ITriggerEventToCommandTranslator<MyTrigger>>();

        public BusinessActionRunnerTrigger() {
            _runner = new BusinessActionRunner<CommandWithTrigger>(_provider);

            // Контейнер служб
            _provider.GetService(typeof(IBusinessActionRunner<CommandWithTrigger>))
                .Returns(_ => new BusinessActionRunner<CommandWithTrigger>(_provider));
            _provider.GetService(typeof(IValidator<CommandWithTrigger>))
                .Returns(null);
            _provider.GetService(typeof(IEnumerable<IBusinessConstraintCheck<CommandWithTrigger>>))
                .Returns(Array.Empty<IBusinessConstraintCheck<CommandWithTrigger>>());
            _provider.GetService(typeof(IBusinessCommandHandler<CommandWithTrigger>))
                .Returns(_handler);
            _provider.GetService(typeof(IEnumerable<ITriggerEventToCommandTranslator<MyTrigger>>))
                .Returns(_ => Enumerable.Repeat(_translator, count: 1));

            // Образцы данных
            _commands = _fixture.CreateMany<CommandWithTrigger>(8)
                .Select((c, i) => c with { number = i }).ToList();
            IList<BusinessEvent> step0 = [
                new EndEventForTrigger(Number: 1, _commands[0]),
                new MyTrigger(Number: 100, _commands[0]),
                new EndEventForTrigger(Number: 2, _commands[0]),
                new MyTrigger(Number: 101, _commands[0]),
                new EndEventForTrigger(Number: 3, _commands[0]),
                new MyTrigger(Number: 102, _commands[0]),
                new EndEventForTrigger(Number: 4, _commands[0])
            ];
            IList<BusinessEvent> step2_0 = [
                new MyTrigger(Number: 103, _commands[1]),
                new EndEventForTrigger(Number: 5, _commands[1])
            ];
            IList<BusinessEvent> step2_1 = [
                new EndEventForTrigger(Number: 6, _commands[2]),
                new MyTrigger(Number: 104, _commands[2])
            ];
            IList<BusinessEvent> step2_2 = [
                new EndEventForTrigger(Number: 7, _commands[3]),
                new MyTrigger(Number: 105, _commands[0]),
                new EndEventForTrigger(Number: 8, _commands[3])
            ];
            IList<BusinessEvent> step3_1 = [
                new EndEventForTrigger(Number: 9, _commands[4]),
                new EndEventForTrigger(Number: 10, _commands[4])
            ];
            IList<BusinessEvent> step3_2 = [
                new EndEventForTrigger(Number: 11, _commands[5])
            ];
            IList<BusinessEvent> step3_3 = [];

            // Команда с ошибкой
            _commands[7] = _commands[7] with { Error = true };
            _errorTrigger = new MyTrigger(Number: -1, _commands[6]);

            // Транслятор команд
            _eventToCmd[(MyTrigger)step0[1]] = _commands[1];
            _eventToCmd[(MyTrigger)step0[3]] = _commands[2];
            _eventToCmd[(MyTrigger)step0[5]] = _commands[3];
            _eventToCmd[(MyTrigger)step2_0[0]] = _commands[4];
            _eventToCmd[(MyTrigger)step2_1[1]] = _commands[5];
            _eventToCmd[(MyTrigger)step2_2[1]] = _commands[6];
            _eventToCmd[_errorTrigger] = _commands[7];

            _translator.Translate(Arg.Any<TriggerEvent>())
                .ReturnsForAnyArgs(x => [_eventToCmd[x.Arg<TriggerEvent>()]]);

            // Обработчик команды
            _cmdToEvents[_commands[0]] = step0;
            _cmdToEvents[_commands[1]] = step2_0;
            _cmdToEvents[_commands[2]] = step2_1;
            _cmdToEvents[_commands[3]] = step2_2;
            _cmdToEvents[_commands[4]] = step3_1;
            _cmdToEvents[_commands[5]] = step3_2;
            _cmdToEvents[_commands[6]] = step3_3;


            _handler.ExecuteAsync(default!)
                .ReturnsForAnyArgs(x => Fin<IList<BusinessEvent>>.Succ(_cmdToEvents[x.Arg<CommandWithTrigger>()]));

            _handler.ExecuteAsync(Arg.Is<CommandWithTrigger>(trigger => trigger.Error), Arg.Any<CancellationToken>())
                .Throws(_exception);
        }

        /// <summary>
        ///     Обрабатывает события-сигналы типа <see cref="TriggerEvent" />
        /// </summary>
        [Fact]
        public async Task ProcessTriggerEvent() {
            // Arrange
            // Act
            var result = await _runner.Execute(_commands[0]);

            // Assert
            using var _ = new AssertionScope();
            result.IsSucc.Should().BeTrue();
            result.SuccSpan().ToArray().SelectMany(x => x).Should()
                .BeEquivalentTo(_cmdToEvents.Values.SelectMany(x => x).OfType<EndEventForTrigger>().ToArray());
            Received.InOrder(async () => {
                await _handler.Received().ExecuteAsync(_commands[0], Arg.Any<CancellationToken>());
                await _handler.Received().ExecuteAsync(_commands[1], Arg.Any<CancellationToken>());
                await _handler.Received().ExecuteAsync(_commands[4], Arg.Any<CancellationToken>());
                await _handler.Received().ExecuteAsync(_commands[2], Arg.Any<CancellationToken>());
                await _handler.Received().ExecuteAsync(_commands[5], Arg.Any<CancellationToken>());
                await _handler.Received().ExecuteAsync(_commands[3], Arg.Any<CancellationToken>());
                await _handler.Received().ExecuteAsync(_commands[6], Arg.Any<CancellationToken>());
            });
        }

        /// <summary>
        ///     Обработка команды прерывается, если обработчик события-сигналы типа <see cref="TriggerEvent" />
        ///     выборосил исключение
        /// </summary>
        [Fact]
        public async Task ProcessTriggerEventInterruptIfThrow() {
            // Arrange
            _cmdToEvents[_commands[6]] = [_errorTrigger];

            // Act
            var act = async () => await _runner.Execute(_commands[0]);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(ExceptionMessage);
        }

        /// <summary>
        ///     Допускает отсутствие <see cref="ITriggerEventToCommandTranslator" />
        /// </summary>
        [Fact]
        public async Task AllowsAbsenceITriggerEventToCommandTranslator() {
            // Arrange
            _provider.GetService(typeof(IEnumerable<ITriggerEventToCommandTranslator<MyTrigger>>))
                .Returns(_ => Enumerable.Empty<ITriggerEventToCommandTranslator<MyTrigger>>());

            // Act
            var result = await _runner.Execute(_commands[0]);

            // Assert
            result.IsSucc.Should().BeTrue();
            result.SuccSpan().ToArray().SelectMany(x => x).Should()
                .Equal(_cmdToEvents[_commands[0]].OfType<EndEventForTrigger>());
        }
    }
}