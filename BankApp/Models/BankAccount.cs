using System;
using System.Collections.Generic;
using BankApp.Data;
using BankApp.Enumerators;
using System.Linq;

namespace BankApp.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public AccountType AccountType { get; set; }
        public Currency Currency { get; set; }
        public string CountryCode { get; set; }
        public string CheckDigits { get; set; }
        public string NationalBankCode { get; set; }
        public string BranchCode { get; set; }
        public int NationalCheckDigit { get; set; }
        public long AccountNumber { get; set; }
        public string AccountNumberText { get; set; }
        public string Iban { get; set; }
        public string IbanSeparated { get; set; }
        public decimal Balance { get; set; }
        public decimal DebitLimit { get; set; }
        public DateTime OpenedDate { get; set; }
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public int CreatedById { get; set; }
        public IList<PaymentCard> PaymentCards { get; set; }
        public IList<BankTransfer> BankTransfers { get; set; }

        private readonly ApplicationDbContext _context;

        public BankAccount()
        {
        }

        public BankAccount(ApplicationDbContext context)
        {
            _context = context;
        }

        public BankAccount GetBankAccount(int bankAccountId)
        {
            return _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountId);
        }
    }
}
