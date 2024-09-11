using Microsoft.AspNetCore.Identity;

namespace Abocar.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } 
        public string LastName { get; set;}
        public string? ImagePath { get; set; }
        public string Nationaltiy { get; set; }
    }

    public class userViewModel
    {
        public ApplicationUser user { get; set; }
        public IdentityRole roles { get; set; }
        public string userRoles { get; set; }
        public int posts {  get; set; } 
    }


    public class UserViewModel
    {
        public ApplicationUser User { get; set; }
        public List<string> UserRoles { get; set; }
        public int Posts { get; set; }
    }




}
