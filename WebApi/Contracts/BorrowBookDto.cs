namespace WebApi.Contracts;

public class BorrowBookDto
{
    public Guid BookId { get; set; }
    public DateTime DueDate { get; set; }
}
