namespace e_learning_app.Models
{
    public class EditGroupViewModel
    {
        public Group Group { get; set; }
        public List<User> AvailableStudents { get; set; }
    }
}