# Обработка бизнес-логики

Механизм обработки бизнес-команд. Включает:

* Валидацию `Validot.ISpecificationHolder<TValidableEntity>`.
* Проверку бизнес-правил `gmafffff.starterKit.BusinessLogic.IBusinessRule`.
* Запуск обработчика команды `gmafffff.starterKit.BusinessLogic.IBusinessCommandHandler`.
* Запуска обработчиков команд в ответ на сигнальные события `gmafffff.starterKit.Messaging.TriggerEvent`.
* Перехватывает исключения `OperationCanceledException` и `DbUpdateConcurrencyException`,
  преобразуя их в монаду `LanguageExt.Fin`

## DI

Определен метод расширения `IServiceCollection.AddBusinessActionRunner`
для регистрации в контейнере DI трансляторов сигнальных событий в команды.