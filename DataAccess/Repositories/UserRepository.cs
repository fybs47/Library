using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context)
        {
            _context = context;
        }

        public void DetachLocal(Guid entityId)
        {
            var local = _context.Set<UserEntity>().Local.FirstOrDefault(entry => entry.Id.Equals(entityId));
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }
        }

        public async Task<UserEntity> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<UserEntity> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserEntity> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserEntity> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.Now);
        }

        public async Task AddUserAsync(UserEntity user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(UserEntity user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict: The data has been modified or deleted since it was last loaded.", ex);
            }
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }

}