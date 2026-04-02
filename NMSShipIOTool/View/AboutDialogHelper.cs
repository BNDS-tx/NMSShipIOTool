using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using NMSShipIOTool.Resources;
using Windows.Storage.Streams;

namespace NMSShipIOTool.View;

public static class AboutDialogHelper
{
    public static async Task ShowAsync(Microsoft.UI.Xaml.Window owner)
    {
        var root = owner.Content as FrameworkElement;

        BitmapImage? bitmap = null;
        using var logoStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("NMSShipIOTool.Resources.logo.ico");
        if (logoStream is not null)
        {
            bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(logoStream.AsRandomAccessStream());
        }

        var stack = new StackPanel { MaxWidth = 560 };
        stack.Children.Add(new Image
        {
            Source = bitmap,
            Height = 80,
            Width = 80,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 12),
        });
        stack.Children.Add(new TextBlock
        {
            Text = Language.AboutDialog_title_Text,
            FontSize = 20,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
            TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
        });
        stack.Children.Add(new TextBlock
        {
            Text = Language.AboutDialog_info_Text,
            FontSize = 12,
            TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
        });

        var dialog = new ContentDialog
        {
            Title = Language.AboutDialog_this_Text,
            Content = new ScrollViewer { Content = stack, MaxHeight = 360 },
            CloseButtonText = Language.确定,
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = root?.XamlRoot,
        };
        await dialog.ShowAsync();
    }
}
