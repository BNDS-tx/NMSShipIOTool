namespace NMSArcTool;

using System;
using System.Collections.Generic;
using System.IO;

public class FixedBuffer : MemoryStream
{
    private static readonly byte[] CleanBytes = new byte[Constants.DecompressedChunkSize];

    private readonly MemoryStream _mainBuffer;
    private readonly bool _compress;
    private readonly Compressor _compressor;
    private int _remainingBytes;
    public readonly List<int> CompressedBlockSizes;

    public FixedBuffer(MemoryStream mainBuffer, bool compress = false, Compressor compressor = null)
    {
        _mainBuffer = mainBuffer;
        _compress = compress;
        _compressor = compressor;
        _remainingBytes = Constants.DecompressedChunkSize;
        CompressedBlockSizes = new List<int>();

        // 初始化清洁缓冲区
        Write(CleanBytes, 0, CleanBytes.Length);
        Position = 0;
    }

    public void AddBytes(byte[] data)
    {
        int dataSize = data.Length;
        int written = 0;

        // 写入第一部分数据
        int toWrite = Math.Min(_remainingBytes, dataSize);
        Write(data, 0, toWrite);
        written += toWrite;
        _remainingBytes -= toWrite;

        // 如果缓冲区已满，刷新到主缓冲区
        if (_remainingBytes == 0)
        {
            WriteToMainBuffer();
            _remainingBytes = Constants.DecompressedChunkSize;
        }

        // 处理剩余数据
        if (written < dataSize)
        {
            int remainingToWrite = dataSize - written;
            Write(data, written, remainingToWrite);
            _remainingBytes -= remainingToWrite;
        }
    }

    public void WriteToMainBuffer()
    {
        byte[] buffer = GetBuffer();

        if (_compress && _compressor != null)
        {
            byte[] compressedData = _compressor.Compress(buffer);
            int compressedSize = compressedData.Length;

            if (compressedSize >= Constants.DecompressedChunkSize)
            {
                // 如果压缩后更大，使用原始数据
                _mainBuffer.Write(buffer, 0, Constants.DecompressedChunkSize);
                CompressedBlockSizes.Add(Constants.DecompressedChunkSize);
            }
            else
            {
                // 写入压缩数据
                _mainBuffer.Write(compressedData, 0, compressedSize);

                // 添加填充以确保16字节对齐
                int paddingSize = CalculatePadding(compressedSize);
                for (int i = 0; i < paddingSize; i++)
                {
                    _mainBuffer.WriteByte(0);
                }

                CompressedBlockSizes.Add(compressedSize);
            }
        }
        else
        {
            _mainBuffer.Write(buffer, 0, (int)Length);
        }

        Clear();
    }

    public void Clear()
    {
        SetLength(0);
        Write(CleanBytes, 0, CleanBytes.Length);
        Position = 0;
    }

    private int CalculatePadding(int size)
    {
        return (16 - size % 16) % 16;
    }
}
