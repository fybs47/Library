using DataAccess.Models;
using DataAccess.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;
        private readonly IValidator<UserEntity> _userValidator;

        public UserRepository(ApplicationContext context, IValidator<UserEntity> userValidator)
        {
            _context = context;
            _userValidator = userValidator;
        }

        public void DetachLocal(Guid entityId)
        {
            var local = _context.Set<UserEntity>().Local.FirstOrDefault(entry => entry.Id.Equals(entityId));
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }
        }

        public async Task<UserEntity> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<UserEntity> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<UserEntity> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<UserEntity> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.Now, cancellationToken);
        }

        public async Task AddUserAsync(UserEntity user, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _userValidator.ValidateAsync(user, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("User entity validation failed", validationResult.Errors);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateUserAsync(UserEntity user, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _userValidator.ValidateAsync(user, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("User entity validation failed", validationResult.Errors);
            }

            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict: The data has been modified or deleted since it was last loaded.", ex);
            }
        }

        public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
