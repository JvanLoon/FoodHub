namespace FoodCalc.Features.Constants;

/// <summary>
/// Domain error and log messages produced by the command/query handlers in this project.
/// Kept here so the backend's wording lives in one place. Not shared with other projects.
/// </summary>
public static class ErrorMessages
{
    // Recipes
    public const string GetAllRecipesFailed = "Failed to get all Recipes";
    public const string AddRecipeFailed = "Failed to add recipe";
    public const string UpdateRecipeFailed = "Failed to update recipe";
    public const string DeleteRecipeFailed = "Failed to delete recipe";
    public const string RecipeNotFound = "Recipe not found";
    public const string AddIngredientToRecipeFailed = "Failed to add ingredient to Recipe";
    public const string UpdateRecipeIngredientFailed = "Failed to update Recipe";

    // Ingredients
    public const string GetAllIngredientsFailed = "Failed to get all ingredients";
    public const string AddIngredientFailed = "Failed to add ingredient";
    public const string UpdateIngredientFailed = "Failed to update ingredient";
    public const string DeleteIngredientFailed = "Failed to delete ingredient";
    public const string DeleteIngredientFromRecipeFailed = "Failed to delete ingredient from recipe";

    // Users / roles
    public const string GetAllUsersFailed = "Failed to get all Users";
    public const string UserNotFound = "User not found";
    public const string GetUserByEmailFailed = "Failed to get User by email";

    // Import / export
    public const string ExportAllFailed = "Failed to export all data";
    public const string ImportAllFailed = "Failed to import all data";
    public const string NoImportData = "No data provided for import.";
}
