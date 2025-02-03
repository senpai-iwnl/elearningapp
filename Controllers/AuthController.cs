using System.Security.Claims;
using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_learning_app.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
        if (user == null)
        {
            ViewBag.ErrorMessage = "Nieprawidłowy email lub hasło.";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Dodanie UserId do claimów
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties();

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        // Przekierowanie w zależności od roli użytkownika
        return user.Role switch
        {
            "Admin" => RedirectToAction("Index", "AdminDashboard"),
            "Teacher" => RedirectToAction("Index", "TeacherDashboard"),
            "Student" => RedirectToAction("Index", "StudentDashboard"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(User user)
    {
        ModelState.Remove("Role");

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            Console.WriteLine("Błędy walidacji: " + string.Join(", ", errors));
            return View(user);
        }

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (existingUser != null)
        {
            ViewBag.ErrorMessage = "Konto z tym adresem email już istnieje.";
            Console.WriteLine("Użytkownik istnieje: " + user.Email);
            return View(user);
        }

        Console.WriteLine("Rejestracja użytkownika: " + user.Email);
        user.Role = "Student";

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login");
    }
}
