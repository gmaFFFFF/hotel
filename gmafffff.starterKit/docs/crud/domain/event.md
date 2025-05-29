# События домена

## Общие сведения

Пространство имен: `gmafffff.starterKit.Domain.Events`.

Для обмена сообщениями внутри ограниченного контекста служат события домена `DomainEvent<T>`.

[!TIP]
Если обработка события предполагается в разных ограниченных контекстах,
то следует использовать сигнальные события `gmafffff.starterKit.Messaging.TriggerEvent`.

Классы обработчики событий должны реализовать интерфейс `IDomainEventHandler<T>`.
В ходе обработки могут выпускаться новые события.

Обработка дочерних событий осуществляется последовательно «в ширину».

## Схема работы

События генерируют сущности, реализующие интерфейс `IDomainEventEmitter<TEntity>`.
Приемник событий подключается автоматически после начала отслеживания сущности в `DbContext`.
Необходимую настройку `DbContext` выполняет
`gmafffff.starterKit.EntityFrameworkCore.Repository`.

![Схема подключения сборщика событий](https://www.plantuml.com/plantuml/svg/bPB1RjGm48RlVehPssv3Tk-HjggqxHsnN3auzcZNQc87Ux9GJWKE227n1e1uWrQYKbj4dy5v8-mOXNIN2oV7_yna_kzFJXnnIsrTiKbZ-QRcG6ZU0DaM6HEcgb2GCjh16krE2NngpKv9jC0Tk8QzRB55EDkXfhNIvxqQJjPmIqgeXck2qt9bDE5hYbLPCic9bzmXFCF6E4N6NiIA3JU6cXHTm5awWXe4naNeWLMbmYLgedzYRBa4_yLVTM-xTzsbV_1t_jvVmNB9N0FvSMy4ZewW08slaXRMxojnxQ1D1v8dVstF68jDmokph0XJ6-K8xNJKrHVJZW9SMpfXLOFJvvQBSvIFwda-xbWJ9rnjkTx4jiJrBV3y23m_ky_T1t_ZV_bRVmF-7WBj--vZk3z4RdyRUUVpEKkX34G5j0u7txBCCeYkfR8eYFKMZarCDx0NmALSS28kjelASCbImy2Rvs7Ow3ya0qYSxaiEwTJKNEaXlYbjbPibbE_-EfYz3Cu3qtNtARhsTtk-5uly1GGZWjIE4uvmuf5eaf1C5O2r6c_z_udaxckmin_pFp8-p5TYdvV42WcdwO-TZH8FkHqa3YcwFClwGCSV_b6gamd4sSDS6Jj1BTkw-Wq0 "Схема подключения сборщика событий")

## Использование

* Унаследуй сущность от `IDomainEventEmitter<TEntity>`.
* Начни отслеживать её с помощью `DbContext` или вручную добавьте приемник событий `IDomainEventSink`.
* Добавь обработчики события `IDomainEventHandler<T>`.

Одному событию может соответствовать множество обработчиков.

## DI

Методы расширения `IServiceCollection` используются для регистрации в контейнере DI:

* `AddDomainEventProcessor` — базовой реализации `IDomainEventSink` и `IDomainEventDispatcher`;
* `AddDomainEventHandlers` — пользовательских обработчиков событий `IDomainEventHandler<T>`.
