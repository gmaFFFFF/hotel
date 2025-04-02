# Обмен сообщениями: команды и запросы

# Общие сведения

Для общения между слоями приложения предусмотрен интерфейс-маркер сообщения `gmafffff.starterKit.Messaging.IMessage`,
а также более специализированные типы:

* `gmafffff.starterKit.Messaging.Message` — наиболее общее представление о сообщении;
* `gmafffff.starterKit.Messaging.BusinessCommand` — для отправки сообщений бизнес-слою;
* `gmafffff.starterKit.Messaging.Query` — для запроса, не изменяющего состояния управляемым приложением;
* `gmafffff.starterKit.Messaging.BusinessEvent` — события, возникающие в результате исполнения
  `gmafffff.starterKit.Messaging.BusinessCommand`.

Для некоторых часто встречающихся команд/событий/запросов предусмотрены стандартные сообщения:

* `gmafffff.starterKit.Messaging.Crud.CreateBusinessCommand` / `CreatedBusinessEvent` — создания сущности.
* `gmafffff.starterKit.Messaging.Crud.UpdateBusinessCommand` / `UpdatedBusinessEvent` — изменение сущности.
* `gmafffff.starterKit.Messaging.Crud.DeleteBusinessCommand` / `DeletedBusinessEvent` — удаления сущности.
* `gmafffff.starterKit.Messaging.Crud.ReadDbQuery` — запрос сущностей.