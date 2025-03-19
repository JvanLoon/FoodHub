using ErrorOr;

using FoodHub.Persistence.Entities;

using MediatR;

namespace FoodCalc.Features.Recepts.Commands.AddIngredientToRecept;
public record AddIngredientToReceptCommand(ReceptIngredient ReceptIngredient) : IRequest<ErrorOr<ReceptIngredient>>;
