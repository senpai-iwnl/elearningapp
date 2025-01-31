namespace e_learning_app.Models;

public class Document
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public byte[] File { get; set; }
    public Guid? SubjectId { get; set; }
    public Subject Subject { get; set; }
    public Guid? ClassId { get; set; }
    public Class Class { get; set; }
}