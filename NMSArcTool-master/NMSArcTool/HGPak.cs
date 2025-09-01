using System.IO;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using NMSArcTool.Model;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace NMSArcTool
{
    public class HGPak
    {
        private const string MAGIC = "HGPAK";

        internal Stream Stream;
        public HGPakHeader Header = new();
        internal HGPakFileIndex FileIndex = new();
        internal HGPakChunkIndex ChunkIndex = new();
        internal Dictionary<string, PackedFile> Files = [];
        public List<string> FileNames = [];
        internal Compressor Compressor;
        public uint TotalDecompressedSize => FileIndex.FinalOffset + FileIndex.FinalOffsetSize;

        public HGPak(Stream stream, Platform platform)
        {
            Stream = stream;
            Compressor = new Compressor(platform);
            Read();
        }

        private void Read()
        {
            Header.Read(new BinaryReader(Stream));
            FileIndex.Read(Header.FileCount, Stream);

            var foundChunkCount = Utils.DetermineBins(
                TotalDecompressedSize,
                Constants.DecompressedChunkSize
            );

            if (foundChunkCount != Header.ChunkCount)
            {
                Console.WriteLine(
                    $"chunk mismatch. Found: {foundChunkCount}, " + $"expected: {Header.ChunkCount}"
                );
            }

            switch (Header.IsCompressed)
            {
                case false:
                {
                    Stream.Seek((long)Header.DataOffset, SeekOrigin.Begin);
                    var fileNameData = new byte[FileIndex.FileInfo[0].DecompressedSize];
                    Stream.ReadExactly(fileNameData);

                    FileNames = Utils
                        .TrimEndString(Encoding.UTF8.GetString(fileNameData), "\r\n")
                        .Split("\r\n")
                        .ToList();
                    for (var i = 0; i < FileNames.Count; i++)
                    {
                        var fileName = FileNames[i];
                        if (string.IsNullOrEmpty(fileName))
                            continue;
                        var fileInfo = FileIndex.FileInfo[i + 1];
                        Files[fileName] = new PackedFile(
                            fileInfo.StartOffset,
                            fileInfo.DecompressedSize,
                            fileName
                        );
                    }
                    return;
                }
                case true:
                {
                    ChunkIndex.Read(Header.ChunkCount, new BinaryReader(Stream));
                    break;
                }
            }

            Stream.Seek((long)Header.DataOffset, SeekOrigin.Begin);

            var chunkCount = 0;
            foreach (var size in ChunkIndex.ChunkSizes)
            {
                ChunkIndex.ChunkOffset.Add((ulong)Stream.Position);

                if (!Utils.SafeCompare(chunkCount, Header.ChunkCount))
                {
                    Stream.Seek((long)Utils.ReqChunkBytes(size), SeekOrigin.Current);
                }
                chunkCount++;
            }

            var chunkForFileNames = Utils.DetermineBins(
                FileIndex.FileInfo[0].DecompressedSize,
                Constants.DecompressedChunkSize
            );

            var firstChunks = new List<byte>();
            for (ulong i = 0; i < chunkForFileNames; i++)
            {
                firstChunks.AddRange(DecompressChunk((int)i));
            }

            var dataBytes = firstChunks.ToArray();
            var decompressedSize = (int)FileIndex.FileInfo[0].DecompressedSize;
            var fileNamesBytes = new byte[decompressedSize];
            Array.Copy(dataBytes, 0, fileNamesBytes, 0, decompressedSize);

            var fileNamesStr = System
                .Text.Encoding.UTF8.GetString(fileNamesBytes)
                .TrimEnd(new char[] { '\r', '\n' });
            FileNames = fileNamesStr.Split(["\r\n"], StringSplitOptions.None).ToList();

            if (!Utils.SafeCompare(FileNames.Count, Header.FileCount - 1))
            {
                throw new InvalidOperationException("File count mismatch");
            }

            for (var i = 0; i < FileNames.Count; i++)
            {
                var fileName = FileNames[i];
                if (!string.IsNullOrEmpty(fileName))
                {
                    var fileInfo = FileIndex.FileInfo[i + 1];
                    Files[fileName] = new PackedFile(
                        fileInfo.StartOffset - Header.DataOffset,
                        fileInfo.DecompressedSize,
                        fileName
                    );
                }
            }
        }

        private byte[] DecompressChunk(int chunkIndex)
        {
            Stream.Seek((long)ChunkIndex.ChunkOffset[chunkIndex], SeekOrigin.Begin);
            var chunkSize = ChunkIndex.ChunkSizes[chunkIndex];
            var buffer = new byte[chunkSize];
            Stream.ReadExactly(buffer);
            return Compressor.Decompress(buffer);
        }

        public int Unpack(
            string outDir,
            List<string>? filters = null,
            List<string>? fileList = null
        )
        {
            var processedCount = 0;
            HashSet<string> filesToProcess;

            // 确定要处理的文件集合
            if (filters != null)
            {
                filesToProcess = new HashSet<string>();
                foreach (var filter in filters)
                {
                    var pattern =
                        "^"
                        + System
                            .Text.RegularExpressions.Regex.Escape(filter)
                            .Replace("\\*", ".*")
                            .Replace("\\?", ".")
                        + "$";
                    var matchedFiles = Files.Keys.Where(f =>
                        System.Text.RegularExpressions.Regex.IsMatch(f, pattern)
                    );
                    foreach (var file in matchedFiles)
                    {
                        filesToProcess.Add(file);
                    }
                }
            }
            else if (fileList != null)
            {
                filesToProcess = new HashSet<string>(Files.Keys.Intersect(fileList));
            }
            else
            {
                filesToProcess = new HashSet<string>(Files.Keys);
            }

            // 如果没有文件需要处理，直接返回
            if (filesToProcess.Count == 0)
            {
                return 0;
            }

            // 选择适当的提取方法
            Action<string, string> extractFunc = Header.IsCompressed
                ? ExtractFileCompressed
                : ExtractFileUncompressed;

            // 创建输出目录（如果不存在）
            Directory.CreateDirectory(outDir);

            // 处理每个文件
            foreach (var filePath in filesToProcess)
            {
                if (Files.TryGetValue(filePath, out var packedFile))
                {
                    extractFunc(filePath, outDir);
                    processedCount++;
                }
            }

            return processedCount;
        }

        private void ExtractFileCompressed(string filePath, string outDir)
        {
            if (!Files.TryGetValue(filePath, out var fileInfo))
            {
                throw new FileNotFoundException("指定的文件路径在此pak中不存在", filePath);
            }

            var (startChunk, endChunk) = fileInfo.InChunks;
            var firstOffset = fileInfo.FirstChunkOffset;
            var lastOffset = fileInfo.LastChunkOffsetEnd;

            // 使用MemoryStream作为数据缓冲区
            using var dataBuffer = new MemoryStream();

            if (startChunk == endChunk)
            {
                // 数据完全包含在同一个块中
                var decompressed = DecompressChunk((int)startChunk);
                if (decompressed.Length == 0)
                {
                    Console.WriteLine($"解压缩块 {startChunk} 时出现问题");
                    Console.WriteLine($"无法提取文件: {filePath}");
                    return;
                }

                if (lastOffset != 0)
                {
                    dataBuffer.Write(
                        decompressed,
                        (int)firstOffset,
                        (int)(lastOffset - firstOffset)
                    );
                }
                else
                {
                    dataBuffer.Write(
                        decompressed,
                        (int)firstOffset,
                        (decompressed.Length - (int)firstOffset)
                    );
                }
            }
            else
            {
                // 数据跨越多个块
                for (var chunkIdx = startChunk; chunkIdx <= endChunk; chunkIdx++)
                {
                    var decompressed = DecompressChunk((int)chunkIdx);
                    if (decompressed.Length == 0)
                    {
                        Console.WriteLine($"解压缩块 {chunkIdx} 时出现问题");
                        Console.WriteLine($"无法提取文件: {filePath}");
                        return;
                    }

                    if (chunkIdx == startChunk)
                    {
                        // 第一个块：从偏移量开始到结束
                        dataBuffer.Write(
                            decompressed,
                            (int)firstOffset,
                            decompressed.Length - (int)firstOffset
                        );
                    }
                    else if (chunkIdx == endChunk)
                    {
                        // 最后一个块：从开始到最后偏移量
                        if (lastOffset == 0)
                        {
                            dataBuffer.Write(decompressed, 0, decompressed.Length);
                        }
                        else
                        {
                            dataBuffer.Write(decompressed, 0, (int)lastOffset);
                        }
                    }
                    else
                    {
                        // 中间块：写入整个块
                        dataBuffer.Write(decompressed, 0, decompressed.Length);
                    }
                }
            }

            // 验证提取的数据大小
            if ((ulong)dataBuffer.Length != fileInfo.Size)
            {
                Console.WriteLine($"提取文件 {filePath} 时出现错误");
                Console.WriteLine($"文件信息详情: {fileInfo} {firstOffset} {lastOffset}");
                return;
            }

            // 创建目录并写入文件
            var exportPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var targetDir = Path.Combine(outDir, exportPath);

            if (!string.IsNullOrEmpty(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            var fullPath = Path.Combine(targetDir, fileName);
            using (var fileStream = File.Create(fullPath))
            {
                dataBuffer.Position = 0;
                dataBuffer.CopyTo(fileStream);
            }
        }

        private void ExtractFileUncompressed(string filePath, string outDir)
        {
            // 获取文件信息
            if (!Files.TryGetValue(filePath, out var fileInfo))
            {
                throw new FileNotFoundException("指定的文件路径在此pak中不存在", filePath);
            }

            // 定位到文件偏移位置
            Stream.Position = (long)fileInfo.Offset;

            // 获取导出路径和文件名
            var exportPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var targetDir = Path.Combine(outDir, exportPath);

            // 如果目标目录不为空，创建目录
            if (!string.IsNullOrEmpty(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // 如果不是演习运行，则写入文件
            var fullPath = Path.Combine(targetDir, fileName);
            using (var fileStream = File.Create(fullPath))
            {
                // 创建缓冲区
                var buffer = new byte[81920]; // 80KB buffer
                var remaining = (long)fileInfo.Size;

                while (remaining > 0)
                {
                    var toRead = (int)Math.Min(remaining, buffer.Length);
                    var bytesRead = Stream.Read(buffer, 0, toRead);

                    if (bytesRead == 0) // 到达流末尾
                    {
                        Console.WriteLine($"提前到达流末尾，文件可能已被截断: {filePath}");
                        break;
                    }

                    fileStream.Write(buffer, 0, bytesRead);
                    remaining -= bytesRead;
                }

                // 验证是否读取了正确的数据量
                if (remaining > 0)
                {
                    Console.WriteLine($"文件提取不完整: {filePath}. 还剩 {remaining} 字节未读取");
                }
            }
        }

        private IEnumerable<byte[]> ChunkedFileReader(IEnumerable<string> filePaths)
        {
            foreach (string filePath in filePaths)
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[Constants.DecompressedChunkSize];
                    int bytesRead;

                    while ((bytesRead = fs.Read(buffer, 0, Constants.DecompressedChunkSize)) > 0)
                    {
                        if (bytesRead < Constants.DecompressedChunkSize)
                        {
                            byte[] paddedBuffer = new byte[Constants.DecompressedChunkSize];
                            Array.Copy(buffer, 0, paddedBuffer, 0, bytesRead);
                            yield return paddedBuffer;
                        }
                        else
                        {
                            yield return buffer;
                            buffer = new byte[Constants.DecompressedChunkSize];
                        }
                    }
                }
            }
        }

        public MemoryStream Pack(
            IEnumerable<string> files,
            string rootDirectory,
            byte[] filenameHash,
            bool compress = false,
            Compressor compressor = null
        )
        {
            var buffer = new MemoryStream();

            // 写入初始header（部分数据待后续填充）
            buffer.Write(Encoding.ASCII.GetBytes(MAGIC), 0, 5);
            buffer.Write(new byte[3], 0, 3); // 3字节填充
            buffer.Write(BitConverter.GetBytes((ulong)2), 0, 8); // 版本
            buffer.Write(BitConverter.GetBytes((ulong)0), 0, 8); // 文件数（待更新）
            buffer.Write(BitConverter.GetBytes((ulong)0), 0, 8); // 块数（待更新）
            buffer.Write(BitConverter.GetBytes(compress), 0, 1); // 是否压缩
            buffer.Write(new byte[7], 0, 7); // 7字节填充
            buffer.Write(BitConverter.GetBytes((ulong)0), 0, 8); // 数据偏移（待更新）

            Console.WriteLine($"[2025-02-02 05:51:12] Started packing files");

            var hashes = new List<byte[]>();
            var fileSizes = new List<ulong>();
            var fileOffsets = new List<ulong>();
            var filePathData = new MemoryStream();
            var fullPaths = new List<string>();
            var relPaths = new List<byte[]>();

            // 收集文件信息
            foreach (var filePath in files)
            {
                if (Directory.Exists(filePath))
                {
                    foreach (
                        var file in Directory.GetFiles(filePath, "*.*", SearchOption.AllDirectories)
                    )
                    {
                        ProcessFile(file);
                    }
                }
                else
                {
                    ProcessFile(filePath);
                }
            }

            void ProcessFile(string filePath)
            {
                fullPaths.Add(filePath);
                fileSizes.Add((ulong)new FileInfo(filePath).Length);
                string relPath = Path.GetRelativePath(rootDirectory, filePath).Replace('\\', '/');
                byte[] relPathBytes = Encoding.UTF8.GetBytes(relPath);
                filePathData.Write(relPathBytes, 0, relPathBytes.Length);
                filePathData.Write(new byte[] { 0x0D, 0x0A }, 0, 2);
                relPaths.Add(relPathBytes);
                Console.WriteLine($"[2025-02-02 05:51:12] Processing: {relPath}");
            }

            // 计算文件名哈希
            foreach (var fname in relPaths)
            {
                using var md5 = MD5.Create();
                hashes.Add(md5.ComputeHash(fname));
            }

            // 计算偏移量和总大小
            ulong currentTotalData = Utils.RoundUp((ulong)filePathData.Length);
            foreach (var fileSize in fileSizes)
            {
                fileOffsets.Add(currentTotalData);
                currentTotalData += Utils.RoundUp(fileSize);
            }

            // 计算块数和数据偏移量
            ulong chunkCount = Utils.DetermineBins(
                currentTotalData,
                Constants.DecompressedChunkSize
            );
            ulong dataOffset =
                0x30 + (ulong)(0x20 * (fileSizes.Count + 1)) + (compress ? 0x8 * chunkCount : 0);
            ulong extraPadding = Utils.Padding(dataOffset);
            dataOffset += extraPadding;

            Console.WriteLine(
                $"[2025-02-02 05:51:12] Total chunks: {chunkCount}, Data offset: {dataOffset}"
            );

            // 写入文件索引数据
            buffer.Write(filenameHash, 0, 16);
            buffer.Write(BitConverter.GetBytes(dataOffset), 0, 8);
            buffer.Write(BitConverter.GetBytes((ulong)filePathData.Length), 0, 8);

            for (int i = 0; i < fileSizes.Count; i++)
            {
                buffer.Write(hashes[i], 0, 16);
                buffer.Write(BitConverter.GetBytes(fileOffsets[i] + dataOffset), 0, 8);
                buffer.Write(BitConverter.GetBytes(fileSizes[i]), 0, 8);
            }

            // 为压缩块大小预留空间
            long chunkIndexOffset = buffer.Position;
            if (compress)
            {
                buffer.Write(
                    new byte[8 * chunkCount + extraPadding],
                    0,
                    (int)(8 * chunkCount + extraPadding)
                );
            }

            // 更新头部信息
            buffer.Seek(0x10, SeekOrigin.Begin);
            buffer.Write(BitConverter.GetBytes((ulong)fileSizes.Count + 1), 0, 8);
            buffer.Write(BitConverter.GetBytes(chunkCount), 0, 8);
            buffer.Seek(0x28, SeekOrigin.Begin);
            buffer.Write(BitConverter.GetBytes(dataOffset), 0, 8);

            // 写入文件数据
            buffer.Seek(0, SeekOrigin.End);
            using (var subBuffer = new FixedBuffer(buffer, compress, compressor))
            {
                // 写入文件路径数据
                filePathData.Position = 0;
                byte[] pathBuffer = filePathData.ToArray();
                for (int i = 0; i < pathBuffer.Length; i += Constants.DecompressedChunkSize)
                {
                    int size = Math.Min(Constants.DecompressedChunkSize, pathBuffer.Length - i);
                    subBuffer.AddBytes(pathBuffer.Skip(i).Take(size).ToArray());
                }

                if (Utils.Padding((ulong)filePathData.Length) > 0)
                {
                    subBuffer.AddBytes(new byte[Utils.Padding((ulong)filePathData.Length)]);
                }

                // 使用ChunkedFileReader写入文件内容
                Console.WriteLine($"[2025-02-02 05:51:12] Writing file contents");
                foreach (var chunk in ChunkedFileReader(fullPaths))
                {
                    subBuffer.AddBytes(chunk);
                }

                subBuffer.WriteToMainBuffer();

                // 如果是压缩模式，写入压缩块大小信息
                if (compress)
                {
                    buffer.Seek(chunkIndexOffset, SeekOrigin.Begin);
                    foreach (var chunkSize in subBuffer.CompressedBlockSizes)
                    {
                        buffer.Write(BitConverter.GetBytes(chunkSize), 0, 8);
                    }
                }
            }

            Console.WriteLine($"[2025-02-02 05:51:12] Packing completed");
            return buffer;
        }
    }
}
