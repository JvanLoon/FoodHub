using System.Text.Json;

using FastEndpoints;

using FoodCalc.Features.ImportExport.Import.Commands.ImportJSON;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.ImportExport;

public class ImportRequest
{
	[BindFrom("file")]
	public IFormFile? File { get; set; }
}

/// <summary>POST api/importexport/import — Admin. Accepts a multipart JSON file.</summary>
public class ImportEndpoint(IMediator mediator)
	: Endpoint<ImportRequest>
{
	public override void Configure()
	{
		Post(ApiRoutes.ImportExport.Import);
		Policies("Admin");
		AllowFileUploads();
	}

	public override async Task HandleAsync(ImportRequest req, CancellationToken ct)
	{
		var file = req.File;
		if (file == null || file.Length == 0)
		{
			await Send.StringAsync("No file uploaded.", 400, cancellation: ct);
			return;
		}

		if (!file.ContentType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
		{
			await Send.StringAsync("Only JSON files are accepted.", 400, cancellation: ct);
			return;
		}

		await using var stream = file.OpenReadStream();
		var importData = await JsonSerializer.DeserializeAsync<ImportExportAllDataDto>(stream, cancellationToken: ct);

		if (importData == null)
		{
			await Send.StringAsync("Invalid file content.", 400, cancellation: ct);
			return;
		}

		var result = await mediator.Send(new ImportAllCommand(importData), ct);

		await result.Match(
			_ => Send.StringAsync("Import successful.", cancellation: ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
