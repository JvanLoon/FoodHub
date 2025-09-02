using ErrorOr;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Features.Authentication.Users.Queries.GetUserByEmail;

public record GetUserByEmailQuery(string Email) : IRequest<ErrorOr<UserDto>>;
