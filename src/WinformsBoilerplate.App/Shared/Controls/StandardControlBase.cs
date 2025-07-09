using WinformsBoilerplate.Core.Abstractions.Components;

namespace WinformsBoilerplate.App.Shared.Controls;

public class StandardControlBase : Control, IControl
{
    public StandardControlBase()
    {
        Disposed += (_, _) => DisposeUnmanagedResources();
    }

    /// <inheritdoc cref="IComponent.DisposeUnmanagedResources" />
    public virtual void DisposeUnmanagedResources()
    {
        // Implement in derived class
    }
}
