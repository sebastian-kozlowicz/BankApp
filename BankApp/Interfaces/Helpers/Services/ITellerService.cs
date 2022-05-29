using System.Collections.Generic;
using System.Threading.Tasks;
using BankApp.Dtos.Auth;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Interfaces.Helpers.Services
{
    public interface ITellerService
    {
        Task<Teller> GeTellerAsync(int userId);
        Task<IEnumerable<Teller>> GeTellersAsync();
        Task<ActionResult<Teller>> CreateManagerAsync(RegisterByAnotherUserDto model);
    }
}