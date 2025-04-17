using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Recepts.Commands.UpdateRecept;
public record UpdateReceptCommand(Recipe Recept) : IRequest<ErrorOr<Recipe>>;

