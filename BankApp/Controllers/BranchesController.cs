﻿using System;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Branch;
using BankApp.Dtos.Branch.WithAddress;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        public ActionResult<BranchDto> CreateBranch([FromBody] BranchWithAddressCreationDto model)
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

            return CreatedAtRoute("GetBranch", new { branchId = branchDto.Id }, branchDto);
        }

        [HttpPost]
        [Route("AssignEmployeeToBranch")]
        public ActionResult AssignEmployeeToBranch([FromBody] WorkerAssignToBranch model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = _context.Employees.SingleOrDefault(e => e.Id == model.WorkerId);
            if (employee == null)
            {
                ModelState.AddModelError(nameof(model.WorkerId), $"Employee with id {model.WorkerId} doesn't exist.");
                return BadRequest(ModelState);
            }

            var branch = _context.Branches.SingleOrDefault(b => b.Id == model.BranchId);
            if (branch == null)
            {
                ModelState.AddModelError(nameof(model.BranchId), $"Branch with id {model.BranchId} doesn't exist.");
                return BadRequest(ModelState);
            }

            if (employee.WorkAtId == model.BranchId)
            {
                ModelState.AddModelError(nameof(model.BranchId), $"Employee with id {model.WorkerId} is currently assigned to branch with id {model.BranchId}.");
                return BadRequest(ModelState);
            }

            var employeeAtBranchFromDb = _context.EmployeeAtBranchHistory.Where(e => e.EmployeeId == employee.Id).ToList().LastOrDefault();
            if (employeeAtBranchFromDb != null && employeeAtBranchFromDb.ExpelDate == null)
                employeeAtBranchFromDb.ExpelDate = DateTime.UtcNow;

            employee.WorkAtId = branch.Id;
            var employeeAtBranch = new EmployeeAtBranchHistory
            {
                AssignDate = DateTime.UtcNow,
                BranchId = branch.Id,
                EmployeeId = employee.Id
            };

            _context.EmployeeAtBranchHistory.Add(employeeAtBranch);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("AssignManagerToBranch")]
        public ActionResult AssignManagerToBranch([FromBody] WorkerAssignToBranch model)
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

            if (manager.WorkAtId == model.BranchId)
            {
                ModelState.AddModelError(nameof(model.BranchId), $"Manager with id {model.WorkerId} is currently assigned to branch with id {model.BranchId}.");
                return BadRequest(ModelState);
            }

            var managerAtBranchFromDb = _context.ManagerAtBranchHistory.Where(e => e.ManagerId == manager.Id).ToList().LastOrDefault();
            if (managerAtBranchFromDb != null && managerAtBranchFromDb.ExpelDate == null)
                managerAtBranchFromDb.ExpelDate = DateTime.UtcNow;

            manager.WorkAtId = branch.Id;
            var managerAtBranch = new ManagerAtBranchHistory
            {
                AssignDate = DateTime.UtcNow,
                BranchId = branch.Id,
                ManagerId = manager.Id
            };

            _context.ManagerAtBranchHistory.Add(managerAtBranch);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("ExpelEmployeeFromBranch")]
        public ActionResult ExpelEmployeeFromBranch([FromBody] WorkerAssignToBranch model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employeeAtBranchFromDb = _context.EmployeeAtBranchHistory.Where(e => e.EmployeeId == model.WorkerId).ToList().LastOrDefault();
            if (employeeAtBranchFromDb != null && employeeAtBranchFromDb.ExpelDate == null)
                employeeAtBranchFromDb.ExpelDate = DateTime.UtcNow;

            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("ExpelManagerFromBranch")]
        public ActionResult ExpelManagerFromBranch([FromBody] WorkerAssignToBranch model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var managerAtBranchFromDb = _context.ManagerAtBranchHistory.Where(e => e.ManagerId == model.WorkerId).ToList().LastOrDefault();
            if (managerAtBranchFromDb != null && managerAtBranchFromDb.ExpelDate == null)
                managerAtBranchFromDb.ExpelDate = DateTime.UtcNow;

            _context.SaveChanges();

            return Ok();
        }
    }
}
