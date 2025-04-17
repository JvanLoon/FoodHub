using ErrorOr;

using FoodCalc.Feature.Ingredient.Queries.GetAllIngredients;
using FoodCalc.Features.Ingredient.Commands.AddIngredient;
using FoodCalc.Features.Recepts.Commands.AddIngredientToRecept;
using FoodCalc.Features.Recepts.Commands.AddRecept;
using FoodCalc.Features.Recepts.Commands.DeleteRecept;
using FoodCalc.Features.Recepts.Commands.UpdateRecept;
using FoodCalc.Features.Recepts.Queries.GetAllRecepts;
using FoodCalc.Features.Recepts.Queries.GetById;

using FoodHub.Persistence.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace FoodCalc.ApiService.Controller;

[Route("api/[controller]")]
public class RecipeController(IMediator mediator) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipes()
	{
		var result = await mediator.Send(new GetAllRecipesQuery());

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Recipe>> GetRecipeById(Guid id)
	{
		var result = await mediator.Send(new GetRecipeByIdQuery(id));

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	[HttpPost]
	public async Task<IActionResult> AddRecipe([FromBody] Recipe recipe)
	{
		Console.WriteLine($"Received Recipe: {recipe.Name}");

		if (string.IsNullOrEmpty(recipe.Name))
		{
			return BadRequest("No name provided");
		}

		var result = await mediator.Send(new AddRecipeCommand(recipe));

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	[HttpPut]
	public async Task<IActionResult> UpdateRecipe([FromBody] Recipe recipe)
	{
		var result = await mediator.Send(new UpdateReceptCommand(recipe));

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteRecipe(Guid id)
	{
		var result = await mediator.Send(new DeleteReceptCommand(id));

		return result.Match(
		success => Ok(success),
		errors => Problem(errors.First().Description));
	}

	[HttpPost("ingredient")]

	public async Task<IActionResult> AddIngredientToRecipe([FromBody] RecipeIngredient recipeIngredient)
	{
		var result = await mediator.Send(new AddIngredientToRecipeCommand(recipeIngredient));
		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}
}
