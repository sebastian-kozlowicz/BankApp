using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Dtos.Administrator;
using BankApp.Dtos.Auth;
using BankApp.Dtos.Customer;
using BankApp.Dtos.Employee;
using BankApp.Dtos.Manager;
using BankApp.Enumerators;
using BankApp.Helpers;
using BankApp.Interfaces;
using BankApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly IMapper _mapper;

        public AuthController(UserManager<ApplicationUser> userManager, IJwtFactory jwtFactory, IMapper mapper)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("register/administrator")]
        public async Task<IActionResult> RegisterAdministrator([FromBody]RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser { UserName = model.User.Email, Email = model.User.Email, PhoneNumber = model.User.PhoneNumber, Name = model.User.Name, Surname = model.User.Surname };

            user.Administrator = new Administrator() { Id = user.Id };
            user.Address = new Address() { Id = user.Id, Country = model.Address.Country, City = model.Address.City, Street = model.Address.Street, HouseNumber = model.Address.HouseNumber, ApartmentNumber = model.Address.ApartmentNumber, PostalCode = model.Address.PostalCode };

            var result = await _userManager.CreateAsync(user, model.User.Password);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRoles.Administrator.ToString());
            else
                return BadRequest(result.Errors);

            var administrator = _mapper.Map<AdministratorDto>(user.Administrator);

            return CreatedAtRoute("GetAdministrator", new { userId = administrator.Id }, administrator);
        }

        [HttpPost]
        [Route("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody]RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser { UserName = model.User.Email, Email = model.User.Email, PhoneNumber = model.User.PhoneNumber, Name = model.User.Name, Surname = model.User.Surname };

            user.Customer = new Customer() { Id = user.Id };
            user.Address = new Address() { Id = user.Id, Country = model.Address.Country, City = model.Address.City, Street = model.Address.Street, HouseNumber = model.Address.HouseNumber, ApartmentNumber = model.Address.ApartmentNumber, PostalCode = model.Address.PostalCode };

            var result = await _userManager.CreateAsync(user, model.User.Password);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRoles.Customer.ToString());
            else
                return BadRequest(result.Errors);

            var customer = _mapper.Map<CustomerDto>(user.Customer);

            return CreatedAtRoute("GetCustomer", new { userId = customer.Id }, customer);
        }

        [HttpPost]
        [Route("register/employee")]
        public async Task<IActionResult> RegisterEmployee([FromBody]RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser { UserName = model.User.Email, Email = model.User.Email, PhoneNumber = model.User.PhoneNumber, Name = model.User.Name, Surname = model.User.Surname };

            user.Employee = new Employee() { Id = user.Id };
            user.Address = new Address() { Id = user.Id, Country = model.Address.Country, City = model.Address.City, Street = model.Address.Street, HouseNumber = model.Address.HouseNumber, ApartmentNumber = model.Address.ApartmentNumber, PostalCode = model.Address.PostalCode };

            var result = await _userManager.CreateAsync(user, model.User.Password);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRoles.Employee.ToString());
            else
                return BadRequest(result.Errors);

            var employee = _mapper.Map<EmployeeDto>(user.Employee);

            return CreatedAtRoute("GetEmployee", new { userId = employee.Id }, employee);
        }

        [HttpPost]
        [Route("register/manager")]
        public async Task<IActionResult> RegisterManager([FromBody]RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser { UserName = model.User.Email, Email = model.User.Email, PhoneNumber = model.User.PhoneNumber, Name = model.User.Name, Surname = model.User.Surname };

            user.Manager = new Manager() { Id = user.Id };
            user.Address = new Address() { Id = user.Id, Country = model.Address.Country, City = model.Address.City, Street = model.Address.Street, HouseNumber = model.Address.HouseNumber, ApartmentNumber = model.Address.ApartmentNumber, PostalCode = model.Address.PostalCode };

            var result = await _userManager.CreateAsync(user, model.User.Password);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRoles.Manager.ToString());
            else
                return BadRequest(result.Errors);

            var manager = _mapper.Map<ManagerDto>(user.Manager);
            return CreatedAtRoute("GetManager", new { userId = manager.Id }, manager);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await GetClaimsIdentity(model.Email, model.Password) is ClaimsIdentity claimsIdentity && claimsIdentity != null)
            {
                var jwt = await JwtTokenHelper.GenerateJwt(claimsIdentity, _jwtFactory, model.Email, new JsonSerializerSettings { Formatting = Formatting.Indented });

                return Ok(jwt);
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return BadRequest(ModelState);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return await Task.FromResult<ClaimsIdentity>(null);

            var roles = await _userManager.GetRolesAsync(user);

            if (await _userManager.CheckPasswordAsync(user, password))
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(user, roles));

            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}