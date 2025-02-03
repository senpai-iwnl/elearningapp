using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace e_learning_app.Controllers;

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

    // 🔹 Szczegóły przedmiotu i lista jego klas
    public async Task<IActionResult> Details(Guid id)
    {
        var userId = GetUserId();

        var subject = await _context.Subjects
            .Include(s => s.Classes)
            .ThenInclude(c => c.Documents)
            .FirstOrDefaultAsync(s => s.Id == id && s.CreatorId == userId);

        if (subject == null)
        {
            return NotFound("Nie znaleziono przedmiotu lub brak dostępu.");
        }

        return View(subject);
    }

    // 🔹 Formularz dodawania klasy
    public IActionResult AddClass(Guid subjectId)
    {
        ViewBag.SubjectId = subjectId;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddClass(Guid subjectId, Class newClass)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.SubjectId = subjectId;
            return View(newClass);
        }

        var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);

        if (subject == null)
        {
            return NotFound("Nie znaleziono przedmiotu.");
        }

        newClass.Id = Guid.NewGuid();
        newClass.SubjectId = subjectId;

        _context.Classes.Add(newClass);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = subjectId });
    }

    // 🔹 Edycja klasy
    public async Task<IActionResult> EditClass(Guid id)
    {
        var classItem = await _context.Classes.Include(c => c.Documents).FirstOrDefaultAsync(c => c.Id == id);
        if (classItem == null)
        {
            return NotFound("Nie znaleziono klasy.");
        }

        return View(classItem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditClass(Guid id, Class updatedClass)
    {
        var classItem = await _context.Classes.FindAsync(id);

        if (classItem == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(updatedClass);
        }

        classItem.Name = updatedClass.Name;
        classItem.Description = updatedClass.Description;

        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = classItem.SubjectId });
    }

    // 🔹 Usuwanie klasy
    public async Task<IActionResult> DeleteClass(Guid id)
    {
        var classItem = await _context.Classes.FindAsync(id);
        if (classItem == null)
        {
            return NotFound();
        }

        _context.Classes.Remove(classItem);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = classItem.SubjectId });
    }

    // 🔹 Dodawanie dokumentów do klasy
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDocuments(Guid classId, List<IFormFile> attachments)
    {
        var classItem = await _context.Classes
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.Id == classId);

        if (classItem == null)
        {
            TempData["Error"] = "Nie znaleziono klasy. Może została usunięta.";
            return RedirectToAction("EditClass", new { id = classId });
        }

        if (attachments == null || attachments.Count == 0)
        {
            TempData["Error"] = "Musisz wybrać co najmniej jeden plik.";
            return RedirectToAction("EditClass", new { id = classId });
        }

        try
        {
            foreach (var attachment in attachments)
            {
                var existingDocument = await _context.Documents
                    .FirstOrDefaultAsync(d => d.FileName == attachment.FileName && d.ClassId == classId);

                if (existingDocument != null)
                {
                    // Jeśli dokument już istnieje, pomiń go lub wyświetl komunikat
                    TempData["Error"] = $"Plik '{attachment.FileName}' już istnieje w tej klasie.";
                    continue;
                }

                var document = new Document
                {
                    Id = Guid.NewGuid(), // Generujemy nowe ID
                    ClassId = classId,
                    FileName = attachment.FileName,
                    ContentType = attachment.ContentType,
                    Data = await ConvertToBytes(attachment)
                };

                _context.Documents.Add(document);
            }

            var changes = await _context.SaveChangesAsync();
            if (changes == 0)
            {
                throw new DbUpdateConcurrencyException("Nie udało się zapisać dokumentów. Spróbuj ponownie.");
            }
        }
        catch (DbUpdateConcurrencyException ex)
        {
            TempData["Error"] = "Wystąpił konflikt podczas zapisu. Spróbuj ponownie.";
            Console.WriteLine($"[Błąd]: {ex.Message}");
            return RedirectToAction("EditClass", new { id = classId });
        }

        TempData["Success"] = "Dokumenty dodane pomyślnie.";
        return RedirectToAction("EditClass", new { id = classId });
    }


    // 🔹 Usuwanie dokumentu
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDocument(Guid documentId)
    {
        var document = await _context.Documents.FindAsync(documentId);

        if (document == null)
        {
            return NotFound("Nie znaleziono dokumentu.");
        }

        var classId = document.ClassId; // Pobieramy ID klasy przed usunięciem dokumentu
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();

        // Przekierowanie z powrotem do widoku przedmiotu po usunięciu dokumentu
        var classItem = await _context.Classes.FindAsync(classId);
        return RedirectToAction("Details", new { id = classItem?.SubjectId });
    }

    
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
    
    [HttpGet]
    public async Task<IActionResult> ManageStudents(Guid id)
    {
        var subject = await _context.Subjects
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subject == null)
        {
            return NotFound("Nie znaleziono przedmiotu.");
        }

        var allStudents = await _context.Users
            .Where(u => u.Role == "Student")
            .ToListAsync();

        var viewModel = new ManageStudentsViewModel
        {
            Subject = subject,
            AllStudents = allStudents,
            SelectedStudents = subject.Students.Select(s => s.Id).ToList()
        };

        return View(viewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddStudent(Guid subjectId, Guid studentId)
    {
        var subject = await _context.Subjects
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == subjectId);

        if (subject == null) return NotFound();

        var student = await _context.Users.FindAsync(studentId);
        if (student == null) return NotFound();

        subject.Students.Add(student);
        await _context.SaveChangesAsync();

        return RedirectToAction("ManageStudents", new { id = subjectId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveStudent(Guid subjectId, Guid studentId)
    {
        var subject = await _context.Subjects
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == subjectId);

        if (subject == null) return NotFound();

        var student = subject.Students.FirstOrDefault(s => s.Id == studentId);
        if (student != null)
        {
            subject.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("ManageStudents", new { id = subjectId });
    }


    // 🔹 Pomocnicza metoda do konwersji pliku na bajty
    private async Task<byte[]> ConvertToBytes(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}