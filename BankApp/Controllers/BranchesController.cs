using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Configuration;
using BankApp.Dtos.Branch;
using BankApp.Dtos.Branch.WithAddress;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using BankApp.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;
        private readonly IMapper _mapper;

        public BranchesController(IBranchService branchService, IMapper mapper)
        {
            _branchService = branchService;
            _mapper = mapper;
        }

        [HttpGet("{branchId}", Name = "GetBranch")]
        public async Task<ActionResult<BranchDto>> GetBranchAsync(int branchId)
        {
            var branch = await _branchService.GetBranchAsync(branchId);

            if (branch == null)
                return NotFound();

            return Ok(_mapper.Map<Branch, BranchDto>(branch));
        }

        [HttpPost]
        [Route("CreateWithAddress")]
        public async Task<ActionResult<BranchDto>> CreateBranchWithAddressAsync(
            [FromBody] BranchWithAddressCreationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var branch = await _branchService.CreateBranchWithAddressAsync(model);

            var branchDto = _mapper.Map<Branch, BranchDto>(branch);

            return CreatedAtRoute("GetBranch", new { branchId = branchDto.Id }, branchDto);
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.UserIdIncludedInJwtToken)]
        [Route("AssignTellerToBranch")]
        public async Task<ActionResult> AssignTellerToBranchAsync([FromBody] WorkerAtBranchDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = int.Parse(User.Claims.Single(c => c.Type == CustomClaimTypes.UserId).Value);

            await _branchService.AssignTellerToBranchAsync(model, currentUserId);
            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.UserIdIncludedInJwtToken)]
        [Route("AssignManagerToBranch")]
        public async Task<ActionResult> AssignManagerToBranchAsync([FromBody] WorkerAtBranchDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = int.Parse(User.Claims.Single(c => c.Type == CustomClaimTypes.UserId).Value);

            await _branchService.AssignManagerToBranchAsync(model, currentUserId);
            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.UserIdIncludedInJwtToken)]
        [Route("ExpelTellerFromBranch")]
        public async Task<ActionResult> ExpelTellerFromBranchAsync([FromBody] WorkerAtBranchDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = int.Parse(User.Claims.Single(c => c.Type == CustomClaimTypes.UserId).Value);

            await _branchService.ExpelTellerFromBranchAsync(model, currentUserId);
            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.UserIdIncludedInJwtToken)]
        [Route("ExpelManagerFromBranch")]
        public async Task<ActionResult> ExpelManagerFromBranchAsync([FromBody] WorkerAtBranchDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var currentUserId = int.Parse(User.Claims.Single(c => c.Type == CustomClaimTypes.UserId).Value);

            await _branchService.ExpelManagerFromBranchAsync(model, currentUserId);
            return Ok();
        }
    }
}