namespace FoodHub.DTOs;

/// <summary>
/// Request bodies for the admin POST endpoints, shared by the API (model binding) and the
/// Web client (the object it posts), so the two cannot drift apart silently.
///
/// Deliberately free of FastEndpoints' <c>[BindFrom]</c> attributes: this project has no
/// package references and is worth keeping that way. The attributes only matter when the
/// incoming name differs from the property name, which is not the case once these arrive
/// as a JSON body.
///
/// Only endpoints that can carry a body live here. GET and DELETE take their arguments
/// from the query string and keep their request types, with their attributes, in the API.
/// </summary>
public class ToggleUserRequest
{
    public string Email { get; set; } = string.Empty;

    public bool Enable { get; set; } = true;
}

/// <summary>Body of POST api/admin/userroles — grant a role to a user.</summary>
public class ModifyUserRoleRequest
{
    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Body of POST api/admin/presence — refresh who is online without re-fetching the user list.
/// A POST for what is really a read, because the client asks about a whole page of accounts at
/// once and a hundred ids do not belong in a query string.
/// </summary>
public class UserPresenceRequest
{
    public List<string> UserIds { get; set; } = [];
}