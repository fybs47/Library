using DataAccess.Models;
using FluentValidation;

namespace DataAccess.Validators
{
    public class AuthorValidator : AbstractValidator<AuthorEntity>
    {
        public AuthorValidator()
        {
            RuleFor(author => author.FirstName)
                .NotEmpty().WithMessage("FirstName is required")
                .MaximumLength(100).WithMessage("FirstName can't be longer than 100 characters");

            RuleFor(author => author.LastName)
                .NotEmpty().WithMessage("LastName is required")
                .MaximumLength(100).WithMessage("LastName can't be longer than 100 characters");

            RuleFor(author => author.DateOfBirth)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("DateOfBirth must be in the past");

            RuleFor(author => author.Country)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(100).WithMessage("Country can't be longer than 100 characters");
        }
    }
}