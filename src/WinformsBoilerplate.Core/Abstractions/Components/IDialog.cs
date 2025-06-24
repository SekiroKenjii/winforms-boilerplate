namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Represents a dialog control that provides functionality for user interaction within a graphical user interface.
/// </summary>
/// <remarks>A dialog is typically used to display information, prompt the user for input, or facilitate specific
/// tasks. Implementations of this interface may include modal or non-modal dialogs, and can support various types of
/// user interactions.</remarks>
public interface IDialog : IControl, IComponentEvent
{
    /// <summary>
    /// Event handler invoked when the dialog is about to close.
    /// </summary>
    /// <param name="sender">The source object initiating the closure.</param>
    /// <param name="e">Contains data about the form-closing event, including the ability to cancel closure.</param>
    void OnDialogClosing(object? sender, FormClosingEventArgs e);
}
