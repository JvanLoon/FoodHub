using ErrorOr;

using FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;
using FoodCalc.Features.Recipes.Commands.AddRecipe;
using FoodCalc.Features.Ingredients.Commands.DeleteIngredientFromRecipe;
using FoodCalc.Features.Recipes.Commands.DeleteRecipe;
using FoodCalc.Features.Recipes.Commands.UpdateRecipe;
using FoodCalc.Features.Recipes.Commands.UpdateRecipeName;
using FoodCalc.Features.Recipes.Queries.GetAllRecipes;
using FoodCalc.Features.Recipes.Queries.GetById;

using FoodHub.DTOs;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FoodCalc.ApiService.Controller;

[Route("api/[controller]")]
public class RecipeController(IMediator mediator) : ControllerBase
{
	[HttpGet("getallrecipes")]
	[Authorize]
	public async Task<ActionResult<IEnumerable<RecipeDto>>> GetAllRecipes([FromQuery] bool withingredient = true)
	{
		var result = await mediator.Send(new GetAllRecipesQuery(withingredient));

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	[HttpGet("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<RecipeDto>> GetRecipeById(Guid id)
	{
		var result = await mediator.Send(new GetRecipeByIdQuery(id));

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	[HttpPost]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> AddRecipe([FromBody] CreateRecipeDto recipe)
    {
        Console.WriteLine($"Received Recipe: {recipe.Name}");

        if (string.IsNullOrEmpty(recipe.Name))
        {
            return BadRequest("No name provided");
        }

        var result = await mediator.Send(new AddRecipeCommand(recipe));

        return result.Match(
			result => Ok(recipe),
            errors => Problem(errors.First().Description));
    }

	[HttpPut("name")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> UpdateRecipe([FromBody] RecipeNameUpdateDto payload)
	{
		var result = await mediator.Send(new UpdateRecipeNameCommand(payload.Id, payload.Name));

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	[HttpPut]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> UpdateRecipe([FromBody] UpdateRecipeDto recipe)
	{
		var result = await mediator.Send(new UpdateRecipeCommand(recipe));

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	[HttpDelete("deleterecipe/{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> DeleteRecipe(Guid id)
	{
		var result = await mediator.Send(new DeleteRecipeCommand(id));

		return result.Match(
		success => Ok(success),
		errors => Problem(errors.First().Description));
	}

	[HttpDelete("deleteingredient/{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> DeleteIngredient(Guid id)
	{
		var result = await mediator.Send(new DeleteIngredientFromRecipeCommand(id));

		return result.Match(
		success => Ok(success),
		errors => Problem(errors.First().Description));
	}

	[HttpPost("ingredient")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> AddIngredientToRecipe([FromBody] RecipeIngredientDto recipeIngredient)
	{
		var result = await mediator.Send(new AddIngredientToRecipeCommand(recipeIngredient));
		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}
}
