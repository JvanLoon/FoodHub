using ErrorOr;

using FoodHub.DTOs;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCalc.Features.Authentication.Users.Commands.AddRecipeFromBlackList;

public record AddRecipeFromBlackListCommand(BlackListDto itemToBlackList) : IRequest<ErrorOr<bool>>;
