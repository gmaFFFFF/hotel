# Обработка бизнес-логики

## Схема работы

Механизм обработки бизнес-команд
`gmafffff.starterKit.BusinessLogic.BusinessActionRunner<TCommand>`
автоматически запускает:

* Валидацию `Validot.ISpecificationHolder<TValidableEntity>`.
* Проверку бизнес-правил `gmafffff.starterKit.BusinessLogic.IBusinessRule`.
* Запуск обработчика команды `gmafffff.starterKit.BusinessLogic.IBusinessCommandHandler`.
* Запуск обработчиков доменных событий `gmafffff.starterKit.Domain.Events.DomainEvent<T>`
* Запуска обработчиков команд в ответ на сигнальные события `gmafffff.starterKit.Messaging.TriggerEvent`.
* Перехват исключений `OperationCanceledException` и `DbUpdateConcurrencyException`
  для преобразования их в монаду `LanguageExt.Fin`

![Принципиальная схема обработки команды](https://www.plantuml.com/plantuml/svg/XLAzRXGn4Exz56PfxXHTGncX42MGaYA9gAwnzeQBXTTUxF45gFWHrRq3l4A2G888ZbVmlX6purrpIyCLUvwPRxxvx6-E0ogFiJ7YNklLkb4ImRsHw2C8eMfqNfuQ3HQb2hBcasYfGzUwLPGzZzQ2xui-Mb4xYzuPMLrwPGErQsUNLeRQjL3r8AJyfF-PCdgbq375fW-8n1ZWrY2hanYqXH2MjgQOshN5m5Gvdf1Lfwvfb5qjxHLz3NW63cT1qZM-bjKJ_kSIfSJB214ekw5WQI-qVS6bG8Sf-MFNq7G96RZIeLLONmr3nd2gVj8o4GL3CCMVG-k2HkUpF4_X573fzNeDNgAZPostnVwrZKANkSSGflgVmlNIPh-gmR07GcHFvUAeZ-N1WJoKy0hgY32hC_zS22eHWea9Sl_ix8H-XBHmBSJQkzZAzBbxctRfI_f6_tVfPknXJmcg0rlzQCnd-xXmeL27vnge27PL23-bMwAwIRTfrxtlAILzTkadfNwbhzrsO1hz9M6BnL_rOamJ0YnI8GOG-QfiRWzjD6Ppzj35IWAK8uE2AB5xk3XoJ5DoF6YaEHnKpRXnlaVFfb8zgCsUnlByV0KF6BsLl9914aOEll_dVRftwNVtWRvtlHFTjllOftQ4snBYU_fHZ1mtX8QLymppaBaegIAiQA8b6lSk36oyI7jhGqin8EQiTtyemMYhMWC8OWMgHhsXCLmI8e_uz_xYc4XYO_u0 "Принципиальная схема обработки команды")

## DI

Регистрация в контейнере DI осуществляется во время вызова
метода расширения `IServiceCollection.AddBusinessLogic`.
