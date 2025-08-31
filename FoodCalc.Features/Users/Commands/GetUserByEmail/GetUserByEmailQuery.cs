using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Users.Commands.GetUserByEmail;

public record GetUserByEmailQuery(string Email) : IRequest<ErrorOr<UserDto>>;
