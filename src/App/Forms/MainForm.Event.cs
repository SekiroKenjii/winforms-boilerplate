using Core.Abstractions.Events;

namespace App.Forms;

partial class MainForm : IMainFormEvent
{
    public Action<bool>? OnShutdownApplication { get; set; }

    public void InitializeFormEvents()
    {
        _eventStore?.Add(this, [new(nameof(OnShutdownApplication), ShutdownApplication)]);
    }
}
