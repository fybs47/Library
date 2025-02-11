namespace Application.Exсeptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}