using WinformsBoilerplate.Core.Enums.Win32.Messages;

namespace WinformsBoilerplate.Core.Win32.Components;

/// <summary>
/// A native window that intercepts the WM_NCHITTEST message to return a transparent hit test result.
/// This allows the window to be transparent to mouse events, effectively making it ignore mouse clicks.
/// </summary>
public class NcHitTestTransparentWindow : NativeWindow
{
    public NcHitTestTransparentWindow(Control ctrl)
    {
        if (ctrl.Handle != IntPtr.Zero)
        {
            AssignHandle(ctrl.Handle);
        }

        ctrl.HandleCreated += (_, _) => AssignHandle(ctrl.Handle);
        ctrl.HandleDestroyed += (_, _) => ReleaseHandle();
    }

    /// <inheritdoc />
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == (int)WindowMessage.NCHitTest)
        {
            m.Result = (int)HitTest.Transparent;

            return;
        }

        base.WndProc(ref m);
    }
}
