using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.Messaging;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.starterKit.Utils;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace gmafffff.starterKit.BusinessLogic.Crud;

public class CreateDbCommandHandler<
    TAddCommand, TDto, TAddedEvent,
    TEntity, TEntityId>(
    IRepository<TEntity, TEntityId> repository,
    IEntityMapperBackward<TEntity, TEntityId, TDto> mapper,
    IServiceProvider serviceProvider,
    ILogger<IBusinessCommandHandler<TAddCommand>>? logger = null)
    : BusinessCommandDbHandler<TAddCommand,
        TEntity, TEntityId, IRepository<TEntity, TEntityId>,
        Unit, TEntity>(repository, serviceProvider, isSaveToDbSeparately: true, logger)
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

    protected override IList<BusinessEvent> PackResultToEvent(IList<TEntity> result) {
        return result
            .Select(r => Activator<TAddedEvent>.CreateInstance(r.Id, Command, default(Guid)))
            .Cast<BusinessEvent>()
            .ToList();
    }
}