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

        [HttpGet("{userId}", Name = "GetManager")]
        public ActionResult<ManagerDto> GetManager(int userId)
        {
            var manager = _context.Managers.Include(m => m.ApplicationUser).SingleOrDefault(m => m.Id == userId);

            if (manager == null)
                return NotFound();

            return Ok(_mapper.Map<Manager, ManagerDto>(manager));
        }

        [HttpGet]
        public ActionResult<IEnumerable<ManagerDto>> GetManagers()
        {
            var managers = _context.Managers.Include(m => m.ApplicationUser).ToList();

            if (!managers.Any())
                return NotFound();

            return Ok(_mapper.Map<List<Manager>, List<ManagerDto>>(managers));
        }

        [HttpPost]
        public async Task<ActionResult<ManagerDto>> CreateManager([FromBody] RegisterByAnotherUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<ApplicationUser>(model);
            user.Manager = new Manager { Id = user.Id };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRole.Manager.ToString());
            else
                return BadRequest(result.Errors);

            var manager = _mapper.Map<ManagerDto>(user.Manager);
            return CreatedAtRoute("GetManager", new { userId = manager.Id }, manager);
        }
    }
}