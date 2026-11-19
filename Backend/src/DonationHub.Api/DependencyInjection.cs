using Asp.Versioning;
using DonationHub.Api.Infrastructure;
using DonationHub.Api.OpenApi.Transformers;
using DonationHub.Api.Services;
using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Common.Options;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace DonationHub.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration) 
        {
            //Register DonationOptions configuration:
            services.AddOptions<DonationOptions>()
                .BindConfiguration(DonationOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();


            services.AddCustomProblemDetails()
                    .AddCustomApiVersioning()
                    .AddExceptionHandling()
                    .AddControllerWithJsonConfiguration()
                    .AddValidation()
                    .AddIdentityInfrastructure();

            return services;

        }

        //TODO: check it later for enum
        public static IServiceCollection AddControllerWithJsonConfiguration(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options => {
                options
                .JsonSerializerOptions
                .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                options.JsonSerializerOptions.Converters.Add(
                     new JsonStringEnumConverter(allowIntegerValues: false));
            });



            return services;
        }
        public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services) 
        {
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    var problemDetails = context.ProblemDetails;
                    var status = problemDetails.Status ?? context.HttpContext.Response.StatusCode;

                    problemDetails.Status = status;
                    problemDetails.Type = $"https://httpstatuses.io/{status}";
                    problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

                    //If no code is provided, use a default value.:

                    if (!problemDetails.Extensions.ContainsKey("code"))
                        problemDetails.Extensions["code"] = DefaultCode(status, problemDetails);

                    if (status >= 500)
                    {
                        problemDetails.Title = "An unexpected error occurred.";
                        problemDetails.Detail = null;
                    }
                };


            });

            static string DefaultCode(int status, ProblemDetails pd) =>
                        pd is ValidationProblemDetails ? "Validation.Failed" : status switch
                        {
                            400 => "Request.BadRequest",
                            401 => "Auth.Unauthorized",
                            403 => "Auth.Forbidden",
                            404 => "Resource.NotFound",
                            >= 500 => "Server.Unexpected",
                            _ => $"Http.{status}"
                        };

            return services;

        }

        public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
        {

            services.AddApiVersioning(options =>
       {
           options.DefaultApiVersion = new ApiVersion(1, 0);
           options.ReportApiVersions = true;
           options.ApiVersionReader = new UrlSegmentApiVersionReader();
       })
       .AddMvc()
       .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;

            })
            .AddOpenApi(options => 
            {
                // Document information
                options.Document.AddDocumentTransformer<VersionInfoTransformer>();

                // JWT Security Scheme
                options.Document.AddDocumentTransformer<BearerSecuritySchemeTransformer>();

                // JWT Security Requirements per operation
                options.Document.AddOperationTransformer<BearerSecuritySchemeTransformer>();

                // Enum values as strings
                options.Document.AddSchemaTransformer<EnumSchemaTransformer>();
            });

            return services;
        }

       

        public static IServiceCollection AddValidation(this IServiceCollection services)
        {
            return services;
        }
        public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            return services;
        }

        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddHttpContextAccessor();
            return services;
        }

        public static IApplicationBuilder UseCoreMiddlewares(this IApplicationBuilder app, IConfiguration configuration) 
        {
            // 1. Exception handling should be FIRST to catch all errors
            app.UseExceptionHandler();

            // 2. Status code pages for handling HTTP status codes
            app.UseStatusCodePages();

            // 3. HTTPS redirection (before any other middleware that might generate URLs)
            app.UseHttpsRedirection();

            // 4. Authentication (must come before authorization)
            app.UseAuthentication();

            // 5. Authorization (must come after authentication)
            app.UseAuthorization();
            return app;

        }

    }
}
