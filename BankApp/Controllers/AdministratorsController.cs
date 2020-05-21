using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Administrator;
using BankApp.Dtos.Auth;
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
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AdministratorsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdministrator([FromBody]RegisterDto model)
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

        [HttpGet]
        public ActionResult<IEnumerable<AdministratorDto>> GetAdministrators()
        {
            var administrators = _context.Administrators.Include(a => a.ApplicationUser).ToList();

            if (administrators == null)
                return NotFound();

            return _mapper.Map<List<Administrator>, List<AdministratorDto>>(administrators);
        }

        [HttpGet("{userId}", Name = "GetAdministrator")]
        public ActionResult<AdministratorDto> GetAdministrator(string userId)
        {
            var administrator = _context.Administrators.Include(a => a.ApplicationUser).SingleOrDefault(a => a.Id == userId);

            if (administrator == null)
                return NotFound();

            return _mapper.Map<Administrator, AdministratorDto>(administrator);
        }
    }
}