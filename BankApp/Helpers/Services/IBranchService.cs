﻿using System.Threading.Tasks;
using BankApp.Dtos.Branch;
using BankApp.Dtos.Branch.WithAddress;
using BankApp.Models;

namespace BankApp.Helpers.Services
{
    public interface IBranchService
    {
        Task<Branch> GetBranchAsync(int branchId);
        Task<Branch> CreateBranchWithAddressAsyncAsync(BranchWithAddressCreationDto model);
        Task<bool> AssignTellerToBranchAsync(WorkerAtBranchDto model, int currentUserId);
        Task<bool> AssignManagerToBranchAsync(WorkerAtBranchDto model, int currentUserId);
        Task<bool> ExpelTellerFromBranchAsync(WorkerAtBranchDto model, int currentUserId);
        Task<bool> ExpelManagerFromBranchAsync(WorkerAtBranchDto model, int currentUserId);
    }
}