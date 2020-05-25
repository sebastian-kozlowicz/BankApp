using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Auth;
using BankApp.Dtos.Customer;
using BankApp.Enumerators;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CustomersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody]RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<ApplicationUser>(model);
            user.Customer = new Customer { Id = user.Id };

            var result = await _userManager.CreateAsync(user, model.User.Password);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRoles.Customer.ToString());
            else
                return BadRequest(result.Errors);

            var customer = _mapper.Map<CustomerDto>(user.Customer);

            return CreatedAtRoute("GetCustomer", new { userId = customer.Id }, customer);
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