using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Administrator;
using BankApp.Dtos.Auth;
using BankApp.Enumerators;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministratorsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context,
            IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{userId}", Name = "GetAdministrator")]
        public ActionResult<AdministratorDto> GetAdministrator(int userId)
        {
            var administrator = _context.Administrators.Include(a => a.ApplicationUser)
                .SingleOrDefault(a => a.Id == userId);

            if (administrator == null)
                return NotFound();

            return Ok(_mapper.Map<Administrator, AdministratorDto>(administrator));
        }

        [HttpGet]
        public ActionResult<IEnumerable<AdministratorDto>> GetAdministrators()
        {
            var administrators = _context.Administrators.Include(a => a.ApplicationUser).ToList();

            if (!administrators.Any())
                return NotFound();

            return Ok(_mapper.Map<List<Administrator>, List<AdministratorDto>>(administrators));
        }

        [HttpPost]
        public async Task<ActionResult<AdministratorDto>> CreateAdministrator([FromBody] RegisterByAnotherUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<ApplicationUser>(model);
            user.Administrator = new Administrator {Id = user.Id};

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRole.Administrator.ToString());
            else
                return BadRequest(result.Errors);

            var administrator = _mapper.Map<AdministratorDto>(user.Administrator);

            return CreatedAtRoute("GetAdministrator", new {userId = administrator.Id}, administrator);
        }
    }
}