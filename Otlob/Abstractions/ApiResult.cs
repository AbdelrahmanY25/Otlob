namespace Otlob.Abstractions;

public class ApiResult
{
    public ApiResult(bool isSuccess, ApiError apiError)
    {
        if (isSuccess && apiError != ApiError.None || !isSuccess && apiError == ApiError.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        ApiError = apiError;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ApiError ApiError { get; } = default!;

    public static ApiResult Success() => new(true, ApiError.None);
    public static ApiResult Failure(ApiError apiError) => new(false, apiError);

    public static ApiResult<TValue> Success<TValue>(TValue value) => new(value, true, ApiError.None);
    public static ApiResult<TValue> Failure<TValue>(ApiError apiError) => new(default!, false, apiError);
}

public class ApiResult<TValue>(TValue value, bool isSuccess, ApiError apiError) : ApiResult(isSuccess, apiError)
{
    private readonly TValue _value = value;

    public TValue Value => IsSuccess ? _value : throw new InvalidOperationException("Failure ApiResult can't have value");
}