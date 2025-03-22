using System.Linq.Expressions;

namespace gmafffff.starterKit.Utils;

/// <summary>
///     Динамическая композиция предикатов
/// </summary>
/// <remarks>
///     Код заимствован у Пита Монтгомери
///     <see href="https://petemontgomery.wordpress.com/2011/02/10/a-universal-predicatebuilder/">
///         A universal
///         PredicateBuilder
///     </see>
/// </remarks>
public static class PredicateBuilder {
    /// <summary>
    ///     Creates a predicate that evaluates to true.
    /// </summary>
    public static Expression<Func<T, bool>> True<T>() {
        return param => true;
    }

    /// <summary>
    ///     Creates a predicate that evaluates to false.
    /// </summary>
    public static Expression<Func<T, bool>> False<T>() {
        return param => false;
    }

    /// <summary>
    ///     Creates a predicate expression from the specified lambda expression.
    /// </summary>
    public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) {
        return predicate;
    }

    /// <summary>
    ///     Combines the first predicate with the second using the logical "and".
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second) {
        return first.Compose(second, Expression.AndAlso);
    }

    /// <summary>
    ///     Combines the first predicate with the second using the logical "or".
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second) {
        return first.Compose(second, Expression.OrElse);
    }

    /// <summary>
    ///     Negates the predicate.
    /// </summary>
    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression) {
        var negated = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
    }

    /// <summary>
    ///     Combines the first expression with the second using the specified merge function.
    /// </summary>
    public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
        Func<Expression, Expression, Expression> merge) {
        // zip parameters (map from parameters of second to parameters of first)
        var map = first.Parameters
            .Select((f, i) => new { f, s = second.Parameters[i] })
            .ToDictionary(keySelector: p => p.s, elementSelector: p => p.f);

        // replace parameters in the second lambda expression with the parameters in the first
        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        // create a merged lambda expression with parameters from the first expression
        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }

    /// Код заимствован у Максима Аршинова
    /// <see href="https://habr.com/ru/articles/313394/">Устранение дублирования Where Expressions в приложении</see>
    public static Expression<Func<TIn, TOut>> Compose<TIn, TInOut, TOut>(
        this Expression<Func<TIn, TInOut>> input,
        Expression<Func<TInOut, TOut>> inOutOut) {
        // это параметр x => blah-blah. Для лямбды нам нужен null
        var param = Expression.Parameter(typeof(TIn), name: null);
        // получаем объект, к которому применяется выражение
        var invoke = Expression.Invoke(input, param);
        // и выполняем "получи объект и примени к нему его выражение"
        var res = Expression.Invoke(inOutOut, invoke);

        // возвращаем лямбду нужного типа
        return Expression.Lambda<Func<TIn, TOut>>(res, param);
    }

    /// Код заимствован у Максима Аршинова
    /// <see href="https://habr.com/ru/companies/jugru/articles/423891/">Деревья выражений в enterprise-разработке</see>
    public static IQueryable<T> Where<T, TProp>(this IQueryable<T> queryable,
        Expression<Func<T, TProp>> propertySelector,
        Expression<Func<TProp, bool>> predicate) {
        return queryable.Where(propertySelector.Compose(predicate));
    }


    private class ParameterRebinder : ExpressionVisitor {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression>? map) {
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map,
            Expression exp) {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p) {
            if (_map.TryGetValue(p, out var replacement)) p = replacement;

            return base.VisitParameter(p);
        }
    }
}