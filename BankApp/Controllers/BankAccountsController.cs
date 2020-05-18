using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.BankAccount;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BankAccountsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public ActionResult CreateBankAccount([FromBody]BankAccountCreationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bankAccount = new BankAccount
            {
                AccountType = model.AccountType,
                Currency = model.Currency,
                ApplicationUserId = model.ApplicationUserId
            };

            _context.BankAccounts.Add(bankAccount);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public ActionResult<IEnumerable<BankAccountDto>> GetAccounts()
        {
            var accounts = _context.BankAccounts.ToList();
            return Ok(_mapper.Map<List<BankAccount>, List<BankAccountDto>>(accounts));
        }
    }
}