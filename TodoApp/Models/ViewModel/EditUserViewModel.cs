using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TodoApp.Models.ViewModel
{
    public class EditUserViewModel
    {
        public string? Id { get; set; } 
        public string Name { get; set; }
        [ValidateNever]
        public string Email { get; set; } 
        public string Address { get; set; }
        public string Role { get; set; }
        [ValidateNever]
        public List<string>? AllRoles { get; set; }

    }
}
