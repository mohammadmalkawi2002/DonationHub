using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.SecurityManager.Authorization;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Identity.Jwt;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Persistence.Interceptors;
using DonationHub.Infrastructure.Persistence.Repositories;
using DonationHub.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
namespace DonationHub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Register TimeProvider as a singleton service to provide the current system time throughout the application.
            services.AddSingleton<TimeProvider>(TimeProvider.System);

            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");


            //========================== Register Interceptor==========================
            services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();


            //========================== Register DbContext==========================
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);

                options.AddInterceptors(
                    sp.GetRequiredService<ISaveChangesInterceptor>());
            });

            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());



            //========================== Register Identity===============================================:
            services.AddIdentityCore<ApplicationUser>(options =>
          {
              // User
              options.User.RequireUniqueEmail = true;

              // Password
              options.Password.RequiredLength = 8;
              options.Password.RequireDigit = true;
              options.Password.RequireUppercase = true;
              options.Password.RequireLowercase = true;
              options.Password.RequireNonAlphanumeric = true;

              ///TODO: later you need to modifiy the login logic to check account attemp:
              // Lockout 
              options.Lockout.AllowedForNewUsers = true;
              options.Lockout.MaxFailedAccessAttempts = 5;
              options.Lockout.DefaultLockoutTimeSpan =
                  TimeSpan.FromMinutes(15);
          })
          .AddRoles<IdentityRole<Guid>>()
          .AddEntityFrameworkStores<AppDbContext>()
          .AddDefaultTokenProviders();


            //========================= Register JwtOptions from configuration and JWT Authentication===============================================
            services.AddOptions<JwtOptions>()
                  .BindConfiguration(JwtOptions.SectionName)
                  .ValidateDataAnnotations()
                  .ValidateOnStart();

            var jwtOptions = configuration.GetSection(JwtOptions.SectionName)
                              .Get<JwtOptions>()
                              ?? throw new InvalidOperationException(
                                "JWT configuration is missing.");


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               // Keep claim names exactly as they are in the token ("sub", "role", ...).
               options.MapInboundClaims = false;

               options.TokenValidationParameters =
                   new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidIssuer = jwtOptions.Issuer,

                       ValidateAudience = true,
                       ValidAudience = jwtOptions.Audience,

                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

                       ValidateLifetime = true,
                       ClockSkew = TimeSpan.FromSeconds(30),

                       NameClaimType = ApplicationClaimTypes.Name,
                       RoleClaimType = ApplicationClaimTypes.Role
                   };
           });

            services.AddAuthorization();


            //==================== Register SeedAdminOptions from configuration=======================
            services.AddOptions<SeedAdminOptions>()
                    .BindConfiguration(SeedAdminOptions.SectionName)
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            services.AddOptions<DevelopmentSeedOptions>()
                    .BindConfiguration(DevelopmentSeedOptions.SectionName);

            services.AddScoped<DatabaseSeeder>();
            services.AddScoped<InitialDataSeeder>();
            services.AddScoped<DevelopmentSeeder>();


            //=====================Register Repositries and services:========================
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //============= Token Service======================
            services.AddScoped<ITokenService, TokenService>();

            //============= Identity Service========================
            services.AddScoped<IIdentityService, IdentityService>();

            //=============TemporaryPasswordGenerator=================
            services.AddSingleton<ITemporaryPasswordGenerator, TemporaryPasswordGenerator>();


            return services;
        }
    }
}
