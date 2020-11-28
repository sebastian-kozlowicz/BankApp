using BankApp.Dtos.ApplicationUser;

namespace BankApp.Dtos.Teller
{
    public class TellerDto
    {
        public string Id { get; set; }
        public ApplicationUserDto ApplicationUser { get; set; }
    }
}
