namespace FoodCalc.Features.Constants;

/// <summary>
/// Domain error and log messages produced by the command/query handlers in this project.
/// The generic CRUD-style failures live in <see cref="Common"/> as entity-parameterized
/// templates; genuinely feature-specific wording stays in its feature group.
/// Not shared with other projects.
/// </summary>
public static class ErrorMessages
{
    /// <summary>
    /// Entity-agnostic templates. Pass the entity name exactly as it should read in the
    /// message (e.g. "recipe", "ingredient", "Users").
    /// </summary>
    public static class Common
    {
        private const string _notFoundTemplate = "{0} not found";
        public static string NotFound(string entityName) =>
            string.Format(_notFoundTemplate, entityName);

        private const string _getAllFailedTemplate = "Failed to get all {0}";
        public static string GetAllFailed(string entityName) =>
            string.Format(_getAllFailedTemplate, entityName);

        private const string _addFailedTemplate = "Failed to add {0}";
        public static string AddFailed(string entityName) =>
            string.Format(_addFailedTemplate, entityName);

        private const string _updateFailedTemplate = "Failed to update {0}";
        public static string UpdateFailed(string entityName) =>
            string.Format(_updateFailedTemplate, entityName);

        private const string _deleteFailedTemplate = "Failed to delete {0}";
        public static string DeleteFailed(string entityName) =>
            string.Format(_deleteFailedTemplate, entityName);
    }

    /// <summary>Recipe-specific wording.</summary>
    public static class Recipe
    {
        public const string AddIngredientFailed = "Failed to add ingredient to Recipe";
        public const string UpdateForIngredientFailed = "Failed to update Recipe";
    }

    /// <summary>Ingredient-specific wording.</summary>
    public static class Ingredient
    {
        public const string DeleteFromRecipeFailed = "Failed to delete ingredient from recipe";
    }

    /// <summary>User-specific wording.</summary>
    public static class User
    {
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
