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
        
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // Relationships
        modelBuilder.Entity<Class>()
            .HasOne(c => c.Subject)
            .WithMany(s => s.Classes)
            .HasForeignKey(c => c.SubjectId);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.Subject)
            .WithMany(s => s.Documents)
            .HasForeignKey(d => d.SubjectId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.Class)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.ClassId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Test>()
            .HasOne(t => t.Subject)
            .WithMany()
            .HasForeignKey(t => t.SubjectId);

        modelBuilder.Entity<Project>()
            .HasOne(p => p.Student)
            .WithMany()
            .HasForeignKey(p => p.StudentId);

        modelBuilder.Entity<Group>()
            .HasMany(g => g.Students)
            .WithMany();
    }
}
