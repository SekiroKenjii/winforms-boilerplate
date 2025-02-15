using Core.Enums.Win32;
using Core.Win32.API;
using Core.Win32.Controls;
using Microsoft.Win32;
using System.ComponentModel;

namespace App.Shared.Forms;

/// <summary>
/// Represents a borderless form that provides custom window behaviors, such as resizing and moving, while supporting high DPI settings.
/// </summary>
/// <remarks>
/// This class extends the <see cref="HiDpiBaseForm"/> class to provide additional functionality for handling a borderless form.
/// It includes methods for maximizing, minimizing, and dragging the window, as well as making controls transparent to hit testing.
/// </remarks>
public partial class BorderlessBaseForm : HiDpiBaseForm
{
    protected const int GRIP_SIZE = 4;

    /// <summary>override this field with the height of the actual form's title bar</summary>
    protected int TopPanelHeight = 0;

    public BorderlessBaseForm()
    {
        InitializeComponent();

        if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
        {
            SetMaxSize();

            SystemEvents.DisplaySettingsChanged += (_, _) => SetMaxSize();
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Overriding the <c>CreateParams</c> property to add the <c>WS_MINIMIZEBOX</c> style is necessary to minimize
    /// and restore a borderless Form when you click its taskbar icon.
    /// </remarks>
    protected override CreateParams CreateParams
    {
        get {
            CreateParams cp = base.CreateParams;
            cp.Style |= 0x20000;

            return cp;
        }
    }

    /// <summary>
    /// Calling the <c>SetMaxSize</c> method to set the Form's MaximumSize is to not cover the taskbar area when you
    /// maximize the Form.
    /// </summary>
    private void SetMaxSize()
    {
        MaximizedBounds = Screen.FromHandle(Handle).WorkingArea;
    }

    /// <inheritdoc />
    /// <remarks>
    /// This method overrides the default window procedure to provide custom handling for specific messages, such as
    /// <c>WM_NCHITTEST</c> and <c>WM_MOUSEMOVE</c>, to enable resizing and moving of the borderless form. It also
    /// ensures that the form's behavior is adjusted when it is maximized.
    /// </remarks>
    protected override void WndProc(ref Message m)
    {
        if (WindowState == FormWindowState.Maximized)
        {
            base.WndProc(ref m);

            return;
        }

        if (m.Msg is (int)Messages.WmNcHitTest or (int)Messages.WmMouseMove)
        {
            Point clientPoint = PointToClient(new(m.LParam.ToInt32()));

            if (clientPoint.X >= ClientSize.Width - GRIP_SIZE && clientPoint.Y >= ClientSize.Height - GRIP_SIZE)
            {
                m.Result = (int)Messages.HtBottomRight;

                return;
            }

            switch (clientPoint.X)
            {
                case <= GRIP_SIZE when clientPoint.Y >= ClientSize.Height - GRIP_SIZE:
                    m.Result = (int)Messages.HtBottomLeft;
                    return;
                case <= GRIP_SIZE when clientPoint.Y <= GRIP_SIZE:
                    m.Result = (int)Messages.HtTopLeft;
                    return;
            }

            if (clientPoint.X >= ClientSize.Width - GRIP_SIZE && clientPoint.Y <= GRIP_SIZE)
            {
                m.Result = (int)Messages.HtTopRight;

                return;
            }

            if (clientPoint.Y <= GRIP_SIZE)
            {
                m.Result = (int)Messages.HtTop;

                return;
            }

            if (clientPoint.Y >= ClientSize.Height - GRIP_SIZE)
            {
                m.Result = (int)Messages.HtBottom;

                return;
            }

            if (clientPoint.X <= GRIP_SIZE)
            {
                m.Result = (int)Messages.HtLeft;

                return;
            }

            if (clientPoint.X >= ClientSize.Width - GRIP_SIZE)
            {
                m.Result = (int)Messages.HtRight;

                return;
            }

            var topRec = new Rectangle(GRIP_SIZE, GRIP_SIZE, Width - (GRIP_SIZE * 2), TopPanelHeight);

            if (topRec.Contains(clientPoint))
            {
                m.Result = (int)Messages.HtCaption;

                return;
            }
        }

        base.WndProc(ref m);
    }

    /// <inheritdoc />
    /// <remarks>
    /// This method overrides the default painting behavior to draw a size grip in the bottom-right corner of the form
    /// when it is not maximized. The size grip allows the user to resize the borderless form.
    /// </remarks>
    protected override void OnPaint(PaintEventArgs e)
    {
        if (WindowState != FormWindowState.Maximized)
        {
            ControlPaint.DrawSizeGrip(
                graphics: e.Graphics,
                backColor: BackColor,
                x: ClientSize.Width - GRIP_SIZE,
                y: ClientSize.Height - GRIP_SIZE,
                width: GRIP_SIZE,
                height: GRIP_SIZE
            );
        }

        base.OnPaint(e);
    }

    /// <summary>
    /// Makes the specified controls transparent to hit testing, allowing mouse events to pass through them.
    /// </summary>
    /// <param name="ctrls">A span of <see cref="Control"/> objects to make transparent to hit testing.</param>
    protected static void MakeCtrlsTransparent(params Span<Control> ctrls)
    {
        Array.ForEach(ctrls.ToArray(), ctrl => _ = new NcHitTestTransparentControl(ctrl));
    }

    /// <summary>
    /// Maximizes the window if it is not already maximized; otherwise, restores it to its previous size.
    /// </summary>
    protected virtual void MaximizeWindow()
    {
        if (WindowState == FormWindowState.Maximized)
        {
            _ = User32.SetWindowShowState(Handle, (int)WindowStates.Restore);

            return;
        }

        _ = User32.SetWindowShowState(Handle, (int)WindowStates.ShowMaximize);
    }

    /// <summary>
    /// Minimizes the window.
    /// </summary>
    protected virtual void MinimizeWindow()
    {
        _ = User32.SetWindowShowState(Handle, (int)WindowStates.Minimize);
    }

    /// <summary>
    /// Handles the mouse down event on the title bar to allow the user to drag the window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
    protected virtual void MouseDownTitleBar(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left || e.Clicks != 1)
        {
            return;
        }

        _ = User32.ReleaseMouseCapture();
        _ = User32.SendMessage(Handle, (int)Messages.NclButtonDown, (int)Messages.HtCaption, 0);
    }
}
