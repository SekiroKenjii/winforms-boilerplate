using System.ComponentModel;

namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Defines an interface for a tabbed UI control, extending basic control functionality
/// with specific properties and behaviors to manage multiple tab pages.
/// </summary>
/// <remarks>
/// Implement this interface for controls handling multiple views or documents
/// within a single UI component, enabling intuitive navigation through tabbed content.
/// </remarks>
public interface ITabControl : IControl
{
    /// <summary>
    /// Gets the total number of tabs contained within the control.
    /// </summary>
    int TabCount { get; }

    /// <summary>
    /// Gets or sets the currently active (selected) tab page within the control.
    /// </summary>

    /// <summary>
    /// Gets or sets the currently active (selected) tab page within the control.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    TabPage? SelectedTab { get; set; }

    /// <summary>
    /// Gets the collection of tab pages associated with this tab control.
    /// </summary>
    TabControl.TabPageCollection TabPages { get; }
}
