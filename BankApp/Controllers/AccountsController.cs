using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AccountsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("api/{controller}")]
        public IEnumerable<AccountDto> GetAccounts()
        {
            var accounts = _context.Accounts.ToList();
            return _mapper.Map<List<Account>, List<AccountDto>>(accounts);
        }
    }
}