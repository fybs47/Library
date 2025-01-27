namespace Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public string RefreshToken { get; set; } = string.Empty; 
        public DateTime RefreshTokenExpiryTime { get; set; }    
    }
}