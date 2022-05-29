using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Dtos.Auth;
using BankApp.Dtos.Customer;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        [HttpGet("{userId}", Name = "GetCustomer")]
        public async Task<ActionResult<CustomerDto>> GetCustomerAsync(int userId)
        {
            var customer = await _customerService.GetCustomerAsync(userId);

            if (customer == null)
                return NotFound();

            return Ok(_mapper.Map<Customer, CustomerDto>(customer));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomersAsync()
        {
            var customers = await _customerService.GetCustomersAsync();

            if (!customers.Any())
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<Customer>, IEnumerable<CustomerDto>>(customers));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomerAsync([FromBody] RegisterByAnotherUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = await _customerService.CreateCustomerAsync(model);

            var customerDto = _mapper.Map<CustomerDto>(customer);

            return CreatedAtRoute("GetCustomer", new { userId = customerDto.Id }, customer);
        }
    }
}