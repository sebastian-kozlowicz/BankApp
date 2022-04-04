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
    public class TellerService : ITellerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public TellerService(ApplicationDbContext context, IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Teller> GeTellerAsync(int userId)
        {
            return await _context.Tellers.Include(e => e.ApplicationUser).SingleOrDefaultAsync(e => e.Id == userId);
        }

        public async Task<IList<Teller>> GeTellersAsync()
        {
            return await _context.Tellers.Include(e => e.ApplicationUser).ToListAsync();
        }

        public async Task<ActionResult<Teller>> CreateManagerAsync(RegisterByAnotherUserDto model)
        {
            var user = _mapper.Map<ApplicationUser>(model);
            user.Teller = new Teller { Id = user.Id };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRole.Teller.ToString());
            else
                throw new Exception(JsonConvert.SerializeObject(result.Errors));

            return user.Teller;
        }
    }
}