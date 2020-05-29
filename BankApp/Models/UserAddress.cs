namespace BankApp.Models
{
    public class UserAddress : Address
    {
        public ApplicationUser ApplicationUser { get; set; }
    }
}
