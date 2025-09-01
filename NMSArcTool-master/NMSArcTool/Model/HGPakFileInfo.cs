using System.Runtime.InteropServices;

namespace NMSArcTool.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct HGPakFileInfo
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] FileHash;
    public ulong StartOffset;
    public ulong DecompressedSize;

    public HGPakFileInfo(byte[] fileHash, ulong startOffset, ulong decompressedSize)
    {
        FileHash = fileHash;
        StartOffset = startOffset;
        DecompressedSize = decompressedSize;
    }
}
