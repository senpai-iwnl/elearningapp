using System.ComponentModel.DataAnnotations;

namespace e_learning_app.Models;

public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Powiązanie z nauczycielem (nadawcą wiadomości)
    public Guid SenderId { get; set; }
    public User? Sender { get; set; }

    // Opcjonalne powiązanie z przedmiotem
    public Guid? SubjectId { get; set; }
    public Subject? Subject { get; set; }
}