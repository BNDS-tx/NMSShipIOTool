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
        private String importPathString = "";
        private String exportPathString = "";
        private String exportNameString = "导出自定义飞船";
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
            updateTabEnabled(savePath, AllJTokens);
            if (savePath == "" || savePath == null) { selectFile(); }
        }

        private void updateTabEnabled(String filePath, IEnumerable<JToken> saveTokens)
        {
            if (filePath == "" || filePath == null)
            {
                tabControl1.TabPages[1].Enabled = false;
                tabControl1.TabPages[2].Enabled = false;
                buttonLoad.Enabled = false;
            }
            else
            {
                if (saveTokens != null)
                {
                    tabControl1.TabPages[1].Enabled = true;
                    tabControl1.TabPages[2].Enabled = true;
                }
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
                if (System.IO.Directory.Exists(nmsPath1)) { dialog.InitialDirectory = nmsPath1; }
                else if (System.IO.Directory.Exists(nmsPath2)) { dialog.InitialDirectory = nmsPath2; }
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // 用户选择了文件
                // 将 dialog.ToString 替换为 dialog.SelectedPath
                textBoxPath.Text = dialog.SelectedPath;
                savePath = dialog.SelectedPath;
                updateTabEnabled(savePath, AllJTokens);
                updateSaveDescription(savePath);
            }
        }

        private void updateSaveDescription(String filePath)
        {
            var platform = "";
            if (hasStLayer(filePath)) { platform = "Steam"; }
            else if (hasXboxLayer(filePath)) { platform = "Xbox"; }
            else { platform = "GOG 或其他平台"; }

            var lastFile = Directory.GetFiles(filePath).OrderByDescending(f => File.GetLastWriteTime(f)).First();
            DateTime fileLastWriteTime = File.GetLastWriteTime(lastFile);

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
            progressBar1.Visible = true;

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

            var choose = 0;
            if (saves == null || saves.Count() == 0)
            {
                MessageClass.ErrorMessageBox("没有找到有效的存档");
                updateTabEnabled(savePath, AllJTokens);
                progressBar1.Visible = false;
                return;
            }
            else if (saves.Count() == 1)
            {
                MessageClass.InfoMessageBox("仅找到一个存档，自动选择 " + saves[0].ToString());
            }
            else
            {
                using (var dialog = new ChoiceDialog("选择一个存档", saves))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        choose = dialog.SelectedOption;
                    }
                    else
                    {
                        MessageClass.InfoMessageBox("选择取消，默认 " + saves[choose].ToString());
                    }
                }
            }

            saveSlot = saveIndex[choose];
            platform.Load(saves[choose]); // needs to be loaded before you can modify its JSON
            var save = saves[choose].GetJsonObject();

            AllJTokens = save;

            IEnumerable<JToken> baseTokens = saves[choose].GetJsonTokens(baseJSONPath);
            BaseTokens = baseTokens;
            IEnumerable<JToken> shipOwnerTokens = saves[choose].GetJsonTokens(shipJSONPath);
            ShipOwnerTokens = shipOwnerTokens;

            List<int> shipTokenIndexes = new List<int>();
            var baseTokenIndexes = baseTokens.Children().ToList();
            foreach (JToken t in baseTokenIndexes)
            {
                if (t["peI"] != null && t["peI"]["DPp"] != null && t["peI"]["DPp"].ToString() == "PlayerShipBase")
                {
                    shipTokenIndexes.Add(baseTokenIndexes.IndexOf(t));
                }
            }
            ShipBaseTokens = shipTokenIndexes;

            updateTabEnabled(savePath, AllJTokens);
            progressBar1.Visible = false;

            if (ShipBaseTokens.Count() == 0)
            {
                MessageClass.InfoMessageBox("存档加载成功！当前存档没有自定义飞船，无法进行导入导出操作。");
            }
            else
            {
                MessageClass.InfoMessageBox("存档加载成功！当前存档有 " + ShipBaseTokens.Count() + " 个自定义飞船，可以进行导入导出操作。");

                var shipDected = "已识别自定义飞船：\n\n";
                List<string> shipOptons = new List<string>();
                foreach (int t in ShipBaseTokens)
                {
                    int shipID = int.Parse(BaseTokens.Children().ElementAt(t)["CVX"].ToString());
                    string shipName = ShipOwnerTokens.Children().ElementAt(shipID)["NKm"].ToString();
                    string option = "基地 ID：" + t.ToString() + "，飞船 ID：" + shipID + "，自定义飞船名：" + shipName;
                    shipOptons.Add(option);
                    shipDected = shipDected + option + "\n";
                }
                labelShipDetected.Text = shipDected;
                AddRadioButtons(shipOptons);
            }

            //test only
            //importShip(22, @"D:\导入测试，破坏性更改.json");
        }

        public void AddRadioButtons(List<string> options)
        {
            radioPanelI.Controls.Clear();
            for (int i = 0; i < options.Count; i++)
            {
                var radio = new RadioButton
                {
                    Text = options[i],
                    Name = "radio_" + i,
                    AutoSize = true,
                    Tag = i
                };
                radioPanelI.Controls.Add(radio);
            }

            radioPanelE.Controls.Clear();
            for (int i = 0; i < options.Count; i++)
            {
                var radio = new RadioButton
                {
                    Text = options[i],
                    Name = "radio_" + i,
                    AutoSize = true,
                    Tag = i
                };
                radioPanelE.Controls.Add(radio);
            }
        }

        public int GetSelectedRadioE()
        {
            foreach (Control ctrl in radioPanelE.Controls)
            {
                if (ctrl is RadioButton rb && rb.Checked)
                {
                    return (int)rb.Tag; // 或者返回索引、其他参数
                }
            }
            return 0;
        }

        public int GetSelectedRadioI()
        {
            foreach (Control ctrl in radioPanelI.Controls)
            {
                if (ctrl is RadioButton rb && rb.Checked)
                {
                    return (int)rb.Tag; // 或者返回索引、其他参数
                }
            }
            return 0;
        }

        void importShip(int index, String filePath)
        {
            if (platform == null) { MessageClass.ErrorMessageBox("请先加载存档"); return; }
            if (index < 0 || index >= BaseTokens.Children().Count() || !ShipBaseTokens.Contains(index))
            { MessageClass.ErrorMessageBox("无效的自定义飞船索引"); return; }
            if (filePath == "" || filePath == null) { MessageClass.ErrorMessageBox("导入文件无效"); return; }
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageClass.ErrorMessageBox("存档数据有误，请重新加载存档"); return; }
            if (ShipBaseTokens.Count() == 0) { MessageClass.ErrorMessageBox("当前存档没有自定义飞船，无法导入自定义飞船"); return; }

            var save = platform.GetSaveContainer(saveSlot);
            string jsonString = File.ReadAllText(filePath); // 读取文件内容
            var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
            if (newShip == null) { MessageClass.ErrorMessageBox("无效的自定义飞船数据"); return; }
            // 复制自定义飞船数据
            var shipCopy = checkBoxI.Checked ? Obfuscation.Obfuscate(newShip.DeepClone()) : newShip.DeepClone();
            // 获取自定义飞船
            string targetPath = $"{baseJSONPath}[{index}].@ZJ";
            // 替换自定义飞船的数据
            save.SetJsonValue(shipCopy, targetPath);
            // 保存修改后的存档
            platform.Write(save);
            MessageClass.InfoMessageBox("自定义飞船导入成功！请重新加载存档再查看导入数据或执行其他操作！");
        }

        void exportShip(int index, String filePath, String fileName)
        {
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageClass.ErrorMessageBox("存档数据有误，请重新加载存档"); return; }
            if (index < 0 || index >= BaseTokens.Children().Count() || !ShipBaseTokens.Contains(index))
            { MessageClass.ErrorMessageBox("无效的自定义飞船索引"); return; }
            if (filePath == "" || filePath == null) { MessageClass.ErrorMessageBox("请选择导出路径"); return; }
            if (ShipBaseTokens.Count() == 0) { MessageClass.ErrorMessageBox("当前存档没有自定义飞船，无法导出自定义飞船"); return; }
            var firstShipBaseIndex = index;
            var firstShipBase = BaseTokens.Children().ElementAt(firstShipBaseIndex);
            string jsonString;
            if (firstShipBase["@ZJ"] == null) { jsonString = ""; }
            else
            { jsonString = checkBoxE.Checked 
                    ? JsonConvert.SerializeObject(Obfuscation.Deobfuscate(firstShipBase["@ZJ"]), Formatting.Indented)
                    : JsonConvert.SerializeObject(firstShipBase["@ZJ"], Formatting.Indented); }
            var saveFilePath = System.IO.Path.Combine(filePath, fileName + ".json");
            File.WriteAllText(saveFilePath, jsonString);
            MessageClass.InfoMessageBox("自定义飞船已导出到：" + saveFilePath);
        }

        private void inputImportText_TextChanged(object sender, EventArgs e)
        {
            importPath.Text = "";
            importPathString = "";
        }

        private void impoerSelect_Click(object sender, EventArgs e)
        {
            inputImportText.Text = "";
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "JSON 文件 (*.json)|*.json";
                dialog.Title = "请选择文件";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = dialog.FileName;
                    // 这里可以处理选中的文件路径，例如显示到文本框
                    importPath.Text = selectedFile;
                    importPathString = selectedFile;
                }
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            progressBar3.Visible = true;
            var index = GetSelectedRadioE();
            if (exportName.Text != "") { exportNameString = exportName.Text; }
            if (ShipBaseTokens.Count < 1)
            {
                MessageClass.ErrorMessageBox("存档内没有自定义飞船，请更换存档或建立自定义飞船之后再尝试！");
                progressBar3.Visible = false;
                return;
            }
            exportShip(ShipBaseTokens[index], exportPathString, exportNameString);
            progressBar3.Visible = false;
        }

        private void exportSelect_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = dialog.SelectedPath;
                    // 这里可以处理选中的文件路径，例如显示到文本框
                    exportPath.Text = selectedFile;
                    exportPathString = selectedFile;
                }
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            progressBar2.Visible = true;

            var result = MessageClass.WarningMessageBox("导入自定义飞船会覆盖当前自定义飞船的数据，操作不可逆且可能损坏存档，建议先备份存档！\n\n确认继续吗？");
            if (result == DialogResult.Cancel)
            {
                progressBar2.Visible = false;
                return;
            }

            var index = GetSelectedRadioI();
            if (importPathString == "" && inputImportText.Text == "")
            {
                MessageClass.ErrorMessageBox("还未设置导入来源，请选择导入文件或者手动输入导入内容！");
                progressBar2.Visible = false;
                return;
            }
            else if (importPathString != "" && inputImportText.Text != "")
            {
                MessageClass.ErrorMessageBox("你设置了两个导入来源，请重新选择导入文件或者手动输入导入内容！");
                progressBar2.Visible = false;
                return;
            }
            if (ShipBaseTokens.Count < 1)
            {
                MessageClass.ErrorMessageBox("存档内没有自定义飞船，请更换存档或建立自定义飞船之后再尝试！");
                progressBar2.Visible = false;
                return;
            }
            if (importPathString != "")
            {
                importShip(ShipBaseTokens[index], importPathString);
            }
            else
            {
                // 获取多行文本内容
                string content = inputImportText.Text;

                // 包装为 JSON 对象：
                string formattedJson;
                try
                {
                    var token = JToken.Parse(content);
                    formattedJson = token.ToString(Formatting.Indented);
                }
                catch
                {
                    MessageClass.ErrorMessageBox("输入内容不是合法的 JSON 格式！");
                    progressBar2.Visible = false;
                    return;
                }

                // 写入文件
                string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp.json");
                File.WriteAllText(tempPath, formattedJson);
                importShip(ShipBaseTokens[index], tempPath);
                if (File.Exists(tempPath)) { File.Delete(tempPath); }
            }
            progressBar2.Visible = false;
        }

        private void checkBoxI_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxI.Checked)
            {
                var result = MessageClass.WarningMessageBox("你正在关闭导入的混淆功能。由于游戏存档经过混淆加密，在导入明文内容时需要经过混淆。如果你的设备能流畅地运行无人深空，那么混淆对你的设备性能的影响可以忽略不计。\n" + 
                    "即使你导入的内容未经反混淆仍然是密文，我们也推荐你保持混淆开启。混淆算法不会影响已经经过混淆的内容，开启混淆可以尽可能的降低意外导入明文且未经混淆而导致存档损坏的风险。\n" + 
                    "你确定你想要关闭混淆，直接导入 JSON 内容吗？若确定继续，请确保你导入的内容完全经过混淆。任何不当使用造成的个人损失与本工具无关，请在执行危险操作之前备份您的存档文件。");
                if (result == DialogResult.Cancel) { checkBoxI.Checked = true; }
                }
        }

        private void checkBoxE_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxE.Checked)
            {
                var result = MessageClass.WarningMessageBox("你正在关闭导出的反混淆功能。由于游戏存档经过混淆加密，在导出明文内容时需要经过反混淆。如果你的设备能流畅地运行无人深空，那么反混淆对你的设备性能的影响可以忽略不计。\n" +
                    "开启反混淆可以方便你将飞船的建模分享至其他平台，对其他三方工具的兼容性也更好。若关闭反混淆，导出的内容则为混淆密文，可能仅支持用于本工具之间的导入导出。请自行斟酌牺牲兼容性换来微乎其微的性能优化是否值得。\n" +
                    "你确定你想要关闭反混淆，直接导出混淆加密内容吗？若确定继续，请确保经你在分享至其他平台或使用其他三方工具前有能力自行对分享内容进过反混淆。任何不当使用造成的个人损失与本工具无关，请在执行危险操作之前备份您的存档文件。");
                if (result == DialogResult.Cancel) { checkBoxE.Checked = true; }
            }
        }
    }
}
