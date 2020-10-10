﻿using System;
using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;

namespace BankApp.Helpers.Services
{
    public class InternalTransferService : ITransferService<InternalTransferService>
    {
        private readonly ApplicationDbContext _context;

        public InternalTransferService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(BankAccount bankAccount, BankAccount targetBankAccount, decimal value)
        {
            if (bankAccount.Currency != targetBankAccount.Currency)
                throw new ArgumentException("Currency is different in target bank account.", "Currency");

            bankAccount.Balance -= value;
            targetBankAccount.Balance += value;

            var bankTransfer = new BankTransfer
            {
                ReceiverIban = targetBankAccount.Iban,
                OrderDate = DateTime.Now,
                BankTransferType = BankTransferType.Internal,
                BankAccountId = bankAccount.Id
            };

            _context.BankTransfers.Add(bankTransfer);
            _context.SaveChanges();
        }
    }
}
