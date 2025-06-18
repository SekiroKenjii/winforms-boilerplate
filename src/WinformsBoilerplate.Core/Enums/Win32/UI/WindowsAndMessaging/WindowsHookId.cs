namespace WinformsBoilerplate.Core.Enums.Win32.UI.WindowsAndMessaging;

public enum WindowsHookId
{
    CallWndProc = 4,
    CallWndProcRet = 12,
    CBT = 5,
    Debug = 9,
    ForegroundIdle = 11,
    GetMessage = 3,
    JournalPlayback = 1,
    JournalRecord = 0,
    Keyboard = 2,
    KeyboardLowLevel = 13,
    Mouse = 7,
    MouseLowLevel = 14,
    MessageFilter = -1,
    Shell = 10,
    SystemMessageFilter = 6,
}
