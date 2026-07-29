namespace FoodCalc.Features.Review;

/// <summary>
/// Who is performing a write, as far as content ownership is concerned. Carried on every
/// command that mutates a recipe or a catalog ingredient so the handler — which is the only
/// place that has the row and therefore its author — can enforce ownership itself.
///
/// The role check deliberately stops at <see cref="IsAdmin"/>. Moderator is not an editing
/// role: a moderator approves or rejects submissions through the review endpoints and has no
/// more right to rewrite another user's recipe than any other account.
/// </summary>
/// <param name="UserId">Caller's IdentityUser id, or null when the request carried no usable identity.</param>
/// <param name="IsAdmin">True if the caller holds the Admin role, which may edit anything.</param>
public record ActingUser(string? UserId, bool IsAdmin)
{
    /// <summary>A caller we could not identify. Owns nothing and may edit nothing.</summary>
    public static readonly ActingUser Anonymous = new(null, false);

    /// <summary>True if this caller may edit content authored by <paramref name="ownerUserId"/>.</summary>
    public bool CanEdit(string ownerUserId) =>
        ReviewVisibilityExtensions.CanEditContentOwnedBy(UserId, ownerUserId, IsAdmin);
}
