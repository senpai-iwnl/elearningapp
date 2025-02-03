using System.Collections.Generic;

namespace e_learning_app.Models
{
    public class ManageStudentsViewModel
    {
        public Subject Subject { get; set; }
        public List<User> AllStudents { get; set; }
        public List<Guid> SelectedStudents { get; set; }
    }
}