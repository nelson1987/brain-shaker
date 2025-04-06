using Brainshaker.App.Commons;
using Brainshaker.App.UseCases.CreateUser;
using Brainshaker.App.UseCases.GetUser;
using Brainshaker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Brainshaker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var user = await queryDispatcher.Dispatch<GetUserByIdQuery, Usuario?>(query, cancellationToken);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUsuarioCommand request, CancellationToken cancellationToken)
    {
        var result = await commandDispatcher.Dispatch<CreateUsuarioCommand, Result>(request, cancellationToken);
        if (result.IsFailure)
            return BadRequest(result.Error);

        var novoId = Guid.NewGuid();
        return CreatedAtAction(nameof(GetById), new { id = novoId }, result);
    }
}