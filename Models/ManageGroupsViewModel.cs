namespace e_learning_app.Models
{
    public class ManageGroupsViewModel
    {
        public Subject Subject { get; set; }
        public List<Group> Groups { get; set; }
        public List<User> AllStudents { get; set; }
    }
}