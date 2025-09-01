namespace NMSArcTool.Model;

public class PackedFile
{
    public ulong Offset { get; set; }
    public ulong Size { get; set; }
    public string Path { get; set; }
    private Tuple<ulong, ulong> _inChunks;

    public PackedFile(ulong offset, ulong size, string path)
    {
        Offset = offset;
        Size = size;
        Path = path;
        _inChunks = null;
    }

    public Tuple<ulong, ulong> InChunks
    {
        get
        {
            if (_inChunks == null)
            {
                ulong startChunk,
                    endChunk;
                if (Offset % Constants.DecompressedChunkSize == 0)
                {
                    startChunk = Utils.DetermineBins(Offset, Constants.DecompressedChunkSize);
                }
                else
                {
                    startChunk = Utils.DetermineBins(Offset, Constants.DecompressedChunkSize) - 1;
                }
                endChunk = Utils.DetermineBins(Offset + Size, Constants.DecompressedChunkSize) - 1;
                _inChunks = new Tuple<ulong, ulong>(startChunk, endChunk);
            }
            return _inChunks;
        }
    }

    public ulong FirstChunkOffset => Offset % Constants.DecompressedChunkSize;

    public ulong LastChunkOffsetEnd => (Offset + Size) % Constants.DecompressedChunkSize;

    public override string ToString()
    {
        return $"File: {Path}: Offset: 0x{Offset:X}, Size: 0x{Size:X}, In chunks: {InChunks}";
    }
}
