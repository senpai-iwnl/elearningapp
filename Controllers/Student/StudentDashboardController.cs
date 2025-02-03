using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace e_learning_app.Controllers;

[Authorize(Roles = "Student")]
public class StudentDashboardController : Controller
{
    private readonly AppDbContext _context;

    public StudentDashboardController(AppDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
    }

    // 🔹 Wyświetlanie przedmiotów, do których student jest zapisany
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var subjects = await _context.Subjects
            .Where(s => s.Students.Any(u => u.Id == userId)) // Przedmioty, do których student należy
            .ToListAsync();

        return View(subjects);
    }

    // 🔹 Dołączanie do przedmiotu za pomocą Join Code (GET)
    public IActionResult Join()
    {
        return View();
    }

    // 🔹 Dołączanie do przedmiotu za pomocą Join Code (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(string joinCode)
    {
        if (string.IsNullOrWhiteSpace(joinCode))
        {
            ViewBag.ErrorMessage = "Kod dołączenia nie może być pusty.";
            return View();
        }

        var userId = GetUserId();
        var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.JoinCode == joinCode);

        if (subject == null)
        {
            ViewBag.ErrorMessage = "Nie znaleziono przedmiotu z podanym kodem dołączenia.";
            return View();
        }

        // Sprawdź, czy student już jest zapisany na przedmiot
        var isAlreadyEnrolled = await _context.Subjects
            .AnyAsync(s => s.Id == subject.Id && s.Students.Any(u => u.Id == userId));

        if (isAlreadyEnrolled)
        {
            ViewBag.ErrorMessage = "Jesteś już zapisany na ten przedmiot.";
            return View();
        }

        // Dodaj studenta do przedmiotu
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        subject.Students.Add(user);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // 🔹 Opuszczanie przedmiotu
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Leave(Guid id)
    {
        var userId = GetUserId();
        var subject = await _context.Subjects
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subject == null)
        {
            return NotFound("Nie znaleziono przedmiotu.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || !subject.Students.Contains(user))
        {
            return BadRequest("Nie jesteś zapisany na ten przedmiot.");
        }

        // Usuń studenta z listy przedmiotu
        subject.Students.Remove(user);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}

