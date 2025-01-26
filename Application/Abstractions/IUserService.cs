using Domain.Models;

namespace Application.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByUsernameAsync(string username);
        Task RegisterUserAsync(User user, string password);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid id);
        Task<User> AuthenticateUserAsync(string username, string password);
    }
}