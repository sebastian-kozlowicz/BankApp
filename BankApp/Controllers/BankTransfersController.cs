﻿using BankApp.Data;
using BankApp.Dtos.BankTransfer;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using BankApp.Helpers;
using BankApp.Interfaces;
using System;

namespace BankApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankTransfersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransferService<InternalTransferService> _internalTransferService;
        private readonly ITransferService<ExternalTransferService> _externalTransferService;

        public BankTransfersController(ApplicationDbContext context, ITransferService<InternalTransferService> internalTransferService, ITransferService<ExternalTransferService> externalTransferService)
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

            var bankAccount = _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankTransferCreationDto.RequesterBankAccountId);

            if (bankAccount == null)
                return NotFound();

            if (Math.Abs(bankAccount.Balance - (decimal)bankTransferCreationDto.Value) > bankAccount.DebitLimit)
            {
                ModelState.AddModelError("DebitLimit", "Not sufficient founds. Debit limit is exceeded.");
                return BadRequest(ModelState);
            }

            var targetBankAccount = _context.BankAccounts.FirstOrDefault(ba => ba.Iban == bankTransferCreationDto.ReceiverIban);

            if (targetBankAccount == null)
                _externalTransferService.Create(bankAccount, targetBankAccount, (decimal)bankTransferCreationDto.Value);
            else
                _internalTransferService.Create(bankAccount, targetBankAccount, (decimal)bankTransferCreationDto.Value);

            return Ok();
        }
    }
}
