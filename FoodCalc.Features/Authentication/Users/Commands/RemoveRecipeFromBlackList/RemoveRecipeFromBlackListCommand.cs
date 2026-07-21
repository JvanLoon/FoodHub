using ErrorOr;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Features.Authentication.Users.Commands.RemoveRecipeFromBlackList;
public record RemoveRecipeFromBlackListCommand(BlackListDto itemToBlackList) : IRequest<ErrorOr<bool>>;
