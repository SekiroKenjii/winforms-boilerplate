
using Core.Enums.Win32;

namespace Core.Win32.Controls;

public class NcHitTestTransparentControl : NativeWindow
{
    public NcHitTestTransparentControl(Control ctrl)
    {
        if (ctrl.Handle != IntPtr.Zero)
        {
            AssignHandle(ctrl.Handle);
        }

        ctrl.HandleCreated += (_, _) => AssignHandle(ctrl.Handle);
        ctrl.HandleDestroyed += (_, _) => ReleaseHandle();
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == (int)Messages.WmNcHitTest)
        {
            m.Result = (int)Messages.HtTransparent;

            return;
        }

        base.WndProc(ref m);
    }
}
