using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register API controllers
builder.Services.AddControllers();

// Register the EF Core DbContext (scoped lifetime) using SQLite
// Connection string is read from appsettings.json -> ConnectionStrings:DefaultConnection
builder.Services.AddDbContext<AppDbContext>(opt =>
{
   opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Map attribute-routed controller endpoints
app.MapControllers();

// Create a manual DI scope: DbContext is scoped, and there is no HTTP request scope at startup
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
   var context = services.GetRequiredService<AppDbContext>();

   // Apply any pending migrations (creates the database if it doesn't exist)
   await context.Database.MigrateAsync();

   // Seed initial data
   await DbInitializer.SeedData(context);
}
catch (Exception ex)
{
   // Log migration/seeding errors without crashing the app
   var logger = services.GetRequiredService<ILogger<Program>>();
   logger.LogError(ex, "An error occurred during migration.");
}

// Start the web server and listen for incoming requests
app.Run();