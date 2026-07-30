namespace FoodCalc.Api.Constants;

/// <summary>
/// Request-validation messages surfaced by the API's FluentValidation validators. Dutch:
/// these come back in the 400 body and are toasted verbatim by the Web client, so they are
/// user-facing copy. Every rule states its message explicitly — FluentValidation's built-in
/// defaults are English and would otherwise leak through.
/// Shared, entity-agnostic messages live in <see cref="Common"/> — parameterized where a
/// single template serves several entities. Scoped to this project — not shared with the
/// Web or Features layers.
/// </summary>
public static class ValidationMessages
{
    /// <summary>Messages reused across features.</summary>
    public static class Common
    {
        public const string NameRequired = "Geen naam opgegeven";

        private const string EntityIdRequiredTemplate = "Geen {0}-id opgegeven";

        public static string EntityIdRequired(Entity entityName) =>
            string.Format(EntityIdRequiredTemplate, DisplayName(entityName));

        private const string PageMinimumTemplate = "Paginanummer moet minstens {0} zijn";

        public static string PageMinimum(int minimum) => string.Format(PageMinimumTemplate, minimum);

        private const string PageSizeMinimumTemplate = "Paginagrootte moet minstens {0} zijn";

        public static string PageSizeMinimum(int minimum) => string.Format(PageSizeMinimumTemplate, minimum);

        /// <summary>The Dutch noun used inside <see cref="EntityIdRequired"/>.</summary>
        private static string DisplayName(Entity entity) => entity switch
        {
            Entity.Recipe => "recept",
            Entity.Ingredient => "ingrediënt",
            _ => entity.ToString()
        };
    }

    /// <summary>Identity / sign-in.</summary>
    public static class Account
    {
        public const string EmailRequired = "Vul een e-mailadres in";
        public const string EmailInvalid = "Vul een geldig e-mailadres in";
        public const string PasswordRequired = "Vul een wachtwoord in";

        private const string PasswordLengthTemplate = "Wachtwoord moet tussen {0} en {1} tekens lang zijn";

        public static string PasswordLength(int min, int max) => string.Format(PasswordLengthTemplate, min, max);

        /// <summary>Mirrors the DataAnnotations on RegisterDto / ResetPasswordDto.</summary>
        public const int PasswordMinLength = 6;

        public const int PasswordMaxLength = 100;
    }

    /// <summary>Roles &amp; moderation.</summary>
    public static class Review
    {
        public const string RoleRequired = "Geen rol opgegeven";
        public const string TargetIdRequired = "Geen id opgegeven";
    }

    /// <summary>Import / export.</summary>
    public static class ImportExport
    {
        public const string FormatRequired = "Geen exportformaat opgegeven";
    }
}

public enum Entity
{
    Recipe,
    Ingredient
}
