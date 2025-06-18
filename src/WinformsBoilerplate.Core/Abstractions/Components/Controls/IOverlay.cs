using System.ComponentModel;

namespace WinformsBoilerplate.Core.Abstractions.Components.Controls;

/// <summary>
/// Defines the behavior and properties of an overlay control that can be displayed over a parent control.
/// </summary>
/// <remarks>
/// An overlay is a visual element that can be shown or hidden on top of a parent control, typically used
/// to display temporary information or provide a visual effect. The overlay supports customizable text and visibility
/// control.
/// </remarks>
public interface IOverlay : IControl
{
    /// <summary>
    /// Gets or sets a value indicating whether the overlay is currently visible.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    bool Visible { get; set; }

    /// <summary>
    /// Gets or sets the text to be displayed center-aligned on the overlay.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    string OverlayText { get; set; }

    /// <summary>
    /// Shows the overlay on the specified parent control.
    /// </summary>
    /// <param name="parentCtrl">The parent control to which the overlay will be applied.</param>
    void ShowOverlay(Control parentCtrl);

    /// <summary>
    /// Hides the overlay, optionally disposing of it.
    /// </summary>
    /// <param name="disposing">Indicates whether to dispose the overlay.</param>
    void HideOverlay(bool disposing = false);
}
