using FastEndpoints;
using FoodCalc.Features.Ingredients.Commands.AddIngredient;
using MediatR;

namespace FoodCalc.Api.Endpoints.Ingredients;

public abstract class CreateIngredientDto
{
    [BindFrom("name")]
    public required string Name { get; set; }
}

/// <summary>POST api/ingredient — any authenticated user; the entry is unapproved until reviewed.</summary>
public class CreateIngredientEndpoint(IMediator mediator) : Endpoint<CreateIngredientDto, IngredientDto>
{
    public override void Configure()
    {
        Post(ApiRoutes.Ingredient.Create);
        Policies("Admin,Moderator,User");
    }

    public override async Task HandleAsync(CreateIngredientDto req, CancellationToken ct)
    {
        var result = await mediator.Send(new AddIngredientCommand(req.Name, User.GetUserId()), ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}