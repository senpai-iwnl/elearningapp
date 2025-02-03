using System.ComponentModel.DataAnnotations;

namespace e_learning_app.Models;

public class User
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagane")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Imię jest wymagane")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Nazwisko jest wymagane")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email jest wymagany")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Hasło jest wymagane")]
    [MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków")]
    public string PasswordHash { get; set; }

    public string Role { get; set; } // Admin, Nauczyciel, Student

    // Przedmioty, których użytkownik jest twórcą
    public List<Subject> Subjects { get; set; } = new List<Subject>();

    // Przedmioty, do których użytkownik dołączył jako student
    public List<Subject> EnrolledSubjects { get; set; } = new List<Subject>();
    // 🔹 Relacja wiele-do-wielu z grupami
    public List<Group> Groups { get; set; } = new List<Group>(); // Grupy, do których użytkownik należy
}