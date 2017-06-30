using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Casestudy.Models
{
    // extends IdentityUser class with your own fields
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
