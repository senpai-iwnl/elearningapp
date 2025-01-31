namespace e_learning_app.Models;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid StudentId { get; set; }
    public User Student { get; set; }
    public List<Document> Files { get; set; } = new List<Document>();
}