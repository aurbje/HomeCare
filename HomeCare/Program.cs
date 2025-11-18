using HomeCare.Data;
using HomeCare.Models;
using HomeCare.Repositories.Interfaces;
using HomeCare.Repositories.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// ---------- Logging ----------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ---------- MVC ----------
builder.Services.AddControllersWithViews();

// ---------- Repositories ----------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<ICaregiverRepository, CaregiverRepository>();

// ---------- Database (SQLite) ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

// ---------- Identity (Caregiver + roller) ----------
builder.Services.AddIdentity<Caregiver, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        // her kan du evt. legge på passordkrav osv.
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// (valgfritt, men fint) – standard cookie-paths
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/CaregiverAccount/Login";
    options.AccessDeniedPath = "/CaregiverAccount/AccessDenied";
});

var app = builder.Build();

// ---------- Database seeding + roller ----------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // egen seedinglogikk (tabeller, testdata osv.)
        DbInitializer.Seed(context);

        // sørg for at rollene finnes
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
        logger.LogError(ex, "Error while seeding the database");
    }
}

// ---------- Error pages ----------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// ---------- Exception logging middleware ----------
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unexpected error on path: {Path}", context.Request.Path);
        throw;
    }
});

// ---------- Pipeline ----------
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ---------- Routing ----------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
