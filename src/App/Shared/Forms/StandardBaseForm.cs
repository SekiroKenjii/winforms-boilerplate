namespace App.Shared.Forms;

/// <summary>
/// Represents a standard form that provides high DPI support for Windows Forms applications.
/// </summary>
/// <remarks>
/// This class extends the <see cref="HiDpiBaseForm"/> class to inherit functionality for adjusting controls and menu items
/// to high DPI settings. It serves as a base form for other forms that require high DPI support.
/// </remarks>
public partial class StandardBaseForm : HiDpiBaseForm
{
    public StandardBaseForm()
    {
        InitializeComponent();
    }
}
