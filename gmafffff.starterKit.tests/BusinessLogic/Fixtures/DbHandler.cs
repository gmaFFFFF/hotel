using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;

namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

public class DbHandler(bool isSaveToDbSeparately = true)
    : BusinessCommandDbHandler<DbHandlerCommand, DbHandlerEvent, DbHandlerStatus>(isSaveToDbSeparately) {
    public static EventHandler<BeforeSavingEventArgs> Handler =
        (_, args) => args.IsSaveResult = args.Command.IsSaveResult;

    protected override Task<Fin<Unit>> LoadAsync(CancellationToken cancel = default) {
        PreliminaryResult.Add(DbHandlerStatus.Load);
        return LastCommand!.IsSuccessLoad
            ? base.LoadAsync(cancel)
            : Task.FromResult(Fin<Unit>.Fail(AppErrorHelper.NewError(AppErrorCode.TimedOut)));
    }

    protected override Task<Fin<Unit>> RunActionAsync(CancellationToken cancel = default) {
        PreliminaryResult[index: 0] |= DbHandlerStatus.Action;
        return LastCommand!.Exception is { } exc
            ? throw exc
            : Task.FromResult<Fin<Unit>>(Unit.Default);
    }

    protected override void OnBeforeSaving() {
        PreliminaryResult[index: 0] |= DbHandlerStatus.BeforeSaving;
        base.OnBeforeSaving();
    }

    protected override Task<Fin<Unit>> SaveAsync(CancellationToken cancel = default) {
        PreliminaryResult[index: 0] |= DbHandlerStatus.Save;
        return base.SaveAsync(cancel);
    }

    protected override void PackResultToEvent() {
        PreliminaryResult[index: 0] |= DbHandlerStatus.WithoutSave;
        LastResult = [new DbHandlerEvent(PreliminaryResult[index: 0], LastCommand.MessageId)];
    }
}