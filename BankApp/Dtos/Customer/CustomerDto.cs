using BankApp.Dtos.ApplicationUser;

namespace BankApp.Dtos.Customer
{
    public class CustomerDto
    {
        public string Id { get; set; }
        public ApplicationUserDto ApplicationUser { get; set; }
    }
}
