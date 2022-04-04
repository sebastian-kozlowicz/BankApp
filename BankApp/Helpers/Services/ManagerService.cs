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
    public class ManagerService : IManagerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagerService(ApplicationDbContext context, IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Manager> GetManagerAsync(int userId)
        {
            return await _context.Managers.Include(m => m.ApplicationUser).SingleOrDefaultAsync(m => m.Id == userId);
        }

        public async Task<IList<Manager>> GetManagersAsync()
        {
            return await _context.Managers.Include(m => m.ApplicationUser).ToListAsync();
        }

        public async Task<ActionResult<Manager>> CreateManagerAsync(RegisterByAnotherUserDto model)
        {
            var user = _mapper.Map<ApplicationUser>(model);
            user.Manager = new Manager { Id = user.Id };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRole.Manager.ToString());
            else
                throw new Exception(JsonConvert.SerializeObject(result.Errors));

            return user.Manager;
        }
    }
}