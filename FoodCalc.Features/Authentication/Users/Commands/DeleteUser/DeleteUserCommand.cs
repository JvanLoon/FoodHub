using ErrorOr;
using MediatR;

namespace FoodCalc.Features.Authentication.Users.Commands.DeleteUser;

/// <summary>
/// Deletes the account identified by <see cref="Email"/>, along with the meal calendar that
/// belongs to it. <see cref="RequestingUserId"/> is the caller, used to refuse self-deletion.
/// </summary>
public record DeleteUserCommand(string Email, string RequestingUserId) : IRequest<ErrorOr<bool>>;
