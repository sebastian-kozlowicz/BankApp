using System.Collections.Generic;
using System.Threading.Tasks;
using BankApp.Dtos.Auth;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Interfaces.Helpers.Services
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerAsync(int userId);
        Task<IEnumerable<Customer>> GetCustomersAsync();
        Task<ActionResult<Customer>> CreateCustomerAsync(RegisterByAnotherUserDto model);
    }
}