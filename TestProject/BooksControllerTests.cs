using LibraryPortal.Controllers;
using LibraryPortal.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace TestProject
{
    public class BooksControllerTests
    {

        //BookList Action Method Test
        [Fact]
        public void Book_GetBookList_Test()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;

            var mockContext = new Mock<ApplicationDbContext>(options);
            var mockFinanceHelper = new Mock<IFinancePortalHelper>();

            var mockDbSet = new Mock<DbSet<Book>>();
            // Mock data for books
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", Author = "Author 1", DaysDue = 7, Description = "Description", ISBN = "123 456" },
                new Book { BookId = 2, Title = "Book 2", Author = "Author 2", DaysDue = 14, Description = "Test Description", ISBN = "7548 4575" },
                new Book { BookId = 3, Title = "Book 3", Author = "Author 2", DaysDue = 15, Description = "Description", ISBN = "5897 2554 524" }
            };

        
            mockDbSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(books.AsQueryable().GetEnumerator());

            mockContext.Setup(c => c.Book).Returns(mockDbSet.Object);

            var controller = new BooksController(mockContext.Object, mockFinanceHelper.Object);

            // Act
            var result = controller.BooksList() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Book>>(result.Model);
            var model = result.Model as List<Book>;
            Assert.Equal(3, model.Count); // Assuming there are 3 books in the list
        }

        //Create A book test method
        [Fact]
        public void Create_Post_WithValidModelState_ReturnsRedirectToAction()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;

            // Arrange
            var mockContext = new Mock<ApplicationDbContext>(options);           
            var mockFinanceHelper = new Mock<IFinancePortalHelper>();
            var controller = new BooksController(mockContext.Object, mockFinanceHelper.Object);

            var book = new Book { BookId = 1, Title = "Book 1", Author = "Author 1", DaysDue = 7, Description = "Description", ISBN = "123 456" };

            // Act
            var result = controller.Create(book) as RedirectToActionResult;

            // Assert
            //Check if there are no errors
            Assert.True(controller.ModelState.IsValid);
            Assert.NotNull(result);
            Assert.Equal(nameof(BooksController.BooksList), result.ActionName);
        }

        //Borrowed books by student test
        [Fact]
        public void MyBooks_ReturnsViewResultWithBookListForStudent()
        {
            // Arrange
            var userId = "user123"; // Sample user ID

            //setup fake user with role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, nameof(Role.Student))
            };
            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.Setup(c => c.User.Claims).Returns(claims);
            var identity = new ClaimsIdentity(claims, "Auth");
            var principal = new ClaimsPrincipal(identity);

            mockHttpContext.Setup(c => c.User).Returns(principal);

            var mockControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            //Mock DBSets for mock the fake data
            var mockDbSet = new Mock<DbSet<Book>>();
            var mockSBSet = new Mock<DbSet<StudentBooks>>();
         
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            var mockFinanceHelper = new Mock<IFinancePortalHelper>();
            var mockContext = new Mock<ApplicationDbContext>(options);

            // Mock data for books and student books
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", Author = "Author 1", DaysDue = 7, Description = "Description", ISBN = "123 456" },
                new Book { BookId = 2, Title = "Book 2", Author = "Author 2", DaysDue = 14, Description = "Test Description", ISBN = "7548 4575" },
            };

            var studentBooks = new List<StudentBooks>
            {
                new StudentBooks { StudentBooksId = 1, BookId = 1, StudentId = userId, BorrowDate = DateTime.Now.AddDays(-5) },
                new StudentBooks { StudentBooksId = 2, BookId = 2, StudentId = userId, BorrowDate = DateTime.Now.AddDays(-10) }
            };

            //Create Books Mock Set
            SetupBooks(books, mockDbSet);
            mockContext.Setup(c => c.Book).Returns(mockDbSet.Object);

            SetupStudentBooks(studentBooks, mockSBSet);
            mockContext.Setup(c => c.StudentBooks).Returns(mockSBSet.Object);

            var controller = new BooksController(mockContext.Object, mockFinanceHelper.Object)
            {
                ControllerContext = mockControllerContext
            };

            // Act
            var result = controller.MyBooks() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<BookStudentVM>>(result.Model);

            var model = result.Model as List<BookStudentVM>;
            Assert.Equal(2, model.Count); // Assuming there are 2 books borrowed by the student
        }

        [Fact]
        public void BorrowBook_WhenBookBorrowed_ReturnsJsonResultWithSuccessMessage()
        {
            // Arrange
            var userId = "user123"; // Sample user ID
            var bookId = 1; // Sample book ID

            //setup fake user with role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, nameof(Role.Student))
            };
            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.Setup(c => c.User.Claims).Returns(claims);
            var identity = new ClaimsIdentity(claims, "Auth");
            var principal = new ClaimsPrincipal(identity);

            mockHttpContext.Setup(c => c.User).Returns(principal);

            var mockControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            var mockFinanceHelper = new Mock<IFinancePortalHelper>();
            var mockContext = new Mock<ApplicationDbContext>(options);

            // Mock data for student books
            var studentBooks = new List<StudentBooks>
            {
                new StudentBooks { StudentBooksId = 1, BookId = 1, StudentId = userId, BorrowDate = DateTime.Now.AddDays(-5) },
                new StudentBooks { StudentBooksId = 2, BookId = 2, StudentId = userId, BorrowDate = DateTime.Now.AddDays(-10) }
            };

            var mockSBSet = new Mock<DbSet<StudentBooks>>();

            SetupStudentBooks(studentBooks, mockSBSet);

            mockContext.Setup(c => c.StudentBooks).Returns(mockSBSet.Object);

            var controller = new BooksController(mockContext.Object, mockFinanceHelper.Object)
            {
                ControllerContext = mockControllerContext
            };

            // Act
            var result = controller.BorrowBook(bookId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var res = result.Value.GetType().GetProperty("result")?.GetValue(result.Value);
            //Book is alreay borrowed
            Assert.False((bool)res);
        }

        private void SetupBooks(List<Book> books, Mock<DbSet<Book>> mockDbSet)
        {
            mockDbSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(books.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(books.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(books.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(books.AsQueryable().GetEnumerator());
        }

        private void SetupStudentBooks(List<StudentBooks> studentBooks, Mock<DbSet<StudentBooks>> mockDbSet)
        {
            mockDbSet.As<IQueryable<StudentBooks>>().Setup(m => m.Provider).Returns(studentBooks.AsQueryable().Provider);
            mockDbSet.As<IQueryable<StudentBooks>>().Setup(m => m.Expression).Returns(studentBooks.AsQueryable().Expression);
            mockDbSet.As<IQueryable<StudentBooks>>().Setup(m => m.ElementType).Returns(studentBooks.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<StudentBooks>>().Setup(m => m.GetEnumerator()).Returns(studentBooks.AsQueryable().GetEnumerator());
        }
    }
}
