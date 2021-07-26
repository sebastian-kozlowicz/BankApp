using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Auth;
using BankApp.Dtos.Teller;
using BankApp.Enumerators;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TellersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public TellersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{userId}", Name = "GetTeller")]
        public ActionResult<TellerDto> GeTeller(int userId)
        {
            var teller = _context.Tellers.Include(e => e.ApplicationUser).SingleOrDefault(e => e.Id == userId);

            if (teller == null)
                return NotFound();

            return Ok(_mapper.Map<Teller, TellerDto>(teller));
        }

        [HttpGet]
        public ActionResult<IEnumerable<TellerDto>> GeTellers()
        {
            var tellers = _context.Tellers.Include(e => e.ApplicationUser).ToList();

            if (!tellers.Any())
                return NotFound();

            return Ok(_mapper.Map<List<Teller>, List<TellerDto>>(tellers));
        }

        [HttpPost]
        public async Task<ActionResult<TellerDto>> CreateTeller([FromBody] RegisterByAnotherUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<ApplicationUser>(model);
            user.Teller = new Teller {Id = user.Id};

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRole.Teller.ToString());
            else
                return BadRequest(result.Errors);

            var teller = _mapper.Map<TellerDto>(user.Teller);

            return CreatedAtRoute("GetTeller", new {userId = teller.Id}, teller);
        }
    }
}