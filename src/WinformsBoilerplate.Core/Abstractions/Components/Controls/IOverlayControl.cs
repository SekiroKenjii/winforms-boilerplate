using System.ComponentModel;

namespace WinformsBoilerplate.Core.Abstractions.Components.Controls;

/// <summary>
/// Defines methods and properties for managing an overlay control.
/// </summary>
public interface IOverlayControl : IControl, ITransientDependency
{
    /// <summary>
    /// Gets or sets the text to be displayed center-aligned on the overlay.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    string OverlayText { get; set; }

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
