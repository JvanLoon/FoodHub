using ErrorOr;
using MediatR;

using FoodHub.DTOs;

namespace FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;
public record GetAllUsersQuery() : IRequest<ErrorOr<List<UserDto>>>;
