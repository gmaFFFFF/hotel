using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

public class DbHandler(bool isSaveToDbSeparately = true)
    : BusinessCommandDbHandler<DbHandlerCommand,
        BusinessEntity, int, Repo, DbHandlerStatus,
        DbHandlerStatus>(new Repo(), isSaveToDbSeparately) {
    public static EventHandler<BeforeSavingEventArgs> Handler =
        (_, args) => args.IsSaveResult = args.Command.IsSaveResult;

    public DbHandlerStatus Status;


    protected override Task<Fin<IList<DbHandlerStatus>>> LoadAsync(Repo repo, CancellationToken cancel = default) {
        Status = DbHandlerStatus.Load;
        return Command.IsSuccessLoad
            ? Task.FromResult(Fin<IList<DbHandlerStatus>>.Succ([Status]))
            : Task.FromResult(Fin<IList<DbHandlerStatus>>.Fail(AppErrorHelper.NewError(AppErrorCode.TimedOut)));
    }

    protected override Task<Fin<IList<DbHandlerStatus>>> RunActionAsync(IList<DbHandlerStatus> status,
        CancellationToken cancel = default) {
        Status = status[0] | DbHandlerStatus.Action;
        return Command.Exception is { } exc
            ? throw exc
            : Task.FromResult(Fin<IList<DbHandlerStatus>>.Succ([Status]));
    }

    protected override bool OnBeforeSaving(IList<DbHandlerStatus> status) {
        Status = status[0] | DbHandlerStatus.BeforeSaving;
        return base.OnBeforeSaving([Status]);
    }

    protected override Task<Fin<int>> SaveAsync(Repo repo, CancellationToken cancel = default) {
        Status |= DbHandlerStatus.Save;
        return base.SaveAsync(repo, cancel);
    }

    protected override IList<BusinessEvent> PackResultToEvent(IList<DbHandlerStatus> status) {
        Status |= DbHandlerStatus.Pack;
        return [new DbHandlerEvent(Status, Command.MessageId)];
    }
}