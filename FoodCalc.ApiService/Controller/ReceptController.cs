using FoodHub.Persistence.Entities;
using FoodCalc.Features.Recepts.Commands.AddIngredientToRecept;
using FoodCalc.Features.Recepts.Commands.AddRecept;
using FoodCalc.Features.Recepts.Commands.DeleteRecept;
using FoodCalc.Features.Recepts.Commands.UpdateRecept;
using FoodCalc.Features.Recepts.Queries.GetAllRecepts;
using FoodCalc.Features.Recepts.Queries.GetById;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace FoodCalc.ApiService.Controller;

[Route("api/[controller]")]
public class ReceptController(IMediator mediator) : ControllerBase
{
	//var x = await mediator.Send(new ());
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Recept>>> GetRecepts()
	{
		var recepts = await mediator.Send(new GetAllReceptsQuery());

		return Ok(recepts.Value);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Recept>> GetRecept(Guid id)
	{
		var result = await mediator.Send(new GetReceptByIdQuery(id));

		if (result.IsError)
		{
			return NotFound(result.FirstError);
		}

		return Ok(result.Value);
	}

	[HttpPost]
	public async Task<IActionResult> AddRecept([FromBody]Recept recept)
	{
		Console.WriteLine($"Received Recept: {recept.Name}");

		if (string.IsNullOrEmpty(recept.Name))
		{
			return BadRequest("No name provided");
		}

		var result = await mediator.Send(new AddReceptCommand(recept));

		if (result.IsError)
		{
			return BadRequest(result.FirstError);
		}

		return Ok();
	}

	[HttpPut]
	public async Task<IActionResult> UpdateRecept(Recept recept)
	{
		var result = await mediator.Send(new UpdateReceptCommand(recept));

		if (result.IsError)
		{
			return BadRequest(result.FirstError);
		}

		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteRecept(Guid id)
	{
		var result = await mediator.Send(new DeleteReceptCommand(id));

		if (result.IsError)
		{
			return BadRequest(result.FirstError);
		}

		return NoContent();
	}

	//[HttpPut("{id}")]
	//public async Task<IActionResult> AddIngridientToRecept(Guid receptId, Ingredient ingredient)
	//{
	//    var result = await mediator.Send(new AddIngredientToReceptCommand(receptId, ingredient));

	//    if (result.IsError)
	//    {
	//        return BadRequest(result.FirstError);
	//    }

	//    return NoContent();
	//}
}
