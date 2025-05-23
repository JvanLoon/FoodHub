using ErrorOr;

using FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;
using FoodCalc.Features.Recipes.Commands.AddRecipe;
using FoodCalc.Features.Recipes.Commands.DeleteRecipe;
using FoodCalc.Features.Recipes.Commands.UpdateRecipe;
using FoodCalc.Features.Recipes.Queries.GetAllRecipes;
using FoodCalc.Features.Recipes.Queries.GetById;

using FoodHub.Persistence.Entities;
using FoodHub.ServiceDefaults;

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

	[HttpPut("name")]
	public async Task<IActionResult> UpdateRecipe([FromBody] RecipeNameUpdateDto payload)
	{
		var result = await mediator.Send(new UpdateRecipeNameCommand(payload.Id, payload.Name));

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	[HttpPut]
	public async Task<IActionResult> UpdateRecipe([FromBody] Recipe recipe)
	{
		var result = await mediator.Send(new UpdateRecipeCommand(recipe));

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	[HttpDelete("DeleteRecipe/{id}")]
	public async Task<IActionResult> DeleteRecipe(Guid id)
	{
		var result = await mediator.Send(new DeleteRecipeCommand(id));

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
