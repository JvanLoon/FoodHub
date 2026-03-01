using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodCalc.Features
{
    public class PagedResult<T>
    {
		public IReadOnlyList<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public static class QueryableExtensions
    {
        public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> source, int page, int pageSize)
        {
            var totalCount = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> source, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(source, cancellationToken);
            var items = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                source.Skip((page - 1) * pageSize).Take(pageSize), cancellationToken);
            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
