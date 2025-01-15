using Microsoft.EntityFrameworkCore;
using Shopily.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Register services with dependency injection
builder.Services.AddControllersWithViews();

// Register the DbContext and configure it to use the connection string from appsettings.json
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Read connection string from appsettings.json

// Other services can be added here

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
