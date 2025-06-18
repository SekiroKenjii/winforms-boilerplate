using System.Runtime.InteropServices;

namespace WinformsBoilerplate.Core.Structs.Win32.Security.Credentials;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct CredUIInfoW
{
    public uint CbSize;

    public IntPtr HwndParent;

    public string PszMessageText;

    public string PszCaptionText;

    public IntPtr HbmBanner;
}
