using LanguageExt.Common;

namespace gmafffff.starterKit.AppError;

public enum AppErrorCode {
    None,
    ValidationGeneral = Errors.ValidationFailedCode,
    TimedOut = Errors.TimedOutCode,
    OperationCancel = Errors.CancelledCode,
    NotSupported = -102_334_155,
    DbNotFound,
    DbConcurrentWrite
}