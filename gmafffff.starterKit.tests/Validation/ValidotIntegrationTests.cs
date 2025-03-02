using System.ComponentModel.DataAnnotations;
using System.Globalization;
using gmafffff.starterKit.AppError;
using gmafffff.starterKit.tests.Validation.Fixtures;
using gmafffff.starterKit.Validation;
using LanguageExt.Common;
using Validot;
using Validator = System.ComponentModel.DataAnnotations.Validator;

namespace gmafffff.starterKit.tests.Validation;

[TestSubject(typeof(IValidatableObjectHelperValidot))]
[TestSubject(typeof(LocalizedSpecification))]
public class ValidotIntegrationTests {
    /// <summary>
    ///     Результаты проверки содержат локализованные сообщения
    /// </summary>
    [Fact]
    public void ValidationHasLocalization() {
        // Arrange
        var messages = new ValidationErrorCodeMessages().Messages;
        var vc = new ValidableClass { Age = 5, Name = "" };
        var validator = Validot.Validator.Factory.Create(new ValidableClassSpecHolder());

        // Act
        var result = validator.Validate(vc);

        // Assert
        result.GetTranslatedMessageMap(IErrorMessage<Enum>.Ru)[nameof(ValidableClass.Age)]
            .Should().ContainSingle()
            .Which.Should().Be(messages[IErrorMessage<Enum>.Ru][ValidationErrorCode.AgeLessThan18]);
        result.GetTranslatedMessageMap(IErrorMessage<Enum>.Ru)[nameof(ValidableClass.Name)]
            .Should().Contain(messages[IErrorMessage<Enum>.Ru][ValidationErrorCode.NameEmpty]);
    }

    /// <summary>
    ///     Поддерживается самопроверка объекта
    /// </summary>
    [Fact]
    public void SelfValidationSupported() {
        var currentCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");

        // Arrange
        var messages = new ValidationErrorCodeMessages().Messages;
        var vc = new ValidableClass { Age = 5, Name = "" };
        var sp = Substitute.For<IServiceProvider>();
        var validotValidator = Validot.Validator.Factory.Create(new ValidableClassSpecHolder());
        sp.GetService(typeof(IValidator<ValidableClass>)).Returns(validotValidator);

        // Act
        var context = new ValidationContext(vc, sp, items: null);
        var results = new List<ValidationResult>();

        // Assert
        Validator.TryValidateObject(vc, context, results,
                validateAllProperties: true)
            .Should().BeFalse();
        results.Should().Contain(predicate: v => v.MemberNames.Contains(nameof(ValidableClass.Age)))
            .Which.ErrorMessage.Should()
            .Be(messages[IErrorMessage<Enum>.Ru][ValidationErrorCode.AgeLessThan18]);
        results.Should().ContainSingle(predicate: v =>
                v.ErrorMessage == messages[IErrorMessage<Enum>.Ru][ValidationErrorCode.NameEmpty])
            .Which
            .MemberNames.Should().ContainSingle(nameof(ValidableClass.Name));


        CultureInfo.CurrentUICulture = currentCulture;
    }

    /// <summary>
    ///     Результат валидации можно конвертировать в ожидаемые ошибки приложения
    /// </summary>
    [Fact]
    public void ValidationResultCanBeConvertedToExpectedError() {
        var currentCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");

        // Arrange
        var messages = new ValidationErrorCodeMessages().Messages;
        var vc = new ValidableClass { Age = 5, Name = "" };
        var validator = Validot.Validator.Factory.Create(new ValidableClassSpecHolder());

        // Act
        var result = validator.Validate(vc).ToExceptedError();

        // Assert
        result.Count.Should().BeGreaterThanOrEqualTo(expected: 2);
        result.IsExpected.Should().BeTrue();
        ((ManyErrors)result).Errors
            .Select(f: error => error.Message)
            .AsEnumerable()
            .Should().ContainSingle(predicate: m =>
                m == messages[IErrorMessage<Enum>.Ru][ValidationErrorCode.NameEmpty]);


        CultureInfo.CurrentUICulture = currentCulture;
    }
}