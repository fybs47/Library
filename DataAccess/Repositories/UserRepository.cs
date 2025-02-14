using DataAccess.Models;
using DataAccess.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        private readonly IValidator<UserEntity> _userValidator;

        public UserRepository(ApplicationContext context, IValidator<UserEntity> userValidator)
            : base(context)
        {
            _userValidator = userValidator;
        }

        public void DetachLocal(Guid entityId)
        {
            var local = Context.Set<UserEntity>().Local.FirstOrDefault(entry => entry.Id.Equals(entityId));
            if (local != null)
            {
                Context.Entry(local).State = EntityState.Detached;
            }
        }

        public async Task<UserEntity> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            return await Context.Set<UserEntity>().AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<UserEntity> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await Context.Set<UserEntity>().AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<UserEntity> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            return await Context.Set<UserEntity>().AsNoTracking().FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.Now, cancellationToken);
        }
    }
}