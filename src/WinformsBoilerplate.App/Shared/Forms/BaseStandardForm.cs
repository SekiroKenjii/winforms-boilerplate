using WinformsBoilerplate.Core.Abstractions.Components;

namespace WinformsBoilerplate.App.Shared.Forms;

/// <summary>
/// Represents a standard form that provides high DPI support for Windows Forms applications.
/// </summary>
/// <remarks>
/// This class extends the <see cref="BaseHiDpiForm"/> class to inherit functionality for adjusting controls and menu items
/// to high DPI settings. It serves as a base form for other forms that require high DPI support.
/// </remarks>
public partial class BaseStandardForm : BaseHiDpiForm, IForm
{
    public BaseStandardForm()
    {
        InitializeComponent();

        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        UpdateStyles();

        Disposed += (_, _) => DisposeUnmanagedResources();
    }

    /// <inheritdoc cref="IComponentEvent.InitializeFormEvents" />
    public virtual void InitializeFormEvents()
    {
        // Implement in derived class
    }

    /// <inheritdoc cref="IComponent.DisposeUnmanagedResources" />
    public virtual void DisposeUnmanagedResources()
    {
        //Implement in derived class
    }
}
