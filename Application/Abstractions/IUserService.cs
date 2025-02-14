using Domain.Models;

namespace Application.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
        Task RegisterUserAsync(User user, string password, CancellationToken cancellationToken);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken);
        Task DeleteUserAsync(Guid id, CancellationToken cancellationToken);
        Task<User> AuthenticateUserAsync(string username, string password, CancellationToken cancellationToken);
        Task<String> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken);
        Task<User> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    }
}