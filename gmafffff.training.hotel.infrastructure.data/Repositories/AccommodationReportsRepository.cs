using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.training.hotel.domain.SettlementManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.SettlementManagement.Models;
using gmafffff.training.hotel.infrastructure.data.Sessions;

namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class AccommodationReportsRepository(HotelDbContext context) :
    Repository<AccommodationReport<Guid>, int>(context),
    IAccommodationReportsRepository<Guid> { }