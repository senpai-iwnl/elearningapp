using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace e_learning_app.Controllers;

[Authorize(Roles = "Student")]
public class StudentGroupController : Controller
{
    private readonly AppDbContext _context;

    public StudentGroupController(AppDbContext context)
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

        var groups = await _context.Groups
            .Include(g => g.Subject) // Pobranie przedmiotu powiązanego z grupą
            .Include(g => g.Students) // Pobranie studentów w grupie
            .Where(g => g.Students.Any(s => s.Id == userId)) // Filtrujemy tylko grupy, do których należy student
            .ToListAsync();

        return View(groups);
    }
}