namespace WinformsBoilerplate.Core.Abstractions.Services;

/// <summary>
/// Defines a contract for layout management within the application.
/// </summary>
public interface ILayoutService
{
    /// <summary>
    /// Determines whether the overlay associated with the specified control name is currently visible.
    /// </summary>
    /// <param name="ctrlName">The name of the control whose overlay visibility is being checked. Cannot be null or empty.</param>
    /// <returns><see langword="true"/> if the overlay for the specified control is visible; otherwise, <see langword="false"/>.</returns>
    bool IsOverlayVisible(string ctrlName);

    /// <summary>
    /// Displays an overlay with the specified text on top of the given control.
    /// </summary>
    /// <remarks>The overlay is positioned over the specified control and may obscure its content. Ensure
    /// that the control is visible and properly sized to accommodate the overlay.</remarks>
    /// <param name="ctrl">The control on which the overlay will be displayed. Cannot be <see langword="null"/>.</param>
    /// <param name="overlayText">The text to display in the overlay. Cannot be <see langword="null"/> or empty.</param>
    void ShowOverlay(Control ctrl, string overlayText);

    /// <summary>
    /// Hides the specified overlay control and optionally disposes of it.
    /// </summary>
    /// <param name="ctrl">The overlay control to hide. Cannot be <see langword="null"/>.</param>
    /// <param name="dispose">A value indicating whether the control should be disposed after being hidden.  <see langword="true"/> to dispose
    /// the control; otherwise, <see langword="false"/>.</param>
    void HideOverlay(Control ctrl, bool dispose = true);

    /// <summary>
    /// Toggles the visibility of an overlay on the specified control.
    /// </summary>
    /// <remarks>When <paramref name="state"/> is <see langword="true"/>, the overlay is displayed with the
    /// specified text. If <paramref name="state"/> is <see langword="false"/>, the overlay is hidden.</remarks>
    /// <param name="ctrl">The control on which the overlay will be toggled. Cannot be <see langword="null"/>.</param>
    /// <param name="overlayText">The text to display on the overlay. If <paramref name="state"/> is <see langword="true"/>, this text will be
    /// shown; otherwise, it is ignored.</param>
    /// <param name="state">A value indicating whether the overlay should be displayed. <see langword="true"/> to show the overlay;
    /// otherwise, <see langword="false"/>.</param>
    void ToggleOverlay(Control ctrl, string overlayText, bool state);

    /// <summary>
    /// Updates the text displayed on the specified overlay control.
    /// </summary>
    /// <remarks>This method modifies the visual appearance of the specified control by updating its overlay
    /// text. Ensure that the control supports overlay text functionality before calling this method.</remarks>
    /// <param name="ctrl">The control whose overlay text is to be updated. Cannot be <see langword="null"/>.</param>
    /// <param name="newText">The new text to display on the overlay. If <see langword="null"/> or empty, the overlay text will be cleared.</param>
    void UpdateOverlayText(Control ctrl, string newText);
}
