using Microsoft.AspNetCore.Identity;

namespace TodoApp.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string?Adress { get; set; }
        public string Name { get; set; }
    }
}
