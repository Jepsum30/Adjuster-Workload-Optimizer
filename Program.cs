using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// JSON + Controllers
// ------------------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ------------------------------------------------------------
// Database (MySQL)
// ------------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    )
);

// ------------------------------------------------------------
// Dependency Injection
// ------------------------------------------------------------
builder.Services.AddScoped<AssignmentEngine>();

// ------------------------------------------------------------
// NO CORS NEEDED FOR SAME-ORIGIN
// ------------------------------------------------------------

// ------------------------------------------------------------
// SESSION — SAME-ORIGIN COOKIE
// ------------------------------------------------------------
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    // ⭐ SAME-ORIGIN SETTINGS ⭐
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;

    // ⭐ DO NOT SET DOMAIN ⭐
    // Chrome will automatically bind to https://localhost:7143

    options.Cookie.Path = "/";
    options.IdleTimeout = TimeSpan.FromHours(1);
});

// ------------------------------------------------------------
// AUTH COOKIE — SAME-ORIGIN COOKIE
// ------------------------------------------------------------
builder.Services.AddAuthentication("AuthCookie")
    .AddCookie("AuthCookie", options =>
    {
        options.LoginPath = "/pages/login.html";
        options.AccessDeniedPath = "/pages/access-denied.html";

        options.Cookie.HttpOnly = true;

        // ⭐ SAME-ORIGIN SETTINGS ⭐
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;

        // ⭐ DO NOT SET DOMAIN ⭐
        options.Cookie.Path = "/";
    });

builder.Services.AddAuthorization();

// ------------------------------------------------------------
// Swagger
// ------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ------------------------------------------------------------
// Swagger UI
// ------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ------------------------------------------------------------
// HTTPS Redirect
// ------------------------------------------------------------
app.UseHttpsRedirection();

// ------------------------------------------------------------
// STATIC FILES FIRST (serves your frontend)
// ------------------------------------------------------------
app.UseDefaultFiles();
app.UseStaticFiles();

// ------------------------------------------------------------
// ROUTING
// ------------------------------------------------------------
app.UseRouting();

// ------------------------------------------------------------
// SESSION + AUTH
// ------------------------------------------------------------
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------------------------
// API Controllers
// ------------------------------------------------------------
app.MapControllers();

app.Run();
