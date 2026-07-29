using FastEndpoints;
using FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;
using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>POST api/recipe/ingredient — the recipe's author or an Admin.</summary>
public class AddIngredientToRecipeEndpoint(IMediator mediator) : Endpoint<RecipeItemDto, RecipeItemDto>
{
    public override void Configure()
    {
        Post(ApiRoutes.Recipe.AddIngredient);
        Policies("Admin,Moderator,User");
    }

    public override async Task HandleAsync(RecipeItemDto req, CancellationToken ct)
    {
        var result = await mediator.Send(new AddIngredientToRecipeCommand(req, User.ToActingUser()), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}