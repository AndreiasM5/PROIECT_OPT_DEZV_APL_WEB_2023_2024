namespace Backend.ExceptionHandling;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}