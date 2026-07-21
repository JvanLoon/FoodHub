using ErrorOr;

using FastEndpoints;

namespace FoodCalc.Api.Endpoints.Dev;

public class ErrorTestRequest
{
	/// <summary>How many errors to return. 0 (or less) returns a success response instead.</summary>
	[BindFrom("count")]
	public int Count { get; set; } = 1;

	/// <summary>Status code to fail with, so the client's status fallbacks can be exercised too.</summary>
	[BindFrom("statusCode")]
	public int StatusCode { get; set; } = 400;
}

/// <summary>
/// GET api/dev/errortest?count=&amp;statusCode= — Admin only, and only outside of production.
/// <para>
/// A diagnostics endpoint for the multi-error pipeline: it returns exactly the number of errors
/// you ask for, through the same <c>SendErrorsAsync</c> path every real endpoint uses, so the
/// response shape is the real one rather than a hand-rolled imitation.
/// </para>
/// <para>
/// The two knobs cover both halves of the client's error handling:
/// <list type="bullet">
/// <item><c>count &gt; 0</c> — the server supplies the messages, so the client parses them.</item>
/// <item><c>count = 0</c> — an empty body, so the client falls back to its own status-based
/// message from <c>WebConstants.Messages.Client</c>.</item>
/// </list>
/// </para>
/// </summary>
public class ErrorTestEndpoint(IWebHostEnvironment env) : Endpoint<ErrorTestRequest>
{
	/// <summary>Upper bound so a stray query string can't ask for a million toasts.</summary>
	private const int _maxCount = 20;

	public override void Configure()
	{
		Get(ApiRoutes.Dev.ErrorTest);
		Roles("Admin");
	}

	public override async Task HandleAsync(ErrorTestRequest req, CancellationToken ct)
	{
		// Inert outside development — the endpoint is registered either way, but only ever
		// does anything on a dev machine.
		if (env.IsProduction())
		{
			await Send.NotFoundAsync(ct);
			return;
		}

		// Two cases send a bare status with no body:
		//   * a 2xx, which the client treats as success and never reads a body for, so errors
		//     attached to one would silently vanish;
		//   * count <= 0, which is how you reach the client's StatusFallback — it only fires when
		//     the body yields no message, so an empty body is the point.
		if (req.StatusCode is >= 200 and < 300 || req.Count <= 0)
		{
			await Send.ResultAsync(Results.StatusCode(req.StatusCode));
			return;
		}

		var count = Math.Min(req.Count, _maxCount);

		// Deliberately all Error.Failure with the default code, matching what the real handlers
		// produce — that is the case AllowDuplicateErrors has to survive.
		var errors = Enumerable.Range(1, count)
			.Select(i => Error.Failure(description: $"Test error {i} of {count}."))
			.ToList();

		await this.SendErrorsAsync(errors, req.StatusCode, ct);
	}
}
