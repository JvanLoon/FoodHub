using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Authentication.Roles.Queries.GetAllRoles;

public record GetAllRolesQuery(int Page = 1, int PageSize = 10) : IRequest<ErrorOr<List<string>>>;
