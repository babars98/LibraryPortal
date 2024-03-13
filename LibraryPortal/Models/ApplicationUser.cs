using Microsoft.AspNetCore.Identity;

namespace LibraryPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string StudentId { get; set; }
    }
}
