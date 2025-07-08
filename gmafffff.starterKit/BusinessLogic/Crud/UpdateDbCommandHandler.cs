using gmafffff.starterKit.AppError;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.Messaging;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.starterKit.Utils;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace gmafffff.starterKit.BusinessLogic.Crud;

public partial class UpdateDbCommandHandler<
    TUpdateCommand, TDto, TUpdatedEvent,
    TEntity, TEntityId> : BusinessCommandDbHandler<TUpdateCommand,
    TEntity, TEntityId, IRepository<TEntity, TEntityId>,
    TEntity, TEntity>
    where TUpdateCommand : UpdateBusinessCommand<TEntityId, TDto>
    where TUpdatedEvent : UpdatedBusinessEvent<TEntityId>
    where TEntityId : struct, IEquatable<TEntityId>
    where TEntity : Entity<TEntityId> {
    /// <summary>
    ///     CRC16 для
    ///     <see
    ///         cref="gmafffff.starterKit.BusinessLogic.Crud.UpdateDbCommandHandler{TUpdateCommand,TDto,TUpdatedEvent,TEntity,TEntityId}" />
    /// </summary>
    public const int EventIdBase = 0x0d0c;

    private readonly IEntityMapperBackward<TEntity, TEntityId, TDto> _mapper;

    public UpdateDbCommandHandler(IRepository<TEntity, TEntityId> repository,
        IEntityMapperBackward<TEntity, TEntityId, TDto> mapper,
        IDomainEventDispatcher domainEventDispatcher,
        ILogger<IBusinessCommandHandler<TUpdateCommand>>? logger = null) : base(repository, domainEventDispatcher,
        isSaveToDbSeparately: true, logger) {
        _mapper = mapper;
    }

    protected override async Task<Fin<IList<TEntity>>> LoadAsync(IRepository<TEntity, TEntityId> repo,
        CancellationToken cancel = default) {
        var found = await repo
            .LoadAsync(Command.Id, cancel)
            .ConfigureAwait(false);

        return found is null
            ? AppErrorHelper.NewError(AppErrorCode.DbNotFound)
            : Fin<IList<TEntity>>.Succ([found]);
    }

    protected override Task<Fin<IList<TEntity>>> RunActionAsync(IList<TEntity> loaded,
        CancellationToken cancel = default) {
        _mapper.Update(Command.Changed, loaded[0]);

        UpdateEntityLog(loaded[0].Id);
        return Task.FromResult(Fin<IList<TEntity>>.Succ(loaded));
    }

    protected override IList<BusinessEvent> PackResultToEvent(IList<TEntity> result) {
        return result
            .Select(r => Activator<TUpdatedEvent>.CreateInstance(r.Id, Command, Guid.Empty))
            .Cast<BusinessEvent>()
            .ToList();
    }

    [LoggerMessage(EventId = EventIdBase + 1, Level = LogLevel.Trace,
        Message = "Модифицирован объект: {@updatedEntityId}",
        EventName = "UpdateEntity")]
    private partial void UpdateEntityLog(TEntityId updatedEntityId);
}