using ErrorOr;

using FoodHub.Persistence.Entities;

using MediatR;

namespace FoodCalc.Features.Recepts.Commands.AddIngredientToRecept;
public record AddIngredientToReceptCommand(Guid ReceptId, FoodHub.Persistence.Entities.Ingredient Ingredient) : IRequest<ErrorOr<Recept>>;
