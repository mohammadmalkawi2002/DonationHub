using DonationHub.Api;
using DonationHub.Application;
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.Persistence.Seed;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);
    

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var databaseSeeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await databaseSeeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().WithDocumentPerVersion();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("DonationHub API");

        var descriptions = app.DescribeApiVersions();

        for (var i = 0; i < descriptions.Count; i++)
        {
            var description = descriptions[i];

            options.AddDocument(
                description.GroupName,
                description.GroupName,
                isDefault: i == descriptions.Count - 1);
        }
    });
}

//Custom for the order of Midllewares:
app.UseCoreMiddlewares(builder.Configuration);

app.MapControllers();
app.MapStaticAssets();


await app.RunAsync();
