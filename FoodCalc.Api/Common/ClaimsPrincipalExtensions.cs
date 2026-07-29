using System.Security.Claims;
using FoodCalc.Features.Review;

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

    /// <summary>
    /// The caller as a content-ownership subject, for commands that mutate recipes or catalog
    /// ingredients. Endpoints pass this straight through; the handler does the deciding. Admin
    /// and Moderator are both "can edit anything" — the review side of moderation lets them fix
    /// submissions, not just approve them.
    /// </summary>
    public static ActingUser ToActingUser(this ClaimsPrincipal user) =>
        new(user.GetUserId(), user.IsInRole("Admin") || user.IsInRole("Moderator"));
}