using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace gmafffff.starterKit.EntityFrameworkCore.Conventions;

/// <summary>
///     Конвенция, которая устанавливает имя столбца внешнего ключа по шаблону:
///     {ParentEntityName}{ParentColumnName}
/// </summary>
/// <remarks>Полезно для универсальных классов</remarks>
public class ForeignKeyNameConventionIsParentEntityNameParentColumnName :
    IForeignKeyAddedConvention,
    IForeignKeyPropertiesChangedConvention {
    public void ProcessForeignKeyAdded(IConventionForeignKeyBuilder foreignKeyBuilder,
        IConventionContext<IConventionForeignKeyBuilder> context) {
        SetColumnsNames(foreignKeyBuilder);
    }

    public void ProcessForeignKeyPropertiesChanged(IConventionForeignKeyBuilder relationshipBuilder,
        IReadOnlyList<IConventionProperty> oldDependentProperties,
        IConventionKey oldPrincipalKey, IConventionContext<IReadOnlyList<IConventionProperty>> context) {
        SetColumnsNames(relationshipBuilder);
    }

    private void SetColumnsNames(IConventionForeignKeyBuilder foreignKeyBuilder) {
        var fk = foreignKeyBuilder.Metadata;

        if (fk.PrincipalEntityType.IsOwned() ||
            fk.DeclaringEntityType == fk.PrincipalEntityType ||
            // Для комплексных свойств
            fk.PrincipalEntityType.FindPrimaryKey() is null)
            return;

        var principalName = fk.PrincipalEntityType.ShortName();
        for (var i = 0; i < fk.Properties.Count; i++) {
            var fkProp = fk.Properties[i];
            if (fkProp.GetColumnNameConfigurationSource() == ConfigurationSource.Explicit)
                continue;

            var principalProp = fk.PrincipalKey.Properties[i];
            var keyColName = principalProp.GetColumnName();

            var colName = keyColName.StartsWith(principalName)
                ? keyColName
                : $"{principalName}{keyColName}";

            if (fkProp.GetColumnName() != colName)
                fkProp.Builder.HasColumnName(colName);
        }
    }
}