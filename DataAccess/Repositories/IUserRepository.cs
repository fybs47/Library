using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IUserRepository : IBaseRepository<UserEntity>
    {
        void DetachLocal(Guid entityId);
        Task<UserEntity> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<UserEntity> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
        Task<UserEntity> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    }
}