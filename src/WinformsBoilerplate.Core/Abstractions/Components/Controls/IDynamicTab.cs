using System.ComponentModel;
using System.Linq.Expressions;

namespace WinformsBoilerplate.Core.Abstractions.Components.Controls;

/// <summary>
/// Represents a dynamic tab control that allows adding, managing, and interacting with tab pages and their associated
/// controls.
/// </summary>
/// <remarks>
/// This interface extends <see cref="ITabControl"/> to provide additional functionality for dynamically
/// managing tab pages. It supports operations such as adding new pages, triggering global events on controls, and
/// counting tabs based on control types.
/// </remarks>
public interface IDynamicTab : ITabControl
{
    /// <summary>
    /// Gets or sets the color of the border surrounding the tab control.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    Color BorderColor { get; set; }

    /// <summary>
    /// Adds a new page to the tab control.
    /// </summary>
    /// <param name="control">The control to be added as a new tab page.</param>
    void AddPage(IControl control);

    /// <summary>
    /// Triggers a global event on all controls of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of control on which to trigger the event. Must implement <see cref="IControl"/>.</typeparam>
    /// <param name="expr">An expression that identifies the event to trigger.</param>
    /// <param name="args">Arguments to pass to the event handler.</param>
    void TriggerGlobalEvent<T>(Expression<Func<T, Delegate>> expr, params object[] args) where T : IControl;

    /// <summary>
    /// Counts the number of tabs that contain controls of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of control to count. Must implement <see cref="IControl"/>.</typeparam>
    /// <returns>The number of tabs containing controls of the specified type.</returns>
    int CountTabOf<T>() where T : IControl;
}
