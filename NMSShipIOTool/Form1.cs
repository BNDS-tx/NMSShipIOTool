namespace NMSShipIOTool
{
    using libNOM.collect;
    using libNOM.io;
    using libNOM.io.Enums;
    using libNOM.io.Interfaces;
    using libNOM.io.Settings;
    using libNOM.map;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.Json.Nodes;

    public partial class Form1 : Form
    {
        private String savePath = "";
        private String baseJSONPath = "$.vLc.6f=.F?0";
        private String shipJSONPath = "$.vLc.6f=.@Cs";
        private IPlatform platform = null;
        private int saveSlot = -1;
        private IEnumerable<JToken> AllJTokens = null;
        private IEnumerable<JToken> BaseTokens = null;
        private IEnumerable<JToken> ShipOwnerTokens = null;
        private List<int> ShipBaseTokens = new List<int>();
        public Form1()
        {
            InitializeComponent();
            updateTabEnabled(savePath);
            if (savePath == "" || savePath == null ) { selectFile(); }
        }

        private void updateTabEnabled(String filePath)
        {
            if (filePath == "" || filePath == null)
            {
                tabControl1.TabPages[1].Enabled = false;
                tabControl1.TabPages[2].Enabled = false;
                buttonLoad.Enabled = false;
            }
            else
            {
                tabControl1.TabPages[1].Enabled = true;
                tabControl1.TabPages[2].Enabled = true;
                buttonLoad.Enabled = true;
            }
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            // 在这里编写按钮点击后的逻辑
            selectFile();
        }

        private void selectFile()
        {
            using var dialog = new FolderBrowserDialog();
            // 预设路径，例如 savePath
            if (!string.IsNullOrWhiteSpace(savePath) && System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(savePath)))
            {
                dialog.InitialDirectory = System.IO.Path.GetDirectoryName(savePath);
            }
            else
            {
                // 获取 %AppData% 路径
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                // 获取 %LocalAppData% 路径
                var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                var nmsPath1 = System.IO.Path.Combine(appDataPath, "HelloGames", "NMS");
                var nmsPath2 = System.IO.Path.Combine(localAppDataPath, "Packages", "HelloGames.NoMansSky_bs190hzg1sesy", "SystemAppData", "wgs");
                if (System.IO.Directory.Exists(nmsPath1)){ dialog.InitialDirectory = nmsPath1; }
                else if (System.IO.Directory.Exists(nmsPath2)) { dialog.InitialDirectory = nmsPath2; }
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // 用户选择了文件
                // 将 dialog.ToString 替换为 dialog.SelectedPath
                textBoxPath.Text = dialog.SelectedPath;
                savePath = dialog.SelectedPath;
                updateTabEnabled(savePath);
                updateSaveDescription(savePath);
            }
        }

        private void updateSaveDescription(String filePath)
        {
            var platform = "";
            if (hasStLayer(filePath)) { platform = "Steam"; }
            else if (hasXboxLayer(filePath)) { platform = "Xbox"; }
            else { platform = "GOG 或其他平台"; }

            DateTime fileLastWriteTime = File.GetLastWriteTime(filePath);

            labelDescription.Text = "存档平台：" + platform + "\n最后保存时间：" + fileLastWriteTime;
        }

        bool hasStLayer(string filePath)
        {
            // 拆分路径为各层目录和文件名
            var parts = filePath.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            // 判断是否有目录名以 "st_" 开头
            return parts.Any(part => part.StartsWith("st_"));
        }

        bool hasXboxLayer(String filePath)
        {
            // 拆分路径为各层目录和文件名
            var parts = filePath.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            // 判断是否有目录名为 "HelloGames.NoMansSky_bs190hzg1sesy"
            return parts.Any(part => part.Equals("HelloGames.NoMansSky_bs190hzg1sesy"));
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            // 在这里编写按钮点击后的逻辑
            loadSave(savePath);
        }

        void loadSave(String filePath)
        {
            var path = filePath;
            var settings = new PlatformSettings { LoadingStrategy = LoadingStrategyEnum.Current };

            var collection = new PlatformCollection(); // detects all available PC platforms on a machine
            var platforms = collection.AnalyzePath(path, settings); // get platform in path and add to collection
            platform = platforms.First();

            var account = platform.GetAccountContainer(); // always loaded if exists
            List<IContainer> saves = new List<IContainer>();
            List<int> saveIndex = new List<int>();
            for (int i = 0; i < platform.GetSaveContainers().Count(); i = i + 1)
            {
                if (platform.GetSaveContainer(i).ActiveContext == SaveContextQueryEnum.Main)
                {
                    saves.Add(platform.GetSaveContainer(i));
                    saveIndex.Add(i);
                }
            }
            // Slot1Auto // loaded by default if LoadingStrategyEnum.Full

            String? sv = "";
            for (int i = 0; i < saves.Count(); i = i + 1)
            {
                sv = sv + saves[i].ToString() + "\n";
            }
            if (sv == null) { sv = "无信息"; } else { sv = "已读取的存档：\n" + sv; }
            MessageBox.Show(sv);

            saveSlot = saveIndex[0];
            platform.Load(saves[0]); // needs to be loaded before you can modify its JSON
            var save = saves[0].GetJsonObject();

            AllJTokens = save;

            IEnumerable<JToken> baseTokens = saves[0].GetJsonTokens(baseJSONPath);
            BaseTokens = baseTokens;
            IEnumerable<JToken> shipOwnerTokens = saves[0].GetJsonTokens(shipJSONPath);
            ShipOwnerTokens = shipOwnerTokens;

            List<int> shipTokenIndexes = new List<int>();
            var baseTokenIndexes = baseTokens.Children().ToList();
            foreach (JToken t in baseTokenIndexes)
            {
                if (t["peI"] != null && t["peI"]["DPp"] != null && t["peI"]["DPp"].ToString() == "UITempShipBase")
                {
                    shipTokenIndexes.Add(baseTokenIndexes.IndexOf(t));
                }
            } 
            ShipBaseTokens = shipTokenIndexes;

            //test only
            importShip(22, @"D:\导入测试，破坏性更改.json");
        }

        void importShip(int index, String filePath)
        {
            if (platform == null) { MessageBox.Show("请先加载存档"); return; }
            if (index < 0 || index >= BaseTokens.Children().Count() || !ShipBaseTokens.Contains(index))
                { MessageBox.Show("无效的飞船基座索引"); return; }
            if (filePath == "" || filePath == null) { MessageBox.Show("导入文件无效"); return; }
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageBox.Show("存档数据有误，请重新加载存档"); return; }
            if (ShipBaseTokens.Count() == 0) { MessageBox.Show("当前存档没有自定义飞船基座，无法导入飞船"); return; }
            
            var save = platform.GetSaveContainer(saveSlot);
            string jsonString = File.ReadAllText(filePath); // 读取文件内容
            var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
            if (newShip == null) { MessageBox.Show("无效的飞船数据"); return; }
            // 复制飞船数据
            var shipCopy = newShip.DeepClone();
            // 获取飞船基座
            string targetPath = $"{baseJSONPath}[{index}].@ZJ";
            // 替换飞船基座的数据
            save.SetJsonValue(shipCopy, targetPath);
            // 保存修改后的存档
            platform.Write(save);
            MessageBox.Show("飞船导入成功！");
        }

        void exportShip(int index, String filePath, String fileName)
        {
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageBox.Show("存档数据有误，请重新加载存档"); return; }
            if (index < 0 || index >= BaseTokens.Children().Count() || !ShipBaseTokens.Contains(index)) 
                { MessageBox.Show("无效的飞船基座索引"); return; }
            if (filePath == "" || filePath == null) { MessageBox.Show("请选择导出路径"); return; }
            if (ShipBaseTokens.Count() == 0) { MessageBox.Show("当前存档没有自定义飞船基座，无法导出飞船"); return; }
            var firstShipBaseIndex = index;
            var firstShipBase = BaseTokens.Children().ElementAt(firstShipBaseIndex);
            var jsonString = JsonConvert.SerializeObject(firstShipBase["@ZJ"], Formatting.Indented);
            var saveFilePath = System.IO.Path.Combine(filePath, fileName + ".json");
            File.WriteAllText(saveFilePath, jsonString);
            MessageBox.Show("飞船已导出到：" + saveFilePath);
        }
    }
}
