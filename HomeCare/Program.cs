// Legg til tjenester for MVC
using HomeCare.Data; // ← AppDbContext (namespace)
using Microsoft.EntityFrameworkCore;
using HomeCare.Repositories;


var builder = WebApplication.CreateBuilder(args);

// register AppDbContext in DI
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


// Tjenester for MVC
//>>>>>>> main
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


// DB Initialize

//>>>>>>> main
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(context);
}


//>>>>>>> main
// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// sends user to error page in case of unhandled exceptions
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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