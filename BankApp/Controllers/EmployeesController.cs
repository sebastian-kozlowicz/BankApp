using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Auth;
using BankApp.Dtos.Employee;
using BankApp.Enumerators;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EmployeesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody]RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<ApplicationUser>(model);
            user.Employee = new Employee() { Id = user.Id };

            var result = await _userManager.CreateAsync(user, model.User.Password);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRoles.Employee.ToString());
            else
                return BadRequest(result.Errors);

            var employee = _mapper.Map<EmployeeDto>(user.Employee);

            return CreatedAtRoute("GetEmployee", new { userId = employee.Id }, employee);
        }

        [HttpGet]
        public ActionResult<IEnumerable<EmployeeDto>> GetEmployees()
        {
            var employees = _context.Employees.Include(e => e.ApplicationUser).ToList();

            if (employees == null)
                return NotFound();

            return _mapper.Map<List<Employee>, List<EmployeeDto>>(employees);
        }

        [HttpGet("{userId}", Name = "GetEmployee")]
        public ActionResult<EmployeeDto> GetEmployee(string userId)
        {
            var employee = _context.Employees.Include(e => e.ApplicationUser).SingleOrDefault(e => e.Id == userId);

            if (employee == null)
                return NotFound();

            return _mapper.Map<Employee, EmployeeDto>(employee);
        }
    }
}