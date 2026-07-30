namespace RestaurantManagement.Api.Services;

public sealed record OperationResult<T>
{
    private OperationResult(T? value, OperationFailure failure, string? message)
    {
        Value = value;
        Failure = failure;
        Message = message;
    }

    public T? Value { get; }
    public OperationFailure Failure { get; }
    public string? Message { get; }
    public bool IsSuccess => Failure == OperationFailure.None;

    public static OperationResult<T> Success(T value) =>
        new(value, OperationFailure.None, null);

    public static OperationResult<T> Fail(OperationFailure failure, string message)
    {
        if (failure == OperationFailure.None)
        {
            throw new ArgumentException(
                "Un resultado fallido debe indicar el tipo de error.",
                nameof(failure));
        }

        return new(default, failure, message);
    }
}
