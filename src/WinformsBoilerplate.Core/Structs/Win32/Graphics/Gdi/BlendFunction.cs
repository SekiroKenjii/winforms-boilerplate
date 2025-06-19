using System.Runtime.InteropServices;

namespace WinformsBoilerplate.Core.Structs.Win32.Graphics.Gdi;

[StructLayout(LayoutKind.Sequential)]
public struct BlendFunction
{
    public byte BlendOp;

    public byte BlendFlags;

    public byte SourceConstantAlpha;

    public byte AlphaFormat;
}
