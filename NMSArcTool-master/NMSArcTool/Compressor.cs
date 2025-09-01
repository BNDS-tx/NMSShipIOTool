using LZ4;
using NMSArcTool.Model;
using ZstdSharp;
using static NMSArcTool.HGPak;

namespace NMSArcTool;

public class Compressor
{
    public Compressor(Platform platform)
    {
        _platform = platform;
        switch (platform)
        {
            case Platform.Windows:
            {
                _compressionContext = new ZstdCompressionContext();
                _decompressionContext = new ZstdDecompressionContext();
                break;
            }
        }
    }

    private Platform _platform;
    private ZstdCompressionContext _compressionContext;
    private ZstdDecompressionContext _decompressionContext;

    public byte[] Compress(byte[] buffer)
    {
        if (_platform == Platform.Windows)
        {
            return _compressionContext.Compress(buffer);
        }
        else if (_platform == Platform.Mac)
        {
            return LZ4Codec.Wrap(buffer);
        }
        else
        {
            throw new NotSupportedException("NotSupportedPlatform");
        }
    }

    public byte[] Decompress(byte[] data)
    {
        if (_platform == Platform.Windows)
        {
            try
            {
                return _decompressionContext.Decompress(data, Constants.DecompressedChunkSize);
            }
            catch (Exception e)
            {
                if (Utils.SafeCompare(data.Length, Constants.DecompressedChunkSize))
                {
                    return data;
                }
                else
                {
                    throw;
                }
            }
        }
        else if (_platform == Platform.Mac)
        {
            try
            {
                var decompressed = new byte[Constants.DecompressedChunkSize];

                var decodedLength = LZ4Codec.Decode(
                    data,
                    0,
                    data.Length,
                    decompressed,
                    0,
                    Constants.DecompressedChunkSize
                );

                return decompressed;
            }
            catch (Exception e)
            {
                if (Utils.SafeCompare(data.Length, Constants.DecompressedChunkSize))
                {
                    return data;
                }
                else
                {
                    throw;
                }
            }
        }
        else
        {
            throw new NotSupportedException("NotSupportedPlatform");
        }
    }
}
