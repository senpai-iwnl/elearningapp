namespace e_learning_app.Models;

public class Test
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; }
    public List<Question> Questions { get; set; } = new List<Question>();
}