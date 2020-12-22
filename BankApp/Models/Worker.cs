namespace BankApp.Models
{
    public abstract class Worker
    {
        public int Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Branch WorkAt { get; set; }
        public int? WorkAtId { get; set; }
    }
}
