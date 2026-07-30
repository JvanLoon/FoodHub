namespace FoodCalc.Features.Constants;

/// <summary>
/// Domain error messages produced by the command/query handlers in this project. Everything
/// here is Dutch: these strings travel to the client and end up in a toast, so they are
/// user-facing copy. Log messages stay in English — they are read by developers, not users.
/// The generic CRUD-style failures live in <see cref="Common"/> as entity-parameterized
/// templates; genuinely feature-specific wording stays in its feature group.
/// Not shared with other projects.
/// </summary>
public static class ErrorMessages
{
    /// <summary>
    /// Dutch entity nouns fed into the <see cref="Common"/> templates. Kept here rather than
    /// inline at the call sites so a noun is spelled — and translated — in exactly one place.
    /// Singular and lowercase: the templates capitalize where a sentence starts.
    /// </summary>
    public static class Entities
    {
        public const string Recipe = "recept";
        public const string Recipes = "recepten";
        public const string RecipeLine = "receptregel";
        public const string Ingredient = "ingrediënt";
        public const string Ingredients = "ingrediënten";
        public const string User = "gebruiker";
        public const string Users = "gebruikers";
        public const string Roles = "rollen";
        public const string MealPlan = "maaltijdplanning";
        public const string MealPlanEntry = "maaltijdplanning";
        public const string RandomizedMealPlan = "willekeurige maaltijdplanning";
        public const string ReviewStatus = "beoordelingsstatus";
        public const string ReviewQueue = "de beoordelingswachtrij";
        public const string RejectedItem = "afgekeurd item";
    }

    /// <summary>
    /// Entity-agnostic templates. Pass a noun from <see cref="Entities"/> — Dutch, singular
    /// or plural as the sentence needs it.
    /// </summary>
    public static class Common
    {
        private const string _notFoundTemplate = "{0} niet gevonden";

        public static string NotFound(string entityName) => string.Format(_notFoundTemplate, Capitalize(entityName));

        private const string _getAllFailedTemplate = "Ophalen van {0} mislukt";

        public static string GetAllFailed(string entityName) => string.Format(_getAllFailedTemplate, entityName);

        private const string _addFailedTemplate = "Toevoegen van {0} mislukt";

        public static string AddFailed(string entityName) => string.Format(_addFailedTemplate, entityName);

        private const string _updateFailedTemplate = "Bijwerken van {0} mislukt";

        public static string UpdateFailed(string entityName) => string.Format(_updateFailedTemplate, entityName);

        private const string _deleteFailedTemplate = "Verwijderen van {0} mislukt";

        public static string DeleteFailed(string entityName) => string.Format(_deleteFailedTemplate, entityName);

        // The nouns are stored lowercase so they read correctly mid-sentence; only the
        // templates that put one first need it capitalized.
        private static string Capitalize(string value) =>
            string.IsNullOrEmpty(value) ? value : char.ToUpperInvariant(value[0]) + value[1..];
    }

    /// <summary>Recipe-specific wording.</summary>
    public static class Recipe
    {
        public const string AddIngredientFailed = "Ingrediënt toevoegen aan recept mislukt";
        public const string UpdateForIngredientFailed = "Recept bijwerken mislukt";

        private const string _getByIdFailedTemplate = "Recept met id {0} ophalen mislukt";

        public static string GetByIdFailed(Guid id) => string.Format(_getByIdFailedTemplate, id);
    }

    /// <summary>Ingredient-specific wording.</summary>
    public static class Ingredient
    {
        public const string DeleteFromRecipeFailed = "Ingrediënt verwijderen uit recept mislukt";
    }

    /// <summary>Moderation / approval wording.</summary>
    public static class Review
    {
        private const string _notOwnedTemplate = "Je kunt alleen een {0} bewerken die je zelf hebt aangemaakt.";

        public static string NotOwned(string entityName) => string.Format(_notOwnedTemplate, entityName);

        public const string NoUser = "Een recept kan niet worden aangemaakt zonder ingelogde gebruiker.";

        private const string _unknownTargetTypeTemplate = "Onbekend beoordelingstype: {0}.";

        public static string UnknownTargetType(object targetType) =>
            string.Format(_unknownTargetTypeTemplate, targetType);
    }

    /// <summary>User-specific wording.</summary>
    public static class User
    {
        public const string GetByEmailFailed = "Gebruiker ophalen op e-mailadres mislukt";
        public const string BlackListAddFailed = "Recept verbergen mislukt";
        public const string BlackListRemoveFailed = "Recept weer zichtbaar maken mislukt";
    }

    /// <summary>Meal calendar.</summary>
    public static class MealPlan
    {
        private const string _maxPerDayTemplate = "Een dag kan maximaal {0} recepten bevatten.";

        public static string MaxPerDay(int max) => string.Format(_maxPerDayTemplate, max);

        public const string NoDaysSelected = "Selecteer minstens één dag om willekeurig te vullen.";
        public const string NoRecipesToPickFrom = "Er zijn geen recepten om uit te kiezen.";
    }

    /// <summary>Import / export.</summary>
    public static class ImportExport
    {
        public const string ExportFailed = "Exporteren van alle data mislukt";
        public const string ImportFailed = "Importeren van alle data mislukt";
        public const string NoImportData = "Geen data opgegeven om te importeren.";
    }
}
