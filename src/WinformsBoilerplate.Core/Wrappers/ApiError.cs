namespace WinformsBoilerplate.Core.Wrappers;

/// <summary>
/// A representation of an error that can occur during an API call.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Description">A description of the error.</param>
public sealed record ApiError(string Code, string Description)
{
    /// <summary>
    /// No error occurred. This is used to represent a successful API call with no errors.
    /// </summary>
    public static readonly ApiError None = new(string.Empty, string.Empty);
}
