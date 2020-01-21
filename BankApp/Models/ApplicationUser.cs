using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BankApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
    }
}
