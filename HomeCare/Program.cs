using HomeCare.Data;
using HomeCare.Repositories;
using HomeCare.Repositories.Interfaces;
using HomeCare.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HomeCare.Models;

var builder = WebApplication.CreateBuilder(args);

// cleared the default providers and added console + debug logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllersWithViews();

// register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>(); // added booking repository here

//
builder.Services.AddScoped<IPersonnelRepository, PersonnelRepository>();


// database connection (using sqlite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// database initialization
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // DbInitializer.Seed(context);
        // if there is no user in db (temporary)
        if (!context.Users.Any(u => u.Id == 1))
        {
            context.Users.Add(new User
            {
                Id = 1,
                FullName = "Test Personnel",
                Email = "test@example.com",
                Role = "Personnel"
            });
            // context.Users.Add(new User
            // {
            //     FullName = "Admin",
            //     Email = "admin@homecare.com",
            //     Role = "Admin"
            // });
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error while seeding the database");
    }
}

// in production mode we send the user to a friendly error page
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // show full error details while developing
    app.UseDeveloperExceptionPage();
}

// catch all unhandled exceptions and log them
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, $"Unexpected error on path: {context.Request.Path}");
        throw;
    }
});

// middleware setup
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// mvc route setup
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
