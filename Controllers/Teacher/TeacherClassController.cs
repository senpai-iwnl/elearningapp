using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_learning_app.Controllers;

[Authorize(Roles = "Teacher")]
public class TeacherClassController : Controller
{
    private readonly AppDbContext _context;

    public TeacherClassController(AppDbContext context)
    {
        _context = context;
    }

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
        if (subject == null) return NotFound("Nie znaleziono przedmiotu.");

        newClass.Id = Guid.NewGuid();
        newClass.SubjectId = subjectId;

        _context.Classes.Add(newClass);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "TeacherSubject", new { id = subjectId });
    }

    public async Task<IActionResult> EditClass(Guid id)
    {
        var classItem = await _context.Classes.Include(c => c.Documents).FirstOrDefaultAsync(c => c.Id == id);
        if (classItem == null) return NotFound("Nie znaleziono klasy.");
        return View(classItem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditClass(Guid id, Class updatedClass)
    {
        var classItem = await _context.Classes.FindAsync(id);
        if (classItem == null) return NotFound();

        if (!ModelState.IsValid) return View(updatedClass);

        classItem.Name = updatedClass.Name;
        classItem.Description = updatedClass.Description;
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "TeacherSubject", new { id = classItem.SubjectId });
    }
    
    public async Task<IActionResult> DeleteClass(Guid id)
    {
        // Pobierz klasę z bazy danych
        var classItem = await _context.Classes.FindAsync(id);
        if (classItem == null)
        {
            return NotFound("Nie znaleziono klasy.");
        }

        // Zapamiętaj `SubjectId` przed usunięciem klasy
        var subjectId = classItem.SubjectId;

        _context.Classes.Remove(classItem);
        await _context.SaveChangesAsync();

        // Zweryfikuj, czy `SubjectId` jest poprawne, aby przekierować
        if (subjectId == Guid.Empty)
        {
            return RedirectToAction("Details", "TeacherSubject");
        }

        return RedirectToAction("Details", "TeacherSubject", new { id = subjectId });
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


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDocument(Guid documentId)
    {
        // Znajdź dokument na podstawie jego ID
        var document = await _context.Documents.FindAsync(documentId);

        if (document == null)
        {
            return NotFound("Nie znaleziono dokumentu.");
        }

        // Pobierz ID klasy przed usunięciem dokumentu
        var classId = document.ClassId;

        // Usuń dokument z bazy danych
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();

        // Pobierz klasę na podstawie ID
        var classItem = await _context.Classes.FindAsync(classId);

        // Obsługa przypadków, gdy klasa została usunięta
        if (classItem == null)
        {
            TempData["Message"] = "Dokument usunięto, ale powiązana klasa już nie istnieje.";
            return RedirectToAction("Details", "TeacherSubject");
        }

        // Przekierowanie do szczegółów przedmiotu
        return RedirectToAction("Details", "TeacherSubject", new { id = classItem.SubjectId });
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
    
    private async Task<byte[]> ConvertToBytes(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}