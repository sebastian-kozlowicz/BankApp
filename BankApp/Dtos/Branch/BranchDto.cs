using BankApp.Dtos.Address;

namespace BankApp.Dtos.Branch
{
    public class BranchDto
    {
        public int Id { get; set; }
        public string BranchCode { get; set; }
        public AddressDto BranchAddress { get; set; }
    }
}
