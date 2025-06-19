using System.Runtime.InteropServices;

namespace WinformsBoilerplate.Core.Structs.Win32.Controls;

[StructLayout(LayoutKind.Sequential)]
public struct Margins
{
    public int CxLeftWidth;

    public int CxRightWidth;

    public int CyTopHeight;

    public int CyBottomHeight;
}
