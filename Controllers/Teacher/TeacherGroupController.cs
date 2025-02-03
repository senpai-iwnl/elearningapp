using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_learning_app.Controllers;

[Authorize(Roles = "Teacher")]
public class TeacherGroupController : Controller
{
    private readonly AppDbContext _context;

    public TeacherGroupController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ManageGroups(Guid id)
    {
        var subject = await _context.Subjects
            .Include(s => s.Groups)
            .ThenInclude(g => g.Students)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subject == null) return NotFound("Nie znaleziono przedmiotu.");

        var allStudents = await _context.Users
            .Where(u => u.Role == "Student")
            .ToListAsync();

        var viewModel = new ManageGroupsViewModel
        {
            Subject = subject,
            AllStudents = allStudents,
            Groups = subject.Groups
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddGroup(Guid subjectId, string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName)) return BadRequest("Nazwa grupy nie może być pusta.");

        var subject = await _context.Subjects.FindAsync(subjectId);
        if (subject == null) return NotFound();

        var group = new Group { Id = Guid.NewGuid(), Name = groupName, SubjectId = subjectId };
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        return RedirectToAction("ManageGroups", new { id = subjectId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteGroup(Guid groupId)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null) return NotFound();

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();

        return RedirectToAction("ManageGroups", new { id = group.SubjectId });
    }
    
    [HttpGet]
    public async Task<IActionResult> EditGroup(Guid id)
    {
        var group = await _context.Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
        {
            return NotFound("Nie znaleziono grupy.");
        }

        var subject = await _context.Subjects
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == group.SubjectId);

        var availableStudents = subject.Students
            .Where(s => !group.Students.Contains(s))
            .ToList();

        var viewModel = new EditGroupViewModel
        {
            Group = group,
            AvailableStudents = availableStudents
        };

        return View(viewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddStudentToGroup(Guid groupId, Guid studentId)
    {
        var group = await _context.Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null) return NotFound();

        var student = await _context.Users.FindAsync(studentId);
        if (student == null) return NotFound();

        group.Students.Add(student);
        await _context.SaveChangesAsync();

        return RedirectToAction("EditGroup", new { id = groupId });
    }
    
    [HttpPost]
    public async Task<IActionResult> RemoveStudentFromGroup(Guid groupId, Guid studentId)
    {
        var group = await _context.Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null) return NotFound();

        var student = group.Students.FirstOrDefault(s => s.Id == studentId);
        if (student != null)
        {
            group.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("EditGroup", new { id = groupId });
    }
}
