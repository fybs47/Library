namespace WebApi.Contracts;

public class UpdateBookDto
{
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public DateTime BorrowedTime { get; set; }
    public DateTime DueDate { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public bool IsBorrowed { get; set; }
}
