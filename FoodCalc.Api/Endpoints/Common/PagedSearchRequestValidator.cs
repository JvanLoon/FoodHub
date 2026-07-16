using FastEndpoints;

using FluentValidation;

namespace FoodCalc.Api.Endpoints.Common;

/// <summary>
/// The single paging guard for all paged-search requests. PageSize is only
/// lower-bounded: the Blazor "get all" helpers call these endpoints with
/// pageSize = int.MaxValue to fetch every row, so an upper cap would break them.
///
/// FastEndpoints resolves a validator by concrete request type, so each request
/// keeps a thin subclass (e.g. GetRecipesRequestValidator : this&lt;GetRecipesRequest&gt;).
/// Shared rules live here; request-specific rules go in the subclass body.
/// </summary>
public class PagedSearchRequestValidator<TRequest> : Validator<TRequest>
	where TRequest : class, IPagedSearchRequest
{
	public PagedSearchRequestValidator()
	{
		RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
		RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
	}
}
