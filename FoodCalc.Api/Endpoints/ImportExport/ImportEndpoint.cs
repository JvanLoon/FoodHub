using FastEndpoints;
using FoodCalc.Features.ImportExport.Import.Commands.ImportJSON;
using MediatR;
using System.Text.Json;

namespace FoodCalc.Api.Endpoints.ImportExport;

public class ImportRequest
{
    [BindFrom("file")]
    public IFormFile? File { get; set; }
}

/// <summary>POST api/importexport/import — Admin. Accepts a multipart JSON file.</summary>
public class ImportEndpoint(IMediator mediator) : Endpoint<ImportRequest>
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
            await Send.StringAsync(ResponseMessages.Import.NoFileUploaded, 400, cancellation: ct);
            return;
        }

        if (!file.ContentType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
        {
            await Send.StringAsync(ResponseMessages.Import.OnlyJsonAccepted, 400, cancellation: ct);
            return;
        }

        await using var stream = file.OpenReadStream();
        var importData = await JsonSerializer.DeserializeAsync<ImportExportAllDataDto>(stream, cancellationToken: ct);

        if (importData == null)
        {
            await Send.StringAsync(ResponseMessages.Import.InvalidFileContent, 400, cancellation: ct);
            return;
        }

        var userid = User.GetUserId();
        if (string.IsNullOrEmpty(userid))
        {
            await Send.StringAsync(ResponseMessages.Token.InvalidUserId, 400, cancellation: ct);
            return;
        }

        var result = await mediator.Send(new ImportAllCommand(importData, userid), ct);

        await result.Match(_ => Send.StringAsync(ResponseMessages.Import.Succeeded, cancellation: ct),
            errors => this.SendErrorsAsync(errors, ct: ct));
    }
}