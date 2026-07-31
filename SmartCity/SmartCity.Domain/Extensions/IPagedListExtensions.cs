using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Extensions
{
    public static class IPagedListExtensions
    {
        public static IPagedList<TDestination> ToMappedPagedList<TSource, TDestination>(this IPagedList<TSource> list, IMapper mapper)
        {
            var sourceList = mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(list);
            var pagedResult = PagedList.Create(list.PageIndex, list.TotalCount, list.PageSize, list.Order, sourceList);

            return pagedResult;
        }
    }
}
