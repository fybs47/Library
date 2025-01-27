using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity> GetUserByIdAsync(Guid id);
        Task<UserEntity> GetUserByUsernameAsync(string username);
        Task<UserEntity> GetUserByEmailAsync(string email);
        Task AddUserAsync(UserEntity user);
        Task UpdateUserAsync(UserEntity user);
        Task DeleteUserAsync(Guid id);
    }
}