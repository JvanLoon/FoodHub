using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Authentication.Presence.Queries.GetPresence;

/// <summary>
/// Current presence for the given accounts. Ids with no presence row come back as offline with a
/// null timestamp, so the caller gets one entry per id it asked about and never has to guess
/// whether a missing row means offline or means it forgot to ask.
/// </summary>
public record GetPresenceQuery(IReadOnlyList<string> UserIds) : IRequest<ErrorOr<List<UserPresenceDto>>>;
