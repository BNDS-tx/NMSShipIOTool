namespace NMSShipIOTool
{
    using libNOM.io;
    using libNOM.io.Enums;
    using libNOM.io.Interfaces;
    using libNOM.io.Settings;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public partial class Form1 : Form
    {
        private String savePath = "";
        private String importPathString = "";
        private String exportPathString = "";
        private String defaultExportNameString = "导出飞船";
        private String baseJSONPath = "$.vLc.6f=.F?0";
        private String shipJSONPath = "$.vLc.6f=.@Cs";
        private String ccdJSONPath = "$.vLc.6f=.@C9";
        private IPlatform platform = null;
        private int saveSlot = -1;
        private IEnumerable<JToken> AllJTokens = null;
        private IEnumerable<JToken> BaseTokens = null;
        private IEnumerable<JToken> ShipOwnerTokens = null;
        private IEnumerable<JToken> CCBTokens = null;
        private List<int> ShipBaseTokens = new List<int>();
        public Form1()
        {
            InitializeComponent();
            updateTabEnabled(savePath, AllJTokens);
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
                updateSaveDescription(savePath, "待加载");
            }
        }

        private void updateSaveDescription(String filePath, string platform)
        {
            var lastFile = Directory.GetFiles(filePath).OrderByDescending(f => File.GetLastWriteTime(f)).First();
            DateTime fileLastWriteTime = File.GetLastWriteTime(lastFile);

            labelDescription.Text = "存档平台：" + platform + "\n最后保存时间：" + fileLastWriteTime;
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            // 在这里编写按钮点击后的逻辑
            loadSave(savePath);
        }

        async void loadSave(String filePath)
        {
            progressBar.Visible = true;

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
                progressBar.Visible = false;
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

            updateSaveDescription(savePath, platform.ToString());
            saveSlot = saveIndex[choose];
            showAllProgressBar();
            setAllButtonDisabled();
            await Task.Run(() =>
            {
                platform.Load(saves[choose]); // needs to be loaded before you can modify its JSON
            });
            setAllButtonEnabled();
            hideAllProgressBar();
            var save = saves[choose].GetJsonObject();

            AllJTokens = save;

            IEnumerable<JToken> baseTokens = saves[choose].GetJsonTokens(baseJSONPath);
            BaseTokens = baseTokens;
            IEnumerable<JToken> shipOwnerTokens = saves[choose].GetJsonTokens(shipJSONPath);
            ShipOwnerTokens = shipOwnerTokens;
            IEnumerable<JToken> ccbTokens = saves[choose].GetJsonTokens(ccdJSONPath);
            CCBTokens = ccbTokens;

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

            List<string> shipOptons = new List<string>();
            List<string> shipSeedOptions = new List<string>();

            if (ShipOwnerTokens.Children().Count() < 1)
            {
                MessageClass.InfoMessageBox("存档加载成功！当前存档没有任何飞船，无法进行飞船导入导出操作。");
            }
            else
            {
                MessageClass.InfoMessageBox("存档加载成功！当前存档有：\n" +
                    "所有飞船 " + ShipOwnerTokens.Children().Count() + " 艘，其中自定义飞船 " +
                    ShipBaseTokens.Count() + " 艘。");

                var shipBaseDetected = "自定义飞船：" + Environment.NewLine + Environment.NewLine;
                foreach (int t in ShipBaseTokens)
                {
                    int shipID = int.Parse(BaseTokens.Children().ElementAt(t)["CVX"].ToString());
                    string shipName = ShipOwnerTokens.Children().ElementAt(shipID)["NKm"].ToString();
                    string option = "飞船 ID：" + shipID + "，基地 ID：" + t.ToString() + "，飞船名：" + shipName;
                    shipOptons.Add(option);
                    shipBaseDetected = shipBaseDetected + option + Environment.NewLine;
                }

                var allShipDetected = "常规飞船：" + Environment.NewLine + Environment.NewLine;
                foreach (var t in ShipOwnerTokens.Children().ToList())
                {
                    string fileName = t["NTx"]["93M"].ToString();
                    if (fileName == "" || fileName == null) { continue; }
                    int shipID = ShipOwnerTokens.Children().ToList().IndexOf(t);
                    string shipName = t["NKm"].ToString();
                    string shipSeed = t["NTx"]["@EL"].Children().ElementAt(1).ToString();
                    string shipType = checkType(fileName);
                    if (shipType == "特殊船") { shipSeed += " 种子无效"; }
                    if (shipType == "自定义") { continue; }
                    string option = "飞船 ID：" + shipID + "，类型：" + shipType + "，飞船名：" + shipName + "，种子：" + shipSeed;
                    shipSeedOptions.Add(option);
                    allShipDetected = allShipDetected + option + Environment.NewLine;
                }

                labelShipDetected.Text = allShipDetected + Environment.NewLine + Environment.NewLine +
                    shipBaseDetected;

                AddRadioButtons(shipOptons, shipSeedOptions);
            }
        }

        private string checkType(string input)
        {
            var parts = input.Split('/');
            if (parts[parts.Count() - 1].Split("_")[0].Equals("FIGHTER")) { return "战斗"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("SENTINELSHIP")) { return "护卫"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("BIOSHIP")) { return "生物"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("SHUTTLE")) { return "飞艇"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("SAILSHIP")) { return "太阳帆"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("S-CLASS")) { return "异星"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("SCIENTIFIC")) { return "探险家"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("DROPSHIP")) { return "托运"; }
            else if (parts[parts.Count() - 1].Split(".")[0].Equals("BIGGS")) { return "自定义"; }
            else { return "特殊船"; }
        }

        private void setAllButtonDisabled()
        {
            buttonSelect.Enabled = false;
            buttonLoad.Enabled = false;
            buttonImport.Enabled = false;
            buttonExport.Enabled = false;
            buttonSeedShipExport.Enabled = false;
            buttonSeedShipImport.Enabled = false;
            buttonSetSeed.Enabled = false;
        }

        private void setAllButtonEnabled()
        {

            buttonSelect.Enabled = true;
            buttonLoad.Enabled = true;
            buttonImport.Enabled = true;
            buttonExport.Enabled = true;
            buttonSeedShipExport.Enabled = true;
            buttonSeedShipImport.Enabled = true;
            if (GetSelectedRadioSSeed() != null &&
                GetSelectedRadioSSeed().Contains("种子无效"))
                buttonSetSeed.Enabled = true;
        }

        private void showAllProgressBar()
        {
            progressBar.Visible = true;
        }

        private void hideAllProgressBar()
        {
            progressBar.Visible = false;
        }

        public void AddRadioButtons(List<string> options1, List<string> options2)
        {
            radioPanelI.Controls.Clear();
            for (int i = 0; i < options1.Count; i++)
            {
                var radio = new RadioButton
                {
                    Text = options1[i],
                    Name = "radioI_" + i,
                    AutoSize = true,
                    Tag = i
                };
                radioPanelI.Controls.Add(radio);
            }

            radioPanelS.Controls.Clear();
            for (int i = 0; i < options2.Count; i++)
            {
                var radio = new RadioButton
                {
                    Text = options2[i].Split("，种子：")[0],
                    Name = options2[i].Split("，种子：")[1],
                    AutoSize = true,
                    Tag = int.Parse(options2[i].Split("飞船 ID：")[1].Split("，类型：")[0])
                };
                radio.CheckedChanged += (s, e) =>
                {
                    if (((RadioButton)s).Checked)
                    {
                        string seed = ((RadioButton)s).Name;
                        if (seed.Contains("种子无效"))
                        { seed = ""; buttonSetSeed.Enabled = false; }
                        else { buttonSetSeed.Enabled = true; }
                        shipSeed.Text = seed;
                    }
                };
                radioPanelS.Controls.Add(radio);
            }
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

        public int? GetSelectedRadioSIndex()
        {
            foreach (Control ctrl in radioPanelS.Controls)
            {
                if (ctrl is RadioButton rb && rb.Checked)
                {
                    return (int)rb.Tag; // 或者返回索引、其他参数
                }
            }
            return null;
        }

        public String? GetSelectedRadioSSeed()
        {
            foreach (Control ctrl in radioPanelS.Controls)
            {
                if (ctrl is RadioButton rb && rb.Checked)
                {
                    return (string)rb.Name;
                }
            }
            return null;
        }

        public string? GetSelectedRadioSType()
        {
            foreach (Control ctrl in radioPanelS.Controls)
            {
                if (ctrl is RadioButton rb && rb.Checked)
                {
                    return (string)rb.Text.Split("类型：")[1].Split("，飞船名：")[0];
                }
            }
            return null;
        }

        public string? GetSelectedRadioSName()
        {
            foreach (Control ctrl in radioPanelS.Controls)
            {
                if (ctrl is RadioButton rb && rb.Checked)
                {
                    return (string)rb.Text.Split("飞船名：")[1];
                }
            }
            return null;
        }

        //
        // 数据读写
        //

        // 写入基地/货船基地/自定义飞船数据
        void writeBaseObject(string filePath, int index, IContainer save, bool isO)
        {
            string jsonString = File.ReadAllText(filePath); // 读取文件内容
            var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
            if (newShip == null || jsonString == "") { return; }
            // 复制自定义飞船数据
            var shipCopy = isO == true ? Obfuscation.Obfuscate(newShip.DeepClone()) : newShip.DeepClone();
            // 获取自定义飞船
            string targetPath = $"{baseJSONPath}[{index}].@ZJ";
            // 替换自定义飞船的数据
            save.SetJsonValue(shipCopy, targetPath);
        }

        // 写入拼接飞船自定义部件数据
        void writeCCD(string filePath, int index, IContainer save, bool isO)
        {
            string jsonString = File.ReadAllText(filePath); // 读取文件内容
            var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
            if (newShip == null || jsonString == "") { return; }
            // 复制飞船数据
            var shipCopy = isO == true ? Obfuscation.Obfuscate(newShip.DeepClone()) : newShip.DeepClone();
            // 获取路径
            int slot = -1;
            foreach (var item in Obfuscation.SlotTrack)
            { if (item.Key == index) { slot = item.Value; break; } }
            string targetPath = $"{ccdJSONPath}[{slot}]";
            // 替换飞船的数据
            save.SetJsonValue(shipCopy, targetPath);
        }

        // 写入飞船所有权数据
        void writeSO(string filePath, int index, IContainer save, bool isO)
        {
            string jsonString = File.ReadAllText(filePath); // 读取文件内容
            var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
            if (newShip == null || jsonString == "") { return; }
            // 复制飞船数据
            var shipCopy = isO == true ? Obfuscation.Obfuscate(newShip.DeepClone()) : newShip.DeepClone();
            // 获取飞船
            string targetPath = $"{shipJSONPath}[{index}]";
            // 替换飞船的数据
            save.SetJsonValue(shipCopy, targetPath);
        }

        // 读取基地/货船基地/自定义飞船数据
        string readBaseObject(int baseIndex, bool isO)
        {
            var baseToken = BaseTokens.Children().ElementAt(baseIndex);
            string jsonString;
            if (baseToken["@ZJ"] == null) { jsonString = ""; }
            else
            {
                jsonString = isO == true
                    ? JsonConvert.SerializeObject(Obfuscation.Deobfuscate(baseToken["@ZJ"]), Formatting.Indented)
                    : JsonConvert.SerializeObject(baseToken["@ZJ"], Formatting.Indented);
            }
            return jsonString;
        }

        // 读取拼接飞船自定义部件数据
        string readCCD(int shipIndex, bool isO)
        {
            int slot = -1;
            foreach (var item in Obfuscation.SlotTrack)
            { if (item.Key == shipIndex) { slot = item.Value; break; } }
            var ccdToken = CCBTokens.Children().ElementAt(slot);
            string jsonString;
            if (ccdToken == null) { jsonString = ""; }
            else
            {
                jsonString = isO == true
                    ? JsonConvert.SerializeObject(Obfuscation.Deobfuscate(ccdToken), Formatting.Indented)
                    : JsonConvert.SerializeObject(ccdToken, Formatting.Indented);
            }
            return jsonString;
        }

        // 读取飞船所有权数据
        string readSO(int shipIndex, bool isO)
        {
            var shipToken = ShipOwnerTokens.Children().ElementAt(shipIndex);
            string jsonString;
            if (shipToken == null) { jsonString = ""; }
            else
            {
                jsonString = isO == true
                    ? JsonConvert.SerializeObject(Obfuscation.Deobfuscate(shipToken), Formatting.Indented)
                    : JsonConvert.SerializeObject(shipToken, Formatting.Indented);
            }
            return jsonString;
        }

        // 解包并解析飞船完整包
        void unpackNWriteShip(string filePath, int baseIndex, int shipIndex, IContainer save, bool isO)
        {
            var tempDir = Path.Combine(System.IO.Path.GetTempPath(), "NMSMIOT_Temp");
            ShipPackage.Unpack(filePath, tempDir);
            var objectsPath = System.IO.Path.Combine(tempDir, "objects.json");
            var ccdPath = System.IO.Path.Combine(tempDir, "ccd.json");
            var soPath = System.IO.Path.Combine(tempDir, "so.json");
            writeBaseObject(objectsPath, baseIndex, save, isO);
            writeCCD(ccdPath, shipIndex, save, isO);
            writeSO(soPath, shipIndex, save, isO);
        }

        // 读取并打包飞船完整包
        string readNPackShip(int baseIndex, int shipIndex, bool isO, string filePath, string fileName)
        {
            string jsonString1 = baseIndex == -1 ? "" : readBaseObject(baseIndex, isO);
            string jsonString2 = readCCD(shipIndex, isO);
            string jsonString3 = readSO(shipIndex, isO);

            JToken thisShip = ShipOwnerTokens.Children().ElementAt(shipIndex);
            var saveFileName = fileName;
            if (saveFileName == "")
            {
                if (thisShip["NKm"].ToString() != "")
                    saveFileName = thisShip["NKm"].ToString();
                else saveFileName = defaultExportNameString;
            }
            var tempDir = Path.Combine(System.IO.Path.GetTempPath(), "NMSMIOT_Temp");
            if (Directory.Exists(tempDir)) { Directory.Delete(tempDir, true); }
            Directory.CreateDirectory(tempDir);
            var objectsPath = System.IO.Path.Combine(tempDir, "objects.json");
            var ccdPath = System.IO.Path.Combine(tempDir, "ccd.json");
            var soPath = System.IO.Path.Combine(tempDir, "so.json");
            File.WriteAllText(objectsPath, jsonString1);
            File.WriteAllText(ccdPath, jsonString2);
            File.WriteAllText(soPath, jsonString3);
            ShipPackage.Pack(new[] { objectsPath, ccdPath, soPath }, System.IO.Path.Combine(filePath, saveFileName + ".nmsship"));
            return System.IO.Path.Combine(filePath, saveFileName + ".nmsship");
        }

        async void importShip(int index, String filePath)
        {
            if (platform == null) { MessageClass.ErrorMessageBox("请先加载存档"); return; }
            if (index < 0 || index >= BaseTokens.Children().Count() || !ShipBaseTokens.Contains(index))
            { MessageClass.ErrorMessageBox("无效的自定义飞船索引"); return; }
            if (filePath == "" || filePath == null) { MessageClass.ErrorMessageBox("导入文件无效"); return; }
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageClass.ErrorMessageBox("存档数据有误，请重新加载存档"); return; }
            if (ShipBaseTokens.Count() == 0) { MessageClass.ErrorMessageBox("当前存档没有自定义飞船，无法导入自定义飞船"); return; }

            var save = platform.GetSaveContainer(saveSlot);
            if (filePath.Split(".").Last() == "json")
            {
                string jsonString = File.ReadAllText(filePath); // 读取文件内容
                var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
                if (newShip == null) { MessageClass.ErrorMessageBox("无效的自定义飞船数据"); return; }
                // 复制自定义飞船数据
                var shipCopy = checkBoxI.Checked ? Obfuscation.Obfuscate(newShip.DeepClone()) : newShip.DeepClone();
                // 获取自定义飞船
                string targetPath = $"{baseJSONPath}[{index}].@ZJ";
                // 替换自定义飞船的数据
                save.SetJsonValue(shipCopy, targetPath);
            } else if (filePath.Split(".").Last() == "nmsship")
            {
                var firstShipBaseIndex = index;
                var firstShipBase = BaseTokens.Children().ElementAt(firstShipBaseIndex);
                if (firstShipBase["peI"] == null || firstShipBase["peI"]["DPp"] == null ||
                    firstShipBase["peI"]["DPp"].ToString() != "PlayerShipBase")
                { MessageClass.ErrorMessageBox("所选索引处不是自定义飞船，无法导入自定义飞船"); return; }
                int shipID = int.Parse(firstShipBase["CVX"].ToString());
                unpackNWriteShip(filePath, firstShipBaseIndex, shipID, save, checkBoxI.Checked);
            }
            else
            {
                MessageClass.ErrorMessageBox("无效的导入文件，仅支持 .json 和 .nmsship 格式");
                return;
            }
            // 保存修改后的存档
            showAllProgressBar();
            setAllButtonDisabled();
            await Task.Run(() =>
            {
                platform.Write(save);
            });
            setAllButtonEnabled();
            hideAllProgressBar();
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
            if (checkBoxNMSSHIP1.Checked)
            {
                var firstShipBase = BaseTokens.Children().ElementAt(firstShipBaseIndex);
                if (firstShipBase["peI"] == null || firstShipBase["peI"]["DPp"] == null ||
                    firstShipBase["peI"]["DPp"].ToString() != "PlayerShipBase")
                { MessageClass.ErrorMessageBox("所选索引处不是自定义飞船，无法导出自定义飞船"); return; }
                int shipID = int.Parse(firstShipBase["CVX"].ToString());
                var saveFilePath = readNPackShip(firstShipBaseIndex, shipID, checkBoxI.Checked, filePath, fileName);
                MessageClass.InfoMessageBox("自定义飞船已导出到：" + saveFilePath);
                return;
            }
            else
            {
                var firstShipBase = BaseTokens.Children().ElementAt(firstShipBaseIndex);
                string jsonString;
                if (firstShipBase["@ZJ"] == null) { jsonString = ""; }
                else
                {
                    jsonString = checkBoxI.Checked
                        ? JsonConvert.SerializeObject(Obfuscation.Deobfuscate(firstShipBase["@ZJ"]), Formatting.Indented)
                        : JsonConvert.SerializeObject(firstShipBase["@ZJ"], Formatting.Indented);
                }
                var saveFilePath = System.IO.Path.Combine(filePath, fileName + ".json");
                File.WriteAllText(saveFilePath, jsonString);
                MessageClass.InfoMessageBox("自定义飞船已导出到：" + saveFilePath);
            }
        }

        async void setSeed(int index, String seed)
        {
            if (platform == null) { MessageClass.ErrorMessageBox("请先加载存档"); return; }
            if (index < 0 || index >= ShipOwnerTokens.Children().Count())
            { MessageClass.ErrorMessageBox("无效的飞船索引"); return; }
            if (seed == "" || seed == null) { MessageClass.ErrorMessageBox("种子无效"); return; }
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageClass.ErrorMessageBox("存档数据有误，请重新加载存档"); return; }
            if (ShipOwnerTokens.Count() == 0) { MessageClass.ErrorMessageBox("当前存档没有飞船，无法导入种子"); return; }
            var save = platform.GetSaveContainer(saveSlot);
            string targetPath = $"{shipJSONPath}[{index}].NTx.@EL[1]";
            save.SetJsonValue(seed, targetPath);
            showAllProgressBar();
            setAllButtonDisabled();
            await Task.Run(() =>
            {
                platform.Write(save);
            });
            setAllButtonEnabled();
            hideAllProgressBar();
            MessageClass.InfoMessageBox("飞船种子导入成功！请重新加载存档再查看导入数据或执行其他操作！");
        }

        async void importSeedShip(int index, String filePath)
        {
            if (platform == null) { MessageClass.ErrorMessageBox("请先加载存档"); return; }
            if (index < -1 || index >= ShipOwnerTokens.Children().Count())
            { MessageClass.ErrorMessageBox("无效的飞船索引"); return; }
            if (index == -1)
            {
                for (int i = 0; i < ShipOwnerTokens.Children().Count(); i++)
                {
                    var fileName = ShipOwnerTokens.Children().ElementAt(i)["NTx"]["93M"].ToString();
                    if (fileName == null || fileName == "")
                    { index = i; break; }
                    else if (i == ShipOwnerTokens.Children().Count() - 1)
                    {
                        MessageClass.ErrorMessageBox("当前存档没有空闲飞船位，无法导入新飞船");
                        return;
                    }
                }
            }
            if (filePath == "" || filePath == null) { MessageClass.ErrorMessageBox("导入文件无效"); return; }
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageClass.ErrorMessageBox("存档数据有误，请重新加载存档"); return; }
            if (ShipOwnerTokens.Count() == 0) { MessageClass.ErrorMessageBox("当前存档没有飞船，无法导入飞船"); return; }

            var save = platform.GetSaveContainer(saveSlot);
            if (filePath.Split(".").Last() == "json")
            {
                string jsonString = File.ReadAllText(filePath); // 读取文件内容
                var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
                if (newShip == null) { MessageClass.ErrorMessageBox("无效的飞船数据"); return; }
                // 复制飞船数据
                var shipCopy = checkBoxS.Checked ? Obfuscation.Obfuscate(newShip.DeepClone()) : newShip.DeepClone();
                // 获取飞船
                if (shipCopy == null)
                { MessageClass.ErrorMessageBox("无效的飞船数据"); return; }
                string targetPath = $"{shipJSONPath}[{index}]";
                // 替换飞船的数据
                if (!string.IsNullOrEmpty(targetPath))
                { save.SetJsonValue(shipCopy, targetPath); }
                else
                { MessageClass.ErrorMessageBox("无效的飞船数据路径"); return; }
                // 保存修改后的存档
                showAllProgressBar();
                setAllButtonDisabled();
                await Task.Run(() =>
                {
                    platform.Write(save);
                });
                setAllButtonEnabled();
                hideAllProgressBar();
                MessageClass.InfoMessageBox("飞船导入成功！请重新加载存档再查看导入数据或执行其他操作！");
            }
            else if (filePath.Split(".").Last() == "nmsship")
            {
                var firstShipIndex = index;
                unpackNWriteShip(filePath, -1, firstShipIndex, save, checkBoxS.Checked);
                // 保存修改后的存档
                showAllProgressBar();
                setAllButtonDisabled();
                await Task.Run(() =>
                {
                    platform.Write(save);
                });
                setAllButtonEnabled();
                hideAllProgressBar();
                MessageClass.InfoMessageBox("飞船导入成功！请重新加载存档再查看导入数据或执行其他操作！");
            }
            else
            {
                MessageClass.ErrorMessageBox("无效的导入文件，仅支持 .json 和 .nmsship 格式");
                return;
            }
        }
        void exportSeedShip(int index, String filePath, String fileName)
        {
            if (ShipOwnerTokens == null) { MessageClass.ErrorMessageBox("存档数据有误，请重新加载存档"); return; }
            if (index < 0 || index >= ShipOwnerTokens.Children().Count())
            { MessageClass.ErrorMessageBox("无效的飞船索引"); return; }
            if (filePath == "" || filePath == null) { MessageClass.ErrorMessageBox("请选择导出路径"); return; }
            if (ShipOwnerTokens.Count() == 0) { MessageClass.ErrorMessageBox("当前存档没有飞船，无法导出飞船"); return; }
            
            var firstShipIndex = index;
            if (checkBoxNMSSHIP3.Checked)
            {
                var saveFilePath = readNPackShip(-1, firstShipIndex, checkBoxS.Checked, filePath, fileName);
                MessageClass.InfoMessageBox("飞船已导出到：" + saveFilePath);
                return;
            } else {
                var firstShip = ShipOwnerTokens.Children().ElementAt(firstShipIndex);
                string jsonString;
                if (firstShip == null) { jsonString = ""; }
                else
                {
                    jsonString = checkBoxS.Checked
                        ? JsonConvert.SerializeObject(Obfuscation.Deobfuscate(firstShip.DeepClone()), Formatting.Indented)
                        : JsonConvert.SerializeObject(firstShip, Formatting.Indented);
                }
                string extension = checkBoxSH0.Checked ? ".sh0" : ".json";
                var saveFilePath = System.IO.Path.Combine(filePath, fileName + extension);
                File.WriteAllText(saveFilePath, jsonString);
                MessageClass.InfoMessageBox("飞船已导出到：" + saveFilePath);
            }
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
                dialog.Filter = "JSON 文件 (*.json); 飞船完整包(*.nmsship)|*.json; *nmsship|所有文件 (*.*)|*.*";
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
            progressBar.Visible = true;
            var index = GetSelectedRadioI();
            if (exportName.Text != "") { defaultExportNameString = exportName.Text; }
            if (ShipBaseTokens.Count < 1)
            {
                MessageClass.ErrorMessageBox("存档内没有自定义飞船，请更换存档或建立自定义飞船之后再尝试！");
                progressBar.Visible = false;
                return;
            }
            exportShip(ShipBaseTokens[index], exportPathString, defaultExportNameString);
            progressBar.Visible = false;
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
            progressBar.Visible = true;

            var result = MessageClass.WarningMessageBox(
                "导入自定义飞船会覆盖当前自定义飞船的数据，操作不可逆且可能损坏存档，建议先备份存档！\n\n" +
                "确认继续吗？"
                );
            if (result == DialogResult.Cancel)
            {
                progressBar.Visible = false;
                return;
            }

            var index = GetSelectedRadioI();
            if (importPathString == "" && inputImportText.Text == "")
            {
                MessageClass.ErrorMessageBox("还未设置导入来源，请选择导入文件或者手动输入导入内容！");
                progressBar.Visible = false;
                return;
            }
            else if (importPathString != "" && inputImportText.Text != "")
            {
                MessageClass.ErrorMessageBox("你设置了两个导入来源，请重新选择导入文件或者手动输入导入内容！");
                progressBar.Visible = false;
                return;
            }
            if (ShipBaseTokens.Count < 1)
            {
                MessageClass.ErrorMessageBox("存档内没有自定义飞船，请更换存档或建立自定义飞船之后再尝试！");
                progressBar.Visible = false;
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
                    progressBar.Visible = false;
                    return;
                }

                // 写入文件
                string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp.json");
                File.WriteAllText(tempPath, formattedJson);
                importShip(ShipBaseTokens[index], tempPath);
                if (File.Exists(tempPath)) { File.Delete(tempPath); }
            }
        }

        private void buttonSetSeed_Click(object sender, EventArgs e)
        {
            progressBar.Visible = true;
            var result = MessageClass.WarningMessageBox(
                "导入飞船种子会覆盖当前飞船的数据，操作不可逆且可能损坏存档，建议先备份存档！\n" +
                "请确保导入种子的来源飞船类型与你要覆盖的飞船类型一致，且种子完整不包含空格。\n\n" +
                "确认继续吗？"
                );
            if (result == DialogResult.Cancel)
            {
                progressBar.Visible = false;
                return;
            }

            if (AllJTokens == null || ShipBaseTokens == null)
            {
                MessageClass.ErrorMessageBox("还未加载存档或存档无效，请重新加载存档！");
                progressBar.Visible = false;
                return;
            }
            if (ShipBaseTokens.Count < 1)
            {
                MessageClass.ErrorMessageBox("存档内没有飞船，请更换存档或获取飞船之后再尝试！");
                progressBar.Visible = false;
                return;
            }
            var index = GetSelectedRadioSIndex();
            var originalSeed = GetSelectedRadioSSeed();
            if (index == null || originalSeed == null)
            {
                MessageClass.ErrorMessageBox("还未选择飞船，请选择一个飞船再导入种子！");
                progressBar.Visible = false;
                return;
            }
            if (originalSeed.Contains("种子无效"))
            {
                MessageClass.ErrorMessageBox("当前选择的飞船不支持导入种子，也不支持使用该飞船的种子，请选择其他飞船！");
                progressBar.Visible = false;
                return;
            }
            int indexInt = (int)index;
            string shipSeedString = shipSeed.Text;
            if (shipSeedString == "")
            {
                MessageClass.ErrorMessageBox("还未设置种子，请输入一个种子再导入！");
                progressBar.Visible = false;
                return;
            }
            setSeed(indexInt, shipSeedString);
        }

        private void buttonSeedShipImport_Click(object sender, EventArgs e)
        {
            progressBar.Visible = true;
            var result = MessageClass.WarningMessageBox(
                "导入飞船文件会覆盖当前飞船的数据，操作不可逆且可能损坏存档，建议先备份存档！\n" +
                "若你的飞船未满 12 艘，在未选定飞船的情况下我们会默认你在作为新飞船导入。\n\n" +
                "注意！！此功能不适用于导入自定义飞船部件或导入自定义飞船作为新飞船！\n" +
                "请不要导入任何与自定义飞船相关的文件！！！\n" +
                "任何不当使用造成的个人损失与本工具无关，请在执行危险操作之前备份您的存档文件。\n\n" +
                "确认继续吗？"
                );
            if (result == DialogResult.Cancel)
            {
                progressBar.Visible = false;
                return;
            }

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "飞船完整包文件 (*nmsship); JSON 文件 (*.json); SH0 文件 (*.sh0)|*.nmsship; *.json; *.sh0|所有文件 (*.*)|*.*";
                dialog.Title = "请选择文件";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = dialog.FileName;
                    // 这里可以处理选中的文件路径，例如显示到文本框
                    var importPathString = selectedFile;
                    if (GetSelectedRadioSType() == "自定义")
                    {
                        MessageClass.ErrorMessageBox("自定义飞船请使用自定义飞船导入导出功能，请选择其他飞船！");
                        progressBar.Visible = false;
                        return;
                    }
                    var index = GetSelectedRadioSIndex();
                    if (checkBoxNewShip.Checked || index == null)
                    {
                        if (radioPanelS.Controls.Count < 12)
                        {
                            if (!checkBoxNewShip.Checked)
                            { MessageClass.InfoMessageBox("当前存档飞船数量未满 12 艘，未选中飞船则默认导入新的飞船。"); }
                            importSeedShip(-1, importPathString);
                        }
                        else
                        {
                            if (!checkBoxNewShip.Checked)
                            { MessageClass.ErrorMessageBox("还未选择飞船，请选择一个飞船再导入！"); }
                            else
                            { MessageClass.ErrorMessageBox("当前存档飞船数量已满 12 艘，无法导入新飞船！"); }
                            progressBar.Visible = false;
                            return;
                        }
                    }
                    else
                    {
                        int indexInt = (int)index;
                        importSeedShip(indexInt, importPathString);
                    }
                }
                else
                {
                    MessageClass.InfoMessageBox("导入取消！");
                }
            }
        }

        private void buttonSeedShipExport_Click(object sender, EventArgs e)
        {
            progressBar.Visible = true;
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = dialog.SelectedPath;
                    // 这里可以处理选中的文件路径，例如显示到文本框
                    var exportPathString = selectedFile;
                    if (GetSelectedRadioSType() == "自定义")
                    {
                        MessageClass.ErrorMessageBox("自定义飞船请使用自定义飞船导入导出功能，请选择其他飞船！");
                        progressBar.Visible = false;
                        return;
                    }
                    var index = GetSelectedRadioSIndex();
                    if (index == null)
                    {
                        MessageClass.ErrorMessageBox("还未选择飞船，请选择一个飞船再导出！");
                        progressBar.Visible = false;
                        return;
                    }
                    else
                    {
                        int indexInt = (int)index;
                        var fileName = textBoxExportName.Text;
                        if (fileName == "") fileName = GetSelectedRadioSName();
                        if (fileName == null || fileName == "") fileName = "导出飞船";
                        exportSeedShip(indexInt, exportPathString, fileName);
                    }
                }
                else
                {
                    MessageClass.InfoMessageBox("导出取消！");
                }
            }
            progressBar.Visible = false;
        }

        private void checkBoxI_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxI.Checked)
            {
                var result = MessageClass.WarningMessageBox(
                    "你正在关闭混淆功能。\n" +
                    "由于游戏存档经过混淆加密，在导入明文内容和导出明文内容时需要经过混淆与反混淆" +
                    "如果你的设备能流畅地运行无人深空，那么混淆对你的设备性能的影响可以忽略不计。\n" +
                    "即使你导入或导入的内容未经反混淆仍然是密文，我们也推荐你保持混淆开启。" +
                    "混淆算法不会影响已经经过混淆的内容，开启混淆可以尽可能的降低意外导出入明文且未经混淆而导致存档损坏的风险。\n" +
                    "你确定你想要关闭混淆，直接导入或导出内容吗？\n" +
                    "若确定继续，请确保你导入或导出的内容可以被安全地直接写入存档。"
                    );
                if (result == DialogResult.Cancel) { checkBoxI.Checked = true; }
            }
        }

        private void checkBoxS_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxS.Checked)
            {
                var result = MessageClass.WarningMessageBox(
                    "你正在关闭混淆功能。\n" +
                    "由于游戏存档经过混淆加密，在导入明文内容和导出明文内容时需要经过混淆与反混淆。" +
                    "如果你的设备能流畅地运行无人深空，那么混淆对你的设备性能的影响可以忽略不计。\n" +
                    "混淆算法不会影响已经经过混淆的内容，开启混淆可以尽可能的降低意外导出入明文且未经混淆而导致存档损坏的风险。\n" +
                    "你确定你想要关闭混淆，直接导出/入内容吗？\n" +
                    "若确定继续，请确保你导入或导出的内容可以被安全地直接写入存档。"
                    );
                if (result == DialogResult.Cancel) { checkBoxS.Checked = true; }
            }
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            new AboutDialog().ShowDialog(this);
        }

        private void shipSeed_TextChanged(object sender, EventArgs e)
        {
            if (GetSelectedRadioSSeed().Contains("种子无效"))
            { buttonSetSeed.Enabled = false; }
            else if (shipSeed.Text == GetSelectedRadioSSeed()) { buttonSetSeed.Enabled = false; }
            else { buttonSetSeed.Enabled = true; }
        }

        private void checkBoxSH0_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSH0.Checked)
            {
                checkBoxNMSSHIP3.Checked = false;
            }
        }

        private void checkBoxNMSSHIP3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxNMSSHIP3.Checked)
            {
                checkBoxSH0.Checked = false;
            }
        }
    }
}
