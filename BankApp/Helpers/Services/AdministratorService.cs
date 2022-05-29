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
    public class AdministratorService : IAdministratorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministratorService(ApplicationDbContext context, IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Administrator> GetAdministratorAsync(int userId)
        {
            return await _context.Administrators.Include(a => a.ApplicationUser)
                .SingleOrDefaultAsync(a => a.Id == userId);
        }

        public async Task<IEnumerable<Administrator>> GetAdministratorsAsync()
        {
            return await _context.Administrators.Include(a => a.ApplicationUser).ToListAsync();
        }

        public async Task<ActionResult<Administrator>> CreateAdministratorAsync(RegisterByAnotherUserDto model)
        {
            var user = _mapper.Map<ApplicationUser>(model);
            user.Administrator = new Administrator { Id = user.Id };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRole.Administrator.ToString());
            else
                throw new Exception(JsonConvert.SerializeObject(result.Errors));

            return user.Administrator;
        }
    }
}