using ErrorOr;

using FastEndpoints;

using System.Net;

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

		// A 2xx is always success, whatever the count — errors attached to one would silently
		// vanish, so we never send them here. The body names the status (e.g. "200 => OK") so the
		// client can toast a concrete confirmation instead of a generic one. 204 is the exception:
		// the spec forbids a body on No Content, so it goes out bare and the client falls back to
		// its own "No Content" wording.
		if (req.StatusCode is >= 200 and < 300)
		{
			if (req.StatusCode == StatusCodes.Status204NoContent)
			{
				await Send.ResultAsync(Results.StatusCode(req.StatusCode));
				return;
			}

			var reason = ((HttpStatusCode) req.StatusCode).ToString();
			await Send.ResultAsync(Results.Text($"{req.StatusCode} => {reason}", contentType: "text/plain", statusCode: req.StatusCode));
			return;
		}

		// count <= 0 on a non-2xx is how you reach the client's StatusFallback — it only fires when
		// the body yields no message, so an empty body is the point.
		if (req.Count <= 0)
		{
			await Send.ResultAsync(Results.StatusCode(req.StatusCode));
			return;
		}

		var count = Math.Min(req.Count, _maxCount);

		// Deliberately all Error.Failure with the default code, matching what the real handlers
		// produce — that is the case AllowDuplicateErrors has to survive.
		var errors = Enumerable.Range(1, count)
			.Select(i => Error.Failure(description: $"{req.StatusCode} => Test error {i} of {count}."))
			.ToList();

		await this.SendErrorsAsync(errors, req.StatusCode, ct);
	}
}
