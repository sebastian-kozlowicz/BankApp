using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos
{
    public class AddressDto
    {
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        public string Street { get; set; }
        [Required]
        public string HouseNumber { get; set; }
        public string ApartmentNumber { get; set; }
        [Required]
        public string PostalCode { get; set; }
    }
}
