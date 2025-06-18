using System.Runtime.InteropServices;
using WinformsBoilerplate.Core.Structs.Win32.Security.Credentials;

namespace WinformsBoilerplate.Core.Win32.API;

public static unsafe class CredUI
{
    public static bool CredUIPromptForWindowsCredentials(CredUIInfoW credInfo, out IntPtr credBuffer, out uint credSize)
    {
        IntPtr msgTextPtr = Marshal.StringToHGlobalUni(credInfo.PszMessageText);
        PCWSTR msgTextPwcStr = new((char*)msgTextPtr);

        IntPtr captionTextPtr = Marshal.StringToHGlobalUni(credInfo.PszCaptionText);
        PCWSTR captionTextPwcStr = new((char*)captionTextPtr);

        CREDUI_INFOW uiInfo = new() {
            cbSize = (uint)Marshal.SizeOf<CREDUI_INFOW>(),
            hwndParent = (HWND)credInfo.HwndParent,
            pszMessageText = msgTextPwcStr,
            pszCaptionText = captionTextPwcStr,
            hbmBanner = HBITMAP.Null
        };

        uint authPackage = 0;
        BOOL save = new(0);

        uint result = PInvoke.CredUIPromptForWindowsCredentials(
            pUiInfo: uiInfo,
            dwAuthError: 0,
            pulAuthPackage: ref authPackage,
            pvInAuthBuffer: (void*)IntPtr.Zero,
            ulInAuthBufferSize: 0,
            ppvOutAuthBuffer: out void* outCredBuffer,
            pulOutAuthBufferSize: out credSize,
            pfSave: &save,
            dwFlags: CREDUIWIN_FLAGS.CREDUIWIN_GENERIC
        );

        Marshal.FreeCoTaskMem(msgTextPtr);
        Marshal.FreeCoTaskMem(captionTextPtr);

        credBuffer = (IntPtr)outCredBuffer;

        return result == 0;
    }

    public static (string username, string domain, string password) CredUnPackAuthenticationBuffer(IntPtr credBuffer, uint credSize)
    {
        Span<char> usernameSpan = stackalloc char[100];
        Span<char> passwordSpan = stackalloc char[100];
        Span<char> domainSpan = stackalloc char[100];
        uint maxUserName = 100;
        uint maxDomain = 100;
        uint maxPassword = 100;

        _ = PInvoke.CredUnPackAuthenticationBuffer(
            dwFlags: 0,
            pAuthBuffer: (void*)credBuffer,
            cbAuthBuffer: credSize,
            pszUserName: usernameSpan,
            pcchMaxUserName: ref maxUserName,
            pszDomainName: domainSpan,
            pcchMaxDomainName: &maxDomain,
            pszPassword: passwordSpan,
            pcchMaxPassword: ref maxPassword
        );

        string fullUsername = new(usernameSpan);
        string domain = string.Empty;
        string username = fullUsername;

        if (fullUsername.Contains('\\'))
        {
            string[] parts = fullUsername.Split('\\');
            domain = parts[0];
            username = parts[1];
        }

        Marshal.ZeroFreeCoTaskMemUnicode(credBuffer);

        return (username, domain, new(passwordSpan));
    }
}
