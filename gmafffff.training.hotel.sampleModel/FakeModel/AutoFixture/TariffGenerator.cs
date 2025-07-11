using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.domain.SettlementManagement.Models;

namespace gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;

public class TariffGenerator : ISpecimenBuilder {
    /// <summary>Тарифная сетка</summary>
    private Queue<TariffDetails> _tariffSchedule = [];

    public object Create(object request, ISpecimenContext context) {
        return request switch {
            Type t when t == typeof(TariffDetails) => GetNext(context),
            _ => new NoSpecimen()
        };
    }

    private TariffDetails GetNext(ISpecimenContext context) {
        if (_tariffSchedule.Count > 0)
            return _tariffSchedule.Dequeue();

        var types = Enum.GetValues<RoomType>();
        var tariffs = context.CreateMany<decimal>(types.Length)
            .Order()
            .Select(x => x * 11.13m)
            .Zip(types)
            .Select(t => new TariffDetails(t.Second, t.First));
        _tariffSchedule = new Queue<TariffDetails>(tariffs);

        return GetNext(context);
    }
}