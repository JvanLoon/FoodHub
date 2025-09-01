using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Authentication.Roles.Queries.GetAllRoles;

public record GetAllRolesQuery() : IRequest<ErrorOr<List<string>>>;
