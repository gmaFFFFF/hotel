using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

public class DbHandler(bool isSaveToDbSeparately = true, IServiceProvider? provider = null)
    : BusinessCommandDbHandler<DbHandlerCommand,
        BusinessEntity, int, Repo, DbHandlerStatus,
        DbHandlerStatus>
    (new Repo(),
        (provider ?? GetFakeServiceProvider()).GetRequiredService<IDomainEventDispatcher>(),
        isSaveToDbSeparately) {
    public static readonly EventHandler<BeforeSavingEventArgs> Handler =
        (_, args) => args.IsSaveResult = args.Command.IsSaveResult;

    public DbHandlerStatus Status;

    private static IServiceProvider GetFakeServiceProvider() {
        var provider = Substitute.For<IServiceProvider>();
        var dispatcher = Substitute.For<IDomainEventDispatcher>();
        var eventHandler = Substitute.For<IDomainEventHandler<IDomainEvent>>();

        dispatcher.DispatchAsync(Arg.Any<CancellationToken>()).ReturnsForAnyArgs(Fin<Unit>.Succ(Unit.Default));
        eventHandler.HandleAsync(Arg.Any<IDomainEvent>(),
                Arg.Any<DomainEventDispatcherContext>(),
                Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(Fin<Unit>.Succ(Unit.Default));
        provider.GetService(typeof(IDomainEventDispatcher)).Returns(dispatcher);
        provider.GetService(Arg.Is<Type>(t => t.IsAssignableTo(typeof(IDomainEventHandler))))
            .Returns(Enumerable.Repeat(eventHandler, count: 1));


        return provider;
    }

    protected override Task<Fin<IList<DbHandlerStatus>>> LoadAsync(Repo repo, CancellationToken cancel = default) {
        Status = DbHandlerStatus.Load;
        return Command.IsSuccessLoad
            ? Task.FromResult(Fin<IList<DbHandlerStatus>>.Succ([Status]))
            : Task.FromResult(Fin<IList<DbHandlerStatus>>.Fail(AppErrorHelper.NewError(AppErrorCode.TimedOut)));
    }

    protected override Task<Fin<IList<DbHandlerStatus>>> RunActionAsync(IList<DbHandlerStatus> status,
        CancellationToken cancel = default) {
        Status = status[0] | DbHandlerStatus.Action;
        RunAction(Command);
        return Command.Exception is { } exc
            ? throw exc
            : Task.FromResult(Fin<IList<DbHandlerStatus>>.Succ([Status]));
    }

    public virtual void RunAction(DbHandlerCommand command) { }

    protected override bool OnBeforeSaving(IList<DbHandlerStatus> status) {
        Status = status[0] | DbHandlerStatus.BeforeSaving;
        return base.OnBeforeSaving([Status]);
    }

    protected override Task<Fin<int>> SaveAsync(Repo repo, CancellationToken cancel = default) {
        Status |= DbHandlerStatus.Save;
        Save();
        return base.SaveAsync(repo, cancel);
    }

    public virtual void Save() { }

    protected override IList<BusinessEvent> PackResultToEvent(IList<DbHandlerStatus> status) {
        Status |= DbHandlerStatus.Pack;
        return [new DbHandlerEvent(Status, Command.MessageId)];
    }
}