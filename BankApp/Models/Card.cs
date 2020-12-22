namespace BankApp.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public BankAccount BankAccount { get; set; }
        public int BankAccountId { get; set; }
    }
}
