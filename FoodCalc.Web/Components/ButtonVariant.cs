namespace FoodCalc.Web.Components.UI;

public enum ButtonVariant
{
    Primary,
    Secondary,
    Success,
    Danger,
    OutlinePrimary,
    OutlineSecondary,
    OutlineSuccess,
    OutlineDanger,

    /// <summary>
    /// Warm accent, filled softly rather than solid. For a secondary call to action that
    /// still has to be findable next to a primary button — loud enough to see, quiet
    /// enough not to compete.
    /// </summary>
    Accent,

    Ghost,
    Link
}

public enum ButtonSize
{
    Small,
    Medium,
    Large
}