namespace FoodCalc.Features.Review;

/// <summary>
/// Who is performing a write, as far as content ownership is concerned. Carried on every
/// command that mutates a recipe or a catalog ingredient so the handler — which is the only
/// place that has the row and therefore its author — can enforce ownership itself.
/// </summary>
/// <param name="UserId">Caller's IdentityUser id, or null when the request carried no usable identity.</param>
/// <param name="CanEditAnyContent">
/// True for staff (Admin or Moderator), who may edit any recipe or ingredient regardless of
/// who authored it. A moderator's job now includes fixing submissions, not only approving them.
/// </param>
public record ActingUser(string? UserId, bool CanEditAnyContent)
{
    /// <summary>A caller we could not identify. Owns nothing and may edit nothing.</summary>
    public static readonly ActingUser Anonymous = new(null, false);

    /// <summary>True if this caller may edit content authored by <paramref name="ownerUserId"/>.</summary>
    public bool CanEdit(string ownerUserId) =>
        ReviewVisibilityExtensions.CanEditContentOwnedBy(UserId, ownerUserId, CanEditAnyContent);
}
