using NMSShipIOTool.Resources;
using System.Text.Json.Nodes;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace NMSShipIOTool.Model;

internal static class FileOperations
{
    public static IReadOnlyList<string> GetDetectedSaveFolders()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var xboxDataPath = Path.Combine(localAppDataPath, "Packages", "HelloGames.NoMansSky_bs190hzg1sesy", "SystemAppData", "wgs");
        var otherDataPath = Path.Combine(appDataPath, "HelloGames", "NMS");

        var folderGroup = new List<string>();

        try
        {
            if (Directory.Exists(xboxDataPath))
                folderGroup.AddRange(Directory.GetDirectories(xboxDataPath));
        }
        catch
        {
            // ignore invalid or inaccessible locations
        }

        try
        {
            if (Directory.Exists(otherDataPath))
                folderGroup.AddRange(Directory.GetDirectories(otherDataPath));
        }
        catch
        {
            // ignore invalid or inaccessible locations
        }

        return folderGroup
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static async Task<string> SelectSaveFileAsync(Microsoft.UI.Xaml.Window window, string savePath)
    {
        var folderPicker = new FolderPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        folderPicker.FileTypeFilter.Add("*");
        InitializePickerWindow(folderPicker, window);

        var folder = await folderPicker.PickSingleFolderAsync();
        return folder?.Path ?? savePath;
    }

    public static async Task<string> FolderSelectAsync(Microsoft.UI.Xaml.Window window)
    {
        var folderPicker = new FolderPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        folderPicker.FileTypeFilter.Add("*");
        InitializePickerWindow(folderPicker, window);

        var folder = await folderPicker.PickSingleFolderAsync();
        return folder?.Path ?? "";
    }

    public static async Task<string> FileSelectAsync(Microsoft.UI.Xaml.Window window, int type)
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            ViewMode = PickerViewMode.List,
        };
        InitializePickerWindow(picker, window);

        switch (type)
        {
            case 1:
                AddFilters(picker, Language.文件筛选内容BS);
                break;
            case 2:
                AddFilters(picker, Language.文件筛选内容S);
                break;
            case 3:
                AddFilters(picker, Language.文件筛选内容T);
                break;
            default:
                AddFilters(picker, Language.文件筛选内容A);
                break;
        }

        var file = await picker.PickSingleFileAsync();
        return file?.Path ?? "";
    }

    private static void AddFilters(FileOpenPicker picker, string filterString)
    {
        // WinForms 格式: "描述|*.a;*.b|描述2|*.c"
        // WinRT FileTypeFilter 只接受 "*" 或 ".ext" 形式，".*" 是非法值会导致 picker 静默失败
        var segments = filterString.Split('|');
        for (var i = 1; i < segments.Length; i += 2)
        {
            var exts = segments[i].Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var ext in exts)
            {
                var e = ext.Trim();
                if (e.StartsWith('*'))
                    e = e[1..];
                // ".*" 来自 "*.*"（全文件通配符），"" 来自单独的 "*"，均跳过
                if (e == ".*" || e == "" || e == "*")
                    continue;
                if (!e.StartsWith('.'))
                    e = "." + e.TrimStart('.');
                if (e.Length > 1 && !picker.FileTypeFilter.Contains(e))
                    picker.FileTypeFilter.Add(e);
            }
        }
        if (picker.FileTypeFilter.Count == 0)
            picker.FileTypeFilter.Add("*");
    }

    private static void InitializePickerWindow(object picker, Microsoft.UI.Xaml.Window window)
    {
        var hwnd = WindowNative.GetWindowHandle(window);
        InitializeWithWindow.Initialize(picker, hwnd);
    }

    public static string setTempFile(string content)
    {
        if (content == null || content == "") throw new Exception(Language.无导入内容);

        string formattedJson;
        try
        {
            var jn = JsonNode.Parse(content);
            formattedJson = jn!.ToJsonString();
        }
        catch
        {
            throw new Exception(Language.非法JSON格式);
        }

        string tempPath = Path.Combine(AppContext.BaseDirectory, "temp.json");
        File.WriteAllText(tempPath, formattedJson);
        return tempPath;
    }
}
