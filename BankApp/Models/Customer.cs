using System.Collections.Generic;

namespace BankApp.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public IList<BankAccount> BankAccounts { get; set; }
    }
}
