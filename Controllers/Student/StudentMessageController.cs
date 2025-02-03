using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace e_learning_app.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentMessageController : Controller
    {
        private readonly AppDbContext _context;

        public StudentMessageController(AppDbContext context)
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

            // Pobieranie wiadomości ogólnych
            var generalMessages = await _context.Messages
                .Where(m => m.SubjectId == null)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            // Pobieranie wiadomości powiązanych z przedmiotami studenta
            var subjectMessages = await _context.Messages
                .Where(m => m.SubjectId != null && 
                            m.Subject.Students.Any(s => s.Id == userId))
                .Include(m => m.Subject)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            // Łączenie wiadomości w model
            var viewModel = new StudentMessagesViewModel
            {
                GeneralMessages = generalMessages,
                SubjectMessages = subjectMessages
            };

            return View(viewModel);
        }
    }
}