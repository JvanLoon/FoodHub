using FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;
using FoodCalc.Features.ImportExport.Export.Commands.ExportJSON;
using FoodCalc.Features.ImportExport.Import.Commands.ImportJSON;

using FoodHub.DTOs;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FoodCalc.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class ImportExportController(IMediator mediator) : ControllerBase
{
	private readonly bool _includeUsers = false;
	[HttpGet("export")]
	[Authorize("Admin")]
	public async Task<IActionResult> Export(string format)
	{
		var result = await mediator.Send(new ExportAllCommand(format, _includeUsers));

		return result.Match(
		success => Ok(File(Encoding.UTF8.GetBytes(result.Value), "application/json", "users.json")),
		errors => Problem(errors.First().Description));

		//return result.Match<IActionResult>(
		//	json => File(Encoding.UTF8.GetBytes(result.Value), "application/json", "users.json"),
		//	errors => Problem(errors.First().Description));
	}

	// Import users from JSON
	[HttpPost("import")]
	[Authorize("Admin")]
	public async Task<IActionResult> Import([FromForm] IFormFile file)
	{
		if (file == null || file.Length == 0)
			return BadRequest("No file uploaded.");

		if (!file.ContentType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
			return BadRequest("Only JSON files are accepted.");

		using var stream = file.OpenReadStream();

		var importData = await JsonSerializer.DeserializeAsync<ImportExportAllDataDto>(stream);

		if (importData == null)
			return BadRequest("Invalid file content.");

		var result = await mediator.Send(new ImportAllCommand(importData));

		return result.Match(
			_ => Ok("Import successful."),
			errors => Problem(errors.First().Description));
	}
}
