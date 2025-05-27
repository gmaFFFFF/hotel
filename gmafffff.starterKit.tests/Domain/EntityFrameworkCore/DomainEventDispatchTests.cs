using FluentAssertions.Execution;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.starterKit.tests.Domain.EntityFrameworkCore.Fixtures;
using NSubstitute.ExceptionExtensions;

namespace gmafffff.starterKit.tests.Domain.EntityFrameworkCore;

[TestSubject(typeof(DomainEventProcessor))]
public partial class DomainEventsTests {
    public class DomainEventDispatch {
        private readonly SimpleEntity _entity = new(id: 0, "Новая сущность");
        private readonly IDomainEvent _event1;
        private readonly IDomainEvent _event2;
        private readonly IDomainEvent _event3;
        private readonly DomainEventProcessor _eventProcessor;
        private readonly IDomainEventHandler<SimpleDomainEvent> _handler1;
        private readonly IDomainEventHandler<SimpleDomainEvent2> _handler2;
        private readonly IDomainEventHandler<SimpleDomainEvent3> _handler3;
        private readonly IServiceProvider _provider = Substitute.For<IServiceProvider>();

        public DomainEventDispatch() {
            _eventProcessor = new DomainEventProcessor(_provider);

            _event1 = new SimpleDomainEvent(_entity);
            _event2 = new SimpleDomainEvent2(_entity);
            _event3 = new SimpleDomainEvent3(_entity);

            _handler1 = Substitute.For<IDomainEventHandler<SimpleDomainEvent>>();
            _handler1.HandleAsync(_event1, Arg.Any<DomainEventDispatcherContext>(), Arg.Any<CancellationToken>())
                .Returns(Fin<Unit>.Succ(Unit.Empty));
            _handler2 = Substitute.For<IDomainEventHandler<SimpleDomainEvent2>>();
            _handler2.HandleAsync(_event2, Arg.Any<DomainEventDispatcherContext>(), Arg.Any<CancellationToken>())
                .Returns(Fin<Unit>.Succ(Unit.Empty));
            _handler3 = Substitute.For<IDomainEventHandler<SimpleDomainEvent3>>();
            _handler3.HandleAsync(_event3, Arg.Any<DomainEventDispatcherContext>(), Arg.Any<CancellationToken>())
                .Returns(Fin<Unit>.Succ(Unit.Empty));

            _provider.GetService(typeof(IEnumerable<IDomainEventHandler<SimpleDomainEvent>>))
                .Returns(Enumerable.Repeat(_handler1, count: 1));
            _provider.GetService(typeof(IEnumerable<IDomainEventHandler<SimpleDomainEvent2>>))
                .Returns(Enumerable.Repeat(_handler2, count: 1));
            _provider.GetService(typeof(IEnumerable<IDomainEventHandler<SimpleDomainEvent3>>))
                .Returns(Enumerable.Repeat(_handler3, count: 1));
        }

        /// <summary>
        ///     Вызывает обработчики событий
        /// </summary>
        [Fact]
        public async Task RegisteredDbContextConnectEventEmitterToAddedEntities() {
            // Arrange
            _eventProcessor.AddEvent(_event1).AddEvent(_event2);

            // Act
            await _eventProcessor.DispatchAsync();

            // Assert
            Received.InOrder(async () => {
                await _handler1.HandleAsync(_event1,
                    Arg.Is<DomainEventDispatcherContext>(c => !c.ProcessedEvents.Any() &&
                                                              c.UnhandledEvents.Single() == _event2),
                    Arg.Any<CancellationToken>());

                await _handler2.HandleAsync(_event2,
                    Arg.Is<DomainEventDispatcherContext>(c => c.ProcessedEvents.Single() == _event1 &&
                                                              !c.UnhandledEvents.Any()),
                    Arg.Any<CancellationToken>());
            });
        }

        /// <summary>
        ///     Обрабатывает события, сгенерированные обработчиками событий
        /// </summary>
        [Fact]
        public async Task ProcessEventsGeneratedByEventsHandlers() {
            // Arrange
            _handler1.HandleAsync(_event1, Arg.Any<DomainEventDispatcherContext>(), Arg.Any<CancellationToken>())
                .Returns(_ => {
                    _eventProcessor.AddEvent(_event3);
                    return Fin<Unit>.Succ(Unit.Empty);
                });
            _eventProcessor.AddEvent(_event1).AddEvent(_event2);

            // Act
            await _eventProcessor.DispatchAsync();

            // Assert
            Received.InOrder(async () => {
                await _handler1.HandleAsync(_event1,
                    Arg.Is<DomainEventDispatcherContext>(c => !c.ProcessedEvents.Any() &&
                                                              c.UnhandledEvents.Single() == _event2),
                    Arg.Any<CancellationToken>());

                await _handler2.HandleAsync(_event2,
                    Arg.Is<DomainEventDispatcherContext>(c => c.ProcessedEvents.Single() == _event1 &&
                                                              c.UnhandledEvents.Single() == _event3),
                    Arg.Any<CancellationToken>());

                await _handler3.HandleAsync(_event3,
                    Arg.Is<DomainEventDispatcherContext>(c => c.ProcessedEvents.First() == _event1 &&
                                                              c.ProcessedEvents.Last() == _event2 &&
                                                              c.ProcessedEvents.Count() == 2 &&
                                                              !c.UnhandledEvents.Any()),
                    Arg.Any<CancellationToken>());
            });
        }

        /// <summary>
        ///     Допускает отсутствие <see cref="IDomainEventHandler" />
        /// </summary>
        [Fact]
        public async Task AllowsAbsenceIDomainEventHandler() {
            // Arrange
            _provider.GetService(typeof(IEnumerable<IDomainEventHandler<SimpleDomainEvent>>))
                .Returns(Enumerable.Empty<IDomainEventHandler<SimpleDomainEvent>>());

            _eventProcessor.AddEvent(_event1).AddEvent(_event2).AddEvent(_event3);

            // Act
            await _eventProcessor.DispatchAsync();

            // Assert
            Received.InOrder(async () => {
                await _handler2.HandleAsync(_event2,
                    Arg.Is<DomainEventDispatcherContext>(c => c.ProcessedEvents.Single() == _event1 &&
                                                              c.UnhandledEvents.Single() == _event3),
                    Arg.Any<CancellationToken>());

                await _handler3.HandleAsync(_event3,
                    Arg.Is<DomainEventDispatcherContext>(c => c.ProcessedEvents.First() == _event1 &&
                                                              c.ProcessedEvents.Last() == _event2 &&
                                                              c.ProcessedEvents.Count() == 2 &&
                                                              !c.UnhandledEvents.Any()),
                    Arg.Any<CancellationToken>());
            });
        }

        /// <summary>
        ///     Обработка событий может завершиться ошибкой
        /// </summary>
        [Fact]
        public async Task EventProcessingCanEndWithError() {
            // Arrange
            var errorMessage = "Ошибка";
            _handler2.HandleAsync(_event2, Arg.Any<DomainEventDispatcherContext>(), Arg.Any<CancellationToken>())
                .Returns(Fin<Unit>.Fail(errorMessage));

            _eventProcessor.AddEvent(_event1).AddEvent(_event2).AddEvent(_event3);

            // Act
            var result = await _eventProcessor.DispatchAsync();

            // Assert
            using var _ = new AssertionScope();
            result.IsFail.Should().BeTrue();
            result.FailSpan()[0].Message.Should().Be(errorMessage);
            Received.InOrder(async () => {
                await _handler1.HandleAsync(_event1,
                    Arg.Is<DomainEventDispatcherContext>(c => !c.ProcessedEvents.Any() &&
                                                              c.UnhandledEvents.First() == _event2 &&
                                                              c.UnhandledEvents.Last() == _event3),
                    Arg.Any<CancellationToken>());

                await _handler2.HandleAsync(_event2,
                    Arg.Is<DomainEventDispatcherContext>(c => c.ProcessedEvents.Single() == _event1 &&
                                                              c.UnhandledEvents.Count() == 1),
                    Arg.Any<CancellationToken>());
            });

            await _handler3.DidNotReceiveWithAnyArgs().HandleAsync(Arg.Any<IDomainEvent>(),
                Arg.Any<DomainEventDispatcherContext>(),
                Arg.Any<CancellationToken>());
        }

        /// <summary>
        ///     Обработка событий прерывается, если обработчик выбросил исключение
        /// </summary>
        [Fact]
        public async Task EventProcessingInterruptIfThrow() {
            // Arrange
            var exceptionMessage = "Ошибка";
            Exception exception = new(exceptionMessage);

            _handler2.HandleAsync(_event2, Arg.Any<DomainEventDispatcherContext>(), Arg.Any<CancellationToken>())
                .ThrowsAsync(exception);

            _eventProcessor.AddEvent(_event1).AddEvent(_event2).AddEvent(_event3);

            // Act
            var act = async () => await _eventProcessor.DispatchAsync();

            // Assert
            using var _ = new AssertionScope();
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(exceptionMessage);

            Received.InOrder(async () => {
                await _handler1.HandleAsync(_event1,
                    Arg.Is<DomainEventDispatcherContext>(c => !c.ProcessedEvents.Any() &&
                                                              c.UnhandledEvents.First() == _event2 &&
                                                              c.UnhandledEvents.Last() == _event3),
                    Arg.Any<CancellationToken>());

                await _handler2.HandleAsync(_event2,
                    Arg.Is<DomainEventDispatcherContext>(c => c.ProcessedEvents.Single() == _event1 &&
                                                              c.UnhandledEvents.Count() == 1),
                    Arg.Any<CancellationToken>());
            });

            await _handler3.DidNotReceiveWithAnyArgs().HandleAsync(Arg.Any<IDomainEvent>(),
                Arg.Any<DomainEventDispatcherContext>(),
                Arg.Any<CancellationToken>());
        }

        /// <summary>
        ///     Обработка событий прерывается, если запрошена отмена
        /// </summary>
        [Fact]
        public async Task EventProcessingInterruptIfCancelRequest() {
            // Arrange
            CancellationTokenSource cts = new();

            _handler1.HandleAsync(_event1, Arg.Any<DomainEventDispatcherContext>(), Arg.Any<CancellationToken>())
                .Returns(_ => {
                    cts.Cancel();
                    return Fin<Unit>.Succ(Unit.Empty);
                });

            _eventProcessor.AddEvent(_event1).AddEvent(_event2).AddEvent(_event3);

            // Act
            var act = async () => await _eventProcessor.DispatchAsync(cts.Token);

            // Assert
            await act.Should().ThrowAsync<OperationCanceledException>();

            await _handler1.Received().HandleAsync(_event1,
                Arg.Is<DomainEventDispatcherContext>(c => !c.ProcessedEvents.Any() &&
                                                          c.UnhandledEvents.First() == _event2 &&
                                                          c.UnhandledEvents.Last() == _event3),
                Arg.Any<CancellationToken>());

            await _handler2.DidNotReceiveWithAnyArgs().HandleAsync(Arg.Any<IDomainEvent>(),
                Arg.Any<DomainEventDispatcherContext>(),
                Arg.Any<CancellationToken>());


            await _handler3.DidNotReceiveWithAnyArgs().HandleAsync(Arg.Any<IDomainEvent>(),
                Arg.Any<DomainEventDispatcherContext>(),
                Arg.Any<CancellationToken>());
        }
    }
}