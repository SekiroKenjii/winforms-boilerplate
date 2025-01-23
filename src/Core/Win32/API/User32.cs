namespace Core.Win32.API;

public static partial class User32
{
    /// <summary>
    /// Sets the show state of a specified window with parameter validation.
    /// </summary>
    /// <param name="hWnd">A handle to the window. Must be greater than zero.</param>
    /// <param name="cmdShow">
    /// Controls how the window is to be shown. Must be a non-negative value representing
    /// one of the supported window show state commands.
    /// </param>
    /// <returns>
    /// If the window was previously visible and the parameters are valid, the return value is nonzero.
    /// If the window was previously hidden or if any parameter is invalid, the return value is zero.
    /// </returns>
    /// <remarks>
    /// This method performs validation on the input parameters:
    /// <list type="bullet">
    /// <item><description>Verifies that hWnd is a valid window handle (greater than 0)</description></item>
    /// <item><description>Ensures cmdShow is non-negative</description></item>
    /// </list>
    /// If validation fails, the method returns false without calling the underlying API.
    /// </remarks>
    public static bool SetWindowShowState(IntPtr hWnd, int cmdShow)
    {
        if (hWnd <= 0 || cmdShow < 0)
        {
            return false;
        }

        return ShowWindow(hWnd, cmdShow);
    }

    /// <summary>
    /// Releases the mouse capture from a window in the current thread and restores normal mouse input processing.
    /// </summary>
    /// <returns>
    /// True if the function succeeds, false if it fails.
    /// Call GetLastError to get extended error information on failure.
    /// </returns>
    /// <remarks>
    /// This is a direct wrapper around the ReleaseCapture Windows API function.
    /// A window that has captured the mouse receives all mouse input, regardless of the cursor position,
    /// except when a mouse button is clicked while the cursor is in another thread's window.
    /// </remarks>
    public static bool ReleaseMouseCapture()
    {
        return ReleaseCapture();
    }

    /// <summary>
    /// Sends a message to a specified window with parameter validation.
    /// </summary>
    /// <param name="hWnd">A handle to the target window. Must be greater than zero.</param>
    /// <param name="msg">The message to send. Must be greater than zero.</param>
    /// <param name="wParam">Additional message-specific information. Must be greater than zero.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>
    /// The result of the message processing if successful; zero if parameter validation fails.
    /// The specific meaning of the return value depends on the message sent.
    /// </returns>
    /// <remarks>
    /// This method performs validation on the input parameters:
    /// <list type="bullet">
    /// <item><description>Verifies that hWnd is a valid window handle (greater than 0)</description></item>
    /// <item><description>Ensures msg is a valid message ID (greater than 0)</description></item>
    /// <item><description>Validates that wParam is greater than 0</description></item>
    /// </list>
    /// If validation fails, the method returns 0 without calling the underlying API.
    /// The method blocks until the target window processes the message.
    /// </remarks>
    public static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam)
    {
        if (hWnd <= 0 || msg <= 0 || wParam <= 0)
        {
            return 0;
        }

        return SendMessageW(hWnd, msg, wParam, lParam);
    }
}
