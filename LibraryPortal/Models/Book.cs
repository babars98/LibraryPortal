namespace LibraryPortal.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public int DaysDue { get; set; }
    }

    public class BookStudentVM
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int DaysDue { get; set; }
        public DateTime BorrowDate { get; set; }
        public int StudentBooksId { get; set; }
        public bool IsOverDue { get; set; }
    }
}
