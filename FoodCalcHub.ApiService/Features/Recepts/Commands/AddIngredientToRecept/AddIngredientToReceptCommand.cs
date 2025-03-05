using FoodCalcHub.ApiService.Entities;
using MediatR;

namespace FoodCalcHub.ApiService.Features.Recepts.Commands.AddIngredientToRecept;
public record AddIngredientToReceptCommand(Guid ReceptId, Ingredient Ingredient) : IRequest<Recept>;
