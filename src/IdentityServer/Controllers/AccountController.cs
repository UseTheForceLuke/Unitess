using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskManager.SharedKernel.Events;

namespace IdentityServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMediator _mediator;

    public AccountController(
        UserManager<IdentityUser> userManager,
        IMediator mediator)
    {
        _userManager = userManager;
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new IdentityUser
        {
            UserName = model.Username,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, model.Role);

            await _mediator.Publish(new UserRegisteredEvent(
                IdentityId: user.Id,
                Username: user.UserName!,
                Email: user.Email!,
                Role: model.Role
            ));

            return Ok(new { UserId = user.Id });
        }

        return BadRequest(result.Errors);
    }
}

public class RegisterModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } = "User";
}
