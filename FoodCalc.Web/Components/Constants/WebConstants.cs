namespace FoodCalc.Web.Components.Constants;

/// <summary>
/// User-facing messages and magic-string keys used by the Web client. Scoped to this
/// project — messages are intentionally not shared with the API or Features layers, since
/// the wording shown to the user is a UI concern.
/// </summary>
public static class WebConstants
{
    /// <summary>Toast / notification text shown to the user.</summary>
    public static class Messages
    {
        // Generic HTTP client failures (AuthenticatedHttpClientService)
        public const string GenericFailure = "Something went wrong. Please try again.";
        public const string Unauthorized = "You are not signed in, or your session has expired.";
        public const string Forbidden = "You don't have permission to do that.";
        public const string NotFound = "The requested item was not found.";
        public const string BadRequest = "The request was invalid.";
        public const string Conflict = "That action conflicts with the current state.";
        public const string ServerError = "The server encountered an error. Please try again later.";
        public static string RequestFailed(int statusCode) => $"Request failed ({statusCode}).";

        // Import / export
        public const string NoFileContent = "No file content.";
        public const string ExportUnexpectedResponse = "Export failed: the server returned an unexpected response.";
        public const string ExportEmpty = "Export failed: file content is empty.";
        public const string PreparingExport = "Preparing export...";
        public const string SelectFileFirst = "Please select a file first.";
        public const string OnlyJsonAccepted = "Only .json files are accepted.";
        public const string CouldNotReadFile = "Could not read the selected file.";

        // Authentication
        public const string LoginFailed = "Failed to login. Invalid email or password.";
        public const string LoginInvalidResponse = "Login failed: invalid server response.";
        public const string RegisterSuccess = "Registration successful! You can now login.";
        public const string RegisterFailed = "Error registering";

        // Roles / password
        public const string RoleAdded = "Role added successfully!";
        public const string RoleRemoved = "Role removed successfully!";
        public const string PasswordReset = "Password reset successfully!";

        // Recipes
        public const string RecipeNameUpdated = "Recipe name updated";
        public const string CreateRecipeFailed = "Failed to create recipe.";

        // Ingredients (EditRecipe)
        public const string IngredientAddedOrUpdated = "Ingredient added/updated successfully";
        public const string IngredientDeleted = "Ingredient deleted successfully";
        public const string IngredientAdded = "Ingredient added";

        // Ingredients (IngredientList)
        public const string IngredientUpdated = "Ingredient updated successfully!";
        public const string IngredientNameUpdated = "Ingredient name updated successfully!";
        public const string IngredientDeletedWithReload = "Ingredient deleted successfully!";
    }

    /// <summary>Browser storage keys.</summary>
    public static class Storage
    {
        public const string AuthToken = "Authorization";
        public const string AggregatedIngredients = "aggregated-ingredients";
        public const string ShoppingListItems = "shopping-list-items";
    }

    /// <summary>Misc client-side constants.</summary>
    public static class Files
    {
        public const string ExportBaseName = "export";
    }
}
