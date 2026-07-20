using System.Text.Json;

using FastEndpoints;

using FoodCalc.Features.ImportExport.Import.Commands.ImportJSON;

using FoodHub.DTOs;
using FoodHub.DTOs.Legacy;

using MediatR;

namespace FoodCalc.Api.Endpoints.ImportExport;

/// <summary>
/// POST api/importexport/importold — Admin. Accepts a multipart JSON file in the
/// legacy (pre-RecipeItem) export format and imports it via the normal import
/// pipeline after converting it to the current shape.
/// </summary>
public class ImportOldEndpoint(IMediator mediator)
	: Endpoint<ImportRequest>
{
	public override void Configure()
	{
		Post(ApiRoutes.ImportExport.ImportOld);
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

		LegacyImportExportAllDataDto? legacy;
		try
		{
			legacy = await JsonSerializer.DeserializeAsync<LegacyImportExportAllDataDto>(
				stream,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
				ct);
		}
		catch (JsonException)
		{
			await Send.StringAsync("Invalid file content.", 400, cancellation: ct);
			return;
		}

		if (legacy == null)
		{
			await Send.StringAsync("Invalid file content.", 400, cancellation: ct);
			return;
		}

		var importData = LegacyImportConverter.ToCurrent(legacy);
		var result = await mediator.Send(new ImportAllCommand(importData), ct);

		await result.Match(
			_ => Send.StringAsync("Import successful.", cancellation: ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
