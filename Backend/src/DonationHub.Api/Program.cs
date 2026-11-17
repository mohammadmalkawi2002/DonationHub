using DonationHub.Api;
using DonationHub.Application;
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);
    
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var databaseSeeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await databaseSeeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//Custom for the order of Midllewares:
app.UseCoreMiddlewares(builder.Configuration);

app.MapControllers();
app.MapStaticAssets();


await app.RunAsync();
