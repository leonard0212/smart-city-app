using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SmartCity.Domain.Extensions
{
    public static class IQuerableExtensions
    {
        public static IQueryable<T> IncludeMany<T>(this IQueryable<T> query, IEnumerable<Expression<Func<T, object>>> includes)
            where T : class
        {
            return includes.Aggregate(query, (current, include) => current.Include(include));
        }
    }
}
