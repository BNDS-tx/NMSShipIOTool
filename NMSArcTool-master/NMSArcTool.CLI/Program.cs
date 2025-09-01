using NMSArcTool.Model;

namespace NMSArcTool.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var fs = new FileStream(@"D:\nmsarctest\NMSARC.MetadataEtc.pak", FileMode.Open);
            var hgPak = new HGPak(fs, Platform.Windows);
            hgPak.Unpack(@"D:\nmsarctest\unpack");
        }
    }
}
