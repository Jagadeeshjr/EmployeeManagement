using EmployeeManagement.BusinessLogic.Repository.Contracts;
using EmployeeManagement.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();

            if (employees == null || employees.Count ==0)
            {
                return NotFound();
            }
            return Ok(employees);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllEmployeesFiltering(string? term, string? sort, int page = 1, int limit = 10)
        {

            var filterRecords = await _employeeRepository.GetAllEmployeesBySortingAsync(term, sort, page, limit);

            if (filterRecords.Employees == null)
            {
                return NotFound();
            }

            return Ok(filterRecords.Employees);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById([FromRoute] int id)
        {

            if (id <= 0)
            {
                return BadRequest();
            }

            var employee = await _employeeRepository.GetEmployeeById(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewEmployee([FromBody] Employee employeeModel)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest();
            }

            var id = await _employeeRepository.AddEmployeeAsync(employeeModel); 

            return CreatedAtAction(nameof(GetEmployeeById), new { id, controller = "employee" }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromBody] Employee employeeModel, [FromRoute] int id)
        {
            if (id <= 0 || id != employeeModel.Id)
            {
                return BadRequest();
            }

            if (!await _employeeRepository.EmployeeExistsAsync(id))
            {
                return NotFound();
            }

            await _employeeRepository.UpdateEmployeeAsync(employeeModel);
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateEmployeePatch([FromBody] JsonPatchDocument employeeModel, [FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var employee = await _employeeRepository.GetEmployeeById(id);

            if (employee == null)
            {
                return NotFound();
            }

            await _employeeRepository.UpdateEmployeePatchAsync(id, employeeModel);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {
            var employee = await _employeeRepository.DeleteEmployeeAsync(id);

            if (employee != true)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
