using System.ComponentModel;

namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Represents a UI control that manages visual rendering, layout, and basic properties.
/// Inherits basic unmanaged resource handling from <see cref="IComponent"/>.
/// </summary>
/// <remarks>
/// Use this interface to standardize common properties and methods across UI controls,
/// enabling consistent manipulation, rendering, and event management.
/// </remarks>
public interface IControl : IComponent
{
    /// <summary>
    /// Gets the collection of child controls contained within this control.
    /// </summary>
    Control.ControlCollection Controls { get; }

    /// <summary>
    /// Gets or sets the name used to identify this control within a collection or designer environment.
    /// </summary>
    /// <summary>
    /// Gets or sets the name used to identify this control within a collection or designer environment.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the internal spacing between the edges of the control and its content.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    Padding Padding { get; set; }

    /// <summary>
    /// Gets or sets the width of the control in pixels.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the control in pixels.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    int Height { get; set; }

    /// <summary>
    /// Gets or sets the text displayed on or associated with this control.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    string Text { get; set; }

    /// <summary>
    /// Renders the visual representation of the control onto the specified bitmap within the provided bounds.
    /// </summary>
    /// <param name="bitmap">The bitmap object to draw onto.</param>
    /// <param name="targetBounds">The bounding rectangle defining the area to draw.</param>
    void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds);

    /// <summary>
    /// Temporarily suspends the layout logic of the control for batch operations.
    /// </summary>
    void SuspendLayout();

    /// <summary>
    /// Resumes normal layout logic after suspension.
    /// </summary>
    /// <param name="performLayout">If true, forces immediate layout update.</param>
    void ResumeLayout(bool performLayout);

    /// <summary>
    /// Marks the control for repainting, causing it to be redrawn.
    /// </summary>
    void Invalidate();
}

