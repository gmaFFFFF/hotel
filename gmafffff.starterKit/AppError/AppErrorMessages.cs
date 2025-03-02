namespace gmafffff.starterKit.AppError;

public class AppErrorMessages : IErrorMessage<AppErrorCode> {
    public Dictionary<string, Dictionary<AppErrorCode, string>> Messages { get; set; } = new() {
        [IErrorMessage<AppErrorCode>.Ru] = new Dictionary<AppErrorCode, string> {
            [AppErrorCode.OperationCancel] = "Операция отменена",
            [AppErrorCode.TimedOut] = "Разумное время ожидания истекло",
            [AppErrorCode.NotSupported] = "Операция не поддерживается",

            [AppErrorCode.ValidationGeneral] = "Некорректные данные",

            [AppErrorCode.DbConcurrentWrite] = "Запись была изменена другим пользователем",
            [AppErrorCode.DbNotFound] = "Запись не найдена"
        },

        [IErrorMessage<AppErrorCode>.En] = new Dictionary<AppErrorCode, string> {
            [AppErrorCode.OperationCancel] = "The operation is canceled",
            [AppErrorCode.TimedOut] = "The reasonable waiting time has expired",
            [AppErrorCode.NotSupported] = "The operation is not supported",

            [AppErrorCode.ValidationGeneral] = "Not correct data",

            [AppErrorCode.DbConcurrentWrite] = "The recording was changed by another user",
            [AppErrorCode.DbNotFound] = "Record not found"
        }
    };
}