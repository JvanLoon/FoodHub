using FoodCalc.Web.Components.Services.Auth;

using FoodHub.DTOs;

using Microsoft.JSInterop;

using System.Net;
using System.Net.Http.Headers;

namespace FoodCalc.Web.Components.Services.Admin;

public class ImportExportService(AuthenticatedHttpClientService httpClient, IJSRuntime js)
{
	private readonly string _exportFileName = WebConstants.Files.ExportBaseName;

	public Task<ApiResult> ImportAsync(byte[] fileContent, string fileName)
		=> UploadAsync(ApiRoutes.ImportExport.Import, fileContent, fileName);

	/// <summary>Imports a JSON export produced before the RecipeItem rework (legacy format).</summary>
	public Task<ApiResult> ImportOldAsync(byte[] fileContent, string fileName)
		=> UploadAsync(ApiRoutes.ImportExport.ImportOld, fileContent, fileName);

	private async Task<ApiResult> UploadAsync(string route, byte[] fileContent, string fileName)
	{
		if (fileContent == null || fileContent.Length == 0)
			return ApiResult.Fail(WebConstants.Messages.ImportExport.NoFileContent, (int) HttpStatusCode.NoContent);

		using var content = new MultipartFormDataContent();
		var streamContent = new StreamContent(new MemoryStream(fileContent));
		streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		content.Add(streamContent, "file", fileName);

		return await httpClient.PostAsync(route, content);
	}

	public async Task<ApiResult> ExportAsync(string exportFormat)
	{
		var result = await httpClient.GetAsync<string>($"{ApiRoutes.ImportExport.Export}?format={exportFormat}");
		if (!result.Success)
			return result;

		var fileName = string.Format(_exportFileName) + $".{exportFormat.ToLower()}";

		string? base64;
		string mimeType;
		try
		{
			using var doc = System.Text.Json.JsonDocument.Parse(result.Data!);

			// Extract the base64 file contents and content type
			base64 = doc.RootElement.GetProperty("fileContents").GetString();
			mimeType = doc.RootElement.TryGetProperty("contentType", out var ct)
				? ct.GetString() ?? "application/octet-stream"
				: "application/octet-stream";
		}
		catch (Exception)
		{
			return ApiResult.Fail(WebConstants.Messages.ImportExport.ExportUnexpectedResponse, (int) HttpStatusCode.BadRequest);
		}

		if (string.IsNullOrWhiteSpace(base64))
			return ApiResult.Fail(WebConstants.Messages.ImportExport.ExportEmpty, (int)HttpStatusCode.BadRequest);

		await js.InvokeVoidAsync("blazorDownloadFile", fileName, mimeType, base64);
		return ApiResult.Ok(result.StatusCode);
	}
}
