using System.Text;

namespace NMSShipIOTool;

/// <summary>启动诊断日志（由 AppHost.Main 与 App 写入）。</summary>
internal static class StartupLog
{
    private static readonly string BootLogPath = Path.Combine(Path.GetTempPath(), "nmsshipiotool_boot.log");

    internal static void LogBoot(string line) => Log(line);

    private static void Log(string line)
    {
        try
        {
            File.AppendAllText(BootLogPath, $"{DateTimeOffset.Now:O} {line}\n", Encoding.UTF8);
        }
        catch
        {
            // ignore
        }
    }
}
