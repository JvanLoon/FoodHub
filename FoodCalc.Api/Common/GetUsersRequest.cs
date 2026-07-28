namespace FoodCalc.Api.Common;

/// <summary>
/// Query parameters for the paged user list endpoints. Shared by api/admin/users and
/// api/user/users, which differ only in the policy they require — the two used to declare
/// byte-identical copies in their own namespaces.
/// </summary>
public class GetUsersRequest : PagedSearchRequest;
