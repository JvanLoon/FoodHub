namespace FoodCalc.Api.Constants;

/// <summary>
/// Request-validation messages surfaced by the API's FluentValidation validators,
/// grouped by feature. Scoped to this project — not shared with the Web or Features layers.
/// </summary>
public static class ValidationMessages
{
    /// <summary>Messages reused across features.</summary>
    public static class Common
    {
        public const string NameRequired = "No name provided";
    }

    /// <summary>Recipe validators.</summary>
    public static class Recipe
    {
        public const string IdRequired = "No recipe id provided";
    }

    /// <summary>Ingredient validators.</summary>
    public static class Ingredient
    {
        public const string IdRequired = "No ingredient id provided";
    }
}
