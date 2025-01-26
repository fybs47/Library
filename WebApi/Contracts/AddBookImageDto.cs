namespace WebApi.Contracts;

public class AddBookImageDto
{
    public Guid BookId { get; set; }
    public string ImagePath { get; set; } = string.Empty;
}
