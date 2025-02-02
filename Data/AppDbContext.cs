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

        // Unikalność adresu email
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // Relacja: Class -> Subject (Jeden do wielu)
        modelBuilder.Entity<Class>()
            .HasOne(c => c.Subject)
            .WithMany(s => s.Classes)
            .HasForeignKey(c => c.SubjectId);

        // Relacja: Class -> User (Twórca kursu, jeden-do-wielu)
        modelBuilder.Entity<Class>()
            .HasOne(c => c.Creator)
            .WithMany(u => u.Classes) // Użytkownik może stworzyć wiele klas
            .HasForeignKey(c => c.CreatorId)
            .OnDelete(DeleteBehavior.Restrict); // Nie pozwalamy na usunięcie twórcy klasy

        // Relacja wiele-do-wielu: User <-> Class (studenci zapisani na kurs)
        modelBuilder.Entity<Class>()
            .HasMany(c => c.Students)
            .WithMany(u => u.EnrolledClasses)
            .UsingEntity<Dictionary<string, object>>(
                "ClassStudent",
                j => j.HasOne<User>().WithMany().HasForeignKey("StudentId"),
                j => j.HasOne<Class>().WithMany().HasForeignKey("ClassId")
            );

        // Relacja: Document -> Subject (Jeden do wielu)
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Subject)
            .WithMany(s => s.Documents)
            .HasForeignKey(d => d.SubjectId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacja: Document -> Class (Jeden do wielu)
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Class)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.ClassId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacja: Test -> Subject (Jeden do wielu)
        modelBuilder.Entity<Test>()
            .HasOne(t => t.Subject)
            .WithMany()
            .HasForeignKey(t => t.SubjectId);

        // Relacja: Project -> User (Jeden do wielu)
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Student)
            .WithMany()
            .HasForeignKey(p => p.StudentId);

        // Relacja wiele-do-wielu: Group <-> User
        modelBuilder.Entity<Group>()
            .HasMany(g => g.Students)
            .WithMany();
    }
}