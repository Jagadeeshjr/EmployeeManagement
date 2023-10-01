using EmployeeManagement.BusinessLogic.Repository.Contracts;
using EmployeeManagement.Caching;
using EmployeeManagement.DataAccess.Data;
using EmployeeManagement.Models.Models;
using LazyCache;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace EmployeeManagement.BusinessLogic.Repository.Implementation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeDBContext _context;
        private ICacheProvider _cacheProvider;

        public EmployeeRepository(EmployeeDBContext context, ICacheProvider cacheProvider)
        {
            _context = context;
            _cacheProvider = cacheProvider;
        }

        public async Task<PagedEmployeeResult> GetAllEmployeesBySortingAsync(string term, string? sort, int page, int limit)
        {
            IQueryable<Employee> employees;
            if (string.IsNullOrWhiteSpace(term))
            {
                employees = _context.Employees;
            }
            else
            {
                term = term.Trim().ToLower();

                employees = _context
                    .Employees
                    .Where(x => x.Name.ToLower().Contains(term)
                    || x.Qualification.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortFields = sort.Split(',');
                StringBuilder orderQueryBuilder = new StringBuilder();

                PropertyInfo[] propertyInfo = typeof(Employee).GetProperties();

                foreach (var field in sortFields)
                {
                    string sortOrder = "ascending";
                    var sortField = field.Trim();
                    if (sortField.StartsWith('-'))
                    {
                        sortField = sortField.TrimStart('-');
                        sortOrder = "descending";
                    }
                    var property = propertyInfo.FirstOrDefault(a =>
                                    a.Name.Equals(sortField,
                                    StringComparison.OrdinalIgnoreCase));
                    if (property == null)
                    {
                        continue;
                    }
                    orderQueryBuilder.Append($"{property.Name.ToString()}" +
                                             $"{sortOrder}, ");
                }

                string orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
                if (!string.IsNullOrWhiteSpace(orderQuery))
                {
                    employees = employees.OrderBy(orderQuery);
                }
                else
                {
                    employees = employees.OrderBy(x => x.Id);
                }
            }

            //applying pagination
            var totalCount = await _context.Employees.CountAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)limit);

            var pagedEmployees = await employees.Skip((page - 1) * limit).Take(limit).ToListAsync();

            var pagedEmployeeData = new PagedEmployeeResult
            {
                Employees = pagedEmployees,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return pagedEmployeeData;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            if (!_cacheProvider.TryGetValue(CacheKey.Employee, out List<Employee> employees))
            {
                employees = await _context.Employees.ToListAsync();

                var cacheEntryOption = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                    SlidingExpiration = TimeSpan.FromSeconds(30),
                    Size = 1024
                };
                _cacheProvider.Set(CacheKey.Employee, employees, cacheEntryOption);
            }
            return employees;
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            if (!_cacheProvider.TryGetValue(CacheKey.Employee, out Employee employee))
            {
                employee = await _context.Employees.FindAsync(id);

                var cacheEntryOption = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                    SlidingExpiration = TimeSpan.FromSeconds(30),
                    Size = 1024
                };
                _cacheProvider.Set(CacheKey.Employee, employee, cacheEntryOption);
            }
            return employee;
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(x => x.Id == id);
        }

        public async Task<int> AddEmployeeAsync(Employee employeeModel)
        {
            employeeModel.CreatedDate = DateTime.Now;
            _context.Employees.Add(employeeModel);
            await _context.SaveChangesAsync();
            return employeeModel.Id;
        }

        public async Task UpdateEmployeeAsync(Employee employeeModel)
        {
            employeeModel.UpdatedDate = DateTime.Now;
            _context.Employees.Update(employeeModel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployeePatchAsync(int id, JsonPatchDocument employeeModel)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                employee.UpdatedDate = DateTime.Now;
                employeeModel.ApplyTo(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
