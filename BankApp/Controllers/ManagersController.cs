using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Auth;
using BankApp.Dtos.Manager;
using BankApp.Enumerators;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ManagersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateManager([FromBody]RegisterDto model)
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

        [HttpGet]
        public ActionResult<IEnumerable<ManagerDto>> GetManagers()
        {
            var managers = _context.Managers.Include(m => m.ApplicationUser).ToList();

            if (managers == null)
                return NotFound();

            return _mapper.Map<List<Manager>, List<ManagerDto>>(managers);
        }

        [HttpGet("{userId}", Name = "GetManager")]
        public ActionResult<ManagerDto> GetManager(string userId)
        {
            var manager = _context.Managers.Include(m => m.ApplicationUser).SingleOrDefault(m => m.Id == userId);

            if (manager == null)
                return NotFound();

            return _mapper.Map<Manager, ManagerDto>(manager);
        }
    }
}