using System.Text;

namespace NMSArcTool.Model;

public class HGPakHeader
{
    public ulong Version { get; set; }
    public ulong FileCount { get; set; }
    public ulong ChunkCount { get; set; }
    public bool IsCompressed { get; set; }
    public ulong DataOffset { get; set; }

    public void Read(BinaryReader reader)
    {
        reader.BaseStream.Seek(0, SeekOrigin.Begin);

        byte[] magic = reader.ReadBytes(5);
        if (Encoding.ASCII.GetString(magic) != "HGPAK")
        {
            throw new InvalidOperationException(
                $"{reader.BaseStream} does not appear to be a valid HGPak file."
            );
        }

        reader.BaseStream.Seek(8, SeekOrigin.Begin);
        Version = reader.ReadUInt64();
        FileCount = reader.ReadUInt64();
        ChunkCount = reader.ReadUInt64();
        IsCompressed = reader.ReadBoolean();
        reader.BaseStream.Seek(7, SeekOrigin.Current); // Skip 7 bytes
        DataOffset = reader.ReadUInt64();
    }

    public override string ToString()
    {
        return $"HGPak Header:\n Version {Version}\n Files: {FileCount}\n Chunks: {ChunkCount}\n"
            + $" is Compressed: {IsCompressed}\n Data offset: 0x{DataOffset:X}\n";
    }
}
