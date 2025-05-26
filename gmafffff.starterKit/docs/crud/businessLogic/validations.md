# Валидация (форматно-логический контроль)

## Общие сведения

Стартовый набор обеспечивает интеграцию [ошибок приложения](./errors.md)
и библиотеки [Validot](https://github.com/bartoszlenar/Validot).

## Интеграция библиотеки Validot

Если нужна самопроверка типа, то вместо `System.ComponentModel.DataAnnotations.IValidatableObject`
унаследуйте его от `gmafffff.starterKit.Validation.IValidatableObjectHelperValidot`.

Определите коды и текстовое описание ошибок приложения, как описывается в разделе [ошибки приложения](./errors.md).

Запишите правила валидации в класс, производный от:

1. `gmafffff.starterKit.Validation.LocalizedSpecification` — обеспечивает автозагрузку локализованных на русский и
   английский язык сообщений об ошибка
2. 1…∞ `Validot.ISpecificationHolder<TValidableEntity>`, где TValidableEntity - проверяемая сущность.
   Если проверяемых сущностей будет несколько, то потребуется явная реализация интерфейса.

При определении спецификации вместо стандартных Validot методов описания ошибки `WithMessage` и `WithExtraCode`
используйте метод расширения `WithErrorCode`, который по Enum-коду ошибки внесёт в спецификацию:

* ключ к локализованному сообщению об ошибке (для человека);
* краткий строковый код ошибки (для парсеров);
* строковый код, позволяющий создать `LanguageExt.Common.Error`.

Зарегистрируйте в контейнере DI «проверяющих» (на основе спецификаций из классов, реализующих `ISpecificationHolder<T>`)
с помощью метода расширения `IServiceCollection` `AddValidotValidators`.

## Использование

Ручную проверку объекта можно проводить запросив у ServiceProvider'а проверяющего типа `IValidator<TValidableEntity>>`,
самопроверямые объекты проверяются в обычном порядке.

Результат проверки `Validot.Results.IValidationResult` при необходимости можно преобразовать
в ошибки приложения `LanguageExt.Common.Error` с помощью метода расширения `ToExceptedError`.