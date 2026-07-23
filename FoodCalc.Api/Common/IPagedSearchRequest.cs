namespace FoodCalc.Api.Common;

/// <summary>
/// Query parameters shared by every paged + searchable "get all" endpoint.
/// Individual GetXRequest types implement this so they can share one validator,
/// while still being free to add their own request-specific properties.
/// </summary>
public interface IPagedSearchRequest
{
	int Page { get; set; }
	int PageSize { get; set; }
	string? Search { get; set; }
}