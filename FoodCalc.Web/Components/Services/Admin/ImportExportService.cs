using FoodCalc.Web.Components.Services.Auth;

using FoodHub.DTOs;

using Microsoft.JSInterop;

using System.Net.Http.Headers;

namespace FoodCalc.Web.Components.Services.Admin;
public class ImportExportService(AuthenticatedHttpClientService httpClient, IJSRuntime js, MessageService messageService)
{
	private readonly int _maxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB
	private readonly string _exportFileName = $"export";

	public async Task<bool> ImportAsync(byte[] fileContent, string fileName)
	{
		if (fileContent == null || fileContent.Length == 0)
		{
			await messageService.ShowMessageAsync("No file content.", true);
			return false;
		}

		try
		{
			var content = new MultipartFormDataContent();
			var streamContent = new StreamContent(new MemoryStream(fileContent));
			streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			content.Add(streamContent, "file", fileName);

			var response = await httpClient.PostAsync("api/importexport/import", content);

			if (!response.IsSuccessStatusCode)
			{
				var error = await response.Content.ReadAsStringAsync();
				await messageService.ShowMessageAsync($"Import failed: {error}", true);
				return false;
			}

			await messageService.ShowMessageAsync("Import successful.", false);
			return true;
		}
		catch (Exception ex)
		{
			await messageService.ShowMessageAsync(ex.Message, true);
			return false;
		}
	}

	public async Task<bool> ExportAsync(string exportFormat)
	{
		try
		{
			var response = await httpClient.GetAsync($"api/importexport/export?format={exportFormat}");
			var fileName = string.Format(_exportFileName) + $".{exportFormat.ToLower()}";

			if (response.IsSuccessStatusCode)
			{
				var json = await response.Content.ReadAsStringAsync();

				using var doc = System.Text.Json.JsonDocument.Parse(json);

				// Extract the base64 file contents and content type
				var base64 = doc.RootElement.GetProperty("fileContents").GetString();
				var mimeType = doc.RootElement.TryGetProperty("contentType", out var ct)
					? ct.GetString() ?? "application/octet-stream"
					: "application/octet-stream";

				if (string.IsNullOrWhiteSpace(base64))
				{
					await messageService.ShowMessageAsync("Export failed: file content is empty.", true);
					return false;
				}

				await js.InvokeVoidAsync("blazorDownloadFile", fileName, mimeType, base64);
				return true;
			}
			else
			{
				await messageService.ShowMessageAsync("Export failed.", true);
				return false;
			}

		}
		catch (Exception ex)
		{
			await messageService.ShowMessageAsync(ex.Message, true);
			return false;
		}


	}


}
