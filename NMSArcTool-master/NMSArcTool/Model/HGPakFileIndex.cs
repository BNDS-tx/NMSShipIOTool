namespace NMSArcTool.Model;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

public class HGPakFileIndex
{
    private List<HGPakFileInfo> fileInfo;
    private uint finalOffset;
    private uint finalOffsetSize;
    public List<HGPakFileInfo> FileInfo => fileInfo;
    public uint FinalOffset => finalOffset;
    public uint FinalOffsetSize => finalOffsetSize;
    public int Count => fileInfo.Count;

    public HGPakFileIndex()
    {
        fileInfo = new List<HGPakFileInfo>();
        finalOffset = 0;
        finalOffsetSize = 0;
    }

    public void Read(ulong fileCount, Stream s)
    {
        var structSize = Marshal.SizeOf<HGPakFileInfo>();
        var buffer = new byte[structSize];

        for (ulong i = 0; i < fileCount; i++)
        {
            if (s.Read(buffer, 0, structSize) != structSize)
            {
                throw new EndOfStreamException("Unexpected end of file");
            }

            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                var HGPakFileInfo = Marshal.PtrToStructure<HGPakFileInfo>(
                    handle.AddrOfPinnedObject()
                );

                var hgPakFileInfo = new HGPakFileInfo
                {
                    FileHash = HGPakFileInfo.FileHash,
                    StartOffset = HGPakFileInfo.StartOffset,
                    DecompressedSize = HGPakFileInfo.DecompressedSize
                };

                fileInfo.Add(hgPakFileInfo);

                if (hgPakFileInfo.StartOffset > finalOffset)
                {
                    finalOffset = (uint)hgPakFileInfo.StartOffset;
                    finalOffsetSize = (uint)hgPakFileInfo.DecompressedSize;
                }
            }
            finally
            {
                handle.Free();
            }
        }
    }

    public void Write(Stream s)
    {
        var structSize = Marshal.SizeOf<HGPakFileInfo>();
        var buffer = new byte[structSize];

        foreach (var hgPakFileInfo in fileInfo)
        {
            var pakFileInfo = new HGPakFileInfo
            {
                FileHash = hgPakFileInfo.FileHash,
                StartOffset = hgPakFileInfo.StartOffset,
                DecompressedSize = hgPakFileInfo.DecompressedSize
            };

            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(pakFileInfo, handle.AddrOfPinnedObject(), false);
                s.Write(buffer, 0, structSize);
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
