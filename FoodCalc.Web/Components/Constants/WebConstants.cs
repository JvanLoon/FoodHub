namespace FoodCalc.Web.Components.Constants;

/// <summary>
/// User-facing messages and magic-string keys used by the Web client. Scoped to this
/// project — messages are intentionally not shared with the API or Features layers, since
/// the wording shown to the user is a UI concern.
/// </summary>
public static class WebConstants
{
    /// <summary>Toast / notification text shown to the user, grouped by feature.</summary>
    public static class Messages
    {
        /// <summary>Generic HTTP client failures (AuthenticatedHttpClientService).</summary>
        public static class Client
        {
            public const string GenericFailure = "Something went wrong. Please try again.";
            public const string Unauthorized = "You are not signed in, or your session has expired.";
            public const string Forbidden = "You don't have permission to do that.";
            public const string NotFound = "The requested item was not found.";
            public const string BadRequest = "The request was invalid.";
            public const string Conflict = "That action conflicts with the current state.";
            public const string ServerError = "The server encountered an error. Please try again later.";
            public static string RequestFailed(int statusCode) => $"Request failed ({statusCode}).";
        }

        /// <summary>Import / export.</summary>
        public static class ImportExport
        {
            public const string NoFileContent = "No file content.";
            public const string ExportUnexpectedResponse = "Export failed: the server returned an unexpected response.";
            public const string ExportEmpty = "Export failed: file content is empty.";
            public const string PreparingExport = "Preparing export...";
            public const string SelectFileFirst = "Please select a file first.";
            public const string OnlyJsonAccepted = "Only .json files are accepted.";
            public const string CouldNotReadFile = "Could not read the selected file.";
            public const string ImportSucceeded = "Import successful.";
            public const string ImportFailed = "Import failed.";
		}

        /// <summary>Authentication.</summary>
        public static class Auth
        {
            public const string LoginFailed = "Failed to login. Invalid email or password.";
            public const string LoginInvalidResponse = "Login failed: invalid server response.";
            public const string RegisterSuccess = "Registration successful! You can now login.";
            public const string RegisterFailed = "Error registering";
			public const string UserActiveSwitchFailed = "Failed to switch users active status to: {0}. still {1}.";
            public const string UserLoadFailed = "Failed to load users.";
		}

        /// <summary>Role &amp; password management.</summary>
        public static class Roles
        {
            public const string Added = "Role added successfully!";
            public const string Removed = "Role removed successfully!";
            public const string PasswordReset = "Password reset successfully!";

			public const string AddedFailed = "Failed to add role.";
			public const string RemovedFailed = "Failed to remove role.";
			public const string PasswordResetFailed = "Failed to reset password.";

		}

        /// <summary>Recipes.</summary>
        public static class Recipe
        {
			public const string Created = "Recipe created successfully!";
			public const string NameUpdated = "Recipe name updated successfully!";
            public const string CreateFailed = "Failed to create recipe.";
			public const string Deleted = "Recipe deleted successfully!";
			public const string Updated = "Recipe updated successfully!";

			public const string NameUpdateFailed = "Failed to update recipe name.";
			public const string DeleteFailed = "Failed to delete recipe.";
			public const string GetAllRecipesFailed = "Failed to get all recipes.";
		}

        /// <summary>Ingredients.</summary>
        public static class Ingredient
        {
            public const string AddedOrUpdated = "Ingredient added/updated successfully";
            public const string Deleted = "Ingredient deleted successfully";
            public const string Added = "Ingredient added";
            public const string Updated = "Ingredient updated successfully!";
            public const string NameUpdated = "Ingredient name updated successfully!";
            public const string DeletedWithReload = "Ingredient deleted successfully!";

			public const string AddedOrUpdatedFailed = "Failed to add/update ingredient.";
			public const string NameUpdateFailed = "Failed to update ingredient name.";
			public const string UpdatedFailed = "Failed to update ingredient.";
			public const string UpdatedSetShoppingCartFailed = "Failed to update the shopping cart status.";
			public const string DeletedFailed = "Failed to delete ingredient.";
			public const string GetAllIngredientsFailed = "Failed to get all ingredients.";

			public const string LoadFailed = "Failed to load ingredients.";
		}
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
