using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.BankTransfer
{
    public class BankTransferCreationDto
    {
        [Required]
        public int? RequesterBankAccountId { get; set; }
        [Required]
        public string ReceiverIban { get; set; }
        [Required]
        [RegularExpression(@"^\d+.?\d{0,2}$", ErrorMessage = "The {0} is accepting maximum two decimal points")]
        [Range(typeof(decimal), "0.01", "9999999999999999.99", ErrorMessage = "The {0} must be at least {1} and at max {2} value")]
        public decimal? Value { get; set; }
    }
}
