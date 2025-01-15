using System.Reflection;

namespace gmafffff.training.hotel.domain.tests.FakeModel.AutoFixture;

/// <summary>
///     Генерирует ФИО для <see cref="PersonFullName" />
/// </summary>
public class PersonFullNameGenerator : ISpecimenBuilder {
    private readonly ElementsBuilder<string> _firstNameGen;

    private readonly string[] _firstNames = [
        "Александр", "Алексей", "Анатолий", "Андрей", "Борис", "Валентин", "Валерий", "Василий", "Виктор", "Владимир",
        "Георгий", "Денис", "Дмитрий", "Евгений", "Иван", "Игорь", "Илья", "Михаил", "Николай", "Павел", "Пётр",
        "Сергей", "Юрий"
    ];

    private readonly ElementsBuilder<string> _patronymicGen;

    private readonly string[] _patronymics = [
        "Александрович", "Алексеевич", "Анатольевич", "Андреевич", "Борисович", "Валентинович", "Валерьевич",
        "Васильевич", "Викторович", "Владимирович", "Георгиевич", "Денисович", "Дмитриевич", "Евгеньевич", "Иванович",
        "Игоревич", "Ильич", "Михайлович", "Николаевич", "Павлович", "Петрович", "Сергеевич", "Юрьевич"
    ];

    private readonly ElementsBuilder<string> _surNameGen;

    private readonly string[] _surNames = [
        "Иванов", "Смирнов", "Кузнецов", "Попов", "Васильев", "Петров", "Соколов", "Михайлов", "Новиков", "Фёдоров",
        "Морозов", "Волков", "Алексеев", "Лебедев", "Семёнов", "Егоров", "Павлов", "Козлов", "Степанов", "Николаев",
        "Орлов", "Андреев", "Макаров", "Никитин", "Захаров"
    ];

    public PersonFullNameGenerator() {
        _surNameGen = new ElementsBuilder<string>(_surNames);
        _firstNameGen = new ElementsBuilder<string>(_firstNames);
        _patronymicGen = new ElementsBuilder<string>(_patronymics);
    }

    public object Create(object request, ISpecimenContext context) {
        var name = request switch {
            PropertyInfo { DeclaringType: not null } pi
                when pi.DeclaringType.IsAssignableTo(typeof(PersonFullName)) &&
                     pi.PropertyType == typeof(string) => pi.Name,
            ParameterInfo { Member.DeclaringType: not null } pri
                when pri.Member.DeclaringType.IsAssignableTo(typeof(PersonFullName)) &&
                     pri.ParameterType == typeof(string) => pri.Name,
            _ => string.Empty
        };

        return name switch {
            nameof(PersonFullName.SurName) => _surNameGen.Create(typeof(string), context),
            nameof(PersonFullName.FirstName) => _firstNameGen.Create(typeof(string), context),
            nameof(PersonFullName.Patronymic) => _patronymicGen.Create(typeof(string), context),
            _ => new NoSpecimen()
        };
    }
}