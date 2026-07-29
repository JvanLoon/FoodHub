using FastEndpoints;
using FoodCalc.Features.Recipes.Commands.DeleteRecipe;
using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>DELETE api/recipe/deleterecipe/{id} — the recipe's author or an Admin.</summary>
public class DeleteRecipeEndpoint(IMediator mediator) : Endpoint<RecipeByIdRequest, bool>
{
    public override void Configure()
    {
        Delete(ApiRoutes.Recipe.DeleteRecipeTemplate);
        Policies("Admin,Moderator,User");
    }

    public override async Task HandleAsync(RecipeByIdRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteRecipeCommand(req.Id, User.ToActingUser()), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}