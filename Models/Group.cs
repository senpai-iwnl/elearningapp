namespace e_learning_app.Models;

public class Group
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    // 🔹 Relacja wiele-do-wielu: Grupa może mieć wielu studentów
    public List<User> Students { get; set; } = new List<User>();

    // 🔹 Relacja jeden-do-wielu: Grupa należy do jednego przedmiotu
    public Guid SubjectId { get; set; } // Klucz obcy
    public Subject Subject { get; set; } // Nawigacyjne pole
}