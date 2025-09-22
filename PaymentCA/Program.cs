using Application;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Options;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payments API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Введите токен без Bearer",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>()!;

var issuer = jwtOptions.Issuer;
var audience = jwtOptions.Audience;
var signingKey = jwtOptions.Secret;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ClockSkew = TimeSpan.Zero
        };

        o.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                var db = ctx.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

                var sub = ctx.Principal!.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? ctx.Principal!.FindFirst("sub")?.Value;
                var jti = ctx.Principal!.FindFirst("jti")?.Value;

                if (!int.TryParse(sub, out var userId) || string.IsNullOrEmpty(jti))
                {
                    ctx.Fail("Invalid token claims.");
                    return;
                }

                var active = await db.Sessions
                    .AnyAsync(s => s.UserId == userId
                                   && s.Jti == jti
                                   && s.RevokedAt == null
                                   && (s.ExpiresAt == null || s.ExpiresAt > DateTimeOffset.UtcNow));

                if (!active)
                    ctx.Fail("Session is not active.");
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler(_ => { });

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.EnvironmentName.Equals("Docker", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await EnsureDbAndSeedAsync(app);

app.Run();

static async Task EnsureDbAndSeedAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var u = await users.FindByNameAsync("user");
    if (u is null)
    {
        u = new ApplicationUser
        {
            UserName = "user",
            Email = "user@example.com",
            BalanceCents = 800
        };

        var result = await users.CreateAsync(u, "abc1234");
        if (!result.Succeeded)
        {
            throw new Exception("Failed to create seed user: " +
                                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
