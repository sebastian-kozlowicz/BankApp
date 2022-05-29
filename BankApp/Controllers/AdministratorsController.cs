using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Dtos.Administrator;
using BankApp.Dtos.Auth;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorsController : ControllerBase
    {
        private readonly IAdministratorService _administratorService;
        private readonly IMapper _mapper;

        public AdministratorsController(IAdministratorService administratorService, IMapper mapper)
        {
            _administratorService = administratorService;
            _mapper = mapper;
        }

        [HttpGet("{userId}", Name = "GetAdministrator")]
        public async Task<ActionResult<AdministratorDto>> GetAdministratorAsync(int userId)
        {
            var administrator = await _administratorService.GetAdministratorAsync(userId);

            if (administrator == null)
                return NotFound();

            return Ok(_mapper.Map<Administrator, AdministratorDto>(administrator));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdministratorDto>>> GetAdministratorsAsync()
        {
            var administrators = await _administratorService.GetAdministratorsAsync();

            if (!administrators.Any())
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<Administrator>, IEnumerable<AdministratorDto>>(administrators));
        }

        [HttpPost]
        public async Task<ActionResult<AdministratorDto>> CreateAdministratorAsync(
            [FromBody] RegisterByAnotherUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var administrator = await _administratorService.CreateAdministratorAsync(model);

            var administratorDto = _mapper.Map<AdministratorDto>(administrator);

            return CreatedAtRoute("GetAdministrator", new { userId = administratorDto.Id }, administratorDto);
        }
    }
}