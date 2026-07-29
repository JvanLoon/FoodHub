using FastEndpoints;
using FoodCalc.Features.Ingredients.Commands.DeleteIngredientFromRecipe;
using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>DELETE api/recipe/deleteingredient/{id} — the recipe's author or an Admin.</summary>
public class DeleteIngredientEndpoint(IMediator mediator) : Endpoint<RecipeItemByIdRequest, bool>
{
    public override void Configure()
    {
        Delete(ApiRoutes.Recipe.DeleteIngredientTemplate);
        Policies("Admin,Moderator,User");
    }

    public override async Task HandleAsync(RecipeItemByIdRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteIngredientFromRecipeCommand(req.Id, User.ToActingUser()), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}