using BankApp.Enumerators;
using System;

namespace BankApp.Models
{
    public class BankTransfer
    {
        public int Id { get; set; }
        public string ReceiverIban { get; set; }
        public DateTime OrderDate { get; set; }
        public BankTransferType BankTransferType { get; set; }
        public BankAccount BankAccount { get; set; }
        public int BankAccountId { get; set; }
    }
}
