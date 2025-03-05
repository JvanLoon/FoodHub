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
        IEnumerable<Recept> recepts = await mediator.Send(new GetAllReceptsQuery());

        return Ok(recepts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Recept>> GetRecept(Guid id)
    {
        Recept result = await mediator.Send(new GetReceptByIdQuery(id));

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Recept>> AddRecept(Recept recept)
    {
        Recept result = await mediator.Send(new AddReceptCommand(recept));

        if (result == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(AddRecept), new { id = result.Id }, result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRecept(Recept recept)
    {
        Recept result = await mediator.Send(new UpdateReceptCommand(recept));

        if (result == null)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecept(Guid id)
    {
        Guid result = await mediator.Send(new DeleteReceptCommand(id));

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AddIngridientToRecept(Guid receptId, Ingredient ingredient)
    {
        Recept result = await mediator.Send(new AddIngredientToReceptCommand(receptId, ingredient));

        return NoContent();
    }
}