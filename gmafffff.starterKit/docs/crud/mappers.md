# Автоматическое преобразование сущность ⟷ DTO

## Общие сведения

Для единообразного преобразования сущностей в DTO и обратно объявлена группа интерфейсов:

* `gmafffff.starterKit.Mappers.IEntityMapperBackward` — DTO → сущность;
* `gmafffff.starterKit.Mappers.IEntityMapperForward` — сущность → DTO;
* `gmafffff.starterKit.Mappers.IEntityMapperDuplex` — сущность ⇆ DTO;
* `gmafffff.starterKit.Mappers.IEntityMapperForwardExpression` — Expression для проекции сущность → DTO;
* `gmafffff.starterKit.Mappers.IEntityMapper` — интерфейс-маркер.

## Использование

Реализация интерфейсов предполагается с помощью кодогенерации от Mapster.
Основная опасность использования авто преобразователей типов — скрытие факта неполной трансформации объекта.
Для навязывания политики предварительной проверки конфигурации вместо использования интерфейса `Mapster.IRegister`
конфигурацию преобразователя следует помещать в метод `Configure` класса,
производного от `gmafffff.starterKit.Mappers.StandardMapsterConfig`.

Регистрация преобразователей в контейнере DI и регистрация конфигураций преобразователей осуществляется
вызовом `AddEntityMappers` — метода расширения `IServiceCollection`.


