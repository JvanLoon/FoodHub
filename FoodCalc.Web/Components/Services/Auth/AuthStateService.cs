namespace FoodCalc.Web.Components.Services.Auth;

public class AuthStateService(AuthTokenService authTokenService)
{
    public event Func<Task>? OnAuthStateChanged;

    public async Task<bool> IsLoggedInAsync()
        => !await authTokenService.IsTokenExpiredAsync();

    public async Task<string?> GetEmailAsync()
        => await authTokenService.GetEmailAsync();

    public async Task<List<string>> GetRolesAsync()
        => await authTokenService.GetRolesAsync();

    public async Task<bool> IsAdminAsync()
    {
        var roles = await authTokenService.GetRolesAsync();
        return roles.Contains("Admin");
    }

    public async Task SignInAsync(string token)
    {
        await authTokenService.SetTokenAsync(token);
        await NotifyAuthStateChangedAsync();
    }

    public async Task SignOutAsync()
    {
        await authTokenService.RemoveTokenAsync();
        await NotifyAuthStateChangedAsync();
    }

    public async Task NotifyAuthStateChangedAsync()
    {
        if (OnAuthStateChanged == null) return;
        foreach (var handler in OnAuthStateChanged.GetInvocationList().Cast<Func<Task>>())
        {
            try { 
				await handler();
			}
            catch { }
        }
    }
}
