using System.Globalization;

namespace NMSShipIOTool
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;
            // 调试用
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}