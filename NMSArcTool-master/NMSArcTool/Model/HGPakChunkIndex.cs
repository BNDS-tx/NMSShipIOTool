namespace NMSArcTool.Model;

using System;
using System.IO;

public class HGPakChunkIndex
{
    public List<ulong> ChunkSizes;
    public List<ulong> ChunkOffset;

    public HGPakChunkIndex()
    {
        ChunkSizes = new List<ulong>();
        ChunkOffset = new List<ulong>();
    }

    public void Read(ulong chunkCount, BinaryReader reader)
    {
        ChunkSizes.Clear();

        for (ulong i = 0; i < chunkCount; i++)
        {
            var size = reader.ReadUInt64();
            ChunkSizes.Add(size);
        }
    }
}
