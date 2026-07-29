using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Review;

/// <summary>
/// The one place that decides who may see unapproved content. Every read of recipes or
/// catalog ingredients goes through here, so the rule cannot drift between call sites.
///
/// The rule is: approved content is public; unapproved content is visible only to the
/// account that created it. That applies to administrators too — an admin browsing the
/// recipe list sees the same library everyone else does. Moderating unapproved content is
/// a separate, explicit path (the review queue), not a side effect of holding a role.
/// </summary>
public static class ReviewVisibilityExtensions
{
    /// <param name="userId">
    /// The caller's IdentityUser id, or null/empty for a caller we cannot identify — who
    /// then sees approved content only, since an empty id must never match a stored author.
    /// </param>
    public static IQueryable<Recipe> VisibleTo(this IQueryable<Recipe> recipes, string? userId) =>
        string.IsNullOrEmpty(userId)
            ? recipes.Where(r => r.IsReviewed)
            : recipes.Where(r => r.IsReviewed || r.CreatedByUserId == userId);

    /// <inheritdoc cref="VisibleTo(IQueryable{Recipe},string)"/>
    public static IQueryable<Ingredient> VisibleTo(this IQueryable<Ingredient> ingredients, string? userId) =>
        string.IsNullOrEmpty(userId)
            ? ingredients.Where(i => i.IsReviewed)
            : ingredients.Where(i => i.IsReviewed || i.CreatedByUserId == userId);

    /// <summary>
    /// True if <paramref name="userId"/> may modify content authored by
    /// <paramref name="ownerUserId"/>. Authors edit their own work; staff edit anything.
    /// <paramref name="canEditAnyContent"/> is the staff flag — true for Admin and Moderator,
    /// both of whom may edit any recipe or ingredient (a moderator curating the library needs
    /// to fix as well as approve).
    /// </summary>
    public static bool CanEditContentOwnedBy(string? userId, string ownerUserId, bool canEditAnyContent) =>
        canEditAnyContent || (!string.IsNullOrEmpty(userId) && userId == ownerUserId);
}
