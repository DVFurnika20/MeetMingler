using System.Text.Json.Serialization;
using MeetMingler.BLL;
using MeetMingler.DAL.Data;
using MeetMingler.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using WeatherStationManagement.Domain;

namespace MeetMingler.WebHost;

public static class ServiceExtensions
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddServices();
        builder.Services
            .AddControllers(
                opt => opt.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider()))
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                opt.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            });
    }

    public static void AddDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
    }

    public static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    public static void AddIdentity(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddIdentity<User, IdentityRole<Guid>>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequiredLength = 8;
                opt.SignIn.RequireConfirmedAccount = false;
                opt.SignIn.RequireConfirmedEmail = false;
                opt.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }

    public static void AddLogging(this WebApplicationBuilder builder)
    {
        builder.Services.AddLogging(config => config.AddConsole());
    }

    public static void AddCors(this WebApplicationBuilder builder)
    {
        var origins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
        
        builder.Services.AddCors(options =>
        {
            // ReSharper disable once VariableHidesOuterVariable
            options.AddDefaultPolicy(builder =>
            {
                if (origins != null)
                    builder
                        .WithOrigins(origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
            });
        });
    }

    public static void AddAutoMapper(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper();
    }
}