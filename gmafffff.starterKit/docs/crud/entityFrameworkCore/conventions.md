# Конвенции Ef Core

## Общие сведения

Пространство имён: `gmafffff.starterKit.EntityFrameworkCore.Conventions`.

`PrimaryKeyNameConventionIsEntityNameId`
устанавливает наименование столбца первичного ключа по шаблону `{EntityName}{Id}`.
Принадлежащие сущности не поддерживаются.

`ForeignKeyNameConventionIsParentEntityNameParentColumnName`
добавляет название родительской таблицы к столбцу внешнего ключа.

## Использование

Переопределить в производном классе метод `Microsoft.EntityFrameworkCore.DbContext.ConfigureConventions`:

```
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) {
    configurationBuilder.Conventions.Add(_ => new PrimaryKeyNameConventionIsEntityNameId());
    configurationBuilder.Conventions.Add(_ => new ForeignKeyNameConventionIsParentEntityNameParentColumnName());
}
```
