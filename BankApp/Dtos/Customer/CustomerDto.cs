using BankApp.Dtos.ApplicationUser;

namespace BankApp.Dtos.Customer
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public ApplicationUserDto ApplicationUser { get; set; }
    }
}
