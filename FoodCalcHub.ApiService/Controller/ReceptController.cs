using FoodCalcHub.ApiService.Entities;
using FoodCalcHub.ApiService.Features.Recepts.Commands.AddIngredientToRecept;
using FoodCalcHub.ApiService.Features.Recepts.Commands.AddRecept;
using FoodCalcHub.ApiService.Features.Recepts.Commands.DeleteRecept;
using FoodCalcHub.ApiService.Features.Recepts.Commands.UpdateRecept;
using FoodCalcHub.ApiService.Features.Recepts.Queries.GetAllRecepts;
using FoodCalcHub.ApiService.Features.Recepts.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FoodCalcHub.ApiService.Controller;

[Route("api/[controller]")]
public class ReceptController(IMediator mediator) : ControllerBase
{
    //var x = await mediator.Send(new ());
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recept>>> GetRecepts()
    {
        var recepts = await mediator.Send(new GetAllReceptsQuery());

        return Ok(recepts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Recept>> GetRecept(Guid id)
    {
        var result = await mediator.Send(new GetReceptByIdQuery(id));

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddRecept(Recept recept)
    {
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