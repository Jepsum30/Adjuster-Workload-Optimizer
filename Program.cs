using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Services;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddScoped<AssignmentEngine>();   // ⭐ Required for auto‑reassign delete

// ------------------------------------------------------------
// CORS — REQUIRED for HTTPS frontend + cookies
// ------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
            "https://127.0.0.1:5500",
            "https://localhost:5500",
            "https://localhost:7143"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// ------------------------------------------------------------
// SESSION — REQUIRED for cookie‑based auth
// ------------------------------------------------------------
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.IdleTimeout = TimeSpan.FromHours(1);
});

// ------------------------------------------------------------
// Swagger
// ------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ------------------------------------------------------------
// STATIC FILES — MUST COME BEFORE ROUTING
// ------------------------------------------------------------
app.UseDefaultFiles();

app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,   // ⭐ Required for pages with query params
    DefaultContentType = "text/html"
});

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
// CORS MUST COME BEFORE SESSION + AUTH
// ------------------------------------------------------------
app.UseCors("FrontendPolicy");

// ------------------------------------------------------------
// SESSION MUST COME BEFORE AUTH
// ------------------------------------------------------------
app.UseSession();

// ------------------------------------------------------------
// Authentication + Authorization
// ------------------------------------------------------------
app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------------------------
// Controllers
// ------------------------------------------------------------
app.MapControllers();

app.Run();
