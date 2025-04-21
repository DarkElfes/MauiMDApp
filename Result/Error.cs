namespace Results;

public sealed record Error(string Description)
{
    public static readonly Error None = new(string.Empty);
    public static readonly Error NullValue = new("Not have value");

    public static implicit operator Result(Error error) => Result.Failure(error);
}