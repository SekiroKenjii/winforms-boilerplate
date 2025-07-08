namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Represents a user interface control that can host child controls and participate in layout and rendering logic.
/// </summary>
/// <remarks>
/// This interface extends <see cref="IControl"/> and <see cref="IContainerControl"/> to provide a unified
/// interface for user controls in a UI framework.
/// </remarks>
public interface IUserControl : IControl, IContainerControl, ITransientDependency { }
