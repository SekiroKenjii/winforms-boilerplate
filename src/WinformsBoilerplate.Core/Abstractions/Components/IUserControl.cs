namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Defines a reusable, self-contained UI component that can host child controls and
/// participate in layout and rendering logic within a form or container.
/// </summary>
/// <remarks>This interface combines the capabilities of <see cref="IControl"/>, <see cref="IContainerControl"/>, 
/// and <see cref="IComponentEvent"/> to provide a unified contract for user interface controls  that manage child
/// components, handle events, and interact with the control hierarchy.</remarks>
public interface IUserControl : IControl, IContainerControl, IComponentEvent;
