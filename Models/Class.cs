using System.ComponentModel.DataAnnotations;

namespace e_learning_app.Models;

public class Class
{
    public Guid Id { get; set; }
        
    [Required]
    public string Name { get; set; }
        
    public string Description { get; set; }
        
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public List<Document> Documents { get; set; } = new List<Document>();
}