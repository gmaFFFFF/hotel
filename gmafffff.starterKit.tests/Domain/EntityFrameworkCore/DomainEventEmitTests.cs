using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.starterKit.tests.Domain.EntityFrameworkCore.Fixtures;

namespace gmafffff.starterKit.tests.Domain.EntityFrameworkCore;

[TestSubject(typeof(IDomainEventEmitter))]
[TestSubject(typeof(DomainEventProcessor))]
public partial class DomainEventsTests {
    public class DomainEventEmit {
        private readonly SimpleEntity _entity;
        private readonly IDomainEventSink _eventSink;
        private readonly IServiceProvider _provider = Substitute.For<IServiceProvider>();

        public DomainEventEmit() {
            _eventSink = new DomainEventProcessor(new DomainEventHandlerFactory(_provider));
            _entity = new SimpleEntity(id: 1, "My test prop");
            ((IDomainEventEmitter)_entity).SetDomainEventSink(_eventSink);
        }

        /// <summary>
        ///     Сущность может отправлять события
        /// </summary>
        [Fact]
        public void EntityCanEmitEvents() {
            // Arrange
            var event1 = new SimpleDomainEvent(_entity);
            var event2 = new SimpleDomainEvent2(_entity);

            // Act
            ((IDomainEventEmitter<SimpleEntity>)_entity)
                .EmitDomainEvent(event1)
                .EmitDomainEvent(event2);

            // Assert
            _eventSink.Events
                .Should().Equal(event1, event2);
        }

        /// <summary>
        ///     Одинаковые события игнорируются
        /// </summary>
        [Fact]
        public void SameEventIsIgnored() {
            // Arrange
            var event1 = new SimpleDomainEvent(_entity);
            var event2 = new SimpleDomainEvent(_entity);

            // Act
            ((IDomainEventEmitter<SimpleEntity>)_entity)
                .EmitDomainEvent(event1)
                .EmitDomainEvent(event2);

            // Assert
            _eventSink.Events
                .Should().ContainSingle()
                .And.Equal(event2)
                .And.Subject.First().Should().BeSameAs(event1);
        }

        /// <summary>
        ///     Отсутствие приемника событий не нарушит работоспособность сущности
        /// </summary>
        [Fact]
        public void NoEventSinkNoEvents() {
            // Arrange
            var event1 = new SimpleDomainEvent(_entity);
            var event2 = new SimpleDomainEvent2(_entity);

            // Act        
            ((IDomainEventEmitter)_entity)
                .ResetDomainEventSink();
            ((IDomainEventEmitter<SimpleEntity>)_entity)
                .EmitDomainEvent(event1)
                .EmitDomainEvent(event2);

            // Assert
            _eventSink.Events
                .Should().BeEmpty();
        }
    }
}