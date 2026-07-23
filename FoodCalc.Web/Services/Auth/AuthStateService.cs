namespace FoodCalc.Web.Services.Auth;

public class AuthStateService(AuthTokenService authTokenService)
{
	public event Func<Task>? OnAuthStateChanged;

	public async Task<bool> IsLoggedInAsync() => !await authTokenService.IsTokenExpiredAsync();

	public async Task<string?> GetEmailAsync() => await authTokenService.GetEmailAsync();

	public async Task<List<string>> GetRolesAsync() => await authTokenService.GetRolesAsync();

	public async Task<bool> IsAdminAsync()
	{
		var roles = await authTokenService.GetRolesAsync();
		return roles.Contains("Admin");
	}

	/// <summary>True if the logged-in user holds at least one of the given roles.</summary>
	public async Task<bool> IsInAnyRoleAsync(params string[] roles)
	{
		var mine = await authTokenService.GetRolesAsync();
		return mine.Any(r => roles.Contains(r, StringComparer.OrdinalIgnoreCase));
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
		foreach (var handler in OnAuthStateChanged.GetInvocationList()
												  .Cast<Func<Task>>())
		{
			try { await handler(); }
			catch {}
		}
	}
}