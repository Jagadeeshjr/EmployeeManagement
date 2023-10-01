using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EmployeeManagement.Models.Models;
using EmployeeManagement.Models.Models.Authentication;

namespace EmployeeManagement.DataAccess.Data
{
    public class EmployeeDBContext : IdentityDbContext<ApplicationUser>
    {
        public EmployeeDBContext(DbContextOptions<EmployeeDBContext> options)
            : base(options)
        {

        }
        public DbSet<Employee> Employees { get; set; }
    }
}
