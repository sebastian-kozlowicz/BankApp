using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Branch;
using BankApp.Dtos.Branch.WithAddress;
using BankApp.Exceptions;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Helpers.Services
{
    public class BranchService : IBranchService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BranchService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Branch> GetBranchAsync(int branchId)
        {
            return await _context.Branches.Include(b => b.BranchAddress).SingleOrDefaultAsync(c => c.Id == branchId);
        }

        public async Task<Branch> CreateBranchWithAddressAsyncAsync(BranchWithAddressCreationDto model)
        {
            if (await _context.Branches.FirstOrDefaultAsync(b => b.BranchCode == model.Branch.BranchCode) != null)
                throw new ValidationException("Branch code is already in use.");

            var branch = _mapper.Map<Branch>(model);

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            return branch;
        }

        public async Task<bool> AssignTellerToBranchAsync(WorkerAtBranchDto model, int currentUserId)
        {
            var teller = await _context.Tellers.SingleOrDefaultAsync(t => t.Id == model.WorkerId);
            if (teller == null)
                throw new ValidationException($"Teller with id {model.WorkerId} doesn't exist.");

            var branch = await _context.Branches.SingleOrDefaultAsync(b => b.Id == model.BranchId);
            if (branch == null)
                throw new ValidationException($"Branch with id {model.BranchId} doesn't exist.");

            if (teller.WorkAtId != null)
                throw new ValidationException(
                    $"Teller with id {model.WorkerId} is currently assigned to branch with id {teller.WorkAtId}.");

            teller.WorkAtId = branch.Id;
            var tellerAtBranch = new TellerAtBranchHistory
            {
                AssignDate = DateTime.UtcNow,
                BranchId = branch.Id,
                TellerId = teller.Id,
                AssignedById = currentUserId
            };

            await _context.TellerAtBranchHistory.AddAsync(tellerAtBranch);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AssignManagerToBranchAsync(WorkerAtBranchDto model, int currentUserId)
        {
            var manager = await _context.Managers.SingleOrDefaultAsync(e => e.Id == model.WorkerId);
            if (manager == null)
                throw new ValidationException($"Manager with id {model.WorkerId} doesn't exist.");

            var branch = await _context.Branches.SingleOrDefaultAsync(b => b.Id == model.BranchId);
            if (branch == null)
                throw new ValidationException($"Branch with id {model.BranchId} doesn't exist.");

            if (manager.WorkAtId != null)
                throw new ValidationException(
                    $"Manager with id {model.WorkerId} is currently assigned to branch with id {manager.WorkAtId}.");

            manager.WorkAtId = branch.Id;
            var managerAtBranch = new ManagerAtBranchHistory
            {
                AssignDate = DateTime.UtcNow,
                BranchId = branch.Id,
                ManagerId = manager.Id,
                AssignedById = currentUserId
            };

            await _context.ManagerAtBranchHistory.AddAsync(managerAtBranch);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExpelTellerFromBranchAsync(WorkerAtBranchDto model, int currentUserId)
        {
            var teller = await _context.Tellers.SingleOrDefaultAsync(t => t.Id == model.WorkerId);
            if (teller == null)
                throw new ValidationException($"Teller with id {model.WorkerId} doesn't exist.");

            var branch = await _context.Branches.SingleOrDefaultAsync(b => b.Id == model.BranchId);
            if (branch == null)
                throw new ValidationException($"Branch with id {model.BranchId} doesn't exist.");

            if (teller.WorkAtId == null)
                throw new ValidationException(
                    $"Teller with id {model.WorkerId} is currently not assigned to any branch.");

            if (teller.WorkAtId != branch.Id)
                throw new ValidationException(
                    $"Teller with id {model.WorkerId} is currently not assigned to branch with id {teller.WorkAtId}.");

            teller.WorkAtId = null;
            var tellerAtBranchFromDb = _context.TellerAtBranchHistory.Where(t => t.TellerId == model.WorkerId).ToList()
                .LastOrDefault();

            if (tellerAtBranchFromDb != null)
            {
                tellerAtBranchFromDb.ExpelDate = DateTime.UtcNow;
                tellerAtBranchFromDb.ExpelledById = currentUserId;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExpelManagerFromBranchAsync(WorkerAtBranchDto model, int currentUserId)
        {
            var manager = await _context.Managers.SingleOrDefaultAsync(e => e.Id == model.WorkerId);
            if (manager == null)
                throw new ValidationException($"Manager with id {model.WorkerId} doesn't exist.");

            var branch = _context.Branches.SingleOrDefaultAsync(b => b.Id == model.BranchId);
            if (branch == null)
                throw new ValidationException($"Branch with id {model.BranchId} doesn't exist.");

            if (manager.WorkAtId == null)
                throw new ValidationException(
                    $"Manager with id {model.WorkerId} is currently not assigned to any branch.");

            if (manager.WorkAtId != model.BranchId)
                throw new ValidationException(
                    $"Manager with id {model.WorkerId} is currently not assigned to branch with id {manager.WorkAtId}.");

            manager.WorkAtId = null;
            var managerAtBranchFromDb = _context.ManagerAtBranchHistory.Where(e => e.ManagerId == model.WorkerId)
                .ToList().LastOrDefault();

            if (managerAtBranchFromDb != null)
            {
                managerAtBranchFromDb.ExpelDate = DateTime.UtcNow;
                managerAtBranchFromDb.ExpelledById = currentUserId;
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}