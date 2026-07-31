using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain
{
    public interface IPagedList
    {
        int PageIndex { get; }

        int PageCount { get; }

        int PageSize { get; }

        int TotalCount { get; }

        bool HasPreviousPage { get; }

        bool HasNextPage { get; }

        bool IsEmpty { get; }

        int LastItemIndex { get; }

        int FirstItemIndex { get; }

        string Order { get; }
    }

    public interface IPagedList<TEntity> : IPagedList, IList<TEntity>
    {
        int PagedIndexOf(TEntity item);
    }
}
