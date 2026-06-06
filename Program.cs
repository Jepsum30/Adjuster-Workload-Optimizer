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
builder.Services.AddScoped<AssignmentEngine>();

// ------------------------------------------------------------
// SESSION — REQUIRED for cookie‑based auth
// ------------------------------------------------------------
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    // HTTPS ONLY (required for class security)
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    // SameSite=None because cookies are secure
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
// Serve Frontend (STATIC FILES)
// ------------------------------------------------------------
app.UseDefaultFiles();
app.UseStaticFiles();

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
