using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.training.hotel.domain.PropertyManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.infrastructure.data.Sessions;

namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class RoomRepositoryReadOnly(HotelDbContext dbContext) : RepositoryReadOnly<Room<Guid>, int>(dbContext,
        autoInclude: static room =>
            room.Include(r => r.RoomCleanings.AsQueryable().Where(cleaning => !cleaning.IsClean))),
    IRoomRepositoryReadOnly;