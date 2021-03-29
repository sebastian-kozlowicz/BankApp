namespace BankApp.Dtos.Auth
{
    public class RefreshTokenRequestDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}