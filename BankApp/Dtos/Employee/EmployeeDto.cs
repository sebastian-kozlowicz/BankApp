using BankApp.Dtos.ApplicationUser;

namespace BankApp.Dtos.Employee
{
    public class EmployeeDto
    {
        public string Id { get; set; }
        public ApplicationUserDto ApplicationUser { get; set; }
    }
}
