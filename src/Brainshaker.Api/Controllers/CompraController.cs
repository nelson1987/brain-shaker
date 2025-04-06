using Brainshaker.App.Commons;
using Brainshaker.App.UseCases.CreateCategoria;
using Brainshaker.App.UseCases.GetUser;
using Brainshaker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Brainshaker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprasController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCompraByIdQuery(id);
        var result = await queryDispatcher.Dispatch<GetCompraByIdQuery, Usuario?>(query, cancellationToken);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCompraCommand request, CancellationToken cancellationToken)
    {
        var result = await commandDispatcher.Dispatch<CreateCompraCommand, Result>(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(result.Error);

        var novoId = Guid.NewGuid();
        return CreatedAtAction(nameof(GetById), new { id = novoId }, result);
    }
}