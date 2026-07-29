using FastEndpoints;
using FoodCalc.Features.Ingredients.Commands.DeleteIngredient;
using MediatR;

namespace FoodCalc.Api.Endpoints.Ingredients;

public class DeleteIngredientRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// DELETE api/ingredient/deleteingredient/{id} — the entry's author or an Admin.
/// Deletes the ingredient itself (distinct from removing an ingredient from a
/// recipe, which lives under Endpoints/Recipes).
/// </summary>
public class DeleteIngredientEndpoint(IMediator mediator) : Endpoint<DeleteIngredientRequest, bool>
{
    public override void Configure()
    {
        Delete(ApiRoutes.Ingredient.DeleteTemplate);
        Policies("Admin,Moderator,User");
    }

    public override async Task HandleAsync(DeleteIngredientRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteIngredientCommand(req.Id, User.ToActingUser()), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}