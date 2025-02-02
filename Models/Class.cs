using System.ComponentModel.DataAnnotations;

namespace e_learning_app.Models;

public class Class
{
    public Guid Id { get; set; }
        
    [Required]
    public string Name { get; set; }
        
    public string Description { get; set; }
        
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; }

    // Twórca klasy (relacja jeden-do-wielu)
    public Guid CreatorId { get; set; }
    public User Creator { get; set; }

    // Studenci zapisani do klasy (relacja wiele-do-wielu)
    public List<User> Students { get; set; } = new List<User>();

    // Kod dołączenia do kursu - 10 losowych znaków
    [Required]
    [MaxLength(10)]
    public string JoinCode { get; set; } = GenerateJoinCode();

    public List<Document> Documents { get; set; } = new List<Document>();

    private static string GenerateJoinCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, 10)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }
}