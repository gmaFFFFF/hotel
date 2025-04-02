using gmafffff.starterKit.AppError;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.starterKit.Utils;
using LanguageExt;

namespace gmafffff.starterKit.BusinessLogic.Crud;

public class UpdateDbCommandHandler<
    TUpdateCommand, TDto, TUpdatedEvent,
    TEntity, TEntityId>(
    IRepository<TEntity, TEntityId> repository,
    IEntityMapperBackward<TEntity, TEntityId, TDto> mapper)
    : BusinessCommandDbHandler<TUpdateCommand, TUpdatedEvent, TEntity, TEntityId, IRepository<TEntity, TEntityId>,
            TEntity, TEntity>
        (repository, isSaveToDbSeparately: true)
    where TUpdateCommand : UpdateBusinessCommand<TEntityId, TDto>
    where TUpdatedEvent : UpdatedBusinessEvent<TEntityId>
    where TEntityId : struct, IEquatable<TEntityId>
    where TEntity : Entity<TEntityId> {
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
        mapper.Update(Command.Changed, loaded[0]);
        return Task.FromResult(Fin<IList<TEntity>>.Succ(loaded));
    }

    protected override IList<TUpdatedEvent> PackResultToEvent(IList<TEntity> result) {
        return result
            .Select(r => Activator<TUpdatedEvent>.CreateInstance(r.Id, Command, default(Guid)))
            .ToArray();
    }
}