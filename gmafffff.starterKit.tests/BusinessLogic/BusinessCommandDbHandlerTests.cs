using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.tests.BusinessLogic.Fixtures;

namespace gmafffff.starterKit.tests.BusinessLogic;

[TestSubject(typeof(BusinessCommandDbHandler<,,,,,,>))]
public class BusinessCommandDbHandlerTests {
    public static TheoryData<DbHandlerCommand, bool, DbHandlerStatus, bool> TestRuns {
        get {
            var testData = new TheoryData<DbHandlerCommand, bool, DbHandlerStatus, bool>();

            var success = new DbHandlerCommand();
            var discardChanges = new DbHandlerCommand(IsSaveResult: false);
            var notLoad = new DbHandlerCommand(IsSuccessLoad: false);
            var exc = new Exception("Возникло неожиданное исключение");
            var withException = new DbHandlerCommand(Exception: exc);

            testData.Add(success, p2: true, DbHandlerStatus.All, p4: true);
            testData.Add(success, p2: true, DbHandlerStatus.WithoutSave, p4: false);
            testData.Add(discardChanges, p2: true, DbHandlerStatus.WithoutSave | DbHandlerStatus.BeforeSaving,
                p4: true);
            testData.Add(discardChanges, p2: true, DbHandlerStatus.WithoutSave, p4: false);
            testData.Add(notLoad, p2: false, DbHandlerStatus.LoadFail, p4: true);
            testData.Add(notLoad, p2: false, DbHandlerStatus.LoadFail, p4: false);
            testData.Add(withException, p2: true, DbHandlerStatus.None, p4: true);
            testData.Add(withException, p2: true, DbHandlerStatus.None, p4: false);

            return testData;
        }
    }

    /// <summary>
    ///     Тестирование метода ExecuteAsync методом «прозрачного ящика»
    /// </summary>
    [Theory]
    [MemberData(nameof(TestRuns))]
    public async Task TestingMethodExecuteAsWhiteBoxAsync(
        DbHandlerCommand command,
        bool isSuccess, DbHandlerStatus exceptedResult,
        bool isSaveToDbSeparately) {
        var handler = new DbHandler(isSaveToDbSeparately);
        handler.BeforeSaving += DbHandler.Handler;

        if (command.Exception is { } exc) {
            var act = async () => await handler.ExecuteAsync(command);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(exc.Message);
            return;
        }

        var result = await handler.ExecuteAsync(command);
        result.IsSucc.Should().Be(isSuccess);
        result.IfSucc(events => events
            .Should()
            .ContainSingle(predicate: e => e.Result == exceptedResult));
    }
}