using FoodCalc.Feature.Ingredient.Queries.GetAllIngredients;
using FoodCalc.Features.Ingredient.Commands.AddIngredient;

using FoodHub.Persistence.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace FoodCalc.ApiService.Controller;

[Route("api/[controller]")]
public class IngredientController(IMediator mediator) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Recipe>>> GetAllIngredients()
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
	public async Task<IActionResult> AddIngredient([FromBody]Ingredient ingredient)
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

	//[HttpPut]
	//public async Task<IActionResult> UpdateRecipe([FromBody] Recipe recipe)
	//{
	//	var result = await mediator.Send(new UpdateRecipeNameCommand(recipe));

	//	return result.Match(
	//		Ok,
	//		errors => Problem(errors.First().Description));
	//}

	//[HttpDelete("{id}")]
	//public async Task<IActionResult> DeleteRecipe(Guid id)
	//{
	//	var result = await mediator.Send(new DeleteRecipeCommand(id));

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
