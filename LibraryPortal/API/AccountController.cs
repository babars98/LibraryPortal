using LibraryPortal.Data;
using LibraryPortal.Models;
using LibraryPortal.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace LibraryPortal.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly string password = "Lbu#1234";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(Student student)
        {
            var exist = _userManager.Users.Any(c => c.StudentId == student.StudentId);

            if (exist)
                return BadRequest($"Student with Id {student.StudentId} Alreay Exist");
            
            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, student.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, student.Email, CancellationToken.None);
            await _emailStore.SetEmailConfirmedAsync(user, true, CancellationToken.None);
            user.StudentId = student.StudentId;
            var result = await _userManager.CreateAsync(user, password);
            //Add role as normal User
            //This will allow certain actions to perform only
            var res = await _userManager.AddToRoleAsync(user, Role.Student.ToString());

            if (result.Succeeded)
                return Ok("Account Created");

            var errors = result.Errors.Select(x => x.Description)
                           .Where(y => y.Count() > 0)
                           .ToList();

            return BadRequest(errors);
        }

        /// <summary>
        /// API Endpoint to check if the user account exists in library portal
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        [Route("CheckAccount")]
        [HttpGet]
        public IActionResult CheckAccountExists(string studentId)
        {
            var student = _userManager.Users.FirstOrDefault(c => c.StudentId == studentId);

            if (student == null)
                return BadRequest($"Account not found with Id {studentId}");

            dynamic obj = new ExpandoObject();

            obj.Id = student.Id;
            obj.StudentId = student.StudentId;

            return Ok(obj);
        }

        //Create instance of the user object
        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }


        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
