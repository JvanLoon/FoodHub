using ErrorOr;
using MediatR;

using FoodHub.DTOs;

namespace FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;
public record GetAllUsersQuery(int Page = 1, int PageSize = 25, string? Search = null) : IRequest<ErrorOr<PagedResultDto<UserDto>>>;
