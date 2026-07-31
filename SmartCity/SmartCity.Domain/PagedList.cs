using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain
{
    public static class PagedList
    {
        public const int DefaultPageSize = 10;

        public static PagedList<TEntity> Create<TEntity>(int pageIndex, int totalCount, int pageSize, string order, IEnumerable<TEntity> results)
        {
            return new PagedList<TEntity>(pageIndex, totalCount, pageSize, order, results);
        }

        public static PagedList<TEntity> Create<TEntity>(int pageIndex, int totalCount, string order, IEnumerable<TEntity> results)
        {
            if (totalCount == 0)
                return PagedList<TEntity>.Empty;

            return new PagedList<TEntity>(pageIndex, totalCount, order, results);
        }

        public static PagedList<TEntity> Create<TEntity>(IEnumerable<TEntity> results)
        {
            var nr = results.Count();
            return Create(1, nr, nr, string.Empty, results);
        }

        public static async Task<PagedList<TEntity>> CreateAsync<TEntity>(int pageIndex, int pageSize, int totalCount, string order, IQueryable<TEntity> pagedQuery)
        {
            if (pagedQuery == null)
                throw new ArgumentNullException(nameof(pagedQuery));

            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            if (totalCount == 0)
                return PagedList<TEntity>.Empty;

            var pagedResults = await pagedQuery.ToListAsync();

            return new PagedList<TEntity>(pageIndex, totalCount, pageSize, order, pagedResults);
        }

        public static async Task<PagedList<TEntity>> CreateAsync<TEntity>(int pageIndex, int totalCount, string order, IQueryable<TEntity> pagedQuery)
        {
            return await CreateAsync(pageIndex, DefaultPageSize, totalCount, order, pagedQuery);
        }
    }

    public class PagedList<TEntity> : List<TEntity>, IPagedList<TEntity>
    {
        internal PagedList(int pageIndex, int totalCount, string order, IEnumerable<TEntity> results) :
            this(pageIndex, totalCount, PagedList.DefaultPageSize, order, results)
        { }

        internal PagedList(int pageIndex, int totalCount, int pageSize, string order, IEnumerable<TEntity> results)
            : base(pageSize)
        {
            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));

            if (totalCount < 0)
                throw new ArgumentOutOfRangeException(nameof(totalCount));

            if (results == null)
                throw new ArgumentNullException(nameof(results));

            PageIndex = pageIndex;
            TotalCount = totalCount;
            PageSize = pageSize;
            Order = order;

            // if an empty list, avoid division by zero
            PageCount = pageSize > 0 ? (totalCount - 1) / pageSize + 1 : 0;

            AddRange(results);
        }

        public int PageIndex { get; }

        public int PageCount { get; }

        public int PageSize { get; }

        public int TotalCount { get; }

        public string Order { get; set; }

        public int FirstItemIndex
        {
            get { return (PageIndex - 1) * PageSize; }
        }

        public int LastItemIndex
        {
            get { return Math.Min(TotalCount, PageIndex * PageSize) - 1; }
        }

        public bool HasPreviousPage
        {
            get { return PageIndex > 1; }
        }

        public bool HasNextPage
        {
            get { return PageIndex < PageCount; }
        }

        public bool IsEmpty
        {
            get { return ReferenceEquals(this, Empty) || 0 == TotalCount + Count; }
        }

        public int PagedIndexOf(TEntity item)
        {
            return FirstItemIndex + IndexOf(item) + 1;
        }

        #region Static members

        private sealed class EmptyPagedList : PagedList<TEntity>
        {
            public EmptyPagedList() : base(1, 0, 0, string.Empty, Enumerable.Empty<TEntity>()) { }
        }

        public static PagedList<TEntity> Empty { get; } = new EmptyPagedList();

        #endregion
    }
}
