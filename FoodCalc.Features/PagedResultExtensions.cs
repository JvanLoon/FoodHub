using FoodHub.DTOs;

using Microsoft.EntityFrameworkCore;

namespace FoodCalc.Features
{
	/// <summary>
	/// Shared contract for paged + searchable queries. Every "get all" query
	/// (recipes, ingredients, users, roles) implements this so the paging
	/// helpers below can page any of them the same way.
	/// </summary>
	public interface IPagedSearchQuery
	{
		int Page { get; }
		int PageSize { get; }
		string? Search { get; }
	}

	public static class QueryableExtensions
	{
		/// <summary>
		/// Counts, skips/takes and materialises a page directly into a
		/// <see cref="PagedResultDto{T}"/>. Callers apply their own search
		/// filter (the predicate differs per entity) before calling this.
		/// PageSize is not capped: the Blazor "get all" helpers pass
		/// int.MaxValue to fetch every row.
		/// </summary>
		public static async Task<PagedResultDto<T>> ToPagedResultAsync<T>(
			this IQueryable<T> source,
			int page,
			int pageSize,
			CancellationToken cancellationToken = default
		)
		{
			var totalCount = await source.CountAsync(cancellationToken);
			var items = await source.Skip((page - 1) * pageSize)
									.Take(pageSize)
									.ToListAsync(cancellationToken);

			return new PagedResultDto<T> {Items = items, TotalCount = totalCount, Page = page, PageSize = pageSize};
		}

		/// <summary>Same as above but reads paging straight off an <see cref="IPagedSearchQuery"/>.</summary>
		public static Task<PagedResultDto<T>> ToPagedResultAsync<T>(this IQueryable<T> source,
																	IPagedSearchQuery request,
																	CancellationToken cancellationToken = default
		) =>
			source.ToPagedResultAsync(request.Page, request.PageSize, cancellationToken);

		/// <summary>
		/// Pages the entity query, then projects the page into DTOs via <paramref name="map"/>.
		/// The common case for handlers whose only per-item work is a synchronous mapping.
		/// </summary>
		public static async Task<PagedResultDto<TDto>> ToPagedResultAsync<TEntity, TDto>(
			this IQueryable<TEntity> source,
			IPagedSearchQuery request,
			Func<IReadOnlyList<TEntity>, List<TDto>> map,
			CancellationToken cancellationToken = default
		)
		{
			var paged = await source.ToPagedResultAsync(request, cancellationToken);
			return new PagedResultDto<TDto>
			{
				Items = map(paged.Items),
				TotalCount = paged.TotalCount,
				Page = paged.Page,
				PageSize = paged.PageSize
			};
		}
	}
}
