namespace e_learning_app.Models;

public class Question
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string Type { get; set; } // jednokrotny wybór, wielokrotny, wartość
}