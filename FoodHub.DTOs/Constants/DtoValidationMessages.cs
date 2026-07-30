namespace FoodHub.DTOs.Constants;

/// <summary>
/// Dutch messages for the DataAnnotations on the account DTOs. These sit in the DTO project
/// rather than with the rest of the UI copy because an attribute argument has to be a
/// compile-time constant from an assembly the DTOs can see — and FoodHub.DTOs is referenced
/// by everything, never the other way round.
///
/// Only Blazor renders them (via DataAnnotationsValidator on the register form); the API
/// validates the same rules through FluentValidation, whose wording lives in the API's
/// ValidationMessages. Keep the two in step.
/// </summary>
public static class DtoValidationMessages
{
    public const string EmailRequired = "Vul een e-mailadres in";
    public const string EmailInvalid = "Vul een geldig e-mailadres in";
    public const string PasswordRequired = "Vul een wachtwoord in";
    public const string PasswordLength = "Wachtwoord moet tussen {2} en {1} tekens lang zijn";

    public const int PasswordMinLength = 6;
    public const int PasswordMaxLength = 100;
}
