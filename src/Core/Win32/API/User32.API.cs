using System.Runtime.InteropServices;

namespace Core.Win32.API;

/// <summary>
/// Provides access to <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser">Windows User32 API</see> functions for window and user interface operations.
/// </summary>
public static partial class User32
{
    /// <summary>Sets the specified window's show state.</summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="nCmdShow">
    /// Controls how the window is to be shown.
    /// This parameter is ignored the first time an application calls ShowWindow, if the program that launched the application provides a <see href="https://learn.microsoft.com/en-us/windows/desktop/api/processthreadsapi/ns-processthreadsapi-startupinfoa">STARTUPINFO</see> structure.
    /// Otherwise, the first time ShowWindow is called, the value should be the value obtained by the WinMain function in its nCmdShow parameter.
    /// In subsequent calls, this parameter can be one of the following values.
    /// </param>
    /// <returns>
    /// If the window was previously visible, the return value is nonzero.
    /// If the window was previously hidden, the return value is zero.
    /// </returns>
    [LibraryImport("user32.dll", EntryPoint = "ShowWindow")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);

    /// <summary>
    /// Releases the mouse capture from a window in the current thread and restores normal mouse input processing.
    /// A window that has captured the mouse receives all mouse input, regardless of the position of the cursor, except when a mouse button is clicked while the cursor hot spot is in the window of another thread.
    /// </summary>
    /// <returns>
    /// If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero. To get extended error information, call
    /// <see href="https://learn.microsoft.com/en-us/windows/desktop/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</see>.
    /// </returns>
    [LibraryImport("user32.dll", EntryPoint = "ReleaseCapture")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ReleaseCapture();

    /// <summary>
    /// Sends the specified message to a window or windows.
    /// The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose window procedure will receive the message.</param>
    /// <param name="msg">
    /// The message to be sent.
    /// For lists of the system-provided messages, see <see href="https://learn.microsoft.com/en-us/windows/desktop/winmsg/about-messages-and-message-queues">System-Defined Messages</see>
    /// </param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
    [LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
    private static partial IntPtr SendMessageW(IntPtr hWnd, int msg, int wParam, int lParam);
}
