namespace FoodCalc.Api.Common;

/// <summary>
/// Query parameters for the "get all roles" endpoints. Roles are paged/searchable
/// like every other list; the Blazor role-picker fetches them all via pageSize = int.MaxValue.
/// Shared by both api/user/allroles and api/admin/allroles.
/// </summary>
public class GetRolesRequest : PagedSearchRequest;