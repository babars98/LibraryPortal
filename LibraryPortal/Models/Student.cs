using System.ComponentModel.DataAnnotations;

namespace LibraryPortal.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string StudentId { get; set; }
        public string Password { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class StudentDTO
    {
        public string StudentId { get; set; }
    }
}
