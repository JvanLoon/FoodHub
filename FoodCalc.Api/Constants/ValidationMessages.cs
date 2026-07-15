namespace FoodCalc.Api.Constants;

/// <summary>
/// Request-validation messages surfaced by the API's FluentValidation validators.
/// Scoped to this project — not shared with the Web or Features layers.
/// </summary>
public static class ValidationMessages
{
    public const string NameRequired = "No name provided";
    public const string IngredientIdRequired = "No ingredient id provided";
    public const string RecipeIdRequired = "No recipe id provided";
}
