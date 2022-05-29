using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Dtos.Auth;
using BankApp.Dtos.Teller;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TellersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITellerService _tellerService;

        public TellersController(ITellerService tellerService, IMapper mapper)
        {
            _tellerService = tellerService;
            _mapper = mapper;
        }

        [HttpGet("{userId}", Name = "GetTeller")]
        public async Task<ActionResult<TellerDto>> GeTellerAsync(int userId)
        {
            var teller = await _tellerService.GeTellerAsync(userId);

            if (teller == null)
                return NotFound();

            return Ok(_mapper.Map<Teller, TellerDto>(teller));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TellerDto>>> GeTellersAsync()
        {
            var tellers = await _tellerService.GeTellersAsync();

            if (!tellers.Any())
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<Teller>, IEnumerable<TellerDto>>(tellers));
        }

        [HttpPost]
        public async Task<ActionResult<TellerDto>> CreateTellerAsync([FromBody] RegisterByAnotherUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var teller = await _tellerService.CreateManagerAsync(model);

            var tellerDto = _mapper.Map<TellerDto>(teller);

            return CreatedAtRoute("GetTeller", new { userId = tellerDto.Id }, teller);
        }
    }
}