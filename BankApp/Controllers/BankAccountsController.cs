using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Dtos.BankAccount;
using BankApp.Dtos.BankAccount.WithCustomerCreation;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using BankApp.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IBankAccountNumberFactory _accountNumberFactory;

        public BankAccountsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IMapper mapper, IBankAccountNumberFactory accountNumberFactory)
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
        public ActionResult<IEnumerable<BankAccountDto>> GetBankAccountsForUser(int applicationUserId)
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

            var generatedAccountNumber = _accountNumberFactory.GenerateBankAccountNumber();

            var bankAccount = new BankAccount
            {
                AccountType = (AccountType)model.AccountType,
                Currency = (Currency)model.Currency,
                CountryCode = generatedAccountNumber.CountryCode,
                CheckNumber = generatedAccountNumber.CheckNumber,
                NationalBankCode = generatedAccountNumber.NationalBankCode,
                BranchCode = generatedAccountNumber.BranchCode,
                NationalCheckDigit = generatedAccountNumber.NationalCheckDigit,
                AccountNumber = generatedAccountNumber.AccountNumber,
                AccountNumberText = generatedAccountNumber.AccountNumberText,
                Iban = generatedAccountNumber.Iban,
                IbanSeparated = generatedAccountNumber.IbanSeparated,
                OpenedDate = DateTime.UtcNow,
                CustomerId = (int)model.CustomerId,
                CreatedById = (int)model.CustomerId
            };

            _context.BankAccounts.Add(bankAccount);
            _context.SaveChanges();

            var bankAccountDto = _mapper.Map<BankAccount, BankAccountDto>(bankAccount);
            return CreatedAtRoute("GetBankAccount", new { bankAccountId = bankAccountDto.Id }, bankAccountDto);
        }

        [HttpPost]
        [Route("CreateWithCustomerByCustomer")]
        public async Task<ActionResult<BankAccountDto>> CreateBankAccountWithCustomerByCustomer([FromBody] BankAccountWithCustomerCreationByCustomerDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var generatedAccountNumber = _accountNumberFactory.GenerateBankAccountNumber();

            var user = _mapper.Map<ApplicationUser>(model.Register);
            user.Customer = new Customer { Id = user.Id };

            var bankAccount = new BankAccount
            {
                AccountType = (AccountType)model.BankAccount.AccountType,
                Currency = (Currency)model.BankAccount.Currency,
                CountryCode = generatedAccountNumber.CountryCode,
                CheckNumber = generatedAccountNumber.CheckNumber,
                NationalBankCode = generatedAccountNumber.NationalBankCode,
                BranchCode = generatedAccountNumber.BranchCode,
                NationalCheckDigit = generatedAccountNumber.NationalCheckDigit,
                AccountNumber = generatedAccountNumber.AccountNumber,
                AccountNumberText = generatedAccountNumber.AccountNumberText,
                Iban = generatedAccountNumber.Iban,
                IbanSeparated = generatedAccountNumber.IbanSeparated,
                OpenedDate = DateTime.UtcNow,
                CustomerId = user.Id,
                CreatedById = user.Id
            };

            user.Customer.BankAccounts = new List<BankAccount> { bankAccount };
            user.CreatedBankAccounts = new List<BankAccount> { bankAccount };

            var result = await _userManager.CreateAsync(user, model.Register.User.Password);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());
            else
                return BadRequest(result.Errors);

            var bankAccountDto = _mapper.Map<BankAccount, BankAccountDto>(bankAccount);
            return CreatedAtRoute("GetBankAccount", new { bankAccountId = bankAccountDto.Id }, bankAccountDto);
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.UserIdIncludedInJwtToken)]
        [Route("CreateWithCustomerByWorker")]
        public async Task<ActionResult<BankAccountDto>> CreateBankAccountWithCustomerByWorker([FromBody] BankAccountWithCustomerCreationByWorkerDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = int.Parse(User.FindFirst(CustomClaimTypes.UserId).Value);

            var currentUser = _context.Users.SingleOrDefault(u => u.Id == currentUserId);
            if (currentUser == null)
            {
                ModelState.AddModelError(nameof(currentUser), $"User with id {currentUserId} found in claims doesn't exist in database.");
                return BadRequest(ModelState);
            }

            int? workerBranchId = null;

            if (await _userManager.IsInRoleAsync(currentUser, UserRole.Teller.ToString()))
            {
                await _context.Tellers.Where(t => t.Id == currentUser.Id).LoadAsync();

                if (currentUser.Teller.WorkAtId == null)
                {
                    ModelState.AddModelError(nameof(currentUser.Teller), $"Worker with id {currentUserId} is currently not assigned to any branch.");
                    return BadRequest(ModelState);
                }

                workerBranchId = currentUser.Teller.WorkAtId;
            }
            else if (await _userManager.IsInRoleAsync(currentUser, UserRole.Manager.ToString()))
            {
                await _context.Managers.Where(m => m.Id == currentUser.Id).LoadAsync();

                if (currentUser.Manager.WorkAtId == null)
                {
                    ModelState.AddModelError(nameof(currentUser.Manager), $"Worker with id {currentUserId} is currently not assigned to any branch.");
                    return BadRequest(ModelState);
                }

                workerBranchId = currentUser.Manager.WorkAtId;
            }

            var generatedAccountNumber = _accountNumberFactory.GenerateBankAccountNumber(workerBranchId);

            var user = _mapper.Map<ApplicationUser>(model.Register);
            user.Customer = new Customer { Id = user.Id };
            user.CreatedById = currentUserId;

            var bankAccount = new BankAccount
            {
                AccountType = (AccountType)model.BankAccount.AccountType,
                Currency = (Currency)model.BankAccount.Currency,
                CountryCode = generatedAccountNumber.CountryCode,
                CheckNumber = generatedAccountNumber.CheckNumber,
                NationalBankCode = generatedAccountNumber.NationalBankCode,
                BranchCode = generatedAccountNumber.BranchCode,
                NationalCheckDigit = generatedAccountNumber.NationalCheckDigit,
                AccountNumber = generatedAccountNumber.AccountNumber,
                AccountNumberText = generatedAccountNumber.AccountNumberText,
                Iban = generatedAccountNumber.Iban,
                IbanSeparated = generatedAccountNumber.IbanSeparated,
                OpenedDate = DateTime.UtcNow,
                CustomerId = user.Id,
                CreatedById = currentUserId
            };

            user.Customer.BankAccounts = new List<BankAccount> { bankAccount };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());
            else
                return BadRequest(result.Errors);

            var bankAccountDto = _mapper.Map<BankAccount, BankAccountDto>(bankAccount);
            return CreatedAtRoute("GetBankAccount", new { bankAccountId = bankAccountDto.Id }, bankAccountDto);
        }
    }
}