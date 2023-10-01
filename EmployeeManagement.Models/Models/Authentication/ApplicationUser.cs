using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Models.Models.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
    }
}
