namespace FoodCalc.Api.Constants;

/// <summary>
/// Request-validation messages surfaced by the API's FluentValidation validators.
/// Shared, entity-agnostic messages live in <see cref="Common"/> — parameterized where a
/// single template serves several entities. Scoped to this project — not shared with the
/// Web or Features layers.
/// </summary>
public static class ValidationMessages
{
    /// <summary>Messages reused across features.</summary>
    public static class Common
    {
        public const string NameRequired = "No name provided";

        private const string EntityIdRequiredTemplate = "No {0} id provided";
        public static string EntityIdRequired(string entityName) =>
            string.Format(EntityIdRequiredTemplate, entityName);
    }
}
