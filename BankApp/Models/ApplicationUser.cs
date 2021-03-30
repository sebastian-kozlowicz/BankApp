using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BankApp.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Administrator Administrator { get; set; }
        public Customer Customer { get; set; }
        public Teller Teller { get; set; }
        public Manager Manager { get; set; }
        public UserAddress UserAddress { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public int? CreatedById { get; set; }
        public IList<BankAccount> CreatedBankAccounts { get; set; }
        public IList<TellerAtBranchHistory> AssignedTellersToBranchHistory { get; set; }
        public IList<TellerAtBranchHistory> ExpelledTellersFromBranchHistory { get; set; }
        public IList<ManagerAtBranchHistory> AssignedManagersToBranchHistory { get; set; }
        public IList<ManagerAtBranchHistory> ExpelledManagersFromBranchHistory { get; set; }
        public IList<RefreshTokenData> RefreshTokens { get; set; }
    }
}
