namespace WinformsBoilerplate.Core.Wrappers;

/// <summary>
/// A wrapper for API results that can either be successful or contain an error.
/// </summary>
/// <typeparam name="T">The type of data returned in a successful result.</typeparam>
public class ApiResult<T>
{
    private ApiResult(bool isSuccess, ApiError error, T? data)
    {
        if ((isSuccess && error != ApiError.None) ||
            (!isSuccess && error == ApiError.None))
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        Data = data;
    }

    /// <summary>
    /// True if the API call was successful; otherwise, false.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// The error that occurred during the API call, if any.
    /// </summary>
    public ApiError Error { get; }

    /// <summary>
    /// The data returned by the API call if it was successful; otherwise, null.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Creates a successful API result with the provided data.
    /// </summary>
    /// <param name="data">The data to return in the successful result.</param>
    /// <returns>A successful ApiResult containing the provided data.</returns>
    public static ApiResult<T> Success(T data) => new(true, ApiError.None, data);

    /// <summary>
    /// Creates a failed API result with the provided error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed ApiResult containing the provided error.</returns>
    public static ApiResult<T> Failure(ApiError error) => new(false, error, default);
}
