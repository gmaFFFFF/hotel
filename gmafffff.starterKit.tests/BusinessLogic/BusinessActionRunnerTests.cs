using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.tests.BusinessLogic.Fixtures;
using gmafffff.starterKit.tests.Validation.Fixtures;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using Validot;
using Validot.Results;

namespace gmafffff.starterKit.tests.BusinessLogic;

[TestSubject(typeof(BusinessActionRunner<,>))]
public class BusinessActionRunnerTests {
    private readonly IBusinessRule<BusinessActionCommand> _exceptionRule1 =
        Substitute.For<IBusinessRule<BusinessActionCommand>>();

    private readonly IBusinessRule<BusinessActionCommand> _exceptionRule2 =
        Substitute.For<IBusinessRule<BusinessActionCommand>>();

    private readonly IBusinessRule<BusinessActionCommand> _failRule1 =
        Substitute.For<IBusinessRule<BusinessActionCommand>>();

    private readonly IBusinessRule<BusinessActionCommand> _failRule2 =
        Substitute.For<IBusinessRule<BusinessActionCommand>>();

    private readonly IBusinessCommandHandler<BusinessActionCommand, BusinessActionResult> _handler =
        Substitute.For<IBusinessCommandHandler<BusinessActionCommand, BusinessActionResult>>();

    private readonly BusinessActionCommand _notValidCommand = new(false);
    private readonly IServiceProvider _provider = Substitute.For<IServiceProvider>();

    private readonly BusinessActionRunner<BusinessActionCommand, BusinessActionResult> _runner;

    private readonly IBusinessRule<BusinessActionCommand> _successRule1 =
        Substitute.For<IBusinessRule<BusinessActionCommand>>();

    private readonly IBusinessRule<BusinessActionCommand> _successRule2 =
        Substitute.For<IBusinessRule<BusinessActionCommand>>();

    private readonly IValidator<BusinessActionCommand> _validator =
        Substitute.For<IValidator<BusinessActionCommand>>();

    private readonly BusinessActionCommand _validCommand = new(true);

    public BusinessActionRunnerTests() {
        // Валидатор
        _validator.IsValid(Arg.Any<BusinessActionCommand>())
            .ReturnsForAnyArgs(a => a.Arg<BusinessActionCommand>().IsCorrect);
        _validator.Validate(Arg.Any<BusinessActionCommand>())
            .Returns(GetFakeValidationResult());


        // Бизнес-правила
        _successRule1.IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>()).Returns(true);
        _successRule2.IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>()).Returns(true);

        _failRule1.IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>()).Returns(false);
        _failRule1.ErrorCode.Returns(ValidationErrorCode.ValidationRoomCapacityNo);
        _failRule2.IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>()).Returns(false);
        _failRule2.ErrorCode.Returns(ValidationErrorCode.ValidationRoomNumberNo);

        _exceptionRule1.IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception(nameof(_exceptionRule1)));
        _exceptionRule2.IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception(nameof(_exceptionRule2)));


        // Обработчик команды
        _handler.ExecuteAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>())
            .Returns(Fin<IList<BusinessActionResult>>.Succ([new BusinessActionResult(_validCommand)]));


        // Контейнер служб
        _provider.GetService(typeof(IValidator<BusinessActionCommand>))
            .Returns(_validator);
        _provider.GetService(typeof(IBusinessCommandHandler<BusinessActionCommand, BusinessActionResult>))
            .Returns(_handler);


        _runner = new BusinessActionRunner<BusinessActionCommand, BusinessActionResult>(_provider);
    }

    /// <summary>
    ///     Возвращает результат поддельной валидации
    /// </summary>
    private IValidationResult GetFakeValidationResult() {
        // Заимствуем готовые тестовые данные
        var vc = new ValidableClass { Age = 5, Name = "" };
        var validator = Validator.Factory.Create(new ValidableClassSpecHolder());
        return validator.Validate(vc);
    }

    /// <summary>
    ///     Невозможно выполнить действие с некорректной моделью
    /// </summary>
    [Fact]
    public async Task ImpossibleToPerformActionWithIncorrectModelAsync() {
        // Arrange
        _runner.IncludeFormalValidationError = false;

        // Act
        var result = await _runner.Execute(_notValidCommand);

        // Assert
        result.IsFail.Should().BeTrue();
        result.IfFail(e => e.Code.Should().Be((int)AppErrorCode.ValidationGeneral));
        _validator.Received().IsValid(Arg.Any<BusinessActionCommand>());
    }

    /// <summary>
    ///     Может подробно информировать об ошибках модели
    /// </summary>
    [Fact]
    public async Task CanInformInDetailAboutErrorOfModelAsync() {
        // Arrange
        _runner.IncludeFormalValidationError = true;

        // Act
        var result = await _runner.Execute(_notValidCommand);

        // Assert
        result.IsFail.Should().BeTrue();
        result.IfFail(e => e.Should().Be(GetFakeValidationResult().ToExceptedError()));
        _validator.Received().IsValid(Arg.Any<BusinessActionCommand>());
    }

    /// <summary>
    ///     Невозможно выполнить действие, если нарушается хотя бы одно бизнес-правило
    /// </summary>
    [Fact]
    public async Task ImpossibleToPerformActionWithViolationBusinessRuleAsync() {
        // Arrange
        IBusinessRule<BusinessActionCommand>[] rules = [_successRule1, _successRule2, _failRule1, _failRule2];
        _provider.Configure().GetService(typeof(IEnumerable<IBusinessRule<BusinessActionCommand>>)).Returns(rules);
        _runner.ContinueValidateBusinessRulesAfterFirstError = false;

        // Act
        var result = await _runner.Execute(_validCommand);

        // Assert
        result.IsFail.Should().BeTrue();
        result.IfFail(e =>
            e.Code.Should().Be((int)Convert.ChangeType(_failRule1.ErrorCode, _failRule1.ErrorCode.GetTypeCode())));
        _validator.Received().IsValid(Arg.Any<BusinessActionCommand>());
        await _successRule1.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
        await _successRule2.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
        await _failRule1.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
        await _failRule2.DidNotReceiveWithAnyArgs()
            .IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    ///     Собирает информацию обо всех нарушенных бизнес-правилах
    /// </summary>
    [Fact]
    public async Task CollectsInformationAboutAllViolatedBusinessRulesAsync() {
        // Arrange
        IBusinessRule<BusinessActionCommand>[] rules =
            [_successRule1, _successRule2, _failRule1, _failRule2, _exceptionRule1, _exceptionRule2];
        _provider.Configure().GetService(typeof(IEnumerable<IBusinessRule<BusinessActionCommand>>)).Returns(rules);
        _runner.ContinueValidateBusinessRulesAfterFirstError = true;

        // Act
        var result = await _runner.Execute(_validCommand);

        // Assert
        result.IsFail.Should().BeTrue();
        result.IfFail(ee => ee.AsIterable()
            .AsEnumerable()
            .Should()
            .Contain(e => e.Code == (int)Convert.ChangeType(_failRule1.ErrorCode, _failRule1.ErrorCode.GetTypeCode()))
            .And
            .Contain(e => e.Code == (int)Convert.ChangeType(_failRule1.ErrorCode, _failRule2.ErrorCode.GetTypeCode()))
            .And
            .Contain(e =>
                e.IsExceptional && e.Exception.Map(exc => exc.Message == nameof(_exceptionRule1)).IfNone(false))
            .And
            .Contain(e =>
                e.IsExceptional && e.Exception.Map(exc => exc.Message == nameof(_exceptionRule2)).IfNone(false))
        );
        _validator.Received().IsValid(Arg.Any<BusinessActionCommand>());
        await _successRule1.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
        await _successRule2.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
        await _failRule1.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
        await _failRule2.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
    }

    /// <summary>
    ///     Запускает успешное бизнес-действие
    /// </summary>
    [Fact]
    public async Task RunSuccessActionAsync() {
        // Arrange
        IBusinessRule<BusinessActionCommand>[] rules = [_successRule1, _successRule2];
        _provider.Configure().GetService(typeof(IEnumerable<IBusinessRule<BusinessActionCommand>>)).Returns(rules);

        // Act
        var result = await _runner.Execute(_validCommand);

        // Assert
        result.IsSucc.Should().BeTrue();
        _validator.Received().IsValid(Arg.Any<BusinessActionCommand>());
        await _successRule1.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
        await _successRule2.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
        await _handler.Received().ExecuteAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>());
    }
}