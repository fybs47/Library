using DataAccess.Models;
using DataAccess.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class AuthorRepository : BaseRepository<AuthorEntity>, IAuthorRepository
    {
        private readonly IValidator<AuthorEntity> _authorValidator;

        public AuthorRepository(ApplicationContext context, IValidator<AuthorEntity> authorValidator)
            : base(context)
        {
            _authorValidator = authorValidator;
        }

        public async Task<IEnumerable<BookEntity>> GetBooksByAuthorAsync(Guid authorId, CancellationToken cancellationToken)
        {
            return await Context.Set<BookEntity>().AsNoTracking().Where(b => b.AuthorId == authorId).ToListAsync(cancellationToken);
        }
    }
}