using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.starterKit.Utils;
using LanguageExt;

namespace gmafffff.starterKit.BusinessLogic.Crud;

public class DeleteDbCommandHandler<
    TDeleteCommand, TDeletedEvent,
    TEntity, TEntityId>(IRepository<TEntity, TEntityId> repository)
    : BusinessCommandDbHandler<TDeleteCommand, TDeletedEvent, TEntity, TEntityId, IRepository<TEntity, TEntityId>,
            TEntity, TEntity>
        (repository, isSaveToDbSeparately: true)
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

    protected override IList<TDeletedEvent> PackResultToEvent(IList<TEntity> result) {
        return result
            .Select(r => Activator<TDeletedEvent>.CreateInstance(r.Id, Command, default(Guid)))
            .ToArray();
    }
}