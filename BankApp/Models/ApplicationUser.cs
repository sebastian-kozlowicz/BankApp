using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BankApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Customer Customer { get; set; }
    }
}
