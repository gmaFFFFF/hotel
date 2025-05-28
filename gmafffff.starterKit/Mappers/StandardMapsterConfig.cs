using Mapster;

namespace gmafffff.starterKit.Mappers;

/// <summary>
///     Класс-помощник, стандартизующий ключи кодогенерации Mapster
/// </summary>
/// <remarks>
/// 
/// </remarks>
public abstract class StandardMapsterConfig : IRegister {
    protected const MapType All = MapType.Map | MapType.MapToTarget | MapType.Projection;
    protected const MapType Instance = MapType.Map | MapType.MapToTarget;

    public abstract void Register(TypeAdapterConfig config);
}