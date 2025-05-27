using FluentAssertions.Execution;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.starterKit.tests.Domain.EntityFrameworkCore.Fixtures;

namespace gmafffff.starterKit.tests.Domain.EntityFrameworkCore;

[TestSubject(typeof(DomainEventProcessor))]
[TestSubject(typeof(Repository<,>))]
public partial class DomainEventsTests {
    public class DomainEventSink {
        private readonly SimpleDbContext _context;
        private readonly IDomainEventSink _eventSink;
        private readonly IServiceProvider _provider = Substitute.For<IServiceProvider>();

        public DomainEventSink() {
            _eventSink = new DomainEventProcessor(_provider);
            _context = new SimpleDbContext(nameof(DomainEventsTests) + nameof(DomainEventSink));
        }

        /// <summary>
        ///     Зарегистрированный DbContext подключает приемник событий к добавляемым сущностям
        /// </summary>
        [Fact]
        public void RegisteredDbContextConnectEventEmitterToAddedEntities() {
            // Arrange
            var entity = Substitute.For<SimpleEntity>(0, "Новая сущность");
            _eventSink.RegisterDbContext(_context);

            // Act, Assert
            using var _ = new AssertionScope();

            _context.SimpleEntities.Add(entity);
            ((IDomainEventEmitter<SimpleEntity>)entity).Received().SetDomainEventSink(_eventSink);

            _context.SimpleEntities.Remove(entity);
            ((IDomainEventEmitter<SimpleEntity>)entity).Received().ResetDomainEventSink();
        }

        /// <summary>
        ///     Зарегистрированный DbContext подключает приемник событий к загружаемым сущностям
        /// </summary>
        [Fact]
        public void RegisteredDbContextConnectEventEmitterToLoadedEntities() {
            var id = nameof(RegisteredDbContextConnectEventEmitterToLoadedEntities).GetHashCode();
            // Arrange
            SimpleEntity entity = new(id, "Сущность из БД");
            _context.Add(entity);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _eventSink.RegisterDbContext(_context);

            // Act, Assert
            using var _ = new AssertionScope();

            entity = _context.SimpleEntities.Single(e => e.Id == id);
            entity.DomainEventSink.Should().Be(_eventSink);

            _context.SimpleEntities.Remove(entity);
            _context.SaveChanges();
            entity.DomainEventSink.Should().BeNull();
        }


        /// <summary>
        ///     Зарегистрированный DbContext подключает приемник событий к присоединенным сущностям
        /// </summary>
        [Fact]
        public void RegisteredDbContextConnectEventEmitterToAttachedEntities() {
            var id = nameof(RegisteredDbContextConnectEventEmitterToAttachedEntities).GetHashCode();
            // Arrange
            SimpleEntity entity = new(id, "Присоединенная сущность");
            _context.Add(entity);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            var entityProxy = Substitute.For<SimpleEntity>(id, "Присоединенная сущность");
            _eventSink.RegisterDbContext(_context);


            // Act, Assert
            using var _ = new AssertionScope();

            _context.SimpleEntities.Attach(entityProxy);
            ((IDomainEventEmitter<SimpleEntity>)entityProxy).Received().SetDomainEventSink(_eventSink);

            _context.SimpleEntities.Remove(entityProxy);
            _context.SaveChanges();
            ((IDomainEventEmitter<SimpleEntity>)entityProxy).Received().ResetDomainEventSink();
        }

        /// <summary>
        ///     Репозиторий автоматически регистрирует DbContext
        /// </summary>
        [Fact]
        public void RepositoryAutoRegisterDbContext() {
            // Arrange
            var eventSink = Substitute.For<IDomainEventSink>();
            // Act
            var repo = new SimpleRepo(_context, eventSink);

            // Assert
            eventSink.Received().RegisterDbContext(_context);
        }
    }
}