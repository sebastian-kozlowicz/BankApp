using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.BankTransfer
{
    public class BankTransferCreationDto
    {
        [Required]
        public int RequesterBankAccountId { get; set; }
        [Required]
        public string ReceiverIban { get; set; }
        [Required]
        public decimal Value { get; set; }
    }
}
