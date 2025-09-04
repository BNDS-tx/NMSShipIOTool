using System;
using System.IO;
using System.IO.Compression;

public class ShipPackage
{
    /// <summary>
    /// 打包指定的 JSON 文件到 nmsship 文件
    /// </summary>
    /// <param name="jsonFiles">要打包的 json 文件路径数组，例如 new[] { "a.json", "b.json", "c.json" }</param>
    /// <param name="outputNmsship">输出的 nmsship 文件路径，例如 "shipdata.nmsship"</param>
    public static void Pack(string[] jsonFiles, string outputNmsship)
    {
        string tempZip = Path.GetTempFileName();

        // 创建 zip
        using (FileStream zipToOpen = new FileStream(tempZip, FileMode.Create))
        using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
        {
            foreach (string file in jsonFiles)
            {
                if (File.Exists(file))
                {
                    // 将文件添加到 zip 中，保持文件名不变
                    archive.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }
        }

        // 改名为 nmsship
        if (File.Exists(outputNmsship))
        {
            File.Delete(outputNmsship);
        }
        File.Move(tempZip, outputNmsship);
    }

    /// <summary>
    /// 解包 nmsship 文件到指定目录
    /// </summary>
    /// <param name="nmsshipFile">nmsship 文件路径</param>
    /// <param name="outputDir">解压目标目录</param>
    public static void Unpack(string nmsshipFile, string outputDir)
    {
        if (!File.Exists(nmsshipFile))
            throw new FileNotFoundException("nmsship 文件不存在", nmsshipFile);

        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        // 临时改名为 zip 再解压
        string tempZip = Path.ChangeExtension(Path.GetTempFileName(), ".zip");
        File.Copy(nmsshipFile, tempZip, true);

        foreach (string file in Directory.GetFiles(outputDir))
        {
            File.SetAttributes(file, FileAttributes.Normal); // 避免只读报错
            File.Delete(file);
        }

        ZipFile.ExtractToDirectory(tempZip, outputDir);

        File.Delete(tempZip);
    }
}
