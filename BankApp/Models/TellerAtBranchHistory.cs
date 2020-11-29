namespace BankApp.Models
{
    public class TellerAtBranchHistory : WorkerAtBranchHistory
    {
        public Teller Teller { get; set; }
        public int TellerId { get; set; }
    }
}
