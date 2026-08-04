using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Authentication.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(
    FoodHubDbContext context,
    UserManager<IdentityUser> userManager,
    ILogger<DeleteUserCommandHandler> logger) : IRequestHandler<DeleteUserCommand, ErrorOr<bool>>
{
    public async Task<ErrorOr<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Error.NotFound(description: ErrorMessages.Common.NotFound(ErrorMessages.Entities.User));

            // An admin deleting their own account would drop the last way back into user
            // management, and nothing in the UI could undo it. Refused outright.
            if (user.Id == request.RequestingUserId)
                return Error.Validation(description: ErrorMessages.User.CannotDeleteSelf);

            // The meal calendar is private to the account and worthless without it, so it goes
            // too. Recipes and ingredients deliberately stay: once approved they are shared
            // content that other people's calendars may point at, and orphaning an author is
            // cheaper than tearing meals out of someone else's week. Neither table has a real
            // FK to AspNetUsers, so nothing here depends on delete order.
            var mealPlan = await context.MealPlanEntries.Where(m => m.UserId == user.Id)
                .ToListAsync(cancellationToken);

            if (mealPlan.Count > 0)
            {
                context.MealPlanEntries.RemoveRange(mealPlan);
                await context.SaveChangesAsync(cancellationToken);
            }

            // Presence is per-account bookkeeping with no FK to hang it, so it goes too —
            // otherwise the row outlives the user and a recycled id would inherit it.
            await context.UserPresences.Where(p => p.UserId == user.Id)
                .ExecuteDeleteAsync(cancellationToken);

            // Identity owns the cascade for its own tables (roles, claims, logins, tokens).
            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                string errors = string.Join("; ", result.Errors.Select(e => e.Description));
                // Identity's own descriptions are English; report the Dutch generic instead.
                logger.LogError("Failed to delete user {Email}: {Errors}", request.Email, errors);
                return Error.Failure(description: errors);
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete user {Email}", request.Email);
            return Error.Failure(description: ErrorMessages.Common.DeleteFailed(ErrorMessages.Entities.User));
        }
    }
}
