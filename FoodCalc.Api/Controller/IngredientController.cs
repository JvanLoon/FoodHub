using FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;
using FoodCalc.Features.Ingredients.Commands.AddIngredient;
using FoodCalc.Features.Ingredients.Commands.DeleteIngredient;
using FoodCalc.Features.Ingredients.Commands.DeleteIngredientFromRecipe;
using FoodCalc.Features.Ingredients.Commands.UpdateIngredient;
using FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;

using FoodHub.DTOs;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodCalc.ApiService.Controller;

[Route("api/[controller]")]
[Authorize]
public class IngredientController(IMediator mediator) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<IEnumerable<IngredientDto>>> GetAllIngredients()
	{
		var result = await mediator.Send(new GetAllIngredientsQuery());

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	//[HttpGet("{id}")]
	//public async Task<ActionResult<Recipe>> GetIngredient(Guid id)
	//{
	//	var result = await mediator.Send(new GetRecipeByIdQuery(id));

	//	return result.Match(
	//		Ok,
	//		errors => Problem(errors.First().Description));
	//}

	[HttpPost]
	public async Task<IActionResult> AddIngredient([FromBody]CreateIngredientDto ingredient)
	{
		if (string.IsNullOrEmpty(ingredient.Name))
		{
			return BadRequest("No name provided");
		}

		var result = await mediator.Send(new AddIngredientCommand(ingredient));

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	[HttpDelete("deleteingredient/{id}")]
	public async Task<IActionResult> DeleteIngredient(Guid id)
	{
		var result = await mediator.Send(new DeleteIngredientCommand(id));

		return result.Match(
		success => Ok(success),
		errors => Problem(errors.First().Description));
	}

	[HttpPut]
	public async Task<IActionResult> UpdateIngredient([FromBody] UpdateIngredientDto ingredient)
	{
		var result = await mediator.Send(new UpdateIngredientCommand(ingredient));

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	//[HttpDelete("{id}")]
	//public async Task<IActionResult> DeleteRecipe(Guid id)
	//{
	//	var result = await mediator.Send(new DeleteIngredientFromRecipeCommand(id));

	//	return result.Match(
	//	success => Ok(success),
	//	errors => Problem(errors.First().Description));
	//}

	//[HttpGet("ingredients")]
	//public async Task<ActionResult<IEnumerable<Recipe>>> GetIngredients()
	//{
	//	var result = await mediator.Send(new GetAllIngredientsQuery());

	//	return result.Match(
	//		Ok,
	//		errors => Problem(errors.First().Description));
	//}
}
