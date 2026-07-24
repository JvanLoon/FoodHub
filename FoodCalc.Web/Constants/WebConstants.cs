namespace FoodCalc.Web.Constants;

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
		/// <summary>
		/// Status-based fallbacks used by <c>AuthenticatedHttpClientService.StatusFallback</c> when a
		/// response body carries no usable message of its own. Wording is deliberately
		/// user-facing: it says what happened and what (if anything) the user can do, never what
		/// the server was doing internally.
		/// </summary>
		public static class Client
		{
			public const string GenericFailure = "Something went wrong. Please try again.";

			// --- 2xx Success ---
			public const string OK = "Done.";
			public const string Created = "Created successfully.";
			public const string Accepted = "Accepted — this is still being processed.";
			public const string NoContent = "Done. There was nothing to show.";

			// --- 4xx Client Errors ---
			public const string BadRequest = "The request was invalid.";
			public const string Unauthorized = "You are not signed in, or your session has expired.";
			public const string Forbidden = "You don't have permission to do that.";
			public const string NotFound = "The requested item was not found.";
			public const string RequestTimeout = "The request took too long. Please try again.";
			public const string Conflict = "That action conflicts with the current state.";
			public const string UnsupportedMediaType = "That file type isn't supported.";
			public const string TooManyRequests = "Too many requests. Please wait a moment and try again.";

			// Suggested additions — see the matching arms in StatusFallback.
			/// <summary>413 — import uploads can exceed the server's request body limit.</summary>
			public const string PayloadTooLarge = "That file is too large to upload.";

			/// <summary>405 — a route/verb mismatch; surfaces during endpoint refactors.</summary>
			public const string MethodNotAllowed = "That action isn't allowed here.";

			/// <summary>422 — if FastEndpoints' validation status is ever moved off 400.</summary>
			public const string UnprocessableEntity = "The request was understood but could not be processed.";

			// --- 5xx Server Errors ---
			public const string InternalServerError = "The server encountered an error. Please try again later.";
			public const string NotImplemented = "That feature isn't available yet.";
			public const string BadGateway = "The server got an invalid response upstream. Please try again later.";
			public const string ServiceUnavailable = "The service is temporarily unavailable. Please try again later.";

			public const string NetworkAuthenticationRequired =
				"Your network requires you to sign in before continuing.";

			/// <summary>504 — suggested addition; the upstream counterpart to <see cref="BadGateway"/>.</summary>
			public const string GatewayTimeout = "The server took too long to respond. Please try again later.";

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
		}

		/// <summary>Authentication.</summary>
		public static class Auth
		{
			public const string LoginFailed = "Failed to login. Invalid email or password.";
			public const string LoginInvalidResponse = "Login failed: invalid server response.";
			public const string RegisterSuccess = "Registration successful! You can now login.";
		}

		/// <summary>Role &amp; password management.</summary>
		public static class Roles
		{
			public const string Added = "Role added successfully!";
			public const string Removed = "Role removed successfully!";
			public const string PasswordReset = "Password reset successfully!";
		}

		/// <summary>Recipes.</summary>
		public static class Recipe
		{
			public const string Created = "Recipe created successfully!";
			public const string NameUpdated = "Recipe name updated successfully!";
			public const string NameRequired = "Please enter a recipe name.";
			public const string Deleted = "Recipe deleted successfully!";
			public const string Updated = "Recipe updated successfully!";
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
		}
	}

	/// <summary>Browser storage keys.</summary>
	public static class Storage
	{
		public const string AuthToken = "Authorization";
		public const string AggregatedIngredients = "aggregated-ingredients";
		public const string ShoppingListItems = "shopping-list-items";
		public const string PantryPreferences = "pantry-preferences";
	}

	/// <summary>Misc client-side constants.</summary>
	public static class Files
	{
		public const string ExportBaseName = "export";
	}
}