namespace WebApi.Contracts;

public class AuthorDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DatфeOfBirth { get; set; }
    public string Country { get; set; } = string.Empty;
}
