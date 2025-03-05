using FoodCalcHub.ApiService.Entities;
using MediatR;

namespace FoodCalcHub.ApiService.Features.Recepts.Commands.AddRecept;
public record AddReceptCommand(Recept recept) : IRequest<Recept>;

