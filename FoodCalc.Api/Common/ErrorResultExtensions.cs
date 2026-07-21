using ErrorOr;

using FastEndpoints;

using FluentValidation.Results;

namespace FoodCalc.Api.Common;

/// <summary>
/// Turns a failed <see cref="ErrorOr{TValue}"/> — or any bag of error messages — into a single
/// error response that carries <em>every</em> error instead of only the first one.
/// <para>
/// The messages are pushed through FastEndpoints' validation-failure pipeline, which is not
/// validation-specific: it is just a keyed collection of messages. That means domain failures and
/// validator failures leave the API in the same RFC9457 ProblemDetails shape
/// (<c>errors: [{ name, reason }]</c>), so the Blazor client has exactly one shape to parse.
/// </para>
/// </summary>
public static class ErrorResultExtensions
{
	/// <summary>Field name used for errors that aren't tied to a specific request property.</summary>
	private const string _generalErrorsField = "generalErrors";

	/// <summary>
	/// Sends every <see cref="Error"/> from a failed <c>ErrorOr</c> result. The error's
	/// <see cref="Error.Code"/> becomes the field name, so a future <c>Error.Validation("Name", ...)</c>
	/// lands on the right property without any change here.
	/// </summary>
	public static Task SendErrorsAsync(this BaseEndpoint ep, List<Error> errors, int statusCode = 400, CancellationToken ct = default) =>
		ep.SendFailuresAsync(errors.Select(e => new ValidationFailure(e.Code, e.Description)), statusCode, ct);

	/// <summary>
	/// Sends a set of plain messages (e.g. <c>IdentityResult.Errors</c>) as general errors.
	/// </summary>
	public static Task SendErrorsAsync(this BaseEndpoint ep, IEnumerable<string> messages, int statusCode = 400, CancellationToken ct = default) =>
		ep.SendFailuresAsync(messages.Select(m => new ValidationFailure(_generalErrorsField, m)), statusCode, ct);

	private static Task SendFailuresAsync(this BaseEndpoint ep, IEnumerable<ValidationFailure> failures, int statusCode, CancellationToken ct) =>
		ep.HttpContext.Response.SendErrorsAsync([.. failures], statusCode, cancellation: ct);
}
