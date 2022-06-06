using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Configuration;
using BankApp.Dtos.BankAccount;
using BankApp.Dtos.BankAccount.WithCustomerCreation;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using BankApp.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BankAccountCreationDto = BankApp.Dtos.BankAccount.BankAccountCreationDto;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;
        private readonly IMapper _mapper;

        public BankAccountsController(IMapper mapper, IBankAccountService bankAccountService)
        {
            _mapper = mapper;
            _bankAccountService = bankAccountService;
        }

        [HttpGet]
        [Route("{bankAccountId}", Name = "GetBankAccount")]
        public async Task<ActionResult<BankAccountDto>> GetBankAccountAsync(int bankAccountId)
        {
            var bankAccount = await _bankAccountService.GetBankAccountAsync(bankAccountId);

            if (bankAccount == null)
                return NotFound();

            return Ok(_mapper.Map<BankAccount, BankAccountDto>(bankAccount));
        }

        [HttpGet]
        [Route("GetAllForUser/{applicationUserId}")]
        public async Task<ActionResult<IEnumerable<BankAccountDto>>> GetBankAccountsForUserAsync(int applicationUserId)
        {
            var bankAccounts = await _bankAccountService.GetBankAccountsForUserAsync(applicationUserId);

            if (!bankAccounts.Any())
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<BankAccount>, IEnumerable<BankAccountDto>>(bankAccounts));
        }

        [HttpPost]
        public async Task<ActionResult<BankAccountDto>> CreateBankAccountAsync([FromBody] BankAccountCreationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bankAccount = await _bankAccountService.CreateBankAccountAsync(model);

            var bankAccountDto = _mapper.Map<BankAccount, BankAccountDto>(bankAccount);
            return CreatedAtRoute("GetBankAccount", new { bankAccountId = bankAccountDto.Id }, bankAccountDto);
        }

        [HttpPost]
        [Route("CreateWithCustomerByCustomer")]
        public async Task<ActionResult<BankAccountDto>> CreateBankAccountWithCustomerByCustomerAsync(
            [FromBody] BankAccountWithCustomerCreationByCustomerDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bankAccount = await _bankAccountService.CreateBankAccountWithCustomerByCustomerAsync(model);

            var bankAccountDto = _mapper.Map<BankAccount, BankAccountDto>(bankAccount);
            return CreatedAtRoute("GetBankAccount", new { bankAccountId = bankAccountDto.Id }, bankAccountDto);
        }

        [HttpPost]
        [Authorize(Policy = PolicyName.UserIdIncludedInJwtToken)]
        [Route("CreateWithCustomerByWorker")]
        public async Task<ActionResult<BankAccountDto>> CreateBankAccountWithCustomerByWorker(
            [FromBody] BankAccountWithCustomerCreationByWorkerDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = int.Parse(User.Claims.Single(c => c.Type == CustomClaimTypes.UserId).Value);

            var bankAccount =
                await _bankAccountService.CreateBankAccountWithCustomerByWorkerAsync(model, currentUserId);

            var bankAccountDto = _mapper.Map<BankAccount, BankAccountDto>(bankAccount);
            return CreatedAtRoute("GetBankAccount", new { bankAccountId = bankAccountDto.Id }, bankAccountDto);
        }
    }
}