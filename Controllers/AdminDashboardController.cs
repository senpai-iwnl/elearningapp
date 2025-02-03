using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_learning_app.Controllers;

[Authorize(Roles = "Admin")]
public class AdminDashboardController : Controller
{
    private readonly AppDbContext _context;

    public AdminDashboardController(AppDbContext context)
    {
        _context = context;
    }

    // Wyświetlanie listy użytkowników
    public async Task<IActionResult> Index()
    {
        var users = await _context.Users.ToListAsync();
        return View(users);
    }

    // GET: Formularz tworzenia nowego użytkownika
    public IActionResult Create()
    {
        return View();
    }

    // POST: Tworzenie użytkownika
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (existingUser != null)
        {
            ViewBag.ErrorMessage = "Użytkownik z tym adresem e-mail już istnieje.";
            return View(user);
        }

        user.Role ??= "Student"; // Domyślna rola
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // GET: Formularz edycji użytkownika
    public async Task<IActionResult> Edit(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Aktualizacja użytkownika
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, User updatedUser)
    {
        if (!ModelState.IsValid)
        {
            return View(updatedUser);
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;
        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.Role = updatedUser.Role;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    // Usuwanie użytkownika
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
