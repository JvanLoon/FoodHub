using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Recepts.Commands.AddRecept;
public record AddRecipeCommand(Recipe recept) : IRequest<ErrorOr<Recipe>>;

