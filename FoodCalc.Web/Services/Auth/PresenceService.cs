using FoodHub.DTOs;

namespace FoodCalc.Web.Services.Auth;

/// <summary>
/// Keeps the signed-in user showing as online in the admin user list.
///
/// The API stamps presence on every authenticated call, which covers anyone who is clicking
/// around. This fills the other case: someone who signs in and then reads a page for ten minutes
/// makes no requests at all, and without a heartbeat their dot would go out while they are
/// plainly still there.
///
/// Scoped, so there is one per circuit and it dies with the browser tab.
/// </summary>
public sealed class PresenceService(
    AuthenticatedHttpClientService httpClient,
    ILogger<PresenceService> logger) : IAsyncDisposable
{
    /// <summary>
    /// Comfortably inside the API's three-minute presence window, so a single missed beat — a
    /// blip, a laptop lid — does not blink the dot off.
    /// </summary>
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(60);

    private CancellationTokenSource? _cts;
    private Task? _loop;

    /// <summary>Begins beating. Safe to call repeatedly; only the first call starts a loop.</summary>
    public void Start()
    {
        if (_loop is not null)
            return;

        _cts = new CancellationTokenSource();
        _loop = BeatAsync(_cts.Token);
    }

    /// <summary>Stops beating. Presence then lapses on its own once the API's window expires.</summary>
    public async Task StopAsync()
    {
        if (_cts is null)
            return;

        await _cts.CancelAsync();

        // The loop only ever swallows its own exceptions, so this awaits its completion rather
        // than its result.
        if (_loop is not null)
            try { await _loop; }
            catch (OperationCanceledException) {}

        _cts.Dispose();
        _cts = null;
        _loop = null;
    }

    /// <summary>
    /// Stops beating and tells the API the user is gone. Must run while the token is still in
    /// local storage — the call is authenticated, so dropping the token first would 401.
    /// </summary>
    public async Task SignalOfflineAsync()
    {
        await StopAsync();

        // Failure is not worth reporting: the user is leaving either way, and the presence window
        // takes the dot away within three minutes regardless.
        try { await httpClient.PostContentAsync(ApiRoutes.Authentication.SignOut); }
        catch (Exception ex) { logger.LogDebug(ex, "Sign-out presence ping failed"); }
    }

    private async Task BeatAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(Interval);

        try
        {
            while (await timer.WaitForNextTickAsync(ct))
            {
                // A beat that fails is not worth escalating — a dropped request costs nothing as
                // long as the next one lands, and a token that has expired means the layout is
                // about to redirect to the login page anyway.
                try { await httpClient.PostContentAsync(ApiRoutes.Authentication.Heartbeat); }
                catch (Exception ex) { logger.LogDebug(ex, "Heartbeat failed"); }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown: StopAsync, or the circuit going away.
        }
    }

    public async ValueTask DisposeAsync() => await StopAsync();
}
