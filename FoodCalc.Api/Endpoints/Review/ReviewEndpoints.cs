using FastEndpoints;
using FoodCalc.Features.Review;
using FoodCalc.Features.Review.Commands.ApproveContent;
using FoodCalc.Features.Review.Commands.RejectContent;
using FoodCalc.Features.Review.Queries.GetReviewQueue;
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

/// <summary>POST api/review/recipe/approve — Admin and Moderator. Publishes the recipe.</summary>
public class ApproveRecipeEndpoint(IMediator mediator) : Endpoint<ReviewTargetDto, bool>
{
    public override void Configure()
    {
        Post(ApiRoutes.Review.ApproveRecipe);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(ReviewTargetDto req, CancellationToken ct)
    {
        var result = await mediator.Send(new ApproveContentCommand(ReviewTargetType.Recipe, req.Id), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}

/// <summary>POST api/review/recipe/reject — Admin and Moderator. Deletes the recipe.</summary>
public class RejectRecipeEndpoint(IMediator mediator) : Endpoint<ReviewTargetDto, bool>
{
    public override void Configure()
    {
        Post(ApiRoutes.Review.RejectRecipe);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(ReviewTargetDto req, CancellationToken ct)
    {
        var result = await mediator.Send(new RejectContentCommand(ReviewTargetType.Recipe, req.Id), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}

/// <summary>POST api/review/recipe/line/approve — Admin and Moderator. Clears one line's changed flag.</summary>
public class ApproveRecipeItemEndpoint(IMediator mediator) : Endpoint<ReviewTargetDto, bool>
{
    public override void Configure()
    {
        Post(ApiRoutes.Review.ApproveRecipeItem);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(ReviewTargetDto req, CancellationToken ct)
    {
        var result = await mediator.Send(new ApproveContentCommand(ReviewTargetType.RecipeItem, req.Id), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}

/// <summary>POST api/review/recipe/line/reject — Admin and Moderator. Deletes one line.</summary>
public class RejectRecipeItemEndpoint(IMediator mediator) : Endpoint<ReviewTargetDto, bool>
{
    public override void Configure()
    {
        Post(ApiRoutes.Review.RejectRecipeItem);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(ReviewTargetDto req, CancellationToken ct)
    {
        var result = await mediator.Send(new RejectContentCommand(ReviewTargetType.RecipeItem, req.Id), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}

/// <summary>POST api/review/ingredient/approve — Admin and Moderator. Publishes the ingredient.</summary>
public class ApproveIngredientEndpoint(IMediator mediator) : Endpoint<ReviewTargetDto, bool>
{
    public override void Configure()
    {
        Post(ApiRoutes.Review.ApproveIngredient);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(ReviewTargetDto req, CancellationToken ct)
    {
        var result = await mediator.Send(new ApproveContentCommand(ReviewTargetType.Ingredient, req.Id), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}

/// <summary>POST api/review/ingredient/reject — Admin and Moderator. Deletes the ingredient.</summary>
public class RejectIngredientEndpoint(IMediator mediator) : Endpoint<ReviewTargetDto, bool>
{
    public override void Configure()
    {
        Post(ApiRoutes.Review.RejectIngredient);
        Policies("Admin,Moderator");
    }

    public override async Task HandleAsync(ReviewTargetDto req, CancellationToken ct)
    {
        var result = await mediator.Send(new RejectContentCommand(ReviewTargetType.Ingredient, req.Id), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}
