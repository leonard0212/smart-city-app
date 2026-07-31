using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.Utils
{
    public static class QueryOrderBuilder
    {
        public static Func<IQueryable<T>, IOrderedQueryable<T>> Create<T>(Func<IQueryable<T>, IOrderedQueryable<T>> order)
        {
            return order;
        }
    }
}
