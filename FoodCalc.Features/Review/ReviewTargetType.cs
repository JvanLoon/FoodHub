namespace FoodCalc.Features.Review;

/// <summary>
/// What an approve/reject command acts on. Drives the switch in the approve/reject handlers;
/// no longer persisted (the rejection-record table is gone). <see cref="RecipeItem"/> exists so
/// a moderator can approve or reject one changed line of a recipe without touching the rest.
/// </summary>
public enum ReviewTargetType
{
    Recipe = 1,
    Ingredient = 2,
    RecipeItem = 3
}
