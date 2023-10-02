using EmployeeManagement.Models.Models;

namespace EmployeeManagement
{
    public class PagedEmployeeResult
    {
        public List<Employee> Employees { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}