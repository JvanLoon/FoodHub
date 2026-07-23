using System.Security.Claims;

namespace FoodCalc.Api.Common;

public static class ClaimsPrincipalExtensions
{
	/// <summary>
	/// The authenticated user's IdentityUser id. The JWT carries it as <c>sub</c>, which the
	/// default JWT handler maps to <see cref="ClaimTypes.NameIdentifier"/>; we also check the raw
	/// "sub" claim in case inbound claim mapping is ever disabled.
	/// </summary>
	public static string? GetUserId(this ClaimsPrincipal user) =>
		user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
}
