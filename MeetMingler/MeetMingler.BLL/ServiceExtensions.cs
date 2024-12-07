using MeetMingler.BLL.Services.Contracts;
using MeetMingler.BLL.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace MeetMingler.BLL;

public static class ServiceExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IEventService, EventService>();
    }

    public static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Mappings));
    }
}