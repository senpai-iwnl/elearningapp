using e_learning_app.Models;
using Microsoft.EntityFrameworkCore;

namespace e_learning_app.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Group> Groups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 🔹 Unikalność adresu email dla użytkowników
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // 🔹 Relacja: Subject -> Creator (Twórca przedmiotu, jeden-do-wielu)
        modelBuilder.Entity<Subject>()
            .HasOne(s => s.Creator)
            .WithMany(u => u.Subjects) // Użytkownik może stworzyć wiele przedmiotów
            .HasForeignKey(s => s.CreatorId)
            .OnDelete(DeleteBehavior.Restrict); // Nie pozwalamy na usunięcie twórcy przedmiotu

        // 🔹 Relacja: Subject -> Classes (Jeden przedmiot może mieć wiele klas)
        modelBuilder.Entity<Class>()
            .HasOne(c => c.Subject)
            .WithMany(s => s.Classes)
            .HasForeignKey(c => c.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // 🔹 Relacja wiele-do-wielu: Subject <-> Students (zapisani na przedmiot)
        modelBuilder.Entity<Subject>()
            .HasMany(s => s.Students)
            .WithMany(u => u.EnrolledSubjects)
            .UsingEntity<Dictionary<string, object>>(
                "SubjectStudent",
                j => j.HasOne<User>().WithMany().HasForeignKey("StudentId"),
                j => j.HasOne<Subject>().WithMany().HasForeignKey("SubjectId")
            );

        // 🔹 Relacja: Document -> Class (Jeden do wielu)
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Class)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.ClassId)
            .OnDelete(DeleteBehavior.Cascade); // Jeśli klasa zostanie usunięta, usuwamy dokumenty


        // 🔹 Relacja: Test -> Subject (Jeden do wielu)
        modelBuilder.Entity<Test>()
            .HasOne(t => t.Subject)
            .WithMany()
            .HasForeignKey(t => t.SubjectId);

        // 🔹 Relacja: Project -> User (Jeden do wielu)
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Student)
            .WithMany()
            .HasForeignKey(p => p.StudentId);

        // 🔹 Relacja wiele-do-wielu: Group <-> User
        modelBuilder.Entity<Group>()
            .HasMany(g => g.Students)
            .WithMany(u => u.Groups)
            .UsingEntity<Dictionary<string, object>>(
                "GroupUser",
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                j => j.HasOne<Group>().WithMany().HasForeignKey("GroupId")
            );
        
        modelBuilder.Entity<Group>()
            .HasOne(g => g.Subject)
            .WithMany(s => s.Groups)
            .HasForeignKey(g => g.SubjectId)
            .OnDelete(DeleteBehavior.Cascade); // Usunięcie przedmiotu usuwa wszystkie jego grupy


    }
}
