using System;
using System.Linq;
using AutoMapper;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Dtos.Branch;
using BankApp.Dtos.Branch.WithAddress;
using BankApp.Models;
using BankApp.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BranchesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{branchId}", Name = "GetBranch")]
        public ActionResult<BranchDto> GetBranch(int branchId)
        {
            var branch = _context.Branches.Include(b => b.BranchAddress).SingleOrDefault(c => c.Id == branchId);

            if (branch == null)
                return NotFound();

            return Ok(_mapper.Map<Branch, BranchDto>(branch));
        }

        [HttpPost]
        [Route("CreateWithAddress")]
        public ActionResult<BranchDto> CreateBranchWithAddress([FromBody] BranchWithAddressCreationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Branches.FirstOrDefault(b => b.BranchCode == model.Branch.BranchCode) != null)
            {
                ModelState.AddModelError(nameof(model.Branch.BranchCode), "Branch code is already in use.");
                return BadRequest(ModelState);
            }

            var branch = _mapper.Map<Branch>(model);

            _context.Branches.Add(branch);
            _context.SaveChanges();

            var branchDto = _mapper.Map<Branch, BranchDto>(branch);

            return CreatedAtRoute("GetBranch", new {branchId = branchDto.Id}, branchDto);
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.UserIdIncludedInJwtToken)]
        [Route("AssignTellerToBranch")]
        public ActionResult AssignTellerToBranch([FromBody] WorkerAtBranchDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var teller = _context.Tellers.SingleOrDefault(t => t.Id == model.WorkerId);
            if (teller == null)
            {
                ModelState.AddModelError(nameof(model.WorkerId), $"Teller with id {model.WorkerId} doesn't exist.");
                return BadRequest(ModelState);
            }

            var branch = _context.Branches.SingleOrDefault(b => b.Id == model.BranchId);
            if (branch == null)
            {
                ModelState.AddModelError(nameof(model.BranchId), $"Branch with id {model.BranchId} doesn't exist.");
                return BadRequest(ModelState);
            }

            if (teller.WorkAtId != null)
            {
                ModelState.AddModelError(nameof(model.BranchId),
                    $"Teller with id {model.WorkerId} is currently assigned to branch with id {teller.WorkAtId}.");
                return BadRequest(ModelState);
            }

            teller.WorkAtId = branch.Id;
            var tellerAtBranch = new TellerAtBranchHistory
            {
                AssignDate = DateTime.UtcNow,
                BranchId = branch.Id,
                TellerId = teller.Id,
                AssignedById = int.Parse(User.Claims.Single(c => c.Type == CustomClaimTypes.UserId).Value)
            };

            _context.TellerAtBranchHistory.Add(tellerAtBranch);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.UserIdIncludedInJwtToken)]
        [Route("AssignManagerToBranch")]
        public ActionResult AssignManagerToBranch([FromBody] WorkerAtBranchDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var manager = _context.Managers.SingleOrDefault(e => e.Id == model.WorkerId);
            if (manager == null)
            {
                ModelState.AddModelError(nameof(model.WorkerId), $"Manager with id {model.WorkerId} doesn't exist.");
                return BadRequest(ModelState);
            }

            var branch = _context.Branches.SingleOrDefault(b => b.Id == model.BranchId);
            if (branch == null)
            {
                ModelState.AddModelError(nameof(model.BranchId), $"Branch with id {model.BranchId} doesn't exist.");
                return BadRequest(ModelState);
            }

            if (manager.WorkAtId != null)
            {
                ModelState.AddModelError(nameof(model.BranchId),
                    $"Manager with id {model.WorkerId} is currently assigned to branch with id {manager.WorkAtId}.");
                return BadRequest(ModelState);
            }

            manager.WorkAtId = branch.Id;
            var managerAtBranch = new ManagerAtBranchHistory
            {
                AssignDate = DateTime.UtcNow,
                BranchId = branch.Id,
                ManagerId = manager.Id,
                AssignedById = int.Parse(User.Claims.Single(c => c.Type == CustomClaimTypes.UserId).Value)
            };

            _context.ManagerAtBranchHistory.Add(managerAtBranch);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.UserIdIncludedInJwtToken)]
        [Route("ExpelTellerFromBranch")]
        public ActionResult ExpelTellerFromBranch([FromBody] WorkerAtBranchDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var teller = _context.Tellers.SingleOrDefault(t => t.Id == model.WorkerId);
            if (teller == null)
            {
                ModelState.AddModelError(nameof(model.WorkerId), $"Teller with id {model.WorkerId} doesn't exist.");
                return BadRequest(ModelState);
            }

            var branch = _context.Branches.SingleOrDefault(b => b.Id == model.BranchId);
            if (branch == null)
            {
                ModelState.AddModelError(nameof(model.BranchId), $"Branch with id {model.BranchId} doesn't exist.");
                return BadRequest(ModelState);
            }

            if (teller.WorkAtId == null)
            {
                ModelState.AddModelError(nameof(model.BranchId),
                    $"Teller with id {model.WorkerId} is currently not assigned to any branch.");
                return BadRequest(ModelState);
            }

            if (teller.WorkAtId != branch.Id)
            {
                ModelState.AddModelError(nameof(model.BranchId),
                    $"Teller with id {model.WorkerId} is currently not assigned to branch with id {teller.WorkAtId}.");
                return BadRequest(ModelState);
            }

            teller.WorkAtId = null;
            var tellerAtBranchFromDb = _context.TellerAtBranchHistory.Where(t => t.TellerId == model.WorkerId).ToList()
                .LastOrDefault();
            if (tellerAtBranchFromDb != null)
            {
                tellerAtBranchFromDb.ExpelDate = DateTime.UtcNow;
                tellerAtBranchFromDb.ExpelledById =
                    int.Parse(User.Claims.Single(c => c.Type == CustomClaimTypes.UserId).Value);
            }

            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.UserIdIncludedInJwtToken)]
        [Route("ExpelManagerFromBranch")]
        public ActionResult ExpelManagerFromBranch([FromBody] WorkerAtBranchDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var manager = _context.Managers.SingleOrDefault(e => e.Id == model.WorkerId);
            if (manager == null)
            {
                ModelState.AddModelError(nameof(model.WorkerId), $"Manager with id {model.WorkerId} doesn't exist.");
                return BadRequest(ModelState);
            }

            var branch = _context.Branches.SingleOrDefault(b => b.Id == model.BranchId);
            if (branch == null)
            {
                ModelState.AddModelError(nameof(model.BranchId), $"Branch with id {model.BranchId} doesn't exist.");
                return BadRequest(ModelState);
            }

            if (manager.WorkAtId == null)
            {
                ModelState.AddModelError(nameof(model.BranchId),
                    $"Manager with id {model.WorkerId} is currently not assigned to any branch.");
                return BadRequest(ModelState);
            }

            if (manager.WorkAtId != model.BranchId)
            {
                ModelState.AddModelError(nameof(model.BranchId),
                    $"Manager with id {model.WorkerId} is currently not assigned to branch with id {manager.WorkAtId}.");
                return BadRequest(ModelState);
            }

            manager.WorkAtId = null;
            var managerAtBranchFromDb = _context.ManagerAtBranchHistory.Where(e => e.ManagerId == model.WorkerId)
                .ToList().LastOrDefault();
            if (managerAtBranchFromDb != null)
            {
                managerAtBranchFromDb.ExpelDate = DateTime.UtcNow;
                managerAtBranchFromDb.ExpelledById =
                    int.Parse(User.Claims.Single(c => c.Type == CustomClaimTypes.UserId).Value);
            }

            _context.SaveChanges();

            return Ok();
        }
    }
}