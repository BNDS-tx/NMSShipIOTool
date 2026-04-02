using Microsoft.UI.Xaml;

namespace NMSShipIOTool;

public partial class App : Application
{
    public App()
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        StartupLog.LogBoot($"cwd(set in App ctor)={Environment.CurrentDirectory}");

        UnhandledException += (_, e) =>
        {
            try
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NMSShipIOTool");
                Directory.CreateDirectory(dir);
                File.AppendAllText(
                    Path.Combine(dir, "crash.log"),
                    $"{DateTimeOffset.Now:O}\n{e.Exception}\n\n");
            }
            catch
            {
                // ignore
            }
        };

        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            StartupLog.LogBoot("OnLaunched enter");
            m_window = new MainWindow();
            StartupLog.LogBoot("after new MainWindow()");
            m_window.Activate();
            StartupLog.LogBoot("after MainWindow.Activate()");
        }
        catch (Exception ex)
        {
            try
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NMSShipIOTool");
                Directory.CreateDirectory(dir);
                File.AppendAllText(Path.Combine(dir, "crash.log"), $"{DateTimeOffset.Now:O}\nOnLaunched: {ex}\n");
            }
            catch
            {
                // ignore
            }
        }
    }

    private Window? m_window;
}
