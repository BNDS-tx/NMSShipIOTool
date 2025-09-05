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
        private String defaultExportNameString = "�����ɴ�";
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
            // �������д��ť�������߼�
            selectFile();
        }

        private void selectFile()
        {
            using var dialog = new FolderBrowserDialog();
            // Ԥ��·�������� savePath
            if (!string.IsNullOrWhiteSpace(savePath) && System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(savePath)))
            {
                dialog.InitialDirectory = System.IO.Path.GetDirectoryName(savePath);
            }
            else
            {
                // ��ȡ %AppData% ·��
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                // ��ȡ %LocalAppData% ·��
                var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                var nmsPath1 = System.IO.Path.Combine(appDataPath, "HelloGames", "NMS");
                var nmsPath2 = System.IO.Path.Combine(localAppDataPath, "Packages", "HelloGames.NoMansSky_bs190hzg1sesy", "SystemAppData", "wgs");
                if (System.IO.Directory.Exists(nmsPath1)) { dialog.InitialDirectory = nmsPath1; }
                else if (System.IO.Directory.Exists(nmsPath2)) { dialog.InitialDirectory = nmsPath2; }
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // �û�ѡ�����ļ�
                // �� dialog.ToString �滻Ϊ dialog.SelectedPath
                textBoxPath.Text = dialog.SelectedPath;
                savePath = dialog.SelectedPath;
                updateTabEnabled(savePath, AllJTokens);
                updateSaveDescription(savePath, "������");
            }
        }

        private void updateSaveDescription(String filePath, string platform)
        {
            var lastFile = Directory.GetFiles(filePath).OrderByDescending(f => File.GetLastWriteTime(f)).First();
            DateTime fileLastWriteTime = File.GetLastWriteTime(lastFile);

            labelDescription.Text = "�浵ƽ̨��" + platform + "\n��󱣴�ʱ�䣺" + fileLastWriteTime;
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            // �������д��ť�������߼�
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
                MessageClass.ErrorMessageBox("û���ҵ���Ч�Ĵ浵");
                updateTabEnabled(savePath, AllJTokens);
                progressBar.Visible = false;
                return;
            }
            else if (saves.Count() == 1)
            {
                MessageClass.InfoMessageBox("���ҵ�һ���浵���Զ�ѡ�� " + saves[0].ToString());
            }
            else
            {
                using (var dialog = new ChoiceDialog("ѡ��һ���浵", saves))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        choose = dialog.SelectedOption;
                    }
                    else
                    {
                        MessageClass.InfoMessageBox("ѡ��ȡ����Ĭ�� " + saves[choose].ToString());
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
                MessageClass.InfoMessageBox("�浵���سɹ�����ǰ�浵û���κηɴ����޷����зɴ����뵼��������");
            }
            else
            {
                MessageClass.InfoMessageBox("�浵���سɹ�����ǰ�浵�У�\n" +
                    "���зɴ� " + ShipOwnerTokens.Children().Count() + " �ң������Զ���ɴ� " +
                    ShipBaseTokens.Count() + " �ҡ�");

                var shipBaseDetected = "�Զ���ɴ���" + Environment.NewLine + Environment.NewLine;
                foreach (int t in ShipBaseTokens)
                {
                    int shipID = int.Parse(BaseTokens.Children().ElementAt(t)["CVX"].ToString());
                    string shipName = ShipOwnerTokens.Children().ElementAt(shipID)["NKm"].ToString();
                    string option = "�ɴ� ID��" + shipID + "������ ID��" + t.ToString() + "���ɴ�����" + shipName;
                    shipOptons.Add(option);
                    shipBaseDetected = shipBaseDetected + option + Environment.NewLine;
                }

                var allShipDetected = "����ɴ���" + Environment.NewLine + Environment.NewLine;
                foreach (var t in ShipOwnerTokens.Children().ToList())
                {
                    string fileName = t["NTx"]["93M"].ToString();
                    if (fileName == "" || fileName == null) { continue; }
                    int shipID = ShipOwnerTokens.Children().ToList().IndexOf(t);
                    string shipName = t["NKm"].ToString();
                    string shipSeed = t["NTx"]["@EL"].Children().ElementAt(1).ToString();
                    string shipType = checkType(fileName);
                    if (shipType == "���⴬") { shipSeed += " ������Ч"; }
                    if (shipType == "�Զ���") { continue; }
                    string option = "�ɴ� ID��" + shipID + "�����ͣ�" + shipType + "���ɴ�����" + shipName + "�����ӣ�" + shipSeed;
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
            if (parts[parts.Count() - 1].Split("_")[0].Equals("FIGHTER")) { return "ս��"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("SENTINELSHIP")) { return "����"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("BIOSHIP")) { return "����"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("SHUTTLE")) { return "��ͧ"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("SAILSHIP")) { return "̫����"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("S-CLASS")) { return "����"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("SCIENTIFIC")) { return "̽�ռ�"; }
            else if (parts[parts.Count() - 1].Split("_")[0].Equals("DROPSHIP")) { return "����"; }
            else if (parts[parts.Count() - 1].Split(".")[0].Equals("BIGGS")) { return "�Զ���"; }
            else { return "���⴬"; }
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
                GetSelectedRadioSSeed().Contains("������Ч"))
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
                    Text = options2[i].Split("�����ӣ�")[0],
                    Name = options2[i].Split("�����ӣ�")[1],
                    AutoSize = true,
                    Tag = int.Parse(options2[i].Split("�ɴ� ID��")[1].Split("�����ͣ�")[0])
                };
                radio.CheckedChanged += (s, e) =>
                {
                    if (((RadioButton)s).Checked)
                    {
                        string seed = ((RadioButton)s).Name;
                        if (seed.Contains("������Ч"))
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
                    return (int)rb.Tag; // ���߷�����������������
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
                    return (int)rb.Tag; // ���߷�����������������
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
                    return (string)rb.Text.Split("���ͣ�")[1].Split("���ɴ�����")[0];
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
                    return (string)rb.Text.Split("�ɴ�����")[1];
                }
            }
            return null;
        }

        //
        // ���ݶ�д
        //

        // д�����/��������/�Զ���ɴ�����
        void writeBaseObject(string filePath, int index, IContainer save, bool isO)
        {
            string jsonString = File.ReadAllText(filePath); // ��ȡ�ļ�����
            var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
            if (newShip == null || jsonString == "") { return; }
            // �����Զ���ɴ�����
            var shipCopy = isO == true ? Obfuscation.Obfuscate(newShip.DeepClone()) : newShip.DeepClone();
            // ��ȡ�Զ���ɴ�
            string targetPath = $"{baseJSONPath}[{index}].@ZJ";
            // �滻�Զ���ɴ�������
            save.SetJsonValue(shipCopy, targetPath);
        }

        // д��ƴ�ӷɴ��Զ��岿������
        void writeCCD(string filePath, int index, IContainer save, bool isO)
        {
            string jsonString = File.ReadAllText(filePath); // ��ȡ�ļ�����
            var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
            if (newShip == null || jsonString == "") { return; }
            // ���Ʒɴ�����
            var shipCopy = isO == true ? Obfuscation.Obfuscate(newShip.DeepClone()) : newShip.DeepClone();
            // ��ȡ·��
            int slot = -1;
            foreach (var item in Obfuscation.SlotTrack)
            { if (item.Key == index) { slot = item.Value; break; } }
            string targetPath = $"{ccdJSONPath}[{slot}]";
            // �滻�ɴ�������
            save.SetJsonValue(shipCopy, targetPath);
        }

        // д��ɴ�����Ȩ����
        void writeSO(string filePath, int index, IContainer save, bool isO)
        {
            string jsonString = File.ReadAllText(filePath); // ��ȡ�ļ�����
            var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
            if (newShip == null || jsonString == "") { return; }
            // ���Ʒɴ�����
            var shipCopy = isO == true ? Obfuscation.Obfuscate(newShip.DeepClone()) : newShip.DeepClone();
            // ��ȡ�ɴ�
            string targetPath = $"{shipJSONPath}[{index}]";
            // �滻�ɴ�������
            save.SetJsonValue(shipCopy, targetPath);
        }

        // ��ȡ����/��������/�Զ���ɴ�����
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

        // ��ȡƴ�ӷɴ��Զ��岿������
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

        // ��ȡ�ɴ�����Ȩ����
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

        // ����������ɴ�������
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

        // ��ȡ������ɴ�������
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
            if (platform == null) { MessageClass.ErrorMessageBox("���ȼ��ش浵"); return; }
            if (index < 0 || index >= BaseTokens.Children().Count() || !ShipBaseTokens.Contains(index))
            { MessageClass.ErrorMessageBox("��Ч���Զ���ɴ�����"); return; }
            if (filePath == "" || filePath == null) { MessageClass.ErrorMessageBox("�����ļ���Ч"); return; }
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageClass.ErrorMessageBox("�浵�������������¼��ش浵"); return; }
            if (ShipBaseTokens.Count() == 0) { MessageClass.ErrorMessageBox("��ǰ�浵û���Զ���ɴ����޷������Զ���ɴ�"); return; }

            var save = platform.GetSaveContainer(saveSlot);
            if (filePath.Split(".").Last() == "json")
            {
                string jsonString = File.ReadAllText(filePath); // ��ȡ�ļ�����
                var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
                if (newShip == null) { MessageClass.ErrorMessageBox("��Ч���Զ���ɴ�����"); return; }
                // �����Զ���ɴ�����
                var shipCopy = checkBoxI.Checked ? Obfuscation.Obfuscate(newShip.DeepClone()) : newShip.DeepClone();
                // ��ȡ�Զ���ɴ�
                string targetPath = $"{baseJSONPath}[{index}].@ZJ";
                // �滻�Զ���ɴ�������
                save.SetJsonValue(shipCopy, targetPath);
            } else if (filePath.Split(".").Last() == "nmsship")
            {
                var firstShipBaseIndex = index;
                var firstShipBase = BaseTokens.Children().ElementAt(firstShipBaseIndex);
                if (firstShipBase["peI"] == null || firstShipBase["peI"]["DPp"] == null ||
                    firstShipBase["peI"]["DPp"].ToString() != "PlayerShipBase")
                { MessageClass.ErrorMessageBox("��ѡ�����������Զ���ɴ����޷������Զ���ɴ�"); return; }
                int shipID = int.Parse(firstShipBase["CVX"].ToString());
                unpackNWriteShip(filePath, firstShipBaseIndex, shipID, save, checkBoxI.Checked);
            }
            else
            {
                MessageClass.ErrorMessageBox("��Ч�ĵ����ļ�����֧�� .json �� .nmsship ��ʽ");
                return;
            }
            // �����޸ĺ�Ĵ浵
            showAllProgressBar();
            setAllButtonDisabled();
            await Task.Run(() =>
            {
                platform.Write(save);
            });
            setAllButtonEnabled();
            hideAllProgressBar();
            MessageClass.InfoMessageBox("�Զ���ɴ�����ɹ��������¼��ش浵�ٲ鿴�������ݻ�ִ������������");
        }

        void exportShip(int index, String filePath, String fileName)
        {
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageClass.ErrorMessageBox("�浵�������������¼��ش浵"); return; }
            if (index < 0 || index >= BaseTokens.Children().Count() || !ShipBaseTokens.Contains(index))
            { MessageClass.ErrorMessageBox("��Ч���Զ���ɴ�����"); return; }
            if (filePath == "" || filePath == null) { MessageClass.ErrorMessageBox("��ѡ�񵼳�·��"); return; }
            if (ShipBaseTokens.Count() == 0) { MessageClass.ErrorMessageBox("��ǰ�浵û���Զ���ɴ����޷������Զ���ɴ�"); return; }
            var firstShipBaseIndex = index;
            if (checkBoxNMSSHIP1.Checked)
            {
                var firstShipBase = BaseTokens.Children().ElementAt(firstShipBaseIndex);
                if (firstShipBase["peI"] == null || firstShipBase["peI"]["DPp"] == null ||
                    firstShipBase["peI"]["DPp"].ToString() != "PlayerShipBase")
                { MessageClass.ErrorMessageBox("��ѡ�����������Զ���ɴ����޷������Զ���ɴ�"); return; }
                int shipID = int.Parse(firstShipBase["CVX"].ToString());
                var saveFilePath = readNPackShip(firstShipBaseIndex, shipID, checkBoxI.Checked, filePath, fileName);
                MessageClass.InfoMessageBox("�Զ���ɴ��ѵ�������" + saveFilePath);
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
                MessageClass.InfoMessageBox("�Զ���ɴ��ѵ�������" + saveFilePath);
            }
        }

        async void setSeed(int index, String seed)
        {
            if (platform == null) { MessageClass.ErrorMessageBox("���ȼ��ش浵"); return; }
            if (index < 0 || index >= ShipOwnerTokens.Children().Count())
            { MessageClass.ErrorMessageBox("��Ч�ķɴ�����"); return; }
            if (seed == "" || seed == null) { MessageClass.ErrorMessageBox("������Ч"); return; }
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageClass.ErrorMessageBox("�浵�������������¼��ش浵"); return; }
            if (ShipOwnerTokens.Count() == 0) { MessageClass.ErrorMessageBox("��ǰ�浵û�зɴ����޷���������"); return; }
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
            MessageClass.InfoMessageBox("�ɴ����ӵ���ɹ��������¼��ش浵�ٲ鿴�������ݻ�ִ������������");
        }

        async void importSeedShip(int index, String filePath)
        {
            if (platform == null) { MessageClass.ErrorMessageBox("���ȼ��ش浵"); return; }
            if (index < -1 || index >= ShipOwnerTokens.Children().Count())
            { MessageClass.ErrorMessageBox("��Ч�ķɴ�����"); return; }
            if (index == -1)
            {
                for (int i = 0; i < ShipOwnerTokens.Children().Count(); i++)
                {
                    var fileName = ShipOwnerTokens.Children().ElementAt(i)["NTx"]["93M"].ToString();
                    if (fileName == null || fileName == "")
                    { index = i; break; }
                    else if (i == ShipOwnerTokens.Children().Count() - 1)
                    {
                        MessageClass.ErrorMessageBox("��ǰ�浵û�п��зɴ�λ���޷������·ɴ�");
                        return;
                    }
                }
            }
            if (filePath == "" || filePath == null) { MessageClass.ErrorMessageBox("�����ļ���Ч"); return; }
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageClass.ErrorMessageBox("�浵�������������¼��ش浵"); return; }
            if (ShipOwnerTokens.Count() == 0) { MessageClass.ErrorMessageBox("��ǰ�浵û�зɴ����޷�����ɴ�"); return; }

            var save = platform.GetSaveContainer(saveSlot);
            if (filePath.Split(".").Last() == "json")
            {
                string jsonString = File.ReadAllText(filePath); // ��ȡ�ļ�����
                var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
                if (newShip == null) { MessageClass.ErrorMessageBox("��Ч�ķɴ�����"); return; }
                // ���Ʒɴ�����
                var shipCopy = checkBoxS.Checked ? Obfuscation.Obfuscate(newShip.DeepClone()) : newShip.DeepClone();
                // ��ȡ�ɴ�
                if (shipCopy == null)
                { MessageClass.ErrorMessageBox("��Ч�ķɴ�����"); return; }
                string targetPath = $"{shipJSONPath}[{index}]";
                // �滻�ɴ�������
                if (!string.IsNullOrEmpty(targetPath))
                { save.SetJsonValue(shipCopy, targetPath); }
                else
                { MessageClass.ErrorMessageBox("��Ч�ķɴ�����·��"); return; }
                // �����޸ĺ�Ĵ浵
                showAllProgressBar();
                setAllButtonDisabled();
                await Task.Run(() =>
                {
                    platform.Write(save);
                });
                setAllButtonEnabled();
                hideAllProgressBar();
                MessageClass.InfoMessageBox("�ɴ�����ɹ��������¼��ش浵�ٲ鿴�������ݻ�ִ������������");
            }
            else if (filePath.Split(".").Last() == "nmsship")
            {
                var firstShipIndex = index;
                unpackNWriteShip(filePath, -1, firstShipIndex, save, checkBoxS.Checked);
                // �����޸ĺ�Ĵ浵
                showAllProgressBar();
                setAllButtonDisabled();
                await Task.Run(() =>
                {
                    platform.Write(save);
                });
                setAllButtonEnabled();
                hideAllProgressBar();
                MessageClass.InfoMessageBox("�ɴ�����ɹ��������¼��ش浵�ٲ鿴�������ݻ�ִ������������");
            }
            else
            {
                MessageClass.ErrorMessageBox("��Ч�ĵ����ļ�����֧�� .json �� .nmsship ��ʽ");
                return;
            }
        }
        void exportSeedShip(int index, String filePath, String fileName)
        {
            if (ShipOwnerTokens == null) { MessageClass.ErrorMessageBox("�浵�������������¼��ش浵"); return; }
            if (index < 0 || index >= ShipOwnerTokens.Children().Count())
            { MessageClass.ErrorMessageBox("��Ч�ķɴ�����"); return; }
            if (filePath == "" || filePath == null) { MessageClass.ErrorMessageBox("��ѡ�񵼳�·��"); return; }
            if (ShipOwnerTokens.Count() == 0) { MessageClass.ErrorMessageBox("��ǰ�浵û�зɴ����޷������ɴ�"); return; }
            
            var firstShipIndex = index;
            if (checkBoxNMSSHIP3.Checked)
            {
                var saveFilePath = readNPackShip(-1, firstShipIndex, checkBoxS.Checked, filePath, fileName);
                MessageClass.InfoMessageBox("�ɴ��ѵ�������" + saveFilePath);
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
                MessageClass.InfoMessageBox("�ɴ��ѵ�������" + saveFilePath);
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
                dialog.Filter = "JSON �ļ� (*.json); �ɴ�������(*.nmsship)|*.json; *nmsship|�����ļ� (*.*)|*.*";
                dialog.Title = "��ѡ���ļ�";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = dialog.FileName;
                    // ������Դ���ѡ�е��ļ�·����������ʾ���ı���
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
                MessageClass.ErrorMessageBox("�浵��û���Զ���ɴ���������浵�����Զ���ɴ�֮���ٳ��ԣ�");
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
                    // ������Դ���ѡ�е��ļ�·����������ʾ���ı���
                    exportPath.Text = selectedFile;
                    exportPathString = selectedFile;
                }
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            progressBar.Visible = true;

            var result = MessageClass.WarningMessageBox(
                "�����Զ���ɴ��Ḳ�ǵ�ǰ�Զ���ɴ������ݣ������������ҿ����𻵴浵�������ȱ��ݴ浵��\n\n" +
                "ȷ�ϼ�����"
                );
            if (result == DialogResult.Cancel)
            {
                progressBar.Visible = false;
                return;
            }

            var index = GetSelectedRadioI();
            if (importPathString == "" && inputImportText.Text == "")
            {
                MessageClass.ErrorMessageBox("��δ���õ�����Դ����ѡ�����ļ������ֶ����뵼�����ݣ�");
                progressBar.Visible = false;
                return;
            }
            else if (importPathString != "" && inputImportText.Text != "")
            {
                MessageClass.ErrorMessageBox("������������������Դ��������ѡ�����ļ������ֶ����뵼�����ݣ�");
                progressBar.Visible = false;
                return;
            }
            if (ShipBaseTokens.Count < 1)
            {
                MessageClass.ErrorMessageBox("�浵��û���Զ���ɴ���������浵�����Զ���ɴ�֮���ٳ��ԣ�");
                progressBar.Visible = false;
                return;
            }
            if (importPathString != "")
            {
                importShip(ShipBaseTokens[index], importPathString);
            }
            else
            {
                // ��ȡ�����ı�����
                string content = inputImportText.Text;

                // ��װΪ JSON ����
                string formattedJson;
                try
                {
                    var token = JToken.Parse(content);
                    formattedJson = token.ToString(Formatting.Indented);
                }
                catch
                {
                    MessageClass.ErrorMessageBox("�������ݲ��ǺϷ��� JSON ��ʽ��");
                    progressBar.Visible = false;
                    return;
                }

                // д���ļ�
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
                "����ɴ����ӻḲ�ǵ�ǰ�ɴ������ݣ������������ҿ����𻵴浵�������ȱ��ݴ浵��\n" +
                "��ȷ���������ӵ���Դ�ɴ���������Ҫ���ǵķɴ�����һ�£������������������ո�\n\n" +
                "ȷ�ϼ�����"
                );
            if (result == DialogResult.Cancel)
            {
                progressBar.Visible = false;
                return;
            }

            if (AllJTokens == null || ShipBaseTokens == null)
            {
                MessageClass.ErrorMessageBox("��δ���ش浵��浵��Ч�������¼��ش浵��");
                progressBar.Visible = false;
                return;
            }
            if (ShipBaseTokens.Count < 1)
            {
                MessageClass.ErrorMessageBox("�浵��û�зɴ���������浵���ȡ�ɴ�֮���ٳ��ԣ�");
                progressBar.Visible = false;
                return;
            }
            var index = GetSelectedRadioSIndex();
            var originalSeed = GetSelectedRadioSSeed();
            if (index == null || originalSeed == null)
            {
                MessageClass.ErrorMessageBox("��δѡ��ɴ�����ѡ��һ���ɴ��ٵ������ӣ�");
                progressBar.Visible = false;
                return;
            }
            if (originalSeed.Contains("������Ч"))
            {
                MessageClass.ErrorMessageBox("��ǰѡ��ķɴ���֧�ֵ������ӣ�Ҳ��֧��ʹ�ø÷ɴ������ӣ���ѡ�������ɴ���");
                progressBar.Visible = false;
                return;
            }
            int indexInt = (int)index;
            string shipSeedString = shipSeed.Text;
            if (shipSeedString == "")
            {
                MessageClass.ErrorMessageBox("��δ�������ӣ�������һ�������ٵ��룡");
                progressBar.Visible = false;
                return;
            }
            setSeed(indexInt, shipSeedString);
        }

        private void buttonSeedShipImport_Click(object sender, EventArgs e)
        {
            progressBar.Visible = true;
            var result = MessageClass.WarningMessageBox(
                "����ɴ��ļ��Ḳ�ǵ�ǰ�ɴ������ݣ������������ҿ����𻵴浵�������ȱ��ݴ浵��\n" +
                "����ķɴ�δ�� 12 �ң���δѡ���ɴ�����������ǻ�Ĭ��������Ϊ�·ɴ����롣\n\n" +
                "ע�⣡���˹��ܲ������ڵ����Զ���ɴ����������Զ���ɴ���Ϊ�·ɴ���\n" +
                "�벻Ҫ�����κ����Զ���ɴ���ص��ļ�������\n" +
                "�κβ���ʹ����ɵĸ�����ʧ�뱾�����޹أ�����ִ��Σ�ղ���֮ǰ�������Ĵ浵�ļ���\n\n" +
                "ȷ�ϼ�����"
                );
            if (result == DialogResult.Cancel)
            {
                progressBar.Visible = false;
                return;
            }

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "�ɴ��������ļ� (*nmsship); JSON �ļ� (*.json); SH0 �ļ� (*.sh0)|*.nmsship; *.json; *.sh0|�����ļ� (*.*)|*.*";
                dialog.Title = "��ѡ���ļ�";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = dialog.FileName;
                    // ������Դ���ѡ�е��ļ�·����������ʾ���ı���
                    var importPathString = selectedFile;
                    if (GetSelectedRadioSType() == "�Զ���")
                    {
                        MessageClass.ErrorMessageBox("�Զ���ɴ���ʹ���Զ���ɴ����뵼�����ܣ���ѡ�������ɴ���");
                        progressBar.Visible = false;
                        return;
                    }
                    var index = GetSelectedRadioSIndex();
                    if (checkBoxNewShip.Checked || index == null)
                    {
                        if (radioPanelS.Controls.Count < 12)
                        {
                            if (!checkBoxNewShip.Checked)
                            { MessageClass.InfoMessageBox("��ǰ�浵�ɴ�����δ�� 12 �ң�δѡ�зɴ���Ĭ�ϵ����µķɴ���"); }
                            importSeedShip(-1, importPathString);
                        }
                        else
                        {
                            if (!checkBoxNewShip.Checked)
                            { MessageClass.ErrorMessageBox("��δѡ��ɴ�����ѡ��һ���ɴ��ٵ��룡"); }
                            else
                            { MessageClass.ErrorMessageBox("��ǰ�浵�ɴ��������� 12 �ң��޷������·ɴ���"); }
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
                    MessageClass.InfoMessageBox("����ȡ����");
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
                    // ������Դ���ѡ�е��ļ�·����������ʾ���ı���
                    var exportPathString = selectedFile;
                    if (GetSelectedRadioSType() == "�Զ���")
                    {
                        MessageClass.ErrorMessageBox("�Զ���ɴ���ʹ���Զ���ɴ����뵼�����ܣ���ѡ�������ɴ���");
                        progressBar.Visible = false;
                        return;
                    }
                    var index = GetSelectedRadioSIndex();
                    if (index == null)
                    {
                        MessageClass.ErrorMessageBox("��δѡ��ɴ�����ѡ��һ���ɴ��ٵ�����");
                        progressBar.Visible = false;
                        return;
                    }
                    else
                    {
                        int indexInt = (int)index;
                        var fileName = textBoxExportName.Text;
                        if (fileName == "") fileName = GetSelectedRadioSName();
                        if (fileName == null || fileName == "") fileName = "�����ɴ�";
                        exportSeedShip(indexInt, exportPathString, fileName);
                    }
                }
                else
                {
                    MessageClass.InfoMessageBox("����ȡ����");
                }
            }
            progressBar.Visible = false;
        }

        private void checkBoxI_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxI.Checked)
            {
                var result = MessageClass.WarningMessageBox(
                    "�����ڹرջ������ܡ�\n" +
                    "������Ϸ�浵�����������ܣ��ڵ����������ݺ͵�����������ʱ��Ҫ���������뷴����" +
                    "�������豸������������������գ���ô����������豸���ܵ�Ӱ����Ժ��Բ��ơ�\n" +
                    "��ʹ�㵼����������δ����������Ȼ�����ģ�����Ҳ�Ƽ��㱣�ֻ���������" +
                    "�����㷨����Ӱ���Ѿ��������������ݣ������������Ծ����ܵĽ������⵼����������δ�����������´浵�𻵵ķ��ա�\n" +
                    "��ȷ������Ҫ�رջ�����ֱ�ӵ���򵼳�������\n" +
                    "��ȷ����������ȷ���㵼��򵼳������ݿ��Ա���ȫ��ֱ��д��浵��"
                    );
                if (result == DialogResult.Cancel) { checkBoxI.Checked = true; }
            }
        }

        private void checkBoxS_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxS.Checked)
            {
                var result = MessageClass.WarningMessageBox(
                    "�����ڹرջ������ܡ�\n" +
                    "������Ϸ�浵�����������ܣ��ڵ����������ݺ͵�����������ʱ��Ҫ���������뷴������" +
                    "�������豸������������������գ���ô����������豸���ܵ�Ӱ����Ժ��Բ��ơ�\n" +
                    "�����㷨����Ӱ���Ѿ��������������ݣ������������Ծ����ܵĽ������⵼����������δ�����������´浵�𻵵ķ��ա�\n" +
                    "��ȷ������Ҫ�رջ�����ֱ�ӵ���/��������\n" +
                    "��ȷ����������ȷ���㵼��򵼳������ݿ��Ա���ȫ��ֱ��д��浵��"
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
            if (GetSelectedRadioSSeed().Contains("������Ч"))
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
