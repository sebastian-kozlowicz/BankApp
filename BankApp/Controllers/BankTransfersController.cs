using System;
using System.Linq;
using BankApp.Data;
using BankApp.Dtos.BankTransfer;
using BankApp.Interfaces.Services;
using BankApp.Models;
using BankApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankTransfersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransferService<ExternalTransferService> _externalTransferService;
        private readonly ITransferService<InternalTransferService> _internalTransferService;

        public BankTransfersController(ApplicationDbContext context,
            ITransferService<InternalTransferService> internalTransferService,
            ITransferService<ExternalTransferService> externalTransferService)
        {
            _context = context;
            _internalTransferService = internalTransferService;
            _externalTransferService = externalTransferService;
        }

        [HttpPost]
        public ActionResult CreateBankTransfer([FromBody] BankTransferCreationDto bankTransferCreationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var requesterBankAccount =
                _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankTransferCreationDto.RequesterBankAccountId);

            if (requesterBankAccount == null)
                return NotFound();

            if (requesterBankAccount.Balance - bankTransferCreationDto.Value < requesterBankAccount.DebitLimit * -1)
            {
                ModelState.AddModelError(nameof(requesterBankAccount.DebitLimit),
                    "Not sufficient founds. Debit limit is exceeded.");
                return BadRequest(ModelState);
            }

            var targetBankAccount =
                _context.BankAccounts.FirstOrDefault(ba => ba.Iban == bankTransferCreationDto.ReceiverIban);

            if (targetBankAccount == null)
                _externalTransferService.Create(requesterBankAccount,
                    new BankAccount {Iban = bankTransferCreationDto.ReceiverIban},
                    (decimal) bankTransferCreationDto.Value);
            else
                try
                {
                    _internalTransferService.Create(requesterBankAccount, targetBankAccount,
                        (decimal) bankTransferCreationDto.Value);
                }
                catch (ArgumentException exception)
                {
                    ModelState.AddModelError(exception.ParamName, exception.Message);
                    return BadRequest(ModelState);
                }

            return Ok();
        }
    }
}