using ErrorOr;
using MediatR;
using FoodHub.Persistence.Entities;

namespace FoodCalc.ApiService.Features.Recepts.Commands.AddIngredientToRecept;
public record AddIngredientToReceptCommand(Guid ReceptId, Ingredient Ingredient) : IRequest<ErrorOr<Recept>>;
