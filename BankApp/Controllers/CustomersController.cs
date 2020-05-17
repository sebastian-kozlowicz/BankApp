using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Customer;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CustomersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CustomerDto>> GetCustomers()
        {
            var customers = _context.Customers.Include(c => c.ApplicationUser).ToList();

            if (customers == null)
                return NotFound();

            return _mapper.Map<List<Customer>, List<CustomerDto>>(customers);
        }

        [HttpGet("{userId}", Name = "GetCustomer")]
        public ActionResult<CustomerDto> GetCustomer(string userId)
        {
            var customer = _context.Customers.Include(c => c.ApplicationUser).SingleOrDefault(c => c.Id == userId);

            if (customer == null)
                return NotFound();

            return _mapper.Map<Customer, CustomerDto>(customer);
        }
    }
}