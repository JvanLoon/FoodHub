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
