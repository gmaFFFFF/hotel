using System.Reflection;

namespace gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;

public class HotelBlockNameGenerator : ISpecimenBuilder {
    private readonly ElementsBuilder<string> _hotelBlockNameGen;

    private readonly string[] _hotelBlockNames = [
        "Золотая Гавань", "Городские Облака", "Светлые Сны", "Лунный Приют",
        "Звёздный Портал", "Королевская Гавань", "Серебряный Ключ", "Тайный Уголок", "Сон на Рассвете",
        "Вечерний Замок", "Мечта Мандривника", "Шепот Рассвета", "Берег Сновидений", "Небесная Гармония",
        "Закатные Апартаменты", "Прибой Сердец", "Золотое Перо", "Царство Миражей", "Временный Рай",
        "Городской Островок", "Сокровище Ночи", "Невесомое Облако", "Волшебная Ночь", "Небесное Отражение", "Омут Грёз",
        "Шёпот Леса", "Звездный Путеводитель", "Загадочный Маяк", "Светлая Гавань", "Величественное Убежище",
        "Сердце Города", "Капелька Росы", "Лазурный"
    ];

    public HotelBlockNameGenerator() {
        _hotelBlockNameGen = new ElementsBuilder<string>(_hotelBlockNames);
    }

    public object Create(object request, ISpecimenContext context) {
        return request switch {
            PropertyInfo { DeclaringType: not null } pi
                when pi.DeclaringType.IsAssignableTo(typeof(HotelBlock<int, Guid>)) &&
                     pi.PropertyType == typeof(string) &&
                     pi.Name == nameof(HotelBlock<int, Guid>.Name) => _hotelBlockNameGen.Create(
                    typeof(string), context),
            ParameterInfo { Member.DeclaringType: not null } pri
                when pri.Member.DeclaringType.IsAssignableTo(typeof(HotelBlock<int, Guid>)) &&
                     pri.ParameterType == typeof(string) &&
                     pri.Name == nameof(HotelBlock<int, Guid>.Name) => _hotelBlockNameGen.Create(
                    typeof(string), context),
            _ => new NoSpecimen()
        };
    }
}