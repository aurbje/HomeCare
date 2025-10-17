using Microsoft.EntityFrameworkCore;
using HomeCare.Data;
using HomeCare.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Legg til tjenester for MVC
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUserRepository, UserRepository>();

// Legg til DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // For CSS, JS, bilder

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Endepunkter for MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
