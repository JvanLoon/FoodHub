namespace FoodHub.DTOs;

/// <summary>
/// Canonical API route strings, shared by the API (route registration) and the Web client
/// (request URIs) so each route is defined exactly once — change a route here and both sides
/// follow.
///
/// Routes with a path parameter are exposed two ways:
///   * a <c>*Template</c> constant using the FastEndpoints "{id}" placeholder — used by the API
///     when registering the endpoint, and
///   * a helper method that substitutes a concrete value — used by the client to build the URI.
/// The helper derives its path from the template constant, so the template stays the single source.
///
/// Routes that take only query-string arguments (paging, search, email, …) expose the base path
/// as a constant; the client appends the query string itself.
/// </summary>
public static class ApiRoutes
{
    public static class Authentication
    {
        private const string Base = "api/authentication";
        public const string Login = Base + "/login";
        public const string Register = Base + "/register";
        public const string ResetPassword = Base + "/resetpassword";
        public const string CheckJwtToken = Base + "/checkjwttoken";
        public const string ToggleUser = Base + "/toggleUser";
    }

    public static class Recipe
    {
        private const string Base = "api/recipe";
        public const string GetAll = Base + "/getallrecipes";
        public const string Create = Base;
        public const string Update = Base;
        public const string UpdateName = Base + "/name";
        public const string AddIngredient = Base + "/ingredient";

        public const string GetByIdTemplate = Base + "/{id}";
        public const string DeleteRecipeTemplate = Base + "/deleterecipe/{id}";
        public const string DeleteIngredientTemplate = Base + "/deleteingredient/{id}";

        public static string GetById(Guid id) => GetByIdTemplate.Replace("{id}", id.ToString());
        public static string DeleteRecipe(Guid id) => DeleteRecipeTemplate.Replace("{id}", id.ToString());
        public static string DeleteIngredient(Guid recipeIngredientId) =>
            DeleteIngredientTemplate.Replace("{id}", recipeIngredientId.ToString());
    }

    public static class Ingredient
    {
        private const string Base = "api/ingredient";
        public const string GetAll = Base; // GET, paging/search via query string
        public const string Create = Base; // POST
        public const string Update = Base; // PUT

        public const string DeleteTemplate = Base + "/deleteingredient/{id}";
        public static string Delete(Guid id) => DeleteTemplate.Replace("{id}", id.ToString());
    }

    public static class Admin
    {
        private const string Base = "api/admin";
        public const string Users = Base + "/users";
        public const string AllRoles = Base + "/allroles";
        public const string UserRoles = Base + "/userroles";
    }

    public static class User
    {
        private const string Base = "api/user";
        public const string Users = Base + "/users";
        public const string AllRoles = Base + "/allroles";
    }

    public static class ImportExport
    {
        private const string Base = "api/importexport";
        public const string Import = Base + "/import";
        public const string Export = Base + "/export";
    }
}
