using System.Security.Claims;
using MeetMingler.BLL.Models.User;
using MeetMingler.DAL.Models;

namespace MeetMingler.BLL.Services.Contracts;

public interface IAuthService
{
    public Task SignInAsync(UserLoginIM loginIm, CancellationToken cf = default);

    public Task SignOutAsync(CancellationToken cf = default);

    public Task<User?> AuthenticateUserAsync(ClaimsPrincipal claimsPrincipal, CancellationToken cf = default);
}