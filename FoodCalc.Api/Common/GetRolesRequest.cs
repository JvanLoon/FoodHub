using FastEndpoints;

namespace FoodCalc.Api.Common;

/// <summary>
/// Query parameters for the "get all roles" endpoints. Roles are paged/searchable
/// like every other list; the Blazor role-picker fetches them all via pageSize = int.MaxValue.
/// Shared by both api/user/allroles and api/admin/allroles.
/// </summary>
public class GetRolesRequest : IPagedSearchRequest
{
	[BindFrom("page")]
	public int Page { get; set; } = 1;

	[BindFrom("pageSize")]
	public int PageSize { get; set; } = 25;

	[BindFrom("search")]
	public string? Search { get; set; }
}
