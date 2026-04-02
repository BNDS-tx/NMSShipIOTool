using libNOM.io.Enums;
using libNOM.io.Interfaces;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NMSShipIOTool.Resources;

namespace NMSShipIOTool.View;

public static class ChoiceSaveDialog
{
    public static async Task<(bool ok, int selectedIndex)> ShowAsync(
        Microsoft.UI.Xaml.Window owner,
        string title,
        IReadOnlyList<IContainer> saves)
    {
        var root = owner.Content as FrameworkElement;
        var panel = new StackPanel { Margin = new Thickness(0, 8, 0, 0) };
        var ordinal = 0;
        for (var i = 0; i < saves.Count; i++)
        {
            var save = saves[i];
            if (save.SaveType != SaveTypeEnum.Manual) continue;

            string content;
            try
            {
                var identifier = save.Identifier!;
                identifier = identifier.Substring(0, identifier.IndexOf("Manual", StringComparison.Ordinal));
                var type = save.ActiveContext.ToString() ?? "";
                var difficulty = save.Difficulty.ToString();
                var name = (save.SaveName == null || save.SaveName == "") ? Language.未命名 : save.SaveName;
                content = $"{Language.玩家存档} {identifier} {type} {difficulty} {name}";
            }
            catch
            {
                content = save.ToString();
            }

            var radio = new RadioButton
            {
                Content = content,
                Tag = ordinal,
                GroupName = "SaveChoice",
            };
            panel.Children.Add(radio);
            ordinal++;
        }

        var scroll = new ScrollViewer
        {
            MaxHeight = 280,
            Content = panel,
        };

        var dialog = new ContentDialog
        {
            Title = title ?? Language.请选择一个选项,
            Content = scroll,
            PrimaryButtonText = Language.确定,
            CloseButtonText = Language.取消,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = root?.XamlRoot,
        };

        dialog.PrimaryButtonClick += (_, args) =>
        {
            foreach (var child in panel.Children)
            {
                if (child is RadioButton { IsChecked: true })
                {
                    return;
                }
            }

            args.Cancel = true;
            var unused = MessageHelper.InfoAsync(Language.请先选择一个选项);
        };

        var result = await dialog.ShowAsync();
        if (result != ContentDialogResult.Primary)
            return (false, 0);

        foreach (var child in panel.Children)
        {
            if (child is RadioButton { IsChecked: true, Tag: int idx })
                return (true, idx);
        }

        return (false, 0);
    }
}
