using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EmployeesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<EmployeeDto> GetEmployees()
        {
            var employees = _context.Employees.Include(c => c.ApplicationUser).ToList();
            return _mapper.Map<List<Employee>, List<EmployeeDto>>(employees);
        }
    }
}