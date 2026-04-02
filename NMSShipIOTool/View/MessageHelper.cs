using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NMSShipIOTool.Resources;

namespace NMSShipIOTool.View;

public static class MessageHelper
{
    public static Microsoft.UI.Xaml.Window? Owner { get; set; }

    private static ContentDialog CreateBase(string title, string content)
    {
        var root = Owner?.Content as FrameworkElement;
        return new ContentDialog
        {
            Title = title,
            Content = new TextBlock
            {
                Text = content,
                TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
                IsTextSelectionEnabled = true,
            },
            XamlRoot = root?.XamlRoot,
        };
    }

    public static async Task ErrorAsync(string text)
    {
        var d = CreateBase(Language.错误, text);
        d.CloseButtonText = Language.确定;
        d.DefaultButton = ContentDialogButton.Close;
        await d.ShowAsync();
    }

    public static async Task InfoAsync(string text)
    {
        var d = CreateBase(Language.提示, text);
        d.CloseButtonText = Language.确定;
        d.DefaultButton = ContentDialogButton.Close;
        await d.ShowAsync();
    }

    /// <returns>Primary = OK, Close/Cancel = false</returns>
    public static async Task<bool> WarningOkCancelAsync(string text)
    {
        var d = CreateBase(Language.警告, text);
        d.PrimaryButtonText = Language.确定;
        d.CloseButtonText = Language.取消;
        d.DefaultButton = ContentDialogButton.Primary;
        var r = await d.ShowAsync();
        return r == ContentDialogResult.Primary;
    }
}
