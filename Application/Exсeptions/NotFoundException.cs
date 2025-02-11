namespace Application.Exсeptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}