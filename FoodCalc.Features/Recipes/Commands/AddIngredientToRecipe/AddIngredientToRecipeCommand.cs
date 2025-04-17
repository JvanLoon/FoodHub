using ErrorOr;

using FoodHub.Persistence.Entities;

using MediatR;

namespace FoodCalc.Features.Recepts.Commands.AddIngredientToRecept;
public record AddIngredientToRecipeCommand(RecipeIngredient ReceptIngredient) : IRequest<ErrorOr<RecipeIngredient>>;
