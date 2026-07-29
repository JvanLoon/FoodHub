using FastEndpoints;
using FoodCalc.Features.Recipes.Commands.UpdateRecipeName;
using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>PUT api/recipe/name — the recipe's author or an Admin. Body validated by RecipeNameUpdateValidator.</summary>
public class UpdateRecipeNameEndpoint(IMediator mediator) : Endpoint<RecipeNameUpdateDto, RecipeDto>
{
    public override void Configure()
    {
        Put(ApiRoutes.Recipe.UpdateName);
        Policies("Admin,Moderator,User");
    }

    public override async Task HandleAsync(RecipeNameUpdateDto req, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateRecipeNameCommand(req.Id, req.Name, User.ToActingUser()), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}