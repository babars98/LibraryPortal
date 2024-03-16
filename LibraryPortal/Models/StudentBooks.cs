using System.ComponentModel.DataAnnotations;

namespace LibraryPortal.Models
{
    public class StudentBooks
    {
        [Key]
        public int StudentBooksId { get; set; }
        public string StudentId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
    }
}
