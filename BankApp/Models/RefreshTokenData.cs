using System;

namespace BankApp.Models
{
    public class RefreshTokenData
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        public string Jti { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public bool IsInvalidated { get; set; }
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}