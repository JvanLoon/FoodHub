namespace FoodCalc.Features.Review;

/// <summary>
/// What an approve/reject command acts on. Drives the switch in the approve/reject handlers;
/// not persisted. The recipe is the only review gate — ingredients have no independent review,
/// they are approved as a side effect of approving the recipe that introduced them.
/// <see cref="RecipeItem"/> exists so a moderator can approve or reject one changed line of a
/// recipe without touching the rest.
/// </summary>
public enum ReviewTargetType
{
    Recipe = 1,
    RecipeItem = 2
}
