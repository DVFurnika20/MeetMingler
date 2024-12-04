using MeetMingler.BLL.Services.Contracts;
using MeetMingler.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeetMingler.WebHost.Controllers;

public class BaseController(IAuthService authService, ICurrentUser currentUser) : Controller
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        base.OnActionExecuting(filterContext);

        var hasAllowAnonymous = filterContext.ActionDescriptor.EndpointMetadata
            .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));

        if (hasAllowAnonymous) return;

        var user = authService.AuthenticateUserAsync(User).GetAwaiter().GetResult();
        if (user == null)
            filterContext.Result = Unauthorized();
        else currentUser.User = user;
    }
}