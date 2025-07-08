using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.Messaging;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.starterKit.Utils;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace gmafffff.starterKit.BusinessLogic.Crud;

public partial class DeleteDbCommandHandler<
    TDeleteCommand, TDeletedEvent,
    TEntity, TEntityId> : BusinessCommandDbHandler<TDeleteCommand,
    TEntity, TEntityId, IRepository<TEntity, TEntityId>,
    TEntity, TEntity>
    where TDeleteCommand : DeleteBusinessCommand<TEntityId>
    where TDeletedEvent : DeletedBusinessEvent<TEntityId>
    where TEntityId : struct, IEquatable<TEntityId>
    where TEntity : Entity<TEntityId> {
    /// <summary>
    ///     CRC16 для
    ///     <see
    ///         cref="gmafffff.starterKit.BusinessLogic.Crud.DeleteDbCommandHandler{TDeleteCommand,TDeletedEvent,TEntity,TEntityId}" />
    /// </summary>
    public const int EventIdBase = 0xa864;

    public DeleteDbCommandHandler(IRepository<TEntity, TEntityId> repository,
        IDomainEventDispatcher domainEventDispatcher,
        ILogger<IBusinessCommandHandler<TDeleteCommand>>? logger = null)
        : base(repository, domainEventDispatcher, isSaveToDbSeparately: true, logger) { }

    protected override async Task<Fin<IList<TEntity>>> LoadAsync(IRepository<TEntity, TEntityId> repo,
        CancellationToken cancel = default) {
        return (await repo
                .LoadAsync(Command.Ids, cancel)
                .ConfigureAwait(false))
            .ToList();
    }

    protected override Task<Fin<IList<TEntity>>> RunActionAsync(IList<TEntity> loaded,
        CancellationToken cancel = default) {
        Repository.Delete(loaded);

        RemoveEntitiesLog(loaded.Select(e => e.Id).ToArray());
        return Task.FromResult(Fin<IList<TEntity>>.Succ(loaded));
    }

    protected override IList<BusinessEvent> PackResultToEvent(IList<TEntity> result) {
        return result
            .Select(r => Activator<TDeletedEvent>.CreateInstance(r.Id, Command, Guid.Empty))
            .Cast<BusinessEvent>()
            .ToList();
    }

    [LoggerMessage(EventId = EventIdBase + 1, Level = LogLevel.Trace,
        Message = "Помечены на удаление: {@removedEntitiesIds}",
        EventName = "RemoveEntities")]
    private partial void RemoveEntitiesLog(IList<TEntityId> removedEntitiesIds);
}