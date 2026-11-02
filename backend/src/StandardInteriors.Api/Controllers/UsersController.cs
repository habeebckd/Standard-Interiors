using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StandardInteriors.Api.Dtos.Auth;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users) => _users = users;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) => Ok(await _users.GetAllAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(UserUpsertDto dto, CancellationToken cancellationToken) => Ok(new { id = await _users.CreateAsync(dto, cancellationToken) });

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UserUpsertDto dto, CancellationToken cancellationToken) => await _users.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpPost("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken) => await _users.DeactivateAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpPost("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken) => await _users.ActivateAsync(id, cancellationToken) ? NoContent() : NotFound();
}
