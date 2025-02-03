using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace e_learning_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Pobierz wiadomości ogólne (te, które nie są przypisane do żadnego przedmiotu)
            var generalMessages = await _context.Messages
                .Where(m => m.SubjectId == null)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return View(generalMessages);
        }
    }
}