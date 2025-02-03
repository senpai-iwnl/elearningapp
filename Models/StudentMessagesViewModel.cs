namespace e_learning_app.Models
{
    public class StudentMessagesViewModel
    {
        public List<Message> GeneralMessages { get; set; } = new List<Message>();
        public List<Message> SubjectMessages { get; set; } = new List<Message>();
    }
}