using Domain.Models;

namespace Application.Abstractions
{
    public interface ITokenService
    {
        Task<string> GenerateJwtTokenAsync(User user, CancellationToken cancellationToken);
        Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken);
        Task<User> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    }
}
