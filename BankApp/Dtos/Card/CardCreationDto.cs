using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.Card
{
    public class CardCreationDto
    {
        [Required]
        public int? BankAccountId { get; set; }
    }
}
