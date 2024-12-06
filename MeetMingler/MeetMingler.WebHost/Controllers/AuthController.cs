using AutoMapper;
using MeetMingler.BLL;
using MeetMingler.BLL.Models.User;
using MeetMingler.BLL.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetMingler.WebHost.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController(
    IUserService userService,
    IAuthService authService,
    ICurrentUser currentUser,
    IMapper mapper)
    : BaseController(authService,
        currentUser)
{
    private readonly IAuthService _authService = authService;
    private readonly ICurrentUser _currentUser = currentUser;

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserVM>> Register([FromBody] UserIM im,
        CancellationToken cf = default)
    {
        try
        {
            var user = await userService.CreateAsync(im,
                cf);
            return Json(user);
        }
        catch (Exception e)
        {
            return ValidationProblem(detail: e.Message,
                type: "invalid-user-register-data");
        }
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] UserLoginIM loginIm,
        CancellationToken cf = default)
    {
        try
        {
            await _authService.SignInAsync(loginIm, cf);
        }
        catch (IdentityException e)
        {
            return ValidationProblem(detail: e.Message,
                type: "invalid-login-details");
        }
        
        return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Logout(CancellationToken cf = default)
    {
        await _authService.SignOutAsync();
        return Ok();
    }

    [HttpGet]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<ActionResult<UserVM>> Self(CancellationToken cf = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        return Json(
            mapper.Map<UserVM>(_currentUser.User)
        );
    }
}