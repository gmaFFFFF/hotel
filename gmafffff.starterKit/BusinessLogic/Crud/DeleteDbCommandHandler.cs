using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.Messaging;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.starterKit.Utils;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace gmafffff.starterKit.BusinessLogic.Crud;

public class DeleteDbCommandHandler<
    TDeleteCommand, TDeletedEvent,
    TEntity, TEntityId>(
    IRepository<TEntity, TEntityId> repository,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<IBusinessCommandHandler<TDeleteCommand>>? logger = null)
    : BusinessCommandDbHandler<TDeleteCommand,
        TEntity, TEntityId, IRepository<TEntity, TEntityId>,
        TEntity, TEntity>(repository, domainEventDispatcher, isSaveToDbSeparately: true, logger)
    where TDeleteCommand : DeleteBusinessCommand<TEntityId>
    where TDeletedEvent : DeletedBusinessEvent<TEntityId>
    where TEntityId : struct, IEquatable<TEntityId>
    where TEntity : Entity<TEntityId> {
    protected override async Task<Fin<IList<TEntity>>> LoadAsync(IRepository<TEntity, TEntityId> repo,
        CancellationToken cancel = default) {
        return (await repo
                .LoadAsync(Command.Ids, cancel)
                .ConfigureAwait(false))
            .ToList();
    }

    protected override Task<Fin<IList<TEntity>>> RunActionAsync(IList<TEntity> loaded,
        CancellationToken cancel = default) {
        repository.Delete(loaded);
        return Task.FromResult(Fin<IList<TEntity>>.Succ(loaded));
    }

    protected override IList<BusinessEvent> PackResultToEvent(IList<TEntity> result) {
        return result
            .Select(r => Activator<TDeletedEvent>.CreateInstance(r.Id, Command, Guid.Empty))
            .Cast<BusinessEvent>()
            .ToList();
    }
}