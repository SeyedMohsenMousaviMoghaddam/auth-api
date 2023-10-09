using FluentValidation.Results;


namespace Service.Identity.Application.Common;

public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        var failureGroups = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

        foreach (var failureGroup in failureGroups)
        {
            var propertyName = failureGroup.Key;
            var propertyFailures = failureGroup.ToArray();

            Errors.Add(propertyName, propertyFailures);
        }
    }

    public IDictionary<string, string[]> Errors { get; }
}
public class InvalidCommandException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public InvalidCommandException(Dictionary<string, string[]> errors)
    {
        Errors = errors;
    }
}
public class BadRequestException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(Dictionary<string, string[]> errors)
    {
        Errors = errors;
    }

    public BadRequestException(Dictionary<string, string[]> errors, string message) : base(message)
    {
        Errors = errors;
    }
}
public class ForbiddenException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException(Dictionary<string, string[]> errors)
    {
        Errors = errors;
    }

    public ForbiddenException(Dictionary<string, string[]> errors, string message) : base(message)
    {
        Errors = errors;

    }
}
public class NotFoundException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(Dictionary<string, string[]> errors)
    {
        Errors = errors;
    }

    public NotFoundException(Dictionary<string, string[]> errors, string message) : base(message)
    {
        Errors = errors;
    }
}
public class UnprocessableEntityException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public UnprocessableEntityException(string message)
        : base(message)
    {
    }

    public UnprocessableEntityException(Dictionary<string, string[]> errors)
    {
        Errors = errors;
    }

    public UnprocessableEntityException(Dictionary<string, string[]> errors, string message)
        : base(message)
    {
        Errors = errors;
    }
}