namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Represents a dialog control that provides functionality for user interaction within a graphical user interface.
/// </summary>
/// <remarks>A dialog will be registered into service collection for dependency injection as <c>singleton lifetime</c>.</remarks>
public interface IDialog : IControl, IComponentEvent
{
    /// <summary>
    /// Event handler invoked when the dialog is about to close.
    /// </summary>
    /// <param name="sender">The source object initiating the closure.</param>
    /// <param name="e">Contains data about the form-closing event, including the ability to cancel closure.</param>
    void OnDialogClosing(object? sender, FormClosingEventArgs e);
}
