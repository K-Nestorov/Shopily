using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Shopily.Data;
using Shopily.Domain.Models;
using Shopily.Repositories;
using System.Diagnostics;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Database context configuration
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Custom service registrations
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ShopRepository>();
builder.Services.AddScoped<AdminRepository>();
builder.Services.AddTransient<ScraperService>();  // Add this line

// HttpContext accessor
builder.Services.AddHttpContextAccessor();

// Session management
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Authentication and authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.LogoutPath = "/User/Logout";
        options.AccessDeniedPath = "/Home/Index";
    });

// Stripe configuration
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));

var app = builder.Build();

// Middleware configuration
app.Use(async (context, next) =>
{
    if (context.Request.Cookies.TryGetValue("Language", out string? cookie))
    {
        var culture = new CultureInfo(cookie);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
    else
    {
        var defaultCulture = new CultureInfo("en");
        CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
        CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
    }
    await next.Invoke();
});

// Start Python script in the background
StartPythonScript();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseSession();

// Routing configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "news",
    pattern: "{controller=News}/{action=Scraper}/{id?}");

app.Run();

// Python script runner function
void StartPythonScript()
{
    try
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "py",
            Arguments = "\"C:\\Users\\kristiqn\\Desktop\\scraper_api.py\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
        process.ErrorDataReceived += (sender, e) => Console.WriteLine($"Error: {e.Data}");

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to start Python script: {ex.Message}");
    }
}