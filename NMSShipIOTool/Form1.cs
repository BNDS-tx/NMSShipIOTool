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
    using System.Linq;
    using System.Text.Json.Nodes;

    public partial class Form1 : Form
    {
        private String savePath = "";
        private String importPathString = "";
        private String exportPathString = "";
        private String exportNameString = "�����Զ���ɴ�";
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
        }

        private void updateTabEnabled(String filePath, IEnumerable<JToken> saveTokens)
        {
            if (filePath == "" || filePath == null)
            {
                tabControl1.TabPages[1].Enabled = false;
                tabControl1.TabPages[2].Enabled = false;
                tabControl1.TabPages[3].Enabled = false;
                buttonLoad.Enabled = false;
            }
            else
            {
                if (saveTokens != null)
                {
                    tabControl1.TabPages[1].Enabled = true;
                    tabControl1.TabPages[2].Enabled = true;
                    tabControl1.TabPages[3].Enabled = true;
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
                MessageClass.ErrorMessageBox("û���ҵ���Ч�Ĵ浵");
                updateTabEnabled(savePath, AllJTokens);
                progressBar1.Visible = false;
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

            List<string> shipOptons = new List<string>();
            List<string> shipSeedOptions = new List<string>();

            if (ShipOwnerTokens.Children().Count() < 1)
            {
                MessageClass.InfoMessageBox("�浵���سɹ�����ǰ�浵û���κηɴ����޷����зɴ����뵼��������");
            }
            else
            {
                if (ShipBaseTokens.Count() == 0)
                {
                    MessageClass.InfoMessageBox("�浵���سɹ�����ǰ�浵û���Զ���ɴ����޷������Զ���ɴ����뵼��������");
                }
                else
                {
                    MessageClass.InfoMessageBox("�浵���سɹ�����ǰ�浵�� " + ShipBaseTokens.Count() + " ���Զ���ɴ���");
                    var shipBaseDetected = "��ʶ���Զ���ɴ���" + Environment.NewLine + Environment.NewLine;
                    foreach (int t in ShipBaseTokens)
                    {
                        int shipID = int.Parse(BaseTokens.Children().ElementAt(t)["CVX"].ToString());
                        string shipName = ShipOwnerTokens.Children().ElementAt(shipID)["NKm"].ToString();
                        string option = "�ɴ� ID��" + shipID + "������ ID��" + t.ToString() + "���ɴ�����" + shipName;
                        shipOptons.Add(option);
                        shipBaseDetected = shipBaseDetected + option + Environment.NewLine;
                    }
                    labelShipDetected.Text = shipBaseDetected + Environment.NewLine + Environment.NewLine;
                }

                var allShipDetected = "������ʶ��ɴ���" + Environment.NewLine + Environment.NewLine;
                foreach (var t in ShipOwnerTokens.Children().ToList())
                {
                    string fileName = t["NTx"]["93M"].ToString();
                    if (fileName == "" || fileName == null) { continue; }
                    int shipID = ShipOwnerTokens.Children().ToList().IndexOf(t);
                    string shipName = t["NKm"].ToString();
                    string shipSeed = t["NTx"]["@EL"].Children().ElementAt(1).ToString();
                    string shipType = checkType(fileName);
                    if (shipType == "�Զ���" || shipType == "���⴬") { shipSeed += " ������Ч"; }
                    string option = "�ɴ� ID��" + shipID + "�����ͣ�" + shipType + "���ɴ�����" + shipName + "�����ӣ�" + shipSeed;
                    shipSeedOptions.Add(option);
                    allShipDetected = allShipDetected + option + Environment.NewLine;
                }
                labelShipDetected.Text += allShipDetected;

                AddRadioButtons(shipOptons, shipSeedOptions);
            }

            //test only
            //importShip(22, @"D:\������ԣ��ƻ��Ը���.json");
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

            radioPanelE.Controls.Clear();
            for (int i = 0; i < options1.Count; i++)
            {
                var radio = new RadioButton
                {
                    Text = options1[i],
                    Name = "radioE_" + i,
                    AutoSize = true,
                    Tag = i
                };
                radioPanelE.Controls.Add(radio);
            }

            radioPanelS.Controls.Clear();
            for (int i = 0; i < options2.Count; i++)
            {
                var radio = new RadioButton
                {
                    Text = options2[i],
                    Name = "radioS_" + i,
                    AutoSize = true,
                    Tag = i
                };
                radio.CheckedChanged += (s, e) =>
                {
                    if (((RadioButton)s).Checked)
                    {
                        string seed = ((RadioButton)s).Text.Split("�����ӣ�")[1];
                        shipSeed.Text = seed;
                    }
                };
                radioPanelS.Controls.Add(radio);
            }
        }

        public int GetSelectedRadioE()
        {
            foreach (Control ctrl in radioPanelE.Controls)
            {
                if (ctrl is RadioButton rb && rb.Checked)
                {
                    return (int)rb.Tag; // ���߷�����������������
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
                    return (string)rb.Text.Split("���ӣ�")[1]; 
                }
            }
            return null;
        }

        void importShip(int index, String filePath)
        {
            if (platform == null) { MessageClass.ErrorMessageBox("���ȼ��ش浵"); return; }
            if (index < 0 || index >= BaseTokens.Children().Count() || !ShipBaseTokens.Contains(index))
            { MessageClass.ErrorMessageBox("��Ч���Զ���ɴ�����"); return; }
            if (filePath == "" || filePath == null) { MessageClass.ErrorMessageBox("�����ļ���Ч"); return; }
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageClass.ErrorMessageBox("�浵�������������¼��ش浵"); return; }
            if (ShipBaseTokens.Count() == 0) { MessageClass.ErrorMessageBox("��ǰ�浵û���Զ���ɴ����޷������Զ���ɴ�"); return; }

            var save = platform.GetSaveContainer(saveSlot);
            string jsonString = File.ReadAllText(filePath); // ��ȡ�ļ�����
            var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
            if (newShip == null) { MessageClass.ErrorMessageBox("��Ч���Զ���ɴ�����"); return; }
            // �����Զ���ɴ�����
            var shipCopy = checkBoxI.Checked ? Obfuscation.Obfuscate(newShip.DeepClone()) : newShip.DeepClone();
            // ��ȡ�Զ���ɴ�
            string targetPath = $"{baseJSONPath}[{index}].@ZJ";
            // �滻�Զ���ɴ�������
            save.SetJsonValue(shipCopy, targetPath);
            // �����޸ĺ�Ĵ浵
            platform.Write(save);
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
            var firstShipBase = BaseTokens.Children().ElementAt(firstShipBaseIndex);
            string jsonString;
            if (firstShipBase["@ZJ"] == null) { jsonString = ""; }
            else
            {
                jsonString = checkBoxE.Checked
                    ? JsonConvert.SerializeObject(Obfuscation.Deobfuscate(firstShipBase["@ZJ"]), Formatting.Indented)
                    : JsonConvert.SerializeObject(firstShipBase["@ZJ"], Formatting.Indented);
            }
            var saveFilePath = System.IO.Path.Combine(filePath, fileName + ".json");
            File.WriteAllText(saveFilePath, jsonString);
            MessageClass.InfoMessageBox("�Զ���ɴ��ѵ�������" + saveFilePath);
        }

        void setSeed(int index, String seed)
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
            platform.Write(save);
            MessageClass.InfoMessageBox("�ɴ����ӵ���ɹ��������¼��ش浵�ٲ鿴�������ݻ�ִ������������");
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
                dialog.Filter = "JSON �ļ� (*.json)|*.json";
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
            progressBar3.Visible = true;
            var index = GetSelectedRadioE();
            if (exportName.Text != "") { exportNameString = exportName.Text; }
            if (ShipBaseTokens.Count < 1)
            {
                MessageClass.ErrorMessageBox("�浵��û���Զ���ɴ���������浵�����Զ���ɴ�֮���ٳ��ԣ�");
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
                    // ������Դ���ѡ�е��ļ�·����������ʾ���ı���
                    exportPath.Text = selectedFile;
                    exportPathString = selectedFile;
                }
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            progressBar2.Visible = true;

            var result = MessageClass.WarningMessageBox(
                "�����Զ���ɴ��Ḳ�ǵ�ǰ�Զ���ɴ������ݣ������������ҿ����𻵴浵�������ȱ��ݴ浵��\n\n" +
                "ȷ�ϼ�����"
                );
            if (result == DialogResult.Cancel)
            {
                progressBar2.Visible = false;
                return;
            }

            var index = GetSelectedRadioI();
            if (importPathString == "" && inputImportText.Text == "")
            {
                MessageClass.ErrorMessageBox("��δ���õ�����Դ����ѡ�����ļ������ֶ����뵼�����ݣ�");
                progressBar2.Visible = false;
                return;
            }
            else if (importPathString != "" && inputImportText.Text != "")
            {
                MessageClass.ErrorMessageBox("������������������Դ��������ѡ�����ļ������ֶ����뵼�����ݣ�");
                progressBar2.Visible = false;
                return;
            }
            if (ShipBaseTokens.Count < 1)
            {
                MessageClass.ErrorMessageBox("�浵��û���Զ���ɴ���������浵�����Զ���ɴ�֮���ٳ��ԣ�");
                progressBar2.Visible = false;
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
                    progressBar2.Visible = false;
                    return;
                }

                // д���ļ�
                string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp.json");
                File.WriteAllText(tempPath, formattedJson);
                importShip(ShipBaseTokens[index], tempPath);
                if (File.Exists(tempPath)) { File.Delete(tempPath); }
            }
            progressBar2.Visible = false;
        }

        private void buttonSetSeed_Click(object sender, EventArgs e)
        {
            progressBar4.Visible = true;
            var result = MessageClass.WarningMessageBox(
                "����ɴ����ӻḲ�ǵ�ǰ�ɴ������ݣ������������ҿ����𻵴浵�������ȱ��ݴ浵��\n" +
                "��ȷ���������ӵ���Դ�ɴ���������Ҫ���ǵķɴ�����һ�£������������������ո�\n\n" +
                "ȷ�ϼ�����"
                );
            if (result == DialogResult.Cancel)
            {
                progressBar4.Visible = false;
                return;
            }

            if (AllJTokens == null || ShipBaseTokens == null)
            {
                MessageClass.ErrorMessageBox("��δ���ش浵��浵��Ч�������¼��ش浵��");
                progressBar4.Visible = false;
                return;
            }
            if (ShipBaseTokens.Count < 1)
            {
                MessageClass.ErrorMessageBox("�浵��û�зɴ���������浵���ȡ�ɴ�֮���ٳ��ԣ�");
                progressBar4.Visible = false;
                return;
            }
            var index = GetSelectedRadioSIndex();
            var originalSeed = GetSelectedRadioSSeed();
            if (index == null || originalSeed == null)
            {
                MessageClass.ErrorMessageBox("��δѡ��ɴ�����ѡ��һ���ɴ��ٵ������ӣ�");
                progressBar4.Visible = false;
                return;
            }
            if (originalSeed.Contains("������Ч"))
            {
                MessageClass.ErrorMessageBox("��ǰѡ��ķɴ���֧�ֵ������ӣ�Ҳ��֧��ʹ�ø÷ɴ������ӣ���ѡ�������ɴ���");
                progressBar4.Visible = false;
                return;
            }
            int indexInt = (int)index;
            string shipSeedString = shipSeed.Text;
            if (shipSeedString == "")
            {
                MessageClass.ErrorMessageBox("��δ�������ӣ�������һ�������ٵ��룡");
                progressBar4.Visible = false;
                return;
            }
            setSeed(indexInt, shipSeedString);
            progressBar4.Visible = false;
        }

        private void checkBoxI_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxI.Checked)
            {
                var result = MessageClass.WarningMessageBox("�����ڹرյ���Ļ������ܡ�������Ϸ�浵�����������ܣ��ڵ�����������ʱ��Ҫ�����������������豸������������������գ���ô����������豸���ܵ�Ӱ����Ժ��Բ��ơ�\n" +
                    "��ʹ�㵼�������δ����������Ȼ�����ģ�����Ҳ�Ƽ��㱣�ֻ��������������㷨����Ӱ���Ѿ��������������ݣ������������Ծ����ܵĽ������⵼��������δ�����������´浵�𻵵ķ��ա�\n" +
                    "��ȷ������Ҫ�رջ�����ֱ�ӵ��� JSON ��������ȷ����������ȷ���㵼���������ȫ�����������κβ���ʹ����ɵĸ�����ʧ�뱾�����޹أ�����ִ��Σ�ղ���֮ǰ�������Ĵ浵�ļ���");
                if (result == DialogResult.Cancel) { checkBoxI.Checked = true; }
            }
        }

        private void checkBoxE_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxE.Checked)
            {
                var result = MessageClass.WarningMessageBox("�����ڹرյ����ķ��������ܡ�������Ϸ�浵�����������ܣ��ڵ�����������ʱ��Ҫ�������������������豸������������������գ���ô������������豸���ܵ�Ӱ����Ժ��Բ��ơ�\n" +
                    "�������������Է����㽫�ɴ��Ľ�ģ����������ƽ̨���������������ߵļ�����Ҳ���á����رշ�������������������Ϊ�������ģ����ܽ�֧�����ڱ�����֮��ĵ��뵼�����������������������Ի���΢����΢�������Ż��Ƿ�ֵ�á�\n" +
                    "��ȷ������Ҫ�رշ�������ֱ�ӵ�������������������ȷ����������ȷ�������ڷ���������ƽ̨��ʹ��������������ǰ���������жԷ������ݽ������������κβ���ʹ����ɵĸ�����ʧ�뱾�����޹أ�����ִ��Σ�ղ���֮ǰ�������Ĵ浵�ļ���");
                if (result == DialogResult.Cancel) { checkBoxE.Checked = true; }
            }
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            new AboutDialog().ShowDialog(this);
        }

        private void shipSeed_TextChanged(object sender, EventArgs e)
        {
            if (shipSeed.Text == GetSelectedRadioSSeed()) { buttonSetSeed.Enabled = false; } 
            else { buttonSetSeed.Enabled = true; }
            if (GetSelectedRadioSSeed().Contains("������Ч"))
            { MessageClass.ErrorMessageBox("��ǰѡ��ķɴ���֧�ֵ������ӣ�Ҳ��֧��ʹ�ø÷ɴ������ӣ���ѡ�������ɴ���"); }
        }
    }
}
