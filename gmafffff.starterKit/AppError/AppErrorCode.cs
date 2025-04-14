using LanguageExt.Common;

namespace gmafffff.starterKit.AppError;

public enum AppErrorCode {
    None,
    ValidationGeneral = Errors.ValidationFailedCode,
    TimedOut = Errors.TimedOutCode,
    OperationCancel = Errors.CancelledCode,
    NotSupported = -0xbf7b,    // CRC16 gmafffff.starterKit.AppError.AppErrorCode
    DbNotFound,
    DbConcurrentWrite
}