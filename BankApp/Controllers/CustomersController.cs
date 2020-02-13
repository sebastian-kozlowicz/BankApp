using AutoMapper;
using BankApp.Data;
using BankApp.Dtos;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BankApp.Controllers
{
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CustomersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("api/[controller]")]
        public IEnumerable<CustomerDto> GetCustomers()
        {
            var customers = _context.Customers.Include(c => c.ApplicationUser).ToList();
            return _mapper.Map<List<Customer>, List<CustomerDto>>(customers);
        }
    }
}