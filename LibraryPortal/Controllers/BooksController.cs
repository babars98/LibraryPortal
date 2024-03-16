using LibraryPortal.Data;
using LibraryPortal.Interface;
using LibraryPortal.Models;
using LibraryPortal.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryPortal.Controllers
{
    public class BooksController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IFinancePortalHelper _financeHelper;
        private string admin = Role.Admin.ToString();
        private double FineFee = 5.0; 

        public BooksController(ApplicationDbContext context, IFinancePortalHelper financeHelper)
        {
            _financeHelper = financeHelper;
            _context = context;
        }

        public IActionResult BooksList()
        {
            var bookList = _context.Book.ToList();
            return View(bookList);
        }

        [Authorize(Roles = nameof(Role.Admin))]
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [Authorize(Roles = nameof(Role.Admin))]
        public IActionResult Create(Book book)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "An error while saving.");
                return View(book);
            }

            _context.Add(book);
            _context.SaveChanges();

            return RedirectToAction(nameof(BooksList));
        }

        [Authorize(Roles = nameof(Role.Student))]
        public IActionResult MyBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookList = from b in _context.Book
                           join sb in _context.StudentBooks on b.BookId equals sb.BookId
                           where sb.StudentId == userId
                           select new BookStudentVM
                           {
                              BookId =  b.BookId,
                              Title = b.Title,
                              DaysDue = b.DaysDue,
                              BorrowDate = sb.BorrowDate,
                              StudentBooksId = sb.StudentBooksId
                           };
            var list = bookList.ToList();

            foreach (var book in list)
                book.IsOverDue = IsBookOverDue(book.BorrowDate, book.DaysDue);

            return View(list);
        }

        [HttpPost]
        [Authorize(Roles = nameof(Role.Student))]
        public JsonResult BorrowBook(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //check if the user has already borrowed the book
            var checkAlreadyBorrowed = _context.StudentBooks.Any(c => c.StudentId == userId && c.BookId == bookId);

            if (checkAlreadyBorrowed)
                return Json(new { result = false, message = "Book Already borrowed" });

            var borrowBook = new StudentBooks
            {
                BookId = bookId,
                StudentId = userId,
                BorrowDate = DateTime.Now
            };

            _context.StudentBooks.Add(borrowBook);
            _context.SaveChanges();

            return Json(new { result = true });
        }

        [HttpPost]
        [Authorize(Roles = nameof(Role.Student))]
        public JsonResult ReturnBook(int studentBookId, bool isOverDue)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //check if the user has already borrowed the book

            var borrowBook = _context.StudentBooks.FirstOrDefault(c => c.StudentBooksId == studentBookId);

            _context.StudentBooks.Remove(borrowBook);
            _context.SaveChanges();

            if (isOverDue)
                CreateFinaceInvoice(userId);

            return Json(new { result = true });
        }

        private bool IsBookOverDue(DateTime borrowDate, int daysLimit)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var bDate = DateOnly.FromDateTime(borrowDate);
            var days = today.DayNumber - bDate.DayNumber;

            var b = days >= daysLimit;
            return b;
        }

        private void CreateFinaceInvoice(string userId)
        {
            var student = _context.Users.FirstOrDefault(c => c.Id == userId);

            _financeHelper.CreateInvoice(student.StudentId, FineFee);
        }
    }
}
