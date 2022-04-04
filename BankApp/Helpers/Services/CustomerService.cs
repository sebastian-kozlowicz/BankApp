using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Auth;
using BankApp.Enumerators;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BankApp.Helpers.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerService(ApplicationDbContext context, IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Customer> GetCustomerAsync(int userId)
        {
            return await _context.Customers.Include(c => c.ApplicationUser).SingleOrDefaultAsync(c => c.Id == userId);
        }

        public async Task<IList<Customer>> GetCustomersAsync()
        {
            return await _context.Customers.Include(c => c.ApplicationUser).ToListAsync();
        }

        public async Task<ActionResult<Customer>> CreateCustomerAsync(RegisterByAnotherUserDto model)
        {
            var user = _mapper.Map<ApplicationUser>(model);
            user.Customer = new Customer { Id = user.Id };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());
            else
                throw new Exception(JsonConvert.SerializeObject(result.Errors));

            return user.Customer;
        }
    }
}