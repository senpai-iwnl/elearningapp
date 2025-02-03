using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace e_learning_app.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherSubjectController : Controller
    {
        private readonly AppDbContext _context;

        public TeacherSubjectController(AppDbContext context)
        {
            _context = context;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var userId = GetUserId();

            // Pobieramy przedmiot, włączając wszystkie powiązane dane
            var subject = await _context.Subjects
                .Include(s => s.Classes) // Ładowanie klas
                .ThenInclude(c => c.Documents) // Ładowanie załączników powiązanych z klasami
                .Include(s => s.Groups) // Ładowanie grup
                .ThenInclude(g => g.Students) // Ładowanie studentów w grupach
                .Include(s => s.Students) // Ładowanie studentów przypisanych do przedmiotu
                .Include(s => s.Messages) // Ładowanie wiadomości powiązanych z przedmiotem
                .FirstOrDefaultAsync(s => s.Id == id && s.CreatorId == userId);

            if (subject == null)
            {
                return NotFound("Nie znaleziono przedmiotu lub brak dostępu.");
            }

            return View(subject);
        }
    }
}