# Ошибки приложения

## Общие сведения

Для передачи пользователю информации об ошибке использован
тип [`LanguageExt.Common.Error`](https://louthy.github.io/language-ext/LanguageExt.Core/Common/index.html),
который учитывает, что ошибки бывают:

* Exceptional — неожиданные, например, «OutOfMemoryException»;
* Expected — ожидаемые, например «Пользователь не найден»;
* ManyErrors — много ошибок (возможно ноль).

LanguageExt не определяет способ совместного хранения кодов ошибок и соответствующих им локализованных сообщений.
Класс `gmafffff.starterKit.AppError.AppErrorHelper` решает данную проблему.

## `gmafffff.starterKit.AppError.AppErrorHelper`

Вспомогательный статический класс, помогающий создавать (метод `NewError`) ожидаемые ошибки с локализованными
сообщениями по их enum-коду.
Коды ошибок хранятся в пользовательских Enum'ах, а локализованные сообщения в типах,
реализующих `gmafffff.starterKit.AppError.IErrorMessage<TEnumErrorCode>`.

## Использование

Пользовательские ошибки создаются по аналогии со встроенными — `gmafffff.starterKit.AppError.AppErrorCode`,
так же как и локализованные сообщения к ним — `gmafffff.starterKit.AppError.AppErrorMessages`.

Для этого необходимо:

* определить 1…∞ Enum с кодами ошибок, например, ошибки валидации.
* сохранить локализованные сообщения об ошибке для каждого Enum-кода ошибки в классе, реализующем `IErrorMessage`.

Наверное когда-нибудь пригодится, если в каждом пользовательском Enum'е будет значение `None` с кодом 0.

[!CAUTION]
Не следует допускать пересечения числовых кодов ошибок в разных Enum'ах.
Число, присвоенное первой ошибке, может вычисляться, например,
как CRC-16 полного названия Enum-типа с помощью [Hash Generator](https://codebeautify.org/crc-16-hash-generator)

Ошибки (`LanguageExt.Common.Error`) создаются с помощью статических методов
`gmafffff.starterKit.AppError.AppErrorHelper.NewError(…)`, принимающих Enum-код ошибки.

[!IMPORTANT]
Механизм работает без DI.