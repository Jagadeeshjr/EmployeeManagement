using EmployeeManagement.Models.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace EmployeeManagement.BusinessLogic.Repository.Contracts
{
    public interface IEmployeeRepository
    {
        Task<PagedEmployeeResult> GetAllEmployeesBySortingAsync(string term, string? sort, int page, int limit);

        Task<List<Employee>> GetAllEmployeesAsync();

        Task<Employee> GetEmployeeById(int id);

        Task<bool> EmployeeExistsAsync(int id);

        Task<int> AddEmployeeAsync(Employee employeeModel);

        Task UpdateEmployeeAsync(Employee employeeModel);

        Task UpdateEmployeePatchAsync(int id, JsonPatchDocument employeeModel);

        Task<bool> DeleteEmployeeAsync(int id);
    }
}