using ErrorOr;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Features.Authentication.Users.Commands.AddRecipeFromBlackList;

public record AddRecipeFromBlackListCommand(BlackListDto itemToBlackList) : IRequest<ErrorOr<bool>>;
