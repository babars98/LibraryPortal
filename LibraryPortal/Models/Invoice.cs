using LibraryPortal.Util;

namespace LibraryPortal.Models
{
    public class Invoice
    {
        public string StudentId { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public double Fee { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
