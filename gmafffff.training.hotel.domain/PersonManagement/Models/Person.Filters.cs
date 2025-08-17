using gmafffff.starterKit.Utils;

namespace gmafffff.training.hotel.domain.PersonManagement.Models;

public partial class Person<TId> where TId : struct, IEquatable<TId> {
    /// <summary>
    ///     Фильтрация по первым символам фамилии, или имени, или отчества
    /// </summary>
    /// <param name="Search"></param>
    public record FilterByFullName(string Search) {
        public static implicit operator Expression<Func<Person<TId>, bool>>(FilterByFullName filter) {
            var tokens = filter.Search.Split(' ');
            var predicate = PredicateBuilder.False<Person<TId>>();
            foreach (var token in tokens)
                predicate = predicate
                    .Or(person => person.FullName.SurName.StartsWith(token))
                    .Or(person => person.FullName.FirstName.StartsWith(token))
                    .Or(person => person.FullName.Patronymic != null &&
                                  person.FullName.Patronymic.StartsWith(token));
            return predicate;
        }
    }
}