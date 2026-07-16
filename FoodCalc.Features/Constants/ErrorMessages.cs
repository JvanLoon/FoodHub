namespace FoodCalc.Features.Constants;

/// <summary>
/// Domain error and log messages produced by the command/query handlers in this project,
/// grouped by feature. Kept here so the backend's wording lives in one place.
/// Not shared with other projects.
/// </summary>
public static class ErrorMessages
{
    /// <summary>Entity-agnostic messages, parameterized where one template serves several entities.</summary>
    public static class Common
    {
        private const string NotFoundTemplate = "{0} not found";
        public static string NotFound(string entityName) =>
            string.Format(NotFoundTemplate, entityName);
    }

    /// <summary>Recipes.</summary>
    public static class Recipe
    {
        public const string GetAllFailed = "Failed to get all Recipes";
        public const string AddFailed = "Failed to add recipe";
        public const string UpdateFailed = "Failed to update recipe";
        public const string DeleteFailed = "Failed to delete recipe";
        public const string AddIngredientFailed = "Failed to add ingredient to Recipe";
        public const string UpdateForIngredientFailed = "Failed to update Recipe";
    }

    /// <summary>Ingredients.</summary>
    public static class Ingredient
    {
        public const string GetAllFailed = "Failed to get all ingredients";
        public const string AddFailed = "Failed to add ingredient";
        public const string UpdateFailed = "Failed to update ingredient";
        public const string DeleteFailed = "Failed to delete ingredient";
        public const string DeleteFromRecipeFailed = "Failed to delete ingredient from recipe";
    }

    /// <summary>Users &amp; roles.</summary>
    public static class User
    {
        public const string GetAllFailed = "Failed to get all Users";
        public const string GetByEmailFailed = "Failed to get User by email";
    }

    /// <summary>Import / export.</summary>
    public static class ImportExport
    {
        public const string ExportFailed = "Failed to export all data";
        public const string ImportFailed = "Failed to import all data";
        public const string NoImportData = "No data provided for import.";
    }
}
