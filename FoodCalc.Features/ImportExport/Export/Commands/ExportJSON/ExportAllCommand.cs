using ErrorOr;

using MediatR;

namespace FoodCalc.Features.ImportExport.Export.Commands.ExportJSON;
public record ExportAllCommand(string exportFormat, bool includeUsers) : IRequest<ErrorOr<string>>;
