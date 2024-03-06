using LibraryPortal.Data;
using LibraryPortal.Models;
using LibraryPortal.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryPortal.Controllers
{
    public class BooksController : Controller
    {

        private readonly ApplicationDbContext _context;
        private string admin = Role.Admin.ToString();

        public BooksController(ApplicationDbContext context)
        {
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
    }
}
