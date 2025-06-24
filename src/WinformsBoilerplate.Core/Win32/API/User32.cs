using Windows.Win32.UI.WindowsAndMessaging;
using WinformsBoilerplate.Core.Enums.Win32.Messages;
using WinformsBoilerplate.Core.Enums.Win32.UI.WindowsAndMessaging;
using WinformsBoilerplate.Core.Structs.Win32.Graphics.Gdi;

namespace WinformsBoilerplate.Core.Win32.API;

public static unsafe class User32
{
    public static void ShowWindow(IntPtr hWnd, ShowWindowCmd cmd)
    {
        _ = PInvoke.ShowWindow((HWND)hWnd, (SHOW_WINDOW_CMD)cmd);
    }

    public static void ReleaseCapture()
    {
        _ = PInvoke.ReleaseCapture();
    }

    public static void SendMessage(IntPtr hWnd, WindowMessage msg, nuint wParam, nint lParam)
    {
        _ = PInvoke.SendMessage((HWND)hWnd, (uint)msg, wParam, lParam);
    }

    public static IntPtr GetWindowDC(IntPtr hWnd)
    {
        HDC dc = PInvoke.GetWindowDC((HWND)hWnd);

        return (IntPtr)dc.Value;
    }

    public static void UpdateLayeredWindow(
        IntPtr hWnd,
        IntPtr destinationDc, Point destinationPt,
        Size sz,
        IntPtr sourceDc, Point sourcePt,
        uint clrRef,
        BlendFunction pBlend,
        UpdateLayeredWindowFlags updateFlg)
    {
        _ = PInvoke.UpdateLayeredWindow(
            hWnd: (HWND)hWnd,
            hdcDst: new(destinationDc),
            pptDst: destinationPt,
            psize: sz,
            hdcSrc: new(sourceDc),
            pptSrc: sourcePt,
            crKey: (COLORREF)clrRef,
            pblend: new() {
                BlendOp = pBlend.BlendOp,
                BlendFlags = pBlend.BlendFlags,
                SourceConstantAlpha = pBlend.SourceConstantAlpha,
                AlphaFormat = pBlend.AlphaFormat,
            },
            dwFlags: (UPDATE_LAYERED_WINDOW_FLAGS)updateFlg
        );
    }

    public static void ReleaseDC(IntPtr hWnd, IntPtr dc)
    {
        _ = PInvoke.ReleaseDC((HWND)hWnd, new(dc));
    }

    public static void UnhookWindowsHookEx(IntPtr hkHandle)
    {
        _ = PInvoke.UnhookWindowsHookEx((HHOOK)hkHandle);
    }

    public static void CallNextHookEx(IntPtr hkHandle, int code, nuint wParam, nint lParam)
    {
        _ = PInvoke.CallNextHookEx((HHOOK)hkHandle, code, wParam, lParam);
    }

    public static uint GetDoubleClickTime()
    {
        return PInvoke.GetDoubleClickTime();
    }

    public static void GetWindowRect(IntPtr hWnd, out Rectangle rect)
    {
        _ = PInvoke.GetWindowRect((HWND)hWnd, out RECT lpRect);

        rect = new() {
            Height = lpRect.Height,
            Width = lpRect.Width,
            Size = lpRect.Size,
            X = lpRect.X,
            Y = lpRect.Y
        };
    }

    public static int GetSystemMetrics(SystemMetricsIndex idx)
    {
        return PInvoke.GetSystemMetrics((SYSTEM_METRICS_INDEX)idx);
    }

    public static IntPtr GetDC(IntPtr hWnd)
    {
        HDC dc = PInvoke.GetDC((HWND)hWnd);

        return (IntPtr)dc.Value;
    }
}
