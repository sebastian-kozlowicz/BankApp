﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BankApp.Attributes;
using BankApp.Data;
using BankApp.Dtos.BankAccount;
using BankApp.Dtos.BankAccount.WithCustomer;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountNumberFactory _accountNumberFactory;

        public BankAccountsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IMapper mapper, IAccountNumberFactory accountNumberFactory)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _accountNumberFactory = accountNumberFactory;
        }

        [HttpPost]
        public ActionResult CreateBankAccount([FromBody] Dtos.BankAccount.BankAccountCreationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var generatedAccountNumber = _accountNumberFactory.GenerateAccountNumber();

            var bankAccount = new BankAccount
            {
                AccountType = model.AccountType,
                Currency = model.Currency,
                CountryCode = generatedAccountNumber.CountryCode,
                CheckNumber = generatedAccountNumber.CheckNumber,
                NationalBankCode = generatedAccountNumber.NationalBankCode,
                BranchCode = generatedAccountNumber.BranchCode,
                NationalCheckDigit = generatedAccountNumber.NationalCheckDigit,
                AccountNumber = generatedAccountNumber.AccountNumber,
                AccountNumberText = generatedAccountNumber.AccountNumberText,
                Iban = generatedAccountNumber.Iban,
                IbanSeparated = generatedAccountNumber.IbanSeparated,
                ApplicationUserId = model.ApplicationUserId
            };

            _context.BankAccounts.Add(bankAccount);
            _context.SaveChanges();

            return CreatedAtRoute("GetBankAccount", new { bankAccountId = bankAccount.Id }, bankAccount);
        }

        [HttpPost]
        [AuthorizeRoleEnum(UserRole.Customer)]
        [Route("create-with-customer")]
        public ActionResult CreateBankAccountWithCustomer([FromBody] BankAccountWithCustomerCreationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var generatedAccountNumber = _accountNumberFactory.GenerateAccountNumber();

            var user = _mapper.Map<ApplicationUser>(model.RegisterDto);
            user.Customer = new Customer { Id = user.Id };

            var bankAccount = new BankAccount
            {
                AccountType = model.BankAccountCreationDto.AccountType,
                Currency = model.BankAccountCreationDto.Currency,
                CountryCode = generatedAccountNumber.CountryCode,
                CheckNumber = generatedAccountNumber.CheckNumber,
                NationalBankCode = generatedAccountNumber.NationalBankCode,
                BranchCode = generatedAccountNumber.BranchCode,
                NationalCheckDigit = generatedAccountNumber.NationalCheckDigit,
                AccountNumber = generatedAccountNumber.AccountNumber,
                AccountNumberText = generatedAccountNumber.AccountNumberText,
                Iban = generatedAccountNumber.Iban,
                IbanSeparated = generatedAccountNumber.IbanSeparated,
                ApplicationUserId = user.Id
            };

            user.BankAccounts = new List<BankAccount>() { bankAccount };

            var result = _userManager.CreateAsync(user, model.RegisterDto.User.Password).Result;

            if (result.Succeeded)
                _ = _userManager.AddToRoleAsync(user, UserRole.Customer.ToString()).Result;
            else
                return BadRequest(result.Errors);

            var bankAccountDto = _mapper.Map<BankAccount, BankAccountDto>(bankAccount);
            return CreatedAtRoute("GetBankAccount", new { bankAccountId = bankAccount.Id }, bankAccountDto);
        }

        [HttpGet]
        [Route("{bankAccountId}", Name = "GetBankAccount")]
        public ActionResult GetBankAccount(int bankAccountId)
        {
            var bankAccount = _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountId);
            if (bankAccount == null)
                return NotFound();

            return Ok(_mapper.Map<BankAccount, BankAccountDto>(bankAccount));
        }

        [HttpGet]
        public ActionResult<IEnumerable<BankAccountDto>> GetAccounts()
        {
            var accounts = _context.BankAccounts.ToList();
            return Ok(_mapper.Map<List<BankAccount>, List<BankAccountDto>>(accounts));
        }
    }
}