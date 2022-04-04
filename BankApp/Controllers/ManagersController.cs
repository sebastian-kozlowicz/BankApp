using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Dtos.Auth;
using BankApp.Dtos.Manager;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagersController : ControllerBase
    {
        private readonly IManagerService _managerService;
        private readonly IMapper _mapper;

        public ManagersController(IManagerService managerService, IMapper mapper)
        {
            _managerService = managerService;
            _mapper = mapper;
        }

        [HttpGet("{userId}", Name = "GetManager")]
        public async Task<ActionResult<ManagerDto>> GetManagerAsync(int userId)
        {
            var manager = await _managerService.GetManagerAsync(userId);

            if (manager == null)
                return NotFound();

            return Ok(_mapper.Map<Manager, ManagerDto>(manager));
        }

        [HttpGet]
        public async Task<ActionResult<IList<ManagerDto>>> GetManagersAsync()
        {
            var managers = await _managerService.GetManagersAsync();

            if (!managers.Any())
                return NotFound();

            return Ok(_mapper.Map<IList<Manager>, IList<ManagerDto>>(managers));
        }

        [HttpPost]
        public async Task<ActionResult<ManagerDto>> CreateManagerAsync([FromBody] RegisterByAnotherUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var manager = await _managerService.CreateManagerAsync(model);

            var managerDto = _mapper.Map<ManagerDto>(manager);

            return CreatedAtRoute("GetManager", new { userId = managerDto.Id }, manager);
        }
    }
}