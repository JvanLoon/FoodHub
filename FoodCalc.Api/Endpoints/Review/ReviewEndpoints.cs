using FastEndpoints;
using FoodCalc.Features.Review.Commands.ApproveContent;
using FoodCalc.Features.Review.Commands.RejectContent;
using FoodCalc.Features.Review.Queries.GetReviewQueue;
using FoodHub.Persistence.Entities;
using MediatR;

namespace FoodCalc.Api.Endpoints.Review;

/// <summary>
/// GET api/review/queue — Admin and Moderator. The only read in the app that returns other
/// users' unapproved content, which is why the policy here is not "any authenticated user".
/// </summary>
public class GetReviewQueueEndpoint(IMediator mediator) : EndpointWithoutRequest<ReviewQueueDto>
{
    public override void Configure()
    {
        Get(ApiRoutes.Review.Queue);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new GetReviewQueueQuery(), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}

/// <summary>POST api/review/recipe/approve — Admin and Moderator.</summary>
public class ApproveRecipeEndpoint(IMediator mediator) : Endpoint<ApproveReviewDto, bool>
{
    public override void Configure()
    {
        Post(ApiRoutes.Review.ApproveRecipe);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(ApproveReviewDto req, CancellationToken ct)
    {
        var result = await mediator.Send(new ApproveContentCommand(ReviewTargetType.Recipe, req.Id), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}

/// <summary>POST api/review/ingredient/approve — Admin and Moderator.</summary>
public class ApproveIngredientEndpoint(IMediator mediator) : Endpoint<ApproveReviewDto, bool>
{
    public override void Configure()
    {
        Post(ApiRoutes.Review.ApproveIngredient);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(ApproveReviewDto req, CancellationToken ct)
    {
        var result = await mediator.Send(new ApproveContentCommand(ReviewTargetType.Ingredient, req.Id), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}

/// <summary>POST api/review/recipe/reject — Admin and Moderator. Body validated by RejectReviewValidator.</summary>
public class RejectRecipeEndpoint(IMediator mediator) : Endpoint<RejectReviewDto, bool>
{
    public override void Configure()
    {
        Post(ApiRoutes.Review.RejectRecipe);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(RejectReviewDto req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new RejectContentCommand(ReviewTargetType.Recipe, req.Id, req.Reason, req.Delete, User.GetUserId()), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}

/// <summary>POST api/review/ingredient/reject — Admin and Moderator.</summary>
public class RejectIngredientEndpoint(IMediator mediator) : Endpoint<RejectReviewDto, bool>
{
    public override void Configure()
    {
        Post(ApiRoutes.Review.RejectIngredient);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(RejectReviewDto req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new RejectContentCommand(ReviewTargetType.Ingredient, req.Id, req.Reason, req.Delete, User.GetUserId()),
            ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}
