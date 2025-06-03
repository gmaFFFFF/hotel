using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Domain.Events;
using LanguageExt;
using Light.GuardClauses.FrameworkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace gmafffff.starterKit.EntityFrameworkCore;

public class DomainEventProcessor(
    IServiceProvider serviceProvider,
    ILogger<DomainEventProcessor>? logger = null) : IDomainEventSink, IDomainEventDispatcher {
    /// <summary>
    ///     Журнал
    /// </summary>
    private readonly ILogger<DomainEventProcessor> _logger = logger ?? NullLogger<DomainEventProcessor>.Instance;

    #region Реализация IDomainEventDispatcher

    /// <summary>
    ///     Контейнер DI
    /// </summary>
    protected readonly IServiceProvider ServiceProvider = serviceProvider;

    /// <summary>
    ///     Индекс первого необработанного события
    /// </summary>
    private int _unhandledEventIndex;

    /// <summary>
    ///     Необработанные события
    /// </summary>
    public IEnumerable<IDomainEvent> UnhandledEvents => Events.Skip(_unhandledEventIndex);

    /// <summary>
    ///     Обработанные события
    /// </summary>
    public IEnumerable<IDomainEvent> ProcessedEvents => Events.Take(_unhandledEventIndex);


    /// <summary>
    ///     Вспомогательная структура для сохранения промежуточного статуса обработки
    /// </summary>
    /// <param name="Events"></param>
    /// <param name="UnhandledEventIndex"></param>
    /// <param name="ServiceProvider"></param>
    private record HandleState(
        IReadOnlyList<IDomainEvent> Events,
        int UnhandledEventIndex,
        IServiceProvider ServiceProvider);

    public async Task<Fin<Unit>> DispatchAsync(CancellationToken cancel = default) {
        HandleState initialState = new(Events, _unhandledEventIndex, ServiceProvider);
        var steps = await HandleEventsRecursive(_logger)
            .Run(initialState).As()
            .Run().As()
            .RunAsync(EnvIO.New(token: cancel)).ConfigureAwait(false);

        steps.IfSucc(state => _unhandledEventIndex = state.State.UnhandledEventIndex);

        return steps.Map(Unit.Default).As();

        // Запуск рекурсивной обработки событий
        static StateT<HandleState, FinT<IO>, Unit> HandleEventsRecursive(ILogger<DomainEventProcessor> logger) {
            var steps =
                from hasEvent in HasUnprocessedEvent()
                from _1 in hasEvent
                    ? from @event in GetEvent()
                    from handlers in FindDomainEventHandlers(@event)
                    from context in CreateDomainEventDispatcherContext()
                    from _1 in RunEventHandlers(@event, context, handlers, logger)
                    from _2 in MarkUnprocessedEventAsProcessed()
                    from _3 in HandleEventsRecursive(logger)
                    select Unit.Default
                    : StateT<HandleState, FinT<IO>, Unit>.LiftIO(IO.pure(Unit.Default))
                select Unit.Default;

            return steps;
        }

        // Есть ли ещё необработанные события
        static StateT<HandleState, FinT<IO>, bool> HasUnprocessedEvent() {
            return from state in StateT.get<FinT<IO>, HandleState>()
                select state.UnhandledEventIndex < state.Events.Count;
        }

        // Дай первое необработанное событие. Выход за границы не контролируется.
        static StateT<HandleState, FinT<IO>, IDomainEvent> GetEvent() {
            return from state in StateT.get<FinT<IO>, HandleState>()
                select state.Events[state.UnhandledEventIndex];
        }

        // Отметить, что текущее событие обработано
        static StateT<HandleState, FinT<IO>, HandleState> MarkUnprocessedEventAsProcessed() {
            return from state in StateT.get<FinT<IO>, HandleState>()
                let newState = state with { UnhandledEventIndex = state.UnhandledEventIndex + 1 }
                from _ in StateT.put<FinT<IO>, HandleState>(newState)
                select newState;
        }

        // Найти все обработчики для конкретного события в ServiceProvider
        static StateT<HandleState, FinT<IO>, Iterable<IDomainEventHandler>>
            FindDomainEventHandlers(IDomainEvent @event) {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());
            return from state in StateT.get<FinT<IO>, HandleState>()
                select state.ServiceProvider
                    .GetServices(handlerType)
                    .Cast<IDomainEventHandler>()
                    .AsIterable();
        }

        // Сформировать контекст обработки события
        static StateT<HandleState, FinT<IO>, DomainEventDispatcherContext> CreateDomainEventDispatcherContext() {
            return from state in StateT.get<FinT<IO>, HandleState>()
                select new DomainEventDispatcherContext(
                    state.Events.Skip(state.UnhandledEventIndex).Skip(1).ToArray(),
                    state.Events.Take(state.UnhandledEventIndex).ToArray()
                );
        }

        // Запустить все обработчики для данного типа события
        static FinT<IO, Unit> RunEventHandlers(IDomainEvent @event, DomainEventDispatcherContext context,
            Iterable<IDomainEventHandler> handlers, ILogger<DomainEventProcessor> logger) {
            return handlers
                .Traverse(handler => RunEventHandler(@event, context, handler))
                .As()
                .Map(x => x.FirstOrDefault(result => result.IsFail) ?? Fin<Unit>.Succ(Unit.Default))
                .Map(fin =>
                    fin.BiMap(
                        Succ: _ => {
                            logger.LogTrace("Событие {@Event} успешно обработано", @event);
                            return Unit.Default;
                        }, Fail: error => {
                            logger.LogTrace("Обработка события {@Event} завершилась с ошибкой {@Error}",
                                @event, error);
                            return error;
                        }));
        }

        // Запустить обработчик события
        static IO<Fin<Unit>> RunEventHandler(IDomainEvent @event, DomainEventDispatcherContext context,
            IDomainEventHandler handler) {
            return IO.liftAsync(
                async env => await handler.HandleAsync(@event, context, env.Token).ConfigureAwait(false));
        }
    }

    #endregion

    #region Реализация IDomainEventSink

    /// <summary>
    ///     Зарегистрированные DbContext's
    /// </summary>
    private readonly Dictionary<DbContextId, DbContext> _dbContexts = [];

    /// <summary>
    ///     Все поступившие события
    /// </summary>
    private readonly List<IDomainEvent> _events = [];

    public IReadOnlyList<IDomainEvent> Events => _events.AsReadOnlyList();

    public IDomainEventSink AddEvent(IDomainEvent @event) {
        _logger.LogTrace("Возникло событие домена {@DomainEvent}", @event);
        if (!_events.Contains(@event))
            _events.Add(@event);

        return this;
    }

    public IDomainEventSink RegisterDbContext(DbContext context) {
        if (!_dbContexts.TryAdd(context.ContextId, context))
            return this;

        context.ChangeTracker.Tracked += ChangeTrackerOnTracked;
        context.ChangeTracker.StateChanged += ChangeTrackerOnStateChanged;

        return this;
    }

    /// <summary>
    ///     Подключает к сущностям <see cref="Entity{TId}" />, реализующим интерфейс <see cref="IDomainEventSink" />,
    ///     себя в качестве приемника событий при начале их отслеживания <see cref="DbContext" />
    /// </summary>
    private void ChangeTrackerOnTracked(object? sender, EntityTrackedEventArgs e) {
        if (e.Entry.Entity is IDomainEventEmitter entity)
            entity.SetDomainEventSink(this);
    }

    /// <summary>
    ///     Отключает у сущности <see cref="Entity{TId}" />, реализующей интерфейс <see cref="IDomainEventSink" />,
    ///     приемника событий при прекращении ей отслеживания <see cref="DbContext" />
    /// </summary>
    private void ChangeTrackerOnStateChanged(object? sender, EntityStateChangedEventArgs e) {
        if (e.Entry.Entity is IDomainEventEmitter entity &&
            e.NewState is EntityState.Detached)
            entity.ResetDomainEventSink();
    }

    #endregion
}