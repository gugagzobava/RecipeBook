namespace RecipeBook.Api.Common;

public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public Dictionary<string, string[]>? Errors { get; }

    protected AppException(int statusCode, string message, Dictionary<string, string[]>? errors = null) : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}

public class BadRequestException : AppException
{
    public BadRequestException(string message, Dictionary<string, string[]>? errors = null) : base(400, message, errors) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized.") : base(401, message) { }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You do not have permission to perform this action.") : base(403, message) { }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(404, message) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(409, message) { }
}
