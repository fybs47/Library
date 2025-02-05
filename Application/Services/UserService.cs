using AutoMapper;
using DataAccess.Repositories;
using Domain.Models;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var userEntity = await _userRepository.GetUserByIdAsync(id);
            return _mapper.Map<User>(userEntity);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var userEntity = await _userRepository.GetUserByUsernameAsync(username);
            return _mapper.Map<User>(userEntity);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var userEntity = await _userRepository.GetUserByEmailAsync(email);
            return _mapper.Map<User>(userEntity);
        }

        public async Task RegisterUserAsync(User user, string password)
        {
            if (await _userRepository.GetUserByUsernameAsync(user.Username) != null)
            {
                throw new ArgumentException("User with this username already exists.");
            }

            if (await _userRepository.GetUserByEmailAsync(user.Email) != null)
            {
                throw new ArgumentException("User with this email already exists.");
            }

            user.PasswordHash = HashPassword(password);
            var userEntity = _mapper.Map<DataAccess.Models.UserEntity>(user);
            await _userRepository.AddUserAsync(userEntity);
        }

        public async Task UpdateUserAsync(User user)
        {
            var userEntity = _mapper.Map<DataAccess.Models.UserEntity>(user);
            await _userRepository.UpdateUserAsync(userEntity);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await _userRepository.DeleteUserAsync(id);
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var userEntity = await _userRepository.GetUserByUsernameAsync(username);
            if (userEntity == null || !VerifyPassword(password, userEntity.PasswordHash))
            {
                return null;
            }

            return _mapper.Map<User>(userEntity);
        }

        public async Task<string> GenerateRefreshTokenAsync(User user)
        {
            _userRepository.DetachLocal(user.Id);

            var userEntity = await _userRepository.GetUserByIdAsync(user.Id);
            if (userEntity == null)
            {
                throw new Exception("User not found.");
            }

            var refreshToken = Guid.NewGuid().ToString();
            userEntity.RefreshToken = refreshToken;
            userEntity.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userRepository.UpdateUserAsync(userEntity);
            return refreshToken;
        }

        public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
        {
            var userEntity = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
            return _mapper.Map<User>(userEntity);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == passwordHash;
        } 
    }   
}
