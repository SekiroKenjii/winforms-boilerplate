namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Represents a UI dialog control, extending standard control functionality
/// with additional lifecycle handling for dialog-specific events.
/// </summary>
/// <remarks>
/// Implement this interface for form-based dialogs or pop-ups that require specific logic
/// when the user attempts to close the dialog (e.g., validation, confirmation, cleanup tasks).
/// </remarks>
public interface IDialog : IControl
{
    /// <summary>
    /// Event handler invoked when the dialog is about to close.
    /// </summary>
    /// <param name="sender">The source object initiating the closure.</param>
    /// <param name="e">Contains data about the form-closing event, including the ability to cancel closure.</param>
    void OnDialogClosing(object? sender, FormClosingEventArgs e);
}
