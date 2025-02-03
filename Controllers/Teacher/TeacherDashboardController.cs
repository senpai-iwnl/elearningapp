using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace e_learning_app.Controllers;

[Authorize(Roles = "Teacher")]
public class TeacherDashboardController : Controller
{
    private readonly AppDbContext _context;

    public TeacherDashboardController(AppDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var subjects = await _context.Subjects
            .Where(s => s.CreatorId == userId) // Filtruj tylko przedmioty stworzone przez zalogowanego nauczyciela
            .ToListAsync();

        return View(subjects);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Subject newSubject)
    {
        var userId = GetUserId(); // Pobranie ID zalogowanego nauczyciela
        newSubject.CreatorId = userId; // Ustawienie klucza obcego przed walidacją

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            Console.WriteLine("Błędy walidacji: " + userId + string.Join(", ", errors));

            return View(newSubject);
        }

        newSubject.Id = Guid.NewGuid();
        newSubject.JoinCode = GenerateJoinCode(); // Generowanie kodu dołączenia

        _context.Subjects.Add(newSubject);
        await _context.SaveChangesAsync();

        Console.WriteLine("Dodano przedmiot: " + newSubject.Name);
        return RedirectToAction(nameof(Index));
    }
    

    public async Task<IActionResult> Edit(Guid id)
    {
        var userId = GetUserId();
        var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id && s.CreatorId == userId);

        if (subject == null)
            return NotFound();

        return View(subject);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Subject updatedSubject)
    {
        var userId = GetUserId();
        var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id && s.CreatorId == userId);

        if (subject == null)
            return NotFound();

        if (!ModelState.IsValid)
            return View(updatedSubject);

        subject.Name = updatedSubject.Name;
        subject.Description = updatedSubject.Description;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id && s.CreatorId == userId);

        if (subject == null)
            return NotFound();

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private static string GenerateJoinCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, 10)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }
}
