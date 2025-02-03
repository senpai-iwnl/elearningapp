using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace e_learning_app.Controllers;

[Authorize(Roles = "Student")]
public class StudentSubjectController : Controller
{
    private readonly AppDbContext _context;

    public StudentSubjectController(AppDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
    }

    // 🔹 Wyświetlanie klas przypisanych do danego przedmiotu
    public async Task<IActionResult> Details(Guid id)
    {
        var userId = GetUserId();

        var subject = await _context.Subjects
            .Include(s => s.Classes)
            .ThenInclude(c => c.Documents)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subject == null)
        {
            return NotFound("Nie znaleziono przedmiotu.");
        }

        return View(subject);
    }

    // 🔹 Pobieranie dokumentów
    [HttpGet]
    public async Task<IActionResult> DownloadDocument(Guid documentId)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);

        if (document == null)
        {
            return NotFound("Nie znaleziono dokumentu.");
        }

        return File(document.Data, document.ContentType, document.FileName);
    }
}