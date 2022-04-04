using System.Collections.Generic;
using System.Threading.Tasks;
using BankApp.Dtos.Auth;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Interfaces.Helpers.Services
{
    public interface IManagerService
    {
        Task<Manager> GetManagerAsync(int userId);
        Task<IList<Manager>> GetManagersAsync();
        Task<ActionResult<Manager>> CreateManagerAsync(RegisterByAnotherUserDto model);
    }
}