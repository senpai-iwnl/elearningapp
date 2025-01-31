namespace e_learning_app.Models;

public class Subject
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Class> Classes { get; set; } = new List<Class>();
    public List<Document> Documents { get; set; } = new List<Document>();
}