using Microsoft.AspNetCore.Identity;

namespace BankApp.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Administrator Administrator { get; set; }
        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
        public Manager Manager { get; set; }
        public UserAddress UserAddress { get; set; }
    }
}
