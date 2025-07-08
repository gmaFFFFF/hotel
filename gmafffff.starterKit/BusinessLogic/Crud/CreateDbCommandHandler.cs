using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.Messaging;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.starterKit.Utils;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace gmafffff.starterKit.BusinessLogic.Crud;

public partial class CreateDbCommandHandler<
    TAddCommand, TDto, TAddedEvent,
    TEntity, TEntityId> : BusinessCommandDbHandler<TAddCommand,
    TEntity, TEntityId, IRepository<TEntity, TEntityId>,
    Unit, TEntity>
    where TAddCommand : CreateBusinessCommand<TDto>
    where TAddedEvent : CreatedBusinessEvent<TEntityId>
    where TEntityId : struct, IEquatable<TEntityId>
    where TEntity : Entity<TEntityId> {
    /// <summary>
    ///     CRC16 для
    ///     <see
    ///         cref="gmafffff.starterKit.BusinessLogic.Crud.CreateDbCommandHandler{TAddCommand,TDto,TAddedEvent,TEntity,TEntityId}" />
    /// </summary>
    public const int EventIdBase = 0xef86;

    private readonly IEntityMapperBackward<TEntity, TEntityId, TDto> _mapper;

    public CreateDbCommandHandler(IRepository<TEntity, TEntityId> repository,
        IEntityMapperBackward<TEntity, TEntityId, TDto> mapper,
        IDomainEventDispatcher domainEventDispatcher,
        ILogger<IBusinessCommandHandler<TAddCommand>>? logger = null)
        : base(repository, domainEventDispatcher, isSaveToDbSeparately: true, logger) {
        _mapper = mapper;
    }

    protected override Task<Fin<IList<TEntity>>>
        RunActionAsync(IList<Unit> loaded, CancellationToken cancel = default) {
        var added = _mapper.Map(Command.New);
        Repository.Add(added);

        AddEntityLog(Command.New);
        return Task.FromResult(Fin<IList<TEntity>>.Succ([added]));
    }

    protected override IList<BusinessEvent> PackResultToEvent(IList<TEntity> result) {
        return result
            .Select(r => Activator<TAddedEvent>.CreateInstance(r.Id, Command, Guid.Empty))
            .Cast<BusinessEvent>()
            .ToList();
    }

    [LoggerMessage(EventId = EventIdBase + 1, Level = LogLevel.Trace,
        Message = "В оперативный склад добавлен новый объект: {@NewEntityDto}",
        EventName = "AddEntity")]
    private partial void AddEntityLog(TDto newEntityDto);
}