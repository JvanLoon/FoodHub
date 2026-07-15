using System.Text;

using FastEndpoints;

using FoodCalc.Features.ImportExport.Export.Commands.ExportJSON;

using MediatR;

namespace FoodCalc.Api.Endpoints.ImportExport;

public class ExportRequest
{
	[BindFrom("format")]
	public string Format { get; set; } = string.Empty;
}

/// <summary>
/// Mirrors the JSON shape the old controller produced via Ok(File(...)):
/// a serialized FileContentResult with base64 fileContents + contentType.
/// The Blazor ImportExportService parses exactly these two properties.
/// </summary>
public class ExportFileResponse
{
	public byte[] FileContents { get; set; } = [];
	public string ContentType { get; set; } = "application/octet-stream";
	public string? FileDownloadName { get; set; }
}

/// <summary>GET api/importexport/export?format= — Admin.</summary>
public class ExportEndpoint(IMediator mediator)
	: Endpoint<ExportRequest, ExportFileResponse>
{
	// Matches the old controller's hardcoded field.
	private const bool IncludeUsers = false;

	public override void Configure()
	{
		Get("api/importexport/export");
		Policies("Admin");
	}

	public override async Task HandleAsync(ExportRequest req, CancellationToken ct)
	{
		var result = await mediator.Send(new ExportAllCommand(req.Format, IncludeUsers), ct);

		await result.Match(
			json => Send.OkAsync(new ExportFileResponse
			{
				FileContents = Encoding.UTF8.GetBytes(json),
				ContentType = "application/json",
				FileDownloadName = "users.json"
			}, ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
