using BankApp.Configuration;
using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using System;
using System.Linq;

namespace BankApp.Helpers.Builders
{
    public class VisaPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        private readonly ApplicationDbContext _context;
        private static readonly string VisaPrefix = "4";

        public VisaPaymentCardNumberBuilder(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length)
        {
            if (!VisaAcceptedLength.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Visa payment card number length is invalid.");

            var bankIdentificationNumber = GetBankIdentificationNumber();
            if (!bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(VisaPrefix))
                throw new ArgumentException("Visa bank identifiaction number found in database is invalid.");

            return null;
        }

        private BankIdentificationNumberData GetBankIdentificationNumber()
        {
            return _context.BankIdentificationNumberData.FirstOrDefault(bin => bin.IssuingNetwork == IssuingNetwork.Visa);
        }
    }
}
