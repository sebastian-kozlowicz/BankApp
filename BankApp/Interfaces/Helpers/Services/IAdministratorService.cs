using System.Collections.Generic;
using System.Threading.Tasks;
using BankApp.Dtos.Auth;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Interfaces.Helpers.Services
{
    public interface IAdministratorService
    {
        Task<Administrator> GetAdministratorAsync(int userId);
        Task<IEnumerable<Administrator>> GetAdministratorsAsync();
        Task<ActionResult<Administrator>> CreateAdministratorAsync(RegisterByAnotherUserDto model);
    }
}