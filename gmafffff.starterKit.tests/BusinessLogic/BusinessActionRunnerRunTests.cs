using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.starterKit.Messaging;
using gmafffff.starterKit.tests.BusinessLogic.Fixtures;
using gmafffff.starterKit.tests.Validation.Fixtures;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using Validot;
using Validot.Results;

namespace gmafffff.starterKit.tests.BusinessLogic;

[TestSubject(typeof(BusinessActionRunner<>))]
public partial class BusinessActionRunnerTests {
    public class BusinessActionRunnerRun {
        private readonly IBusinessConstraintCheck<BusinessActionCommand> _exceptionCheck1 =
            Substitute.For<IBusinessConstraintCheck<BusinessActionCommand>>();

        private readonly IBusinessConstraintCheck<BusinessActionCommand> _exceptionCheck2 =
            Substitute.For<IBusinessConstraintCheck<BusinessActionCommand>>();

        private readonly IBusinessConstraintCheck<BusinessActionCommand> _failCheck1 =
            Substitute.For<IBusinessConstraintCheck<BusinessActionCommand>>();

        private readonly IBusinessConstraintCheck<BusinessActionCommand> _failCheck2 =
            Substitute.For<IBusinessConstraintCheck<BusinessActionCommand>>();

        private readonly IBusinessCommandHandler<BusinessActionCommand> _handler =
            Substitute.For<IBusinessCommandHandler<BusinessActionCommand>>();

        private readonly BusinessActionCommand _notValidCommand = new(false);
        private readonly IServiceProvider _provider = Substitute.For<IServiceProvider>();

        private readonly BusinessActionRunner<BusinessActionCommand> _runner;

        private readonly IBusinessConstraintCheck<BusinessActionCommand> _successCheck1 =
            Substitute.For<IBusinessConstraintCheck<BusinessActionCommand>>();

        private readonly IBusinessConstraintCheck<BusinessActionCommand> _successCheck2 =
            Substitute.For<IBusinessConstraintCheck<BusinessActionCommand>>();

        private readonly IValidator<BusinessActionCommand> _validator =
            Substitute.For<IValidator<BusinessActionCommand>>();

        private readonly BusinessActionCommand _validCommand = new(true);

        private readonly DomainEventProcessor _domainEventProcessor;

        public BusinessActionRunnerRun() {
            // Форматно-логический контроль
            _validator.IsValid(Arg.Any<BusinessActionCommand>())
                .ReturnsForAnyArgs(a => a.Arg<BusinessActionCommand>().IsCorrect);
            _validator.Validate(Arg.Any<BusinessActionCommand>())
                .Returns(GetFakeValidationResult());


            // Бизнес-ограничения
            _successCheck1.IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>())
                .Returns(true);
            _successCheck2.IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>())
                .Returns(true);

            _failCheck1.IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>()).Returns(false);
            _failCheck1.ErrorCode.Returns(ValidationErrorCode.ValidationRoomCapacityNo);
            _failCheck2.IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>()).Returns(false);
            _failCheck2.ErrorCode.Returns(ValidationErrorCode.ValidationRoomNumberNo);

            _exceptionCheck1.IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>())
                .ThrowsAsync(new Exception(nameof(_exceptionCheck1)));
            _exceptionCheck2.IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>())
                .ThrowsAsync(new Exception(nameof(_exceptionCheck2)));


            // Обработчик команды
            _handler.ExecuteAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>())
                .Returns(Fin<IList<BusinessEvent>>.Succ([new BusinessActionResult(_validCommand)]));

            // Обработчик событий домена
            _domainEventProcessor = new (_provider);

            // Контейнер служб
            _provider.GetService(typeof(IValidator<BusinessActionCommand>))
                .Returns(_validator);
            _provider.GetService(typeof(IBusinessCommandHandler<BusinessActionCommand>))
                .Returns(_handler);
            _provider.GetService(typeof(IDomainEventDispatcher))
                .Returns(_domainEventProcessor);

            _runner = new BusinessActionRunner<BusinessActionCommand>(_provider);
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
        ///     Невозможно выполнить действие, если нарушается хотя бы одно бизнес-ограничение
        /// </summary>
        [Fact]
        public async Task ImpossibleToPerformActionWithViolationBusinessConstraintAsync() {
            // Arrange
            IBusinessConstraintCheck<BusinessActionCommand>[] checks =
                [_successCheck1, _successCheck2, _failCheck1, _failCheck2];
            _provider.Configure().GetService(typeof(IEnumerable<IBusinessConstraintCheck<BusinessActionCommand>>))
                .Returns(checks);
            _runner.ContinueCheckBusinessConstraintsAfterFirstError = false;

            // Act
            var result = await _runner.Execute(_validCommand);

            // Assert
            result.IsFail.Should().BeTrue();
            result.IfFail(e =>
                e.Code.Should()
                    .Be((int)Convert.ChangeType(_failCheck1.ErrorCode, _failCheck1.ErrorCode.GetTypeCode())));
            _validator.Received().IsValid(Arg.Any<BusinessActionCommand>());
            await _successCheck1.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
            await _successCheck2.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
            await _failCheck1.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
            await _failCheck2.DidNotReceiveWithAnyArgs()
                .IsSatisfiedAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>());
        }

        /// <summary>
        ///     Собирает информацию обо всех нарушенных бизнес-ограничениях
        /// </summary>
        [Fact]
        public async Task CollectsInformationAboutAllViolatedBusinessConstraintsAsync() {
            // Arrange
            IBusinessConstraintCheck<BusinessActionCommand>[] checks =
                [_successCheck1, _successCheck2, _failCheck1, _failCheck2, _exceptionCheck1, _exceptionCheck2];
            _provider.Configure().GetService(typeof(IEnumerable<IBusinessConstraintCheck<BusinessActionCommand>>))
                .Returns(checks);
            _runner.ContinueCheckBusinessConstraintsAfterFirstError = true;

            // Act
            var result = await _runner.Execute(_validCommand);

            // Assert
            result.IsFail.Should().BeTrue();
            result.IfFail(ee => ee.AsIterable()
                .AsEnumerable()
                .Should()
                .Contain(e =>
                    e.Code == (int)Convert.ChangeType(_failCheck1.ErrorCode, _failCheck1.ErrorCode.GetTypeCode()))
                .And
                .Contain(e =>
                    e.Code == (int)Convert.ChangeType(_failCheck1.ErrorCode, _failCheck2.ErrorCode.GetTypeCode()))
                .And
                .Contain(e =>
                    e.IsExceptional && e.Exception.Map(exc => exc.Message == nameof(_exceptionCheck1)).IfNone(false))
                .And
                .Contain(e =>
                    e.IsExceptional && e.Exception.Map(exc => exc.Message == nameof(_exceptionCheck2)).IfNone(false))
            );
            _validator.Received().IsValid(Arg.Any<BusinessActionCommand>());
            await _successCheck1.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
            await _successCheck2.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
            await _failCheck1.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
            await _failCheck2.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
        }

        /// <summary>
        ///     Запускает успешное бизнес-действие
        /// </summary>
        [Fact]
        public async Task RunSuccessActionAsync() {
            // Arrange
            IBusinessConstraintCheck<BusinessActionCommand>[] checks = [_successCheck1, _successCheck2];
            _provider.Configure().GetService(typeof(IEnumerable<IBusinessConstraintCheck<BusinessActionCommand>>))
                .Returns(checks);

            // Act
            var result = await _runner.Execute(_validCommand);

            // Assert
            result.IsSucc.Should().BeTrue();
            _validator.Received().IsValid(Arg.Any<BusinessActionCommand>());
            await _successCheck1.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
            await _successCheck2.Received().IsSatisfiedAsync(_validCommand, Arg.Any<CancellationToken>());
            await _handler.Received().ExecuteAsync(Arg.Any<BusinessActionCommand>(), Arg.Any<CancellationToken>());
        }
    }
}