using DataAccess.Models;
using FluentValidation;

namespace DataAccess.Validators
{
    public class UserValidator : AbstractValidator<UserEntity>
    {
        public UserValidator()
        {
            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("Username is required")
                .Length(1, 100).WithMessage("Username can't be longer than 100 characters");

            RuleFor(user => user.PasswordHash)
                .NotEmpty().WithMessage("PasswordHash is required");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(user => user.Role)
                .NotEmpty().WithMessage("Role is required")
                .Length(1, 20).WithMessage("Role can't be longer than 20 characters");

            RuleFor(user => user.RefreshTokenExpiryTime)
                .Must(expiryTime => expiryTime.Kind == DateTimeKind.Utc)
                .WithMessage("RefreshTokenExpiryTime must be in UTC");
        }
    }
}