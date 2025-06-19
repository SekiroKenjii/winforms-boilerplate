namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Defines a reusable, self-contained UI component that can host child controls and
/// participate in layout and rendering logic within a form or container.
/// </summary>
/// <remarks>
/// Inherits from <see cref="IControl"/> for layout and rendering features,
/// and <see cref="IContainerControl"/> to support child control focus and containment.
/// </remarks>
public interface IUserControl : IControl, IContainerControl;
