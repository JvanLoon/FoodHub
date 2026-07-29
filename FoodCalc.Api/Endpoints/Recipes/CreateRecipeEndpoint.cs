using FastEndpoints;
using FoodCalc.Features.Recipes.Commands.AddRecipe;
using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>POST api/recipe — any authenticated user. Body validated by CreateRecipeValidator.</summary>
public class CreateRecipeEndpoint(IMediator mediator) : Endpoint<CreateRecipeDto, RecipeDto>
{
    public override void Configure()
    {
        Post(ApiRoutes.Recipe.Create);
        Policies("Admin,Moderator,User");
    }

    public override async Task HandleAsync(CreateRecipeDto req, CancellationToken ct)
    {
        var result = await mediator.Send(new AddRecipeCommand(req, User.GetUserId()), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}