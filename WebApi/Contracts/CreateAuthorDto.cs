namespace WebApi.Contracts;

public class CreateAuthorDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    private DateTime _dateOfBirth;
    public DateTime DateOfBirth
    {
        get => _dateOfBirth;
        set => _dateOfBirth = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
    public string Country { get; set; } = string.Empty;
}


