using Microsoft.AspNetCore.Mvc.Rendering;
using SmartCity.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IList<SelectListItem> ToSelectListWithDefault<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text, Func<T, object> orderBy)
        {
            return ToSelectListWithDefault(enumerable, value, text, orderBy);
        }

        public static IList<SelectListItem> ToSelectListWithDefault<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text, Func<T, bool> selected, Func<T, object> orderBy)
        {
            var selectList = enumerable.ToSelectListWithoutDefault(value, text, selected, orderBy);
            selectList.Insert(0, new SelectListItem { Text = "--Selectează--", Value = "" });
            return selectList.ToList();
        }
        public static IList<SelectListItem> ToSelectListWithoutDefault<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text, Func<T, bool> selected, Func<T, object> orderBy)
        {
            var selectList = enumerable.OrderBy(orderBy)
                .Select(item => new SelectListItem
                {
                    Value = value(item),
                    Text = text(item),
                    Selected = selected(item)
                })
                .ToList();

            return selectList;
        }

        public static async Task<IPagedList<TEntity>> ToPagedListAsync<TEntity>(this Task<IEnumerable<TEntity>> @this, int pageIndex, int pageSize, int count, string order)
        {
            var enumerable = await @this;
            var pagedList = PagedList.Create(pageIndex, count, pageSize, order, enumerable);

            return pagedList;
        }

        public static async Task<IPagedList<TEntity>> ToPagedListAsync<TEntity>(this IEnumerable<TEntity> enumerable, int pageIndex, int pageSize, int count, string order)
        {
   
            var pagedList = PagedList.Create(pageIndex, count, pageSize, order, enumerable);

            return pagedList;
        }
    }
}
