namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Represents a modal control that provides functionality for user interaction within a graphical user interface.
/// </summary>
/// <remarks>A modal will be registered into service collection for dependency injection as <c>transient lifetime</c>.</remarks>
public interface IModal : IControl, ITransientDependency
{
    /// <summary>
    /// Event handler invoked when the modal is about to close.
    /// </summary>
    /// <param name="sender">The source object initiating the closure.</param>
    /// <param name="e">Contains data about the form-closing event, including the ability to cancel closure.</param>
    void OnModalClosing(object? sender, FormClosingEventArgs e);
}
