﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.BankAccount;
using BankApp.Dtos.BankAccount.WithCustomerCreation;
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

        [HttpGet]
        [Route("{bankAccountId}", Name = "GetBankAccount")]
        public ActionResult<BankAccountDto> GetBankAccount(int bankAccountId)
        {
            var bankAccount = _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountId);

            if (bankAccount == null)
                return NotFound();

            return Ok(_mapper.Map<BankAccount, BankAccountDto>(bankAccount));
        }

        [HttpGet]
        [Route("GetAllForUser/{applicationUserId}")]
        public ActionResult<IEnumerable<BankAccountDto>> GetBankAccounts(int applicationUserId)
        {
            var bankAccounts = _context.BankAccounts.Where(ba => ba.CustomerId == applicationUserId).ToList();

            if (!bankAccounts.Any())
                return NotFound();

            return Ok(_mapper.Map<List<BankAccount>, List<BankAccountDto>>(bankAccounts));
        }

        [HttpPost]
        public ActionResult<BankAccountDto> CreateBankAccount([FromBody] Dtos.BankAccount.BankAccountCreationDto model)
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
                CustomerId = model.CustomerId
            };

            _context.BankAccounts.Add(bankAccount);
            _context.SaveChanges();

            var bankAccountDto = _mapper.Map<BankAccount, BankAccountDto>(bankAccount);
            return CreatedAtRoute("GetBankAccount", new { bankAccountId = bankAccountDto.Id }, bankAccountDto);
        }

        [HttpPost]
        [Route("CreateWithCustomer")]
        public async Task<ActionResult<BankAccountDto>> CreateBankAccountWithCustomer([FromBody] BankAccountWithCustomerCreationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var generatedAccountNumber = _accountNumberFactory.GenerateAccountNumber();

            var user = _mapper.Map<ApplicationUser>(model.Register);
            user.Customer = new Customer { Id = user.Id };

            var bankAccount = new BankAccount
            {
                AccountType = model.BankAccount.AccountType,
                Currency = model.BankAccount.Currency,
                CountryCode = generatedAccountNumber.CountryCode,
                CheckNumber = generatedAccountNumber.CheckNumber,
                NationalBankCode = generatedAccountNumber.NationalBankCode,
                BranchCode = generatedAccountNumber.BranchCode,
                NationalCheckDigit = generatedAccountNumber.NationalCheckDigit,
                AccountNumber = generatedAccountNumber.AccountNumber,
                AccountNumberText = generatedAccountNumber.AccountNumberText,
                Iban = generatedAccountNumber.Iban,
                IbanSeparated = generatedAccountNumber.IbanSeparated,
                CustomerId = user.Id
            };

            user.Customer.BankAccounts = new List<BankAccount> { bankAccount };

            var result = await _userManager.CreateAsync(user, model.Register.User.Password);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());
            else
                return BadRequest(result.Errors);

            var bankAccountDto = _mapper.Map<BankAccount, BankAccountDto>(bankAccount);
            return CreatedAtRoute("GetBankAccount", new { bankAccountId = bankAccountDto.Id }, bankAccountDto);
        }
    }
}