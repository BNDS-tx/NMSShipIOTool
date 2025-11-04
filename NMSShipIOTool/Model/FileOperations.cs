using NMSShipIOTool.Resources;
using System.Text.Json.Nodes;

namespace NMSShipIOTool.Model
{
    internal class FileOperations
    {
        public static string selectSaveFile(string savePath)
        {
            using var dialog = new FolderBrowserDialog();
            // 预设路径，例如 savePath
            if (!string.IsNullOrWhiteSpace(savePath) && System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(savePath)))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(savePath) ?? "";
            }
            else
            {
                // 获取 %AppData% 路径
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                // 获取 %LocalAppData% 路径
                var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                var nmsPath1 = System.IO.Path.Combine(appDataPath, "HelloGames", "NMS");
                var nmsPath2 = System.IO.Path.Combine(localAppDataPath, "Packages", "HelloGames.NoMansSky_bs190hzg1sesy", "SystemAppData", "wgs");
                if (System.IO.Directory.Exists(nmsPath1)) { dialog.InitialDirectory = nmsPath1; }
                else if (System.IO.Directory.Exists(nmsPath2)) { dialog.InitialDirectory = nmsPath2; }
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // 用户选择了文件
                // 将 dialog.ToString 替换为 dialog.SelectedPath
                savePath = dialog.SelectedPath;
            }
            return savePath;
        }

        // 选择文件夹
        public static string folderSelect()
        {
            var storageProvider = new FolderBrowserDialog();
            if (storageProvider.ShowDialog() == DialogResult.OK)
            {
                var folder = storageProvider.SelectedPath;
                return folder;
            }
            return "";
        }

        // 选择文件
        public static string fileSelect(int type)
        {
            var filter = "";
            switch (type)
            {
                case 1:
                    filter = Language.文件筛选内容BS;
                    break;
                case 2:
                    filter = Language.文件筛选内容S;
                    break;
                case 3:
                    filter = Language.文件筛选内容T;
                    break;
                default:
                    filter = Language.文件筛选内容A;
                    break;
            }
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = filter;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = dialog.FileName;
                    // 这里可以处理选中的文件路径，例如显示到文本框
                    return selectedFile;
                }
            }
            return "";
        }

        public static string setTempFile(string content)
        {
            if (content == null || content == "") throw new Exception(Language.无导入内容);

            // 包装为 JSON 对象：
            string formattedJson;
            try
            {
                var JN = JsonNode.Parse(content);
                formattedJson = JN!.ToJsonString();
            }
            catch
            {
                throw new Exception(Language.非法JSON格式);
            }

            // 写入文件
            string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp.json");
            File.WriteAllText(tempPath, formattedJson);
            return tempPath;
        }
    }
}
