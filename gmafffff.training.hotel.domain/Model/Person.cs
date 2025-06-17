using gmafffff.starterKit.Domain;

namespace gmafffff.training.hotel.domain.Model;

/// <summary>
///     Посетитель
/// </summary>
public class Person<TId> : Entity<TId> where TId : struct, IEquatable<TId> {
    /// <summary>
    ///     ФИО
    /// </summary>
    public PersonFullName FullName { get; set; }

    /// <summary>
    ///     История проживания в гостинице
    /// </summary>
    public VisitorHistory History { get; set; }
}