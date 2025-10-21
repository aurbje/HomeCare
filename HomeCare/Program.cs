using HomeCare.Data; // ← AppDbContext (namespace)
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// register AppDbContext in DI
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Legg til tjenester for MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// DB Initialize
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(context);
}


// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // For CSS, JS, bilder

app.UseRouting();

app.UseAuthorization();

// Endepunkter for MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();