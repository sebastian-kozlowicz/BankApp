using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Dtos;
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

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Name = model.Name, Surname = model.Surname };

            user.Administrator = new Administrator() { Id = user.Id };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(_mapper.Map<AdministratorDto>(user.Administrator));
        }

        [HttpPost]
        [Route("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody]RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Name = model.Name, Surname = model.Surname };

            user.Customer = new Customer() { Id = user.Id };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(_mapper.Map<CustomerDto>(user.Customer));
        }

        [HttpPost]
        [Route("register/employee")]
        public async Task<IActionResult> RegisterEmployee([FromBody]RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Name = model.Name, Surname = model.Surname };

            user.Employee = new Employee() { Id  = user.Id};

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(_mapper.Map<EmployeeDto>(user.Employee));
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

            if (await _userManager.CheckPasswordAsync(user, password))
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(email, user.Id));

            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}