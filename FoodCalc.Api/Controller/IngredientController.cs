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
public class IngredientController(IMediator mediator) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Recept>>> GetAllIngredients()
	{
		var result = await mediator.Send(new GetAllIngredientsQuery());

		return result.Match(
			Ok,
			errors => Problem(errors.First().Description));
	}

	//[HttpGet("{id}")]
	//public async Task<ActionResult<Recept>> GetIngredient(Guid id)
	//{
	//	var result = await mediator.Send(new GetReceptByIdQuery(id));

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
	//public async Task<IActionResult> UpdateRecept([FromBody] Recept recept)
	//{
	//	var result = await mediator.Send(new UpdateReceptCommand(recept));

	//	return result.Match(
	//		Ok,
	//		errors => Problem(errors.First().Description));
	//}

	//[HttpDelete("{id}")]
	//public async Task<IActionResult> DeleteRecept(Guid id)
	//{
	//	var result = await mediator.Send(new DeleteReceptCommand(id));

	//	return result.Match(
	//	success => Ok(success),
	//	errors => Problem(errors.First().Description));
	//}

	//[HttpGet("ingredients")]
	//public async Task<ActionResult<IEnumerable<Recept>>> GetIngredients()
	//{
	//	var result = await mediator.Send(new GetAllIngredientsQuery());

	//	return result.Match(
	//		Ok,
	//		errors => Problem(errors.First().Description));
	//}
}
