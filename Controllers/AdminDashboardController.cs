using e_learning_app.Models;
using e_learning_app.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace e_learning_app.Controllers;

[Authorize(Roles = "Admin")]
public class AdminDashboardController : Controller
{
    private readonly IUserService _userService;

    public AdminDashboardController(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsers();
        return View(users);
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        var existingUser = await _userService.GetUserByEmail(user.Email);
        if (existingUser != null)
        {
            ViewBag.ErrorMessage = "Użytkownik z tym adresem e-mail już istnieje.";
            return View(user);
        }

        user.Role ??= "Student"; 
        await _userService.CreateUser(user);

        return RedirectToAction("Index");
    }

    // ✅ GET: Formularz edycji użytkownika
    public async Task<IActionResult> Edit(Guid id)
    {
        var user = await _userService.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // ✅ POST: Aktualizacja użytkownika
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, User user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        await _userService.UpdateUser(id, user);
        return RedirectToAction("Index");
    }

    // ✅ Usuwanie użytkownika
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteUser(id);
        return RedirectToAction("Index");
    }
}