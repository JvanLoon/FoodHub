namespace FoodCalc.Web.Constants;

/// <summary>
/// User-facing messages and magic-string keys used by the Web client. Scoped to this
/// project — messages are intentionally not shared with the API or Features layers, since
/// the wording shown to the user is a UI concern.
///
/// All copy here is Dutch. This file holds what <em>happened</em> (toasts, confirmations);
/// what a page <em>says</em> — headings, labels, buttons — lives in <see cref="UiText"/>.
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
            public const string GenericFailure = "Er is iets misgegaan. Probeer het opnieuw.";

            // --- 2xx Success ---
            public const string OK = "Gelukt.";
            public const string Created = "Succesvol aangemaakt.";
            public const string Accepted = "Geaccepteerd — dit wordt nog verwerkt.";
            public const string NoContent = "Klaar. Er was niets om te tonen.";

            // --- 4xx Client Errors ---
            public const string BadRequest = "De aanvraag was ongeldig.";
            public const string Unauthorized = "Je bent niet ingelogd, of je sessie is verlopen.";
            public const string Forbidden = "Je hebt geen rechten om dat te doen.";
            public const string NotFound = "Het gevraagde item is niet gevonden.";
            public const string RequestTimeout = "De aanvraag duurde te lang. Probeer het opnieuw.";
            public const string Conflict = "Die actie botst met de huidige status.";
            public const string UnsupportedMediaType = "Dat bestandstype wordt niet ondersteund.";
            public const string TooManyRequests = "Te veel aanvragen. Wacht even en probeer het opnieuw.";

            // Suggested additions — see the matching arms in StatusFallback.
            /// <summary>413 — import uploads can exceed the server's request body limit.</summary>
            public const string PayloadTooLarge = "Dat bestand is te groot om te uploaden.";

            /// <summary>405 — a route/verb mismatch; surfaces during endpoint refactors.</summary>
            public const string MethodNotAllowed = "Die actie is hier niet toegestaan.";

            /// <summary>422 — if FastEndpoints' validation status is ever moved off 400.</summary>
            public const string UnprocessableEntity = "De aanvraag is begrepen maar kon niet worden verwerkt.";

            // --- 5xx Server Errors ---
            public const string InternalServerError = "Er is een serverfout opgetreden. Probeer het later opnieuw.";
            public const string NotImplemented = "Die functie is nog niet beschikbaar.";

            public const string BadGateway =
                "De server kreeg een ongeldig antwoord van een andere server. Probeer het later opnieuw.";

            public const string ServiceUnavailable =
                "De service is tijdelijk niet beschikbaar. Probeer het later opnieuw.";

            public const string NetworkAuthenticationRequired =
                "Je netwerk vereist dat je eerst inlogt voordat je verder kunt.";

            /// <summary>504 — suggested addition; the upstream counterpart to <see cref="BadGateway"/>.</summary>
            public const string GatewayTimeout =
                "De server deed er te lang over om te antwoorden. Probeer het later opnieuw.";

            public static string RequestFailed(int statusCode) => $"Aanvraag mislukt ({statusCode}).";
        }

        /// <summary>Import / export.</summary>
        public static class ImportExport
        {
            public const string NoFileContent = "Geen bestandsinhoud.";
            public const string ExportUnexpectedResponse = "Exporteren mislukt: de server gaf een onverwacht antwoord.";
            public const string ExportEmpty = "Exporteren mislukt: de bestandsinhoud is leeg.";
            public const string PreparingExport = "Export voorbereiden...";
            public const string SelectFileFirst = "Selecteer eerst een bestand.";
            public const string OnlyJsonAccepted = "Alleen .json-bestanden worden geaccepteerd.";
            public const string CouldNotReadFile = "Het geselecteerde bestand kon niet worden gelezen.";
            public const string ImportSucceeded = "Importeren gelukt.";

            public static string ExportFailed(string detail) => $"Exporteren mislukt: {detail}";

            public static string ImportFailed(string detail) => $"Importeren mislukt: {detail}";
        }

        /// <summary>Authentication.</summary>
        public static class Auth
        {
            public const string LoginFailed = "Inloggen mislukt. Ongeldig e-mailadres of wachtwoord.";
            public const string LoginInvalidResponse = "Inloggen mislukt: ongeldig antwoord van de server.";
            /// <summary>
            /// A fresh IdentityUser starts with EmailConfirmed = false and LoginEndpoint
            /// refuses those, so the account really is unusable until an admin enables it —
            /// the old "je kunt nu inloggen" was wrong.
            /// </summary>
            public const string RegisterSuccess = "Account aangemaakt, In afwachting van goedkeuren";
        }

        /// <summary>User administration.</summary>
        public static class Users
        {
            public const string Deleted = "Gebruiker verwijderd.";
        }

        /// <summary>Role &amp; password management.</summary>
        public static class Roles
        {
            public const string Added = "Rol toegevoegd!";
            public const string Removed = "Rol verwijderd!";
            public const string PasswordReset = "Wachtwoord opnieuw ingesteld!";
        }

        /// <summary>Recipes.</summary>
        public static class Recipe
        {
            public const string Created = "Recept aangemaakt!";
            public const string NameUpdated = "Receptnaam bijgewerkt!";
            public const string NameRequired = "Vul een receptnaam in.";
            public const string Deleted = "Recept verwijderd!";
            public const string Updated = "Recept bijgewerkt!";
        }

        /// <summary>Moderation queue.</summary>
        public static class Review
        {
            public const string Approved = "Goedgekeurd.";
            public const string Rejected = "Afgekeurd en verwijderd.";

            public const string PendingApproval =
                "Ingediend ter goedkeuring — alleen jij ziet dit totdat een moderator het goedkeurt.";

            public static string LineRemoved(string lineName, string? recipeName) =>
                $"“{lineName}” verwijderd uit {recipeName}.";
        }

        /// <summary>Ingredients.</summary>
        public static class Ingredient
        {
            public const string AddedOrUpdated = "Ingrediënt toegevoegd/bijgewerkt";
            public const string Deleted = "Ingrediënt verwijderd";
            public const string Added = "Ingrediënt toegevoegd";
            public const string Updated = "Ingrediënt bijgewerkt!";
            public const string NameUpdated = "Ingrediëntnaam bijgewerkt!";
            public const string DeletedWithReload = "Ingrediënt verwijderd!";
        }

        /// <summary>Meal calendar. Fallbacks for when the API returns no message of its own.</summary>
        public static class Calendar
        {
            public const string AddFailed = "Kon het recept niet toevoegen.";
            public const string RemoveFailed = "Kon het recept niet verwijderen.";
            public const string RandomizeFailed = "Willekeurig vullen is mislukt.";

            public static string Randomized(int recipeCount, int dayCount)
            {
                var recipes = recipeCount == 1 ? "1 recept" : $"{recipeCount} recepten";
                var days = dayCount == 1 ? "1 dag" : $"{dayCount} dagen";
                return $"{recipes} toegevoegd over {days}.";
            }

            /// <summary>
            /// One message for a batch of single-day adds. There is no bulk endpoint, so a
            /// save is N calls and can succeed only partly — saying so is more useful than a
            /// row of identical toasts or a success that quietly lost two days.
            /// </summary>
            public static string AddedToDays(int added, int failed)
            {
                var days = added == 1 ? "1 dag" : $"{added} dagen";
                return failed == 0
                    ? $"Toegevoegd aan {days}."
                    : $"Toegevoegd aan {days}. {failed} mislukt.";
            }
        }
    }

    /// <summary>
    /// Browser storage keys. Nothing to do with authentication any more — the token lives in an
    /// httpOnly cookie the browser cannot read, and what is left here is page state that is the
    /// user's own business.
    /// </summary>
    public static class Storage
    {
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
