using System.Globalization;
using FoodHub.DTOs;

namespace FoodCalc.Web.Constants;

/// <summary>
/// Every piece of static Dutch copy rendered by the Web client: page titles, headings,
/// labels, placeholders, button captions and the accessible names behind icon-only
/// controls. Transient toast wording lives in <see cref="WebConstants.Messages"/> instead —
/// this file is what the page says, that one is what happened.
///
/// Grouped per page or component so a screen's copy sits together. Anything that varies at
/// runtime (a count, a name, a date) is a method rather than a const, so plurals and word
/// order stay in Dutch instead of being stitched together at the call site.
/// </summary>
public static class UiText
{
    /// <summary>
    /// Dutch culture, used explicitly wherever a date or month name is formatted. The app's
    /// thread culture is deliberately left alone — switching it would also swap the decimal
    /// separator, and the ingredient amount inputs parse with the invariant one.
    /// </summary>
    public static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("nl-NL");

    /// <summary>Wording shared by several screens.</summary>
    public static class Common
    {
        public const string Save = "Opslaan";
        public const string Cancel = "Annuleren";
        public const string Confirm = "Bevestigen";
        public const string Delete = "Verwijderen";
        public const string Remove = "Verwijderen";
        public const string Edit = "Bewerken";
        public const string Add = "Toevoegen";
        public const string Open = "Openen";
        public const string Back = "Terug";
        public const string Close = "Sluiten";
        public const string Clear = "Wissen";
        public const string Name = "Naam";
        public const string Actions = "Acties";
        public const string Email = "E-mail";
        public const string Loading = "Laden...";
        public const string Nothing = "Nog niets hier.";
        public const string Recipes = "Recepten";
        public const string Ingredients = "Ingrediënten";
    }

    /// <summary>Shell: sidebar, top bar and the Blazor circuit-error strip.</summary>
    public static class Layout
    {
        public const string NavigationMenu = "Navigatiemenu";
        public const string QuickNavigation = "Snelnavigatie";
        public const string Login = "Inloggen";
        public const string Logout = "Uitloggen";
        public const string UnhandledError = "Er is een onverwachte fout opgetreden.";
        public const string Reload = "Herladen";

        /// <summary>
        /// Shown in place of a page that threw while rendering. Distinct from
        /// <see cref="UnhandledError"/>: that one is the circuit dying, this one is a single
        /// page failing while the rest of the app keeps working.
        /// </summary>
        public const string ComponentError =
            "Deze pagina kon niet worden geladen. Probeer het opnieuw of ga terug naar Start.";
    }

    /// <summary>Sidebar entries.</summary>
    public static class Nav
    {
        public const string Home = "Start";
        public const string Calendar = "Kalender";
        public const string Recipes = "Recepten";
        public const string FindRecipes = "Recepten zoeken";

        /// <summary>
        /// Bottom-bar form of <see cref="FindRecipes"/>. A third of a 375px screen is not
        /// enough for "Recepten zoeken" without it wrapping or being clipped.
        /// </summary>
        public const string FindRecipesShort = "Zoeken";

        public const string AddRecipe = "Recept toevoegen";

        /// <summary>
        /// One label for admins and moderators alike: the entry leads to the same page and
        /// the tab set inside it is what actually differs per role.
        /// </summary>
        public const string Admin = "Beheer";

        public const string UserSettings = "Instellingen";
    }

    /// <summary>Defaults and accessible names baked into the shared components.</summary>
    public static class Components
    {
        // SearchBox
        public const string SearchPlaceholder = "Zoeken...";
        public const string ClearSearch = "Zoekopdracht wissen";

        // Paginator
        public const string PageNavigation = "Paginanavigatie";
        public const string FirstPage = "Eerste pagina";
        public const string PreviousPage = "Vorige pagina";
        public const string NextPage = "Volgende pagina";
        public const string LastPage = "Laatste pagina";
        public const string PerPage = "Per pagina:";

        // ThemeToggle
        public const string SwitchToLight = "Overschakelen naar lichte modus";
        public const string SwitchToDark = "Overschakelen naar donkere modus";

        // TextField password reveal
        public const string HidePassword = "Wachtwoord verbergen";
        public const string ShowPassword = "Wachtwoord tonen";

        // ToastHost
        public const string DismissToast = "Melding sluiten";
    }

    /// <summary>Login and registration.</summary>
    public static class Auth
    {
        public const string LoginTitle = "Inloggen";
        public const string LoginHeading = "Welkom terug";
        public const string EmailLabel = "E-mailadres";
        public const string EmailPlaceholder = "Vul je e-mailadres in";
        public const string PasswordLabel = "Wachtwoord";
        public const string PasswordPlaceholder = "Vul je wachtwoord in";
        public const string LoginButton = "Inloggen";
        public const string CreateAccountLink = "Account aanmaken";

        public const string RegisterTitle = "Registreren";
        public const string RegisterHeading = "Maak je account aan";
        public const string RegisterButton = "Registreren";
        public const string HaveAccountLink = "Heb je al een account? Inloggen";
    }

    /// <summary>Landing page and admin dashboard.</summary>
    public static class Home
    {
        public const string Title = "Start";
        public const string WelcomeTitle = "Welkom bij FoodHub";
        public const string WelcomeSubtitle = "Plan je maaltijden en vind iets om te koken.";

        public const string DashboardTitle = "Dashboard";
        public const string DashboardSubtitle = "Een overzicht van je FoodHub-bibliotheek.";
        public const string LoadingDashboard = "Dashboard laden...";

        public const string MealCalendar = "Maaltijdkalender";
        public const string MealCalendarHint = "Plan wat je elke dag eet";
        public const string Recipes = "Recepten";
        public const string RecipesHint = "Blader door de receptenbibliotheek";
        public const string FindRecipes = "Recepten zoeken";
        public const string FindRecipesHint = "Zoek op de ingrediënten die je hebt";

        public const string Ingredients = "Ingrediënten";
        public const string Users = "Gebruikers";
        public const string AvgIngredientsPerRecipe = "Gem. ingrediënten per recept";
        public const string OnShoppingList = "Op boodschappenlijst";
        public const string RecipeIngredientLinks = "Recept-ingrediëntkoppelingen";
        public const string TopIngredients = "Top-ingrediënten";
        public const string TopIngredientsHint = "Meest gebruikt in alle recepten";

        public static string OfIngredients(int total) => $"van {total} ingrediënten";

        public static string UsedInRecipes(int count) => count == 1 ? "1 recept" : $"{count} recepten";
    }

    /// <summary>Recipe overview.</summary>
    public static class Recipes
    {
        public const string Title = "Recepten";
        public const string SearchPlaceholder = "Recepten zoeken...";
        public const string GenerateShoppingList = "Boodschappenlijst maken";

        /// <summary>
        /// Why the shopping-list button is inactive. Rendered as a line under the header
        /// while nothing is selected, and gone once something is — it replaced the page's
        /// permanent subtitle, which said the same thing whether or not it still applied.
        /// </summary>
        public const string ShoppingListHint =
            "Selecteer een of meerdere recepten om een boodschappenlijst te maken";
        public const string Loading = "Recepten laden...";
        public const string AwaitingApproval = "Wacht op goedkeuring";
        public const string IngredientsLabel = "Ingrediënten";
        public const string EmptyTitle = "Geen recepten gevonden.";
        public const string EmptyMessage = "Probeer een andere zoekopdracht of voeg een nieuw recept toe.";

        public const string DeleteTitle = "Recept verwijderen";
        public const string DeleteMessage = "Weet je zeker dat je dit recept wilt verwijderen?";
    }

    /// <summary>Read-only recipe page.</summary>
    public static class RecipeDetail
    {
        public const string Fallback = "Recept";
        public const string Subtitle = "Receptdetails";
        public const string Loading = "Recept laden...";
        public const string NotFoundTitle = "Recept niet gevonden.";
        public const string NotFoundMessage = "Dit recept is mogelijk verwijderd.";
        public const string Ingredients = "Ingrediënten";
        public const string NoIngredients = "Dit recept heeft nog geen ingrediënten.";
    }

    /// <summary>Create / edit recipe.</summary>
    public static class RecipeEditor
    {
        public const string CreateTitle = "Recept aanmaken";
        public const string EditTitle = "Recept bewerken";
        public const string EditNameTitle = "Receptnaam bewerken";
        public const string NamePlaceholder = "bijv. Spaghetti bolognese";
        public const string NameFirstTitle = "Geef je recept eerst een naam.";
        public const string NameFirstMessage = "Sla een naam op en de ingrediënteneditor verschijnt.";

        public const string SearchIngredientLabel = "Ingrediënt zoeken";
        public const string SearchIngredientPlaceholder = "Typ om te zoeken...";

        /// <summary>Deliberately English — asked for by name.</summary>
        public const string ClearForm = "Clear";

        /// <summary>
        /// There is no "+" button any more: an unknown name is created on its way into the
        /// recipe, so the empty dropdown says what will happen rather than what to press.
        /// </summary>
        public const string NoIngredientsFound =
            "Geen ingrediënten gevonden. Dit ingrediënt wordt aangemaakt zodra je het toevoegt.";

        public const string AmountLabel = "Hoeveelheid";
        public const string AmountTypeLabel = "Eenheid";
        public const string AddOrSave = "Toevoegen/Opslaan";
        public const string EditIngredient = "Ingrediënt bewerken";
        public const string DeleteIngredient = "Ingrediënt verwijderen";
        public const string NoIngredientsTitle = "Nog geen ingrediënten.";

        public const string NoIngredientsMessage =
            "Zoek of voeg links een ingrediënt toe om dit recept op te bouwen.";
    }

    /// <summary>Ingredient catalog (staff only).</summary>
    public static class Ingredients
    {
        public const string Title = "Ingrediënten";
        public const string SearchPlaceholder = "Ingrediënten zoeken...";
        public const string Empty = "Geen ingrediënten gevonden.";
        public const string Loading = "Ingrediënten laden...";
        public const string ColumnName = "Naam";
        public const string ColumnShoppingList = "Boodschappenlijst";
        public const string ColumnActions = "Acties";
    }

    /// <summary>Find recipes by the ingredients you have.</summary>
    public static class FindRecipes
    {
        public const string Title = "Recepten zoeken";
        public const string Subtitle = "Voeg de ingrediënten toe die je hebt en ontdek wat je kunt koken.";

        public const string MyIngredients = "Mijn ingrediënten";
        public const string AddPlaceholder = "bijv. eieren, bloem, tomaat...";
        public const string AddAriaLabel = "Een ingrediënt toevoegen";
        public const string AddIngredient = "Ingrediënt toevoegen";
        public const string NothingAdded = "Nog niets toegevoegd. Typ hierboven een ingrediënt en druk op Enter.";
        public const string ClearAll = "Alles wissen";

        public static string AddQuoted(string value) => $"“{value}” toevoegen";

        public static string RemoveAriaLabel(string ingredient) => $"{ingredient} verwijderen";

        public const string Options = "Opties";
        public const string MatchLabel = "Overeenkomst";
        public const string MatchAny = "Bevat een van mijn ingrediënten";
        public const string MatchAll = "Moet al mijn ingrediënten bevatten";
        public const string SortLabel = "Sorteren op";
        public const string SortMostMatches = "Meeste van mijn ingrediënten";
        public const string SortFewestMissing = "Minste ontbrekende ingrediënten";
        public const string SortName = "Naam (A–Z)";
        public const string HighlightLabel = "Mijn ingrediënten in elk recept markeren";

        public const string Loading = "Recepten laden...";
        public const string NoIngredientsTitle = "Voeg ingrediënten toe";

        public const string NoIngredientsMessage =
            "Stel links je ingrediëntenlijst samen om passende recepten te zien.";

        public const string NoMatchesTitle = "Geen passende recepten";

        public const string NoMatchesAll =
            "Geen enkel recept bevat al die ingrediënten. Zet Overeenkomst op “een van”.";

        public const string NoMatchesAny = "Geen van je ingrediënten komt nog voor in een recept.";

        public const string HaveEverything = "Je hebt alles wat erop staat";

        public static string Found(int count) => count == 1 ? "1 recept gevonden" : $"{count} recepten gevonden";

        public static string StillNeeded(int count) =>
            count == 1 ? "nog 1 ingrediënt nodig" : $"nog {count} ingrediënten nodig";
    }

    /// <summary>Meal calendar.</summary>
    public static class Calendar
    {
        public const string Title = "Maaltijdkalender";

        public const string Subtitle =
            "Plan een recept voor elke dag. Selecteer twee of meer dagen om willekeurig te vullen.";

        public const string ViewAriaLabel = "Kalenderweergave";
        public const string Week = "Week";
        public const string Month = "Maand";
        public const string Randomize = "Willekeurig vullen";
        public const string Randomizing = "Bezig met vullen...";
        public const string ClearSelection = "Wis selectie";
        public const string Loading = "Kalender laden...";

        public const string PreviousWeek = "Vorige week";
        public const string NextWeek = "Volgende week";
        public const string PreviousMonth = "Vorige maand";
        public const string NextMonth = "Volgende maand";

        /// <summary>Monday-first, matching <c>StartOfWeek</c> in the page.</summary>
        public static readonly string[] DayHeaders = ["ma", "di", "wo", "do", "vr", "za", "zo"];

        public const string SearchRecipes = "Recepten zoeken...";
        public const string NoRecipesTitle = "Geen recepten";
        public const string NoRecipesMessage = "Er komt niets overeen met die zoekopdracht.";

        public static string AddModalTitle(DateOnly date) =>
            $"Recept toevoegen — {date.ToString("dddd d MMM", UiText.Culture)}";

        public const string AddRecipe = "Recept toevoegen";
        public const string OpenRecipe = "Recept openen";
        public const string RemoveRecipe = "Verwijderen";

        // ---- Add-to-calendar picker (from a recipe card or the recipe page) ----

        public const string AddToCalendar = "Toevoegen aan kalender";
        public const string AlreadyPlanned = "Dit recept staat al op deze dag";

        public const string PickDaysHint =
            "Kies de dagen waarop je dit wilt eten. Dagen waarop het al staat, kun je niet kiezen.";

        public static string AddToCalendarTitle(string recipeName) => $"Toevoegen aan kalender — {recipeName}";

        public static string SaveDays(int count) => count == 0 ? "Opslaan" : $"Opslaan ({count})";

        public static string DaysSelected(int count) =>
            count == 1 ? "1 dag geselecteerd" : $"{count} dagen geselecteerd";

        public static string NothingSelected() => "Nog geen dagen geselecteerd";

        public const string RandomizeTitle = "Geselecteerde dagen willekeurig vullen";

        public static string RandomizeIntro(int dayCount) =>
            dayCount == 1
                ? "Vult de geselecteerde dag met een willekeurig recept uit je bibliotheek."
                : $"Vult de {dayCount} geselecteerde dagen met willekeurige recepten uit je bibliotheek.";

        public const string WantedIngredients = "Gewenste ingrediënten";
        public const string Optional = "(optioneel)";
        public const string IngredientPlaceholder = "Kies of typ een ingrediënt...";
        public const string ToggleIngredientList = "Ingrediëntenlijst tonen of verbergen";
        public const string NoIngredientsAvailable = "Geen ingrediënten beschikbaar";
        public const string NoMatches = "Geen overeenkomsten";
        public const string RecipesPerDay = "Recepten per dag";
        public const string OverwriteLabel = "Bestaande recepten op de geselecteerde dagen overschrijven";
        
        public const string UniqueOnlyLabel = "Elke dag wat anders";

        public static string WeekLabel(DateOnly start) =>
            $"{start.ToString("d MMM", UiText.Culture)} – {start.AddDays(6).ToString("d MMM yyyy", UiText.Culture)}";

        public static string MonthLabel(DateOnly anchor) => anchor.ToString("MMMM yyyy", UiText.Culture);

        public static string DayLabel(DateOnly day) => day.ToString("ddd d", UiText.Culture);
    }

    /// <summary>Printable shopping list.</summary>
    public static class ShoppingList
    {
        public const string AddPlaceholder = "Eigen item toevoegen...";
        public const string RemoveItem = "Item verwijderen";
        public const string EmptyTitle = "Geen ingrediënten om te tonen.";

        public const string EmptyMessage =
            "Selecteer recepten op de receptenpagina of voeg hierboven eigen items toe.";
    }

    /// <summary>Admin panel shell.</summary>
    public static class Admin
    {
        public const string PageTitle = "Beheer";
        public const string AdminHeading = "Beheerpaneel";
        public const string ModerationHeading = "Moderatie";
        public const string TabReview = "Beoordelen";
        public const string TabIngredients = "Ingrediënten";
        public const string TabUsers = "Gebruikers";
        public const string TabImportExport = "Import/Export";
        public const string TabErrorTest = "Fouttest";
    }

    /// <summary>User administration.</summary>
    public static class Users
    {
        public const string SearchPlaceholder = "Gebruikers zoeken...";
        public const string Loading = "Gebruikers laden...";
        public const string Empty = "Geen gebruikers gevonden.";
        public const string DeleteTitle = "Gebruiker verwijderen";
        public const string DeleteConfirm = "Verwijderen";

        public static string DeleteMessage(string? email) =>
            $"Account “{email}” definitief verwijderen? De maaltijdkalender van deze gebruiker " +
            "verdwijnt mee. Ingediende recepten blijven bestaan. Dit kan niet ongedaan worden gemaakt.";
    }

    /// <summary>Role management and password reset.</summary>
    public static class Roles
    {
        public const string PageTitle = "Gebruikersrollen";
        public const string Heading = "Rollen beheren";
        public const string Loading = "Rollen laden...";
        public const string AddRoleHeader = "Rol toevoegen";
        public const string RoleLabel = "Rol";
        public const string SelectRole = "Kies een rol...";
        public const string AssignedRolesHeader = "Toegewezen rollen";
        public const string RemoveRole = "Rol verwijderen";
        public const string NoRoles = "Geen rollen toegewezen";
        public const string ResetPasswordHeader = "Wachtwoord opnieuw instellen";
        public const string NewPasswordLabel = "Nieuw wachtwoord";
        public const string ResetButton = "Opnieuw instellen";
    }

    /// <summary>Import / export tab.</summary>
    public static class ImportExport
    {
        public const string PageTitle = "Data importeren/exporteren";
        public const string ExportHeader = "Exporteren";
        public const string ExportDescription = "Download alle data als .json-bestand.";
        public const string ExportButton = "Exporteren";
        public const string ImportHeader = "Importeren";
        public const string ImportFileLabel = "Importbestand (.json)";
        public const string ImportButton = "Importeren";
        public const string ImportOldButton = "Importeren (oud formaat)";

        public const string OldFormatHint =
            "Gebruik “oud formaat” voor exports van vóór de ingrediëntenherziening.";
    }

    /// <summary>Moderation queue.</summary>
    public static class Review
    {
        public const string Loading = "Beoordelingswachtrij laden...";
        public const string NothingTitle = "Niets te beoordelen.";
        public const string NothingMessage = "Nieuwe recepten verschijnen hier zodra ze zijn ingediend.";
        public const string RecipesHeading = "Recepten";
        public const string BadgeNew = "Nieuw";
        public const string BadgeEdited = "Bewerkt";
        public const string BadgeChanged = "gewijzigd";
        public const string ApprovedTitle = "Goedgekeurd";
        public const string ApproveIngredient = "Dit ingrediënt goedkeuren";
        public const string RejectIngredient = "Dit ingrediënt afkeuren en verwijderen";
        public const string NoIngredients = "Dit recept heeft nog geen ingrediënten.";
        public const string ApproveRecipe = "Recept goedkeuren";
        public const string Reject = "Afkeuren";

        public const string PendingHint =
            "Keur elk gewijzigd ingrediënt goed of af om het recept te kunnen goedkeuren.";

        public const string RejectTitle = "Recept afkeuren";

        public static string Waiting(int count) => count == 1 ? "1 in de wachtrij" : $"{count} in de wachtrij";

        public static string SubmittedBy(string email, DateTime modifiedUtc) =>
            $"door {email} · ingediend {modifiedUtc.ToLocalTime().ToString("g", UiText.Culture)}";

        public static string RejectMessage(string? recipeName) =>
            $"“{recipeName}” afkeuren en verwijderen? Dit kan niet ongedaan worden gemaakt.";
    }

    /// <summary>Error-test tab: a developer tool, but still rendered to a person.</summary>
    public static class ErrorTest
    {
        public const string Intro =
            "Laat de API falen via hetzelfde pad dat elk echt endpoint gebruikt. Elke fout hoort " +
            "als eigen melding te verschijnen én in het resultaat hieronder te staan.";

        public const string BulletPositiveCount = "Aantal > 0";

        public const string BulletPositiveCountText =
            " — de server levert de teksten aan, dus je test de parser: n fouten erin, n meldingen eruit.";

        public const string BulletZeroCount = "Aantal = 0";

        public const string BulletZeroCountText =
            " — de server stuurt een lege body, dus de client valt terug op zijn eigen tekst voor die " +
            "status. Zo controleer je de formuleringen in ";

        public const string BulletSuccessStatus = "Een 2xx-status";

        public const string BulletSuccessStatusText =
            " — altijd een succes, ongeacht het aantal. De server geeft zijn statustekst terug, die " +
            "als melding verschijnt.";

        public const string CountLabel = "Aantal fouten";
        public const string StatusCodeLabel = "Statuscode";
        public const string GroupSuccess = "2xx — Succes";
        public const string GroupClient = "4xx — Client";
        public const string GroupServer = "5xx — Server";
        public const string TriggerButton = "Uitvoeren";
        public const string LastResult = "Laatste resultaat";
        public const string Succeeded = "Gelukt";
        public const string Failed = "Mislukt";
        public const string NoErrorsReturned = "Geen fouten teruggegeven";

        public static string ErrorsParsed(int count) =>
            count == 1 ? "1 fout uit het antwoord gehaald:" : $"{count} fouten uit het antwoord gehaald:";

        // Reason phrases mirror the client-side fallbacks in WebConstants.Messages.Client.
        public const string Status200 = "200 — OK";
        public const string Status201 = "201 — Aangemaakt";
        public const string Status202 = "202 — Geaccepteerd";
        public const string Status204 = "204 — Geen inhoud";
        public const string Status400 = "400 — Ongeldige aanvraag";
        public const string Status401 = "401 — Niet ingelogd";
        public const string Status403 = "403 — Geen rechten";
        public const string Status404 = "404 — Niet gevonden";
        public const string Status405 = "405 — Methode niet toegestaan";
        public const string Status408 = "408 — Aanvraag verlopen";
        public const string Status409 = "409 — Conflict";
        public const string Status413 = "413 — Inhoud te groot";
        public const string Status415 = "415 — Bestandstype niet ondersteund";
        public const string Status422 = "422 — Niet verwerkbaar";
        public const string Status429 = "429 — Te veel aanvragen";
        public const string Status418 = "418 — Ik ben een theepot (niet toegewezen)";
        public const string Status500 = "500 — Interne serverfout";
        public const string Status501 = "501 — Niet geïmplementeerd";
        public const string Status502 = "502 — Ongeldig antwoord upstream";
        public const string Status503 = "503 — Service niet beschikbaar";
        public const string Status504 = "504 — Time-out upstream";
        public const string Status511 = "511 — Netwerkauthenticatie vereist";
    }

    /// <summary>Account settings.</summary>
    public static class UserSettings
    {
        public const string Title = "Gebruikersinstellingen";
        public const string Loading = "Instellingen laden...";
        public const string AccountHeader = "Account";
        public const string EmailLabel = "E-mail";
    }

    /// <summary>Hidden-recipe list.</summary>
    public static class Blacklist
    {
        public const string Header = "Recepten die je niet wilt zien";
        public const string Empty = "Geen verborgen recepten.";
        public const string SelectRecipe = "Kies een recept...";
    }

    /// <summary>Server-error page.</summary>
    public static class ErrorPage
    {
        public const string Title = "Fout";
        public const string Heading = "Er is iets misgegaan.";
        public const string Message = "Er is een fout opgetreden bij het verwerken van je aanvraag.";
        public const string RequestIdLabel = "Aanvraag-ID:";

        public const string DevHintBefore = "Overschakelen naar de ";
        public const string DevHintEnvironment = "Development";

        public const string DevHintAfter =
            "-omgeving (zet ASPNETCORE_ENVIRONMENT=Development en herstart de app) toont uitgebreide " +
            "foutinformatie. Zet dit niet aan voor gepubliceerde applicaties — het kan gevoelige " +
            "informatie uit excepties tonen aan eindgebruikers.";
    }

    /// <summary>
    /// Dutch labels for <see cref="IngredientAmountTypeDto"/>. The enum members keep their
    /// English names — they are persisted and serialized — so the translation lives here.
    /// </summary>
    public static class AmountTypes
    {
        public static string Label(IngredientAmountTypeDto amountType) => amountType switch
        {
            IngredientAmountTypeDto.None => "Geen",
            IngredientAmountTypeDto.Gram => "Gram",
            IngredientAmountTypeDto.Kilogram => "Kilogram",
            IngredientAmountTypeDto.Liter => "Liter",
            IngredientAmountTypeDto.Milliliter => "Milliliter",
            IngredientAmountTypeDto.Piece => "Stuk",
            _ => amountType.ToString()
        };
    }
}
