using LibraryPortal.Data;
using LibraryPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace LibraryPortal.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("Register")]
        [HttpPost]
        public IActionResult Register(StudentDTO student)
        {
            var exist = _context.Student.Any(c => c.StudentId == student.StudentId);

            if (exist)
                return BadRequest($"Student with Id {student.StudentId} Alreay Exist");

            var studentObj = new Student
            {
                StudentId = student.StudentId,
                Password = "000000",
                DateCreated = DateTime.Now
            };

            _context.Student.Add(studentObj);
            _context.SaveChanges();

            return Ok("Account Created");
        }

        [Route("CheckAccount")]
        [HttpGet]
        public IActionResult CheckAccountExists(string studentId)
        {
            var student = _context.Student.FirstOrDefault(c => c.StudentId == studentId);

            if (student == null)
                return BadRequest($"Account not found with Id {studentId}");

            dynamic obj = new ExpandoObject();

            obj.Id = student.Id;
            obj.StudentId = student.StudentId;
            obj.AccountDateCreated = student.DateCreated;

            return Ok(obj);
        }
    }
}
