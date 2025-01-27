using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IUserRepository
    {
        void DetachLocal(Guid entityId);
        Task<UserEntity> GetUserByIdAsync(Guid id);
        Task<UserEntity> GetUserByUsernameAsync(string username);
        Task<UserEntity> GetUserByEmailAsync(string email);
        Task<UserEntity> GetUserByRefreshTokenAsync(string refreshToken);
        Task AddUserAsync(UserEntity user);
        Task UpdateUserAsync(UserEntity user);
        Task DeleteUserAsync(Guid id);
    }
}