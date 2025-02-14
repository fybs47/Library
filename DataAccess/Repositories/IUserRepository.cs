using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IUserRepository
    {
        void DetachLocal(Guid entityId);
        Task<UserEntity> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<UserEntity> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<UserEntity> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
        Task<UserEntity> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task AddUserAsync(UserEntity user, CancellationToken cancellationToken);
        Task UpdateUserAsync(UserEntity user, CancellationToken cancellationToken);
        Task DeleteUserAsync(Guid id, CancellationToken cancellationToken);
    }
}