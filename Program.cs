using e_learning_app.Data;
using e_learning_app.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja połączenia z bazą danych (PostgreSQL lub inna baza)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Rejestracja serwisu użytkowników
builder.Services.AddScoped<IUserService, UserService>();

// Konfiguracja uwierzytelniania (Cookie Authentication)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

// Konfiguracja autoryzacji
builder.Services.AddAuthorization();

// Dodanie MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Obsługa błędów w trybie produkcji
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Ścieżki do plików statycznych
app.UseStaticFiles();

// Routing
app.UseRouting();

// Uwierzytelnianie i autoryzacja
app.UseAuthentication();
app.UseAuthorization();

// Konfiguracja domyślnej trasy
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();