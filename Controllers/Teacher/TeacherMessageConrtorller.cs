

using System.Security.Claims;
using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_learning_app.Controllers.Teacher;

[Authorize(Roles = "Teacher")]
public class TeacherMessageController : Controller
{
    private readonly AppDbContext _context;

    public TeacherMessageController(AppDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
    }

    // 🔹 Lista wiadomości dla nauczyciela
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var messages = await _context.Messages
            .Where(m => m.SenderId == userId)
            .Include(m => m.Subject)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return View(messages);
    }

    // 🔹 Formularz tworzenia wiadomości
    public async Task<IActionResult> Create()
    {
        ViewBag.Subjects = await _context.Subjects.ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Message message)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            return View(message);
        }

        message.SenderId = GetUserId();
        message.CreatedAt = DateTime.UtcNow;

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // 🔹 Usuwanie wiadomości
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var message = await _context.Messages.FindAsync(id);
        if (message == null) return NotFound();

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}