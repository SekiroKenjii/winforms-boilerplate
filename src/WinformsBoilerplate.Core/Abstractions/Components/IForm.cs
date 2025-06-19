namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Represents a UI form, acting as a specialized control container for other controls.
/// Inherits standard UI management features from <see cref="IControl"/>.
/// </summary>
/// <remarks>
/// This interface serves primarily as a marker for distinguishing form-like controls
/// within the application's user interface structure. Additional form-specific functionality
/// or lifecycle events may be implemented in derived classes.
/// </remarks>
public interface IForm : IControl;
