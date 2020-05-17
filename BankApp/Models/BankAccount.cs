using BankApp.Enumerators;

namespace BankApp.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public AccountType AccountType { get; set; }
        public Currency Currency { get; set; }
        public string Number { get; set; }
        public decimal Balance { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
