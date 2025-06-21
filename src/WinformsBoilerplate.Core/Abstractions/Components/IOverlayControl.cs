using System.ComponentModel;

namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Defines methods and properties for managing an overlay control.
/// </summary>
/// <remarks>This interface provides functionality to display and hide overlay controls within a parent control.
/// Implementations of this interface should handle overlay-specific behavior, such as rendering and managing visibility
/// states.</remarks>
public interface IOverlayControl : IControl
{
    /// <summary>
    /// Gets or sets a value indicating whether the component is visible.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    bool Visible { get; set; }

    /// <summary>
    /// Gets or sets the text displayed as an overlay label.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    string OverlayLabelText { get; set; }

    /// <summary>
    /// Displays an overlay on the specified parent control.
    /// </summary>
    /// <remarks>The overlay is rendered as a child of the specified parent control. Ensure that the parent
    /// control  is visible and properly initialized before calling this method.</remarks>
    /// <param name="parentCtrl">The control on which the overlay will be displayed. Cannot be null.</param>
    void ShowOverlay(Control parentCtrl);

    /// <summary>
    /// Hides the overlay from the user interface.
    /// </summary>
    /// <param name="disposing">A value indicating whether the control should be disposed after being hidden.  <see langword="true"/> to dispose
    /// the control; otherwise, <see langword="false"/>.</param>
    void HideOverlay(bool disposing = false);
}
