namespace FoodCalc.Api.Constants;

/// <summary>
/// Plain-text bodies the endpoints send directly via <c>Send.StringAsync</c>, outside the
/// ErrorOr pipeline. The Web client toasts these verbatim, so they are Dutch user-facing
/// copy — same rule as <see cref="ValidationMessages"/>. Domain failures raised by the
/// handlers live in FoodCalc.Features' ErrorMessages instead.
/// </summary>
public static class ResponseMessages
{
    /// <summary>Sign-in, password reset and account state.</summary>
    public static class Account
    {
        public const string UserNotFound = "Gebruiker niet gevonden";
        public const string EmailNotConfirmed = "E-mailadres is niet bevestigd";
        public const string InvalidPassword = "Ongeldig wachtwoord";
        public const string UserLockedOut = "Account is geblokkeerd";
        public const string PasswordReset = "Wachtwoord is opnieuw ingesteld.";
    }

    /// <summary>Requests that need a signed-in user the token did not supply.</summary>
    public static class Token
    {
        public const string NoUserInToken = "Geen gebruiker in het token";
        public const string InvalidUserId = "Ongeldig gebruikers-id.";
    }

    /// <summary>Import uploads.</summary>
    public static class Import
    {
        public const string NoFileUploaded = "Geen bestand geüpload.";
        public const string InvalidFileContent = "Ongeldige bestandsinhoud.";
        public const string OnlyJsonAccepted = "Alleen JSON-bestanden worden geaccepteerd.";
        public const string Succeeded = "Importeren gelukt.";
    }

    /// <summary>The deliberate-failure endpoint behind the admin panel's error-test tab.</summary>
    public static class ErrorTest
    {
        private const string _testErrorTemplate = "{0} => Testfout {1} van {2}.";

        public static string TestError(int statusCode, int index, int total) =>
            string.Format(_testErrorTemplate, statusCode, index, total);
    }
}
