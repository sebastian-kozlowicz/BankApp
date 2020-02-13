﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Controllers
{
    [ApiController]
    public class BankAccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BankAccountsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("api/[controller]")]
        public IEnumerable<BankAccountDto> GetAccounts()
        {
            var accounts = _context.BankAccounts.ToList();
            return _mapper.Map<List<BankAccount>, List<BankAccountDto>>(accounts);
        }
    }
}