using System.Linq.Expressions;

namespace SmartCity.Core.Utils
{
    public static class QueryPredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>()
        {
            return x => true;
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return x => false;
        }

        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> expression)
        {
            return expression;
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var invokedExpr = Expression.Invoke(right, left.Parameters);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left.Body, invokedExpr), left.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var invokedExpr = Expression.Invoke(right, left.Parameters);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, invokedExpr), left.Parameters);
        }




    }
}
