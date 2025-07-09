namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Represents a user interface form that serves as a container for controls and handles component events.
/// </summary>
/// <remarks>This interface defines the contract for a form in a UI framework, enabling the management of controls
/// and interaction with component events. Implementations of <see cref="IForm"/> typically provide functionality  for
/// rendering, layout management, and event handling.</remarks>
public interface IForm : IControl, IComponentEvent { }
