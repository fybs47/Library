using DataAccess.Models;
using FluentValidation;

namespace DataAccess.Validators
{
    public class BookValidator : AbstractValidator<BookEntity>
    {
        public BookValidator()
        {
            RuleFor(book => book.ISBN)
                .NotEmpty().WithMessage("ISBN is required")
                .Length(10, 13).WithMessage("ISBN must be between 10 and 13 characters");

            RuleFor(book => book.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title can't be longer than 200 characters");

            RuleFor(book => book.Genre)
                .NotEmpty().WithMessage("Genre is required")
                .MaximumLength(100).WithMessage("Genre can't be longer than 100 characters");

            RuleFor(book => book.Description)
                .NotEmpty().WithMessage("Description is required");

            RuleFor(book => book.AuthorId)
                .NotEmpty().WithMessage("AuthorId is required");

            RuleFor(book => book.BorrowedTime)
                .LessThanOrEqualTo(book => book.DueDate)
                .WithMessage("BorrowedTime must be less than or equal to DueDate");

            RuleFor(book => book.ImagePath)
                .MaximumLength(1000).WithMessage("ImagePath can't be longer than 1000 characters");
        }
    }
}