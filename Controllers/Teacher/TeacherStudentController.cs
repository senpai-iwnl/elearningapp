using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_learning_app.Controllers;

[Authorize(Roles = "Teacher")]
public class TeacherStudentController : Controller
{
    private readonly AppDbContext _context;

    public TeacherStudentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ManageStudents(Guid id)
    {
        var subject = await _context.Subjects
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subject == null) return NotFound("Nie znaleziono przedmiotu.");

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
}
