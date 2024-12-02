using System.Text.Json.Serialization;
using MeetMingler.DAL.Data;
using MeetMingler.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace MeetMingler.WebHost;

public static class ServiceExtensions
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers(
                opt => opt.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider()))
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                },
            });

            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
            });
        });
    }
    
    // public static void AddIdentity(this WebApplicationBuilder builder)
    // {
    //     builder.Services.AddIdentityCore<User>(opt =>
    //     {
    //         opt.User.RequireUniqueEmail = true;
    //         opt.Password.RequiredLength = 8;
    //         opt.SignIn.RequireConfirmedAccount = false;
    //         opt.SignIn.RequireConfirmedEmail = false;
    //         opt.SignIn.RequireConfirmedPhoneNumber = false;
    //     })
    //     .AddRoles<IdentityRole>()
    //     .AddEntityFrameworkStores<ApplicationDbContext>()
    //     .AddDefaultTokenProviders();
    //     
    //     builder.Services
    //         .AddAuthentication(options =>
    //         {
    //             options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //             options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //             options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    //         })
    //         .AddJwtBearer(options =>
    //         {
    //             options.SaveToken = true;
    //             options.RequireHttpsMetadata = false;
    //             options.TokenValidationParameters = new TokenValidationParameters
    //             {
    //                 ValidateIssuer = false,
    //                 ValidateAudience = false,
    //                 ValidateLifetime = true,
    //                 ValidateIssuerSigningKey = true,
    //                 ClockSkew = TimeSpan.Zero,
    //                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtAccessTokenSecret"]!)),
    //             };
    //         });
    // }

    public static void AddLogging(this WebApplicationBuilder builder)
    {
        builder.Services.AddLogging(config => config.AddConsole());
    }

    public static void AddCors(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddCors(options =>
            {
                // ReSharper disable once VariableHidesOuterVariable
                options.AddDefaultPolicy(builder =>
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }
    }
}