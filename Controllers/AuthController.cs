using System.Security.Claims;
using e_learning_app.Models;
using e_learning_app.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace e_learning_app.Controllers;

public class AuthController : Controller
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _userService.AuthenticateUser(email, password);
        if (user == null)
        {
            ViewBag.ErrorMessage = "Nieprawidłowy email lub hasło.";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties();

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        return RedirectToAction("Index", "Home");
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

        var existingUser = await _userService.GetUserByEmail(user.Email);
        if (existingUser != null)
        {
            ViewBag.ErrorMessage = "Konto z tym adresem email już istnieje.";
            Console.WriteLine("Użytkownik istnieje: " + user.Email);
            return View(user);
        }

        Console.WriteLine("Rejestracja użytkownika: " + user.Email);
        user.Role = "Student";
        await _userService.CreateUser(user);

        return RedirectToAction("Login");
    }
}

