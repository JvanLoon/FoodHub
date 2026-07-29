namespace FoodHub.Persistence.Entities;

/// <summary>What kind of thing a <see cref="ReviewRejection"/> was recorded against.</summary>
public enum ReviewTargetType
{
    Recipe = 1,
    Ingredient = 2
}

/// <summary>
/// A moderator's rejection of a submitted recipe or ingredient, kept as a record rather than
/// applied and forgotten: the author is meant to be told why, and the notification feature that
/// will do the telling does not exist yet. Nothing reads <see cref="Reason"/> today.
///
/// Not a foreign key to its target on purpose — a rejection may delete what it rejected
/// (<see cref="TargetDeleted"/>), and the reason for that deletion has to outlive it. Hence
/// <see cref="TargetName"/>, which snapshots what the thing was called at the time.
/// </summary>
public class ReviewRejection : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public ReviewTargetType TargetType { get; set; }

    /// <summary>Id of the rejected recipe/ingredient. Dangling once <see cref="TargetDeleted"/>.</summary>
    public Guid TargetId { get; set; }

    /// <summary>Name at the time of rejection, so a deleted target is still identifiable.</summary>
    public string TargetName { get; set; } = string.Empty;

    /// <summary>IdentityUser id of the author being rejected — who the future notification is for.</summary>
    public string TargetOwnerUserId { get; set; } = string.Empty;

    /// <summary>IdentityUser id of the moderator/admin who rejected it.</summary>
    public string RejectedByUserId { get; set; } = string.Empty;

    /// <summary>Moderator's explanation. Required — rejecting without one is not allowed.</summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>True if the moderator chose to delete the target as well as reject it.</summary>
    public bool TargetDeleted { get; set; }
}
