# Обмен сообщениями: команды и запросы

# Общие сведения

Пространство имен: `gmafffff.starterKit.Messaging`.

Для общения между слоями приложения предусмотрен интерфейс-маркер сообщения `IMessage`,
а также более специализированные типы:

* `Message` — наиболее общее представление о сообщении;
* `BusinessCommand` — для отправки сообщений бизнес-слою;
* `Query` — для запроса, не изменяющего состояния управляемым приложением;
* `BusinessEvent` — события, возникающие в результате исполнения `BusinessCommand`.

# Примитивный CRUD

Для некоторых часто встречающихся команд/событий/запросов предусмотрены стандартные сообщения:
Пространство имен: `gmafffff.starterKit.Messaging.Crud`.

* `CreateBusinessCommand` / `CreatedBusinessEvent` — создания сущности.
* `UpdateBusinessCommand` / `UpdatedBusinessEvent` — изменение сущности.
* `DeleteBusinessCommand` / `DeletedBusinessEvent` — удаления сущности.
* `ReadDbQuery` — запрос сущностей.