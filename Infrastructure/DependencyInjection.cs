using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<AppDbContext>(o =>
                o.UseNpgsql(cfg.GetConnectionString("Default"))
                 .UseSnakeCaseNamingConvention());

            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
            services
           .AddIdentityCore<Domain.Entities.ApplicationUser>(o =>
           {
               o.Password.RequireDigit = false;
               o.Password.RequireUppercase = false;
               o.Password.RequireNonAlphanumeric = false;
               o.Password.RequiredLength = 4;

               o.Lockout.AllowedForNewUsers = true;
               o.Lockout.MaxFailedAccessAttempts = 5;
               o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
           })
           .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole<int>>()
           .AddEntityFrameworkStores<AppDbContext>();

            return services;
        }
    }
}
