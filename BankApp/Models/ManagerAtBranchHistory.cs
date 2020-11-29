namespace BankApp.Models
{
    public class ManagerAtBranchHistory : WorkerAtBranchHistory
    {
        public Manager Manager { get; set; }
        public int ManagerId { get; set; }
    }
}