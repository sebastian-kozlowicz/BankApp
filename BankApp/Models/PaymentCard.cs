﻿using BankApp.Enumerators;

namespace BankApp.Models
{
    public class PaymentCard
    {
        public int Id { get; set; }
        public byte MajorIndustryIdentifier { get; set; }
        public int BankIdentificationNumber { get; set; }
        public long AccountIdentificationNumber { get; set; }
        public string AccountIdentificationNumberText { get; set; }
        public byte CheckDigit { get; set; }
        public string Number { get; set; }
        public IssuingNetwork IssuingNetwork { get; set; }
        public BankAccount BankAccount { get; set; }
        public int BankAccountId { get; set; }
    }
}
