using ErrorOr;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCalc.Features.ImportExport.Export.Commands.ExportJSON;
public record ExportAllCommand(string exportFormat, bool includeUsers) : IRequest<ErrorOr<string>>;
