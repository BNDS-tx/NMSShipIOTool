using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NMSShipIOTool.Resources;

namespace NMSShipIOTool.View;

internal enum FolderChoiceAction
{
    Cancel,
    UseDetected,
    PickManually,
}

internal static class ChoiceFolderDialog
{
    public static async Task<(FolderChoiceAction action, string? selectedPath)> ShowAsync(
        Microsoft.UI.Xaml.Window owner,
        string title,
        IReadOnlyList<string> folders)
    {
        var root = owner.Content as FrameworkElement;
        var panel = new StackPanel { Margin = new Thickness(0, 8, 0, 0), Spacing = 8 };

        for (var i = 0; i < folders.Count; i++)
        {
            panel.Children.Add(new RadioButton
            {
                Content = folders[i],
                Tag = folders[i],
                GroupName = "FolderChoice",
            });
        }

        var content = new StackPanel { Spacing = 8 };
        content.Children.Add(new TextBlock
        {
            Text = Language.已自动识别目录,
            TextWrapping = TextWrapping.Wrap,
        });
        content.Children.Add(new ScrollViewer
        {
            MaxHeight = 280,
            Content = panel,
        });

        var dialog = new ContentDialog
        {
            Title = title,
            Content = content,
            PrimaryButtonText = Language.使用选中路径,
            SecondaryButtonText = Language.手动选择,
            CloseButtonText = Language.取消,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = root?.XamlRoot,
        };

        dialog.PrimaryButtonClick += (_, args) =>
        {
            foreach (var child in panel.Children)
            {
                if (child is RadioButton { IsChecked: true })
                    return;
            }

            args.Cancel = true;
            var unused = MessageHelper.InfoAsync(Language.请先选择路径或手动选择);
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Secondary)
            return (FolderChoiceAction.PickManually, null);

        if (result != ContentDialogResult.Primary)
            return (FolderChoiceAction.Cancel, null);

        foreach (var child in panel.Children)
        {
            if (child is RadioButton { IsChecked: true, Tag: string path })
                return (FolderChoiceAction.UseDetected, path);
        }

        return (FolderChoiceAction.Cancel, null);
    }
}
