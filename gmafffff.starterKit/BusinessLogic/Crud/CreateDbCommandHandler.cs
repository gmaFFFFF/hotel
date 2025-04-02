using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.starterKit.Utils;
using LanguageExt;

namespace gmafffff.starterKit.BusinessLogic.Crud;

public class CreateDbCommandHandler<
    TAddCommand, TDto, TAddedEvent,
    TEntity, TEntityId>(
    IRepository<TEntity, TEntityId> repository,
    IEntityMapperBackward<TEntity, TEntityId, TDto> mapper)
    : BusinessCommandDbHandler<TAddCommand, TAddedEvent, TEntity, TEntityId, IRepository<TEntity, TEntityId>, Unit,
            TEntity>
        (repository, isSaveToDbSeparately: true)
    where TAddCommand : CreateBusinessCommand<TDto>
    where TAddedEvent : CreatedBusinessEvent<TEntityId>
    where TEntityId : struct, IEquatable<TEntityId>
    where TEntity : Entity<TEntityId> {
    protected override Task<Fin<IList<TEntity>>>
        RunActionAsync(IList<Unit> loaded, CancellationToken cancel = default) {
        var added = mapper.Map(Command.New);
        repository.Add(added);
        return Task.FromResult(Fin<IList<TEntity>>.Succ([added]));
    }

    protected override IList<TAddedEvent> PackResultToEvent(IList<TEntity> result) {
        return result
            .Select(r => Activator<TAddedEvent>.CreateInstance(r.Id, Command, default(Guid)))
            .ToArray();
    }
}