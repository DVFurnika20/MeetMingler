using System.Security.Claims;
using MeetMingler.BLL.Models.User;
using MeetMingler.BLL.Services.Contracts;
using MeetMingler.DAL.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MeetMingler.BLL.Services.Implementations;

public class AuthService(
    SignInManager<User> signInManager,
    UserManager<User> userManager) : IAuthService
{
    public async Task SignInAsync(UserLoginIM loginIm, CancellationToken cf = default)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(e => e.Email == loginIm.Email, cf);

        if (user == null)
            throw new IdentityException([
                new IdentityError
                {
                    Code = "Invalid Credentials",
                    Description = "Invalid email or password."
                }
            ]);

        var result =
            await signInManager.PasswordSignInAsync(user, loginIm.Password, true, false);
        
        if (!result.Succeeded)
            throw new IdentityException([
                new IdentityError() { Code = "InvalidCredentials", Description = "Invalid email or password." }
            ]);
    }

    public async Task SignOutAsync(CancellationToken cf = default)
    {
        await signInManager.SignOutAsync();
    }

    public Task<User?> AuthenticateUserAsync(ClaimsPrincipal userClaims, CancellationToken cf = default)
    {
        return userManager.GetUserAsync(userClaims);
    }
}