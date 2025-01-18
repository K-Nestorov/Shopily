using Microsoft.EntityFrameworkCore;
using Shopily.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Register services with dependency injection
builder.Services.AddControllersWithViews();

// Register the DbContext and configure it to use the connection string from appsettings.json
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Read connection string from appsettings.json

// Add authentication services with cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login"; // Redirect to login page if the user is not authenticated
        options.LogoutPath = "/User/Logout"; // Redirect to logout page
        options.AccessDeniedPath = "/Home/Index"; // Redirect to an access denied page if the user doesn't have permissions
    });

// Other services can be added here (e.g., scoped services, etc.)

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthentication(); // Enables authentication middleware
app.UseAuthorization(); // Enables authorization middleware
app.UseStaticFiles(); // Enables static file middleware (CSS, JS, etc.)

// Default route setup for MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
