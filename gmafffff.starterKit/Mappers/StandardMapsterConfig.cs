using Mapster;

namespace gmafffff.starterKit.Mappers;

/// <summary>
///     Класс, навязывающий политику проверки генерируемых Mapster преобразований
/// </summary>
public abstract class StandardMapsterConfig : IRegister {
    protected const MapType All = MapType.Map | MapType.MapToTarget | MapType.Projection;
    protected const MapType Instance = MapType.Map | MapType.MapToTarget;

    void IRegister.Register(TypeAdapterConfig config) {
        // Конфигурация должна охватывать все свойства назначаемого типа
        config.RequireDestinationMemberSource = true;

        Configure(config);

        // Проверить конфигурацию
        config.Compile();
        config.CompileProjection();
    }

    /// <summary>
    ///     Переопределите в производном классе вместо <see cref="Mapster.IRegister" />
    /// </summary>
    protected abstract void Configure(TypeAdapterConfig config);
}