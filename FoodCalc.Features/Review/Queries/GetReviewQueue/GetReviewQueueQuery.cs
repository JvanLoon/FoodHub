using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Review.Queries.GetReviewQueue;

/// <summary>
/// Everything awaiting approval. Unlike every other read in the app this one deliberately
/// ignores <see cref="ReviewVisibilityExtensions"/> — seeing other people's unapproved content
/// is the entire point — so its endpoint is restricted to Admin and Moderator.
/// </summary>
public record GetReviewQueueQuery : IRequest<ErrorOr<ReviewQueueDto>>;
