namespace BankApp.Models
{
    public class Branch
    {
        public string Id { get; set; }
        public string BranchCode { get; set; }
        public BranchAddress BranchAddress { get; set; }
        public Headquarters Headquarters { get; set; }
    }
}
