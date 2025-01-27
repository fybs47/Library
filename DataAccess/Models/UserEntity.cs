namespace DataAccess.Models
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public string RefreshToken { get; set; } = string.Empty;

        private DateTime _refreshTokenExpiryTime;

        public DateTime RefreshTokenExpiryTime
        {
            get => _refreshTokenExpiryTime;
            set => _refreshTokenExpiryTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}