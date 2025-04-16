using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace gmafffff.starterKit.EntityFrameworkCore.Conventions;

/// <summary>
///     Конвенция, которая устанавливает имя столбца первичного ключа по шаблону:
///     {EntityName}Id
/// </summary>
public class PrimaryKeyNameConventionIsEntityNameId(string idSuffix = PrimaryKeyNameConventionIsEntityNameId.IdSuffixEn)
    : IKeyAddedConvention {
    public const string IdSuffixEn = "Id";

    public void ProcessKeyAdded(IConventionKeyBuilder keyBuilder, IConventionContext<IConventionKeyBuilder> context) {
        var key = keyBuilder.Metadata;
        var keyProp = key.Properties[0];
        if (!key.IsPrimaryKey() ||
            !keyProp.GetColumnName().Contains(idSuffix, StringComparison.OrdinalIgnoreCase) ||
            // TODO: Лень разбираться в принадлежащих типах, поэтому игнорируем их
            key.DeclaringEntityType.IsOwned() ||
            key.Properties.Count > 1 ||
            keyProp.GetColumnNameConfigurationSource() == ConfigurationSource.Explicit ||
            (keyProp.GetConfigurationSource() == ConfigurationSource.Explicit &&
             !string.Equals(keyProp.GetColumnName(), "id", StringComparison.OrdinalIgnoreCase)))
            return;

        var colName = $"{key.DeclaringEntityType.ShortName()}{idSuffix}";

        if (keyProp.GetColumnName() != colName)
            keyProp.Builder.HasColumnName(colName);
    }
}