using ErrorOr;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Features.ImportExport.Import.Commands.ImportJSON;
public record ImportAllCommand(ImportExportAllDataDto Data) : IRequest<ErrorOr<bool>>;
