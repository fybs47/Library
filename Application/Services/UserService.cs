using AutoMapper;
using DataAccess.Repositories;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid user ID.");
                throw new ArgumentException("Invalid user ID.");
            }

            try
            {
                var userEntity = await _userRepository.GetUserByIdAsync(id);
                return _mapper.Map<User>(userEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserByIdAsync");
                throw;
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogError("Username cannot be null or empty.");
                throw new ArgumentException("Username cannot be null or empty.");
            }

            try
            {
                var userEntity = await _userRepository.GetUserByUsernameAsync(username);
                return _mapper.Map<User>(userEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserByUsernameAsync");
                throw;
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogError("Email cannot be null or empty.");
                throw new ArgumentException("Email cannot be null or empty.");
            }

            try
            {
                var userEntity = await _userRepository.GetUserByEmailAsync(email);
                return _mapper.Map<User>(userEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserByEmailAsync");
                throw;
            }
        }

        public async Task RegisterUserAsync(User user, string password)
        {
            if (user == null)
            {
                _logger.LogError("User cannot be null.");
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                _logger.LogError("Password cannot be null or empty.");
                throw new ArgumentException("Password cannot be null or empty.");
            }

            try
            {
                if (await _userRepository.GetUserByUsernameAsync(user.Username) != null)
                {
                    _logger.LogError("User with this username already exists.");
                    throw new ArgumentException("User with this username already exists.");
                }

                if (await _userRepository.GetUserByEmailAsync(user.Email) != null)
                {
                    _logger.LogError("User with this email already exists.");
                    throw new ArgumentException("User with this email already exists.");
                }

                user.PasswordHash = HashPassword(password);
                var userEntity = _mapper.Map<DataAccess.Models.UserEntity>(user);
                await _userRepository.AddUserAsync(userEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterUserAsync");
                throw;
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            if (user == null)
            {
                _logger.LogError("User cannot be null.");
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                var userEntity = _mapper.Map<DataAccess.Models.UserEntity>(user);
                await _userRepository.UpdateUserAsync(userEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateUserAsync");
                throw;
            }
        }

        public async Task DeleteUserAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid user ID.");
                throw new ArgumentException("Invalid user ID.");
            }

            try
            {
                await _userRepository.DeleteUserAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteUserAsync");
                throw;
            }
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogError("Username and password cannot be null or empty.");
                throw new ArgumentException("Username and password cannot be null or empty.");
            }

            try
            {
                var userEntity = await _userRepository.GetUserByUsernameAsync(username);
                if (userEntity == null || !VerifyPassword(password, userEntity.PasswordHash))
                {
                    _logger.LogError("Invalid username or password.");
                    return null;
                }

                return _mapper.Map<User>(userEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AuthenticateUserAsync");
                throw;
            }
        }

        public async Task<string> GenerateRefreshTokenAsync(User user)
        {
            if (user == null)
            {
                _logger.LogError("User cannot be null.");
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                _userRepository.DetachLocal(user.Id);

                var userEntity = await _userRepository.GetUserByIdAsync(user.Id);
                if (userEntity == null)
                {
                    _logger.LogError("User not found.");
                    throw new Exception("User not found.");
                }

                var refreshToken = Guid.NewGuid().ToString();
                userEntity.RefreshToken = refreshToken;
                userEntity.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _userRepository.UpdateUserAsync(userEntity);
                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateRefreshTokenAsync");
                throw;
            }
        }

        public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogError("Refresh token cannot be null or empty.");
                throw new ArgumentException("Refresh token cannot be null or empty.");
            }

            try
            {
                var userEntity = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
                return _mapper.Map<User>(userEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserByRefreshTokenAsync");
                throw;
            }
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
