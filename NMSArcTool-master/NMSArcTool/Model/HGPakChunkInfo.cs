namespace NMSArcTool.Model;

public class HGPakChunkInfo
{
    public ulong Size { get; set; }
    public ulong Offset { get; set; }

    public HGPakChunkInfo(ulong size, ulong offset)
    {
        Size = size;
        Offset = offset;
    }
}