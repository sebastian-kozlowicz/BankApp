using BankApp.Configuration;
using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApp.Helpers.Builders
{
    public class MastercardPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        private readonly ApplicationDbContext _context;
        private static readonly List<string> MastercardValidPrefixes = new List<string> { "51", "52", "53", "54", "55" };

        public MastercardPaymentCardNumberBuilder(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length)
        {
            if (!MastercardAcceptedLength.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Mastercard payment card number length is invalid.");

            var bankIdentificationNumber = GetBankIdentificationNumber();
            if (!MastercardValidPrefixes.Any(prefix => bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(prefix)))
                throw new ArgumentException("Mastercard bank identifiaction number found in database is invalid.");
            return null;
        }

        private BankIdentificationNumberData GetBankIdentificationNumber()
        {
            return _context.BankIdentificationNumberData.FirstOrDefault(bin => bin.IssuingNetwork == IssuingNetwork.Mastercard);
        }
    }
}
