using System.ComponentModel.DataAnnotations.Schema;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Domain.Events;

namespace gmafffff.starterKit.tests.Domain.EntityFrameworkCore.Fixtures;

internal class SimpleEntity(int id, string property = "") : Entity<int>(id), IDomainEventEmitter<SimpleEntity> {
    public string? Property { get; set; } = property;

    [NotMapped] public virtual IDomainEventSink? DomainEventSink { get; set; }

    /// <summary>
    ///     Подключить приемник событий предметной области
    /// </summary>
    /// <remarks> Метод интерфейса переопределён, т.к. NSubstitute не видел вызов default-метода интерфейса</remarks>
    public virtual IDomainEventEmitter SetDomainEventSink(IDomainEventSink domainEventSink) {
        DomainEventSink = domainEventSink;
        return this;
    }

    /// <summary>
    ///     Отключить приемник событий предметной области
    /// </summary>
    /// <remarks> Метод интерфейса переопределён, т.к. NSubstitute не видел вызов default-метода интерфейса</remarks>
    public virtual IDomainEventEmitter ResetDomainEventSink() {
        DomainEventSink = null;
        return this;
    }
}