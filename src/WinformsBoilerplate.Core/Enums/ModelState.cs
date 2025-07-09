namespace WinformsBoilerplate.Core.Enums;

/// <summary>
/// Represents the state of a model in an application, typically used to track changes or initialization status.
/// </summary>
/// <remarks>
/// This enumeration is commonly used to indicate whether a model has been initialized, remains
/// unchanged, or has been modified. It can be useful in scenarios such as data validation, change tracking, or workflow
/// management.
/// </remarks>
public enum ModelState
{
    Init,
    None,
    Modified
}
