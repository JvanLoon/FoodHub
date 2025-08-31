using ErrorOr;
using MediatR;

using FoodHub.DTOs;

namespace FoodCalc.Features.Recipes.Queries.GetAllRecipes;
public record GetAllUsersQuery() : IRequest<ErrorOr<List<UserDto>>>;
