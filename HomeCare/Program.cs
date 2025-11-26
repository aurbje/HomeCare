using HomeCare.Data;
using HomeCare.Models;
using HomeCare.Repositories.Interfaces;
using HomeCare.Repositories.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// logging 
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// controllers / api 
// using controllers as api only, views are not needed anymore
builder.Services.AddControllers();

// cors for frontend 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "https://localhost:5173") // vite default port
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// database (sqlite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

var app = builder.Build();

// database seeding + roles
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // makes sure db exists and seed basic data
        DbInitializer.Seed(context);

        // makes sure required roles exist
        string[] roles = { "User", "Caregiver", "Admin" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "error while seeding the database");
    }
}

// error handling
if (!app.Environment.IsDevelopment())
{
// production setup, could be extended with custom error endpoint
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// exception logging middleware
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "unexpected error on path: {Path}", context.Request.Path);
        throw;
    }
});

// pipeline
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// endpoint mapping
app.MapControllers();

app.Run();
