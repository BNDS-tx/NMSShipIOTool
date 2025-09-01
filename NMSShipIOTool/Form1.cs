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
                if (System.IO.Directory.Exists(nmsPath1)){ dialog.InitialDirectory = nmsPath1; }
                else if (System.IO.Directory.Exists(nmsPath2)) { dialog.InitialDirectory = nmsPath2; }
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // �û�ѡ�����ļ�
                // �� dialog.ToString �滻Ϊ dialog.SelectedPath
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
            else { platform = "GOG ������ƽ̨"; }

            DateTime fileLastWriteTime = File.GetLastWriteTime(filePath);

            labelDescription.Text = "�浵ƽ̨��" + platform + "\n��󱣴�ʱ�䣺" + fileLastWriteTime;
        }

        bool hasStLayer(string filePath)
        {
            // ���·��Ϊ����Ŀ¼���ļ���
            var parts = filePath.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            // �ж��Ƿ���Ŀ¼���� "st_" ��ͷ
            return parts.Any(part => part.StartsWith("st_"));
        }

        bool hasXboxLayer(String filePath)
        {
            // ���·��Ϊ����Ŀ¼���ļ���
            var parts = filePath.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            // �ж��Ƿ���Ŀ¼��Ϊ "HelloGames.NoMansSky_bs190hzg1sesy"
            return parts.Any(part => part.Equals("HelloGames.NoMansSky_bs190hzg1sesy"));
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            // �������д��ť�������߼�
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
            if (sv == null) { sv = "����Ϣ"; } else { sv = "�Ѷ�ȡ�Ĵ浵��\n" + sv; }
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
            importShip(22, @"D:\������ԣ��ƻ��Ը���.json");
        }

        void importShip(int index, String filePath)
        {
            if (platform == null) { MessageBox.Show("���ȼ��ش浵"); return; }
            if (index < 0 || index >= BaseTokens.Children().Count() || !ShipBaseTokens.Contains(index))
                { MessageBox.Show("��Ч�ķɴ���������"); return; }
            if (filePath == "" || filePath == null) { MessageBox.Show("�����ļ���Ч"); return; }
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageBox.Show("�浵�������������¼��ش浵"); return; }
            if (ShipBaseTokens.Count() == 0) { MessageBox.Show("��ǰ�浵û���Զ���ɴ��������޷�����ɴ�"); return; }
            
            var save = platform.GetSaveContainer(saveSlot);
            string jsonString = File.ReadAllText(filePath); // ��ȡ�ļ�����
            var newShip = JsonConvert.DeserializeObject<JToken>(jsonString);
            if (newShip == null) { MessageBox.Show("��Ч�ķɴ�����"); return; }
            // ���Ʒɴ�����
            var shipCopy = newShip.DeepClone();
            // ��ȡ�ɴ�����
            string targetPath = $"{baseJSONPath}[{index}].@ZJ";
            // �滻�ɴ�����������
            save.SetJsonValue(shipCopy, targetPath);
            // �����޸ĺ�Ĵ浵
            platform.Write(save);
            MessageBox.Show("�ɴ�����ɹ���");
        }

        void exportShip(int index, String filePath, String fileName)
        {
            if (BaseTokens == null || ShipOwnerTokens == null) { MessageBox.Show("�浵�������������¼��ش浵"); return; }
            if (index < 0 || index >= BaseTokens.Children().Count() || !ShipBaseTokens.Contains(index)) 
                { MessageBox.Show("��Ч�ķɴ���������"); return; }
            if (filePath == "" || filePath == null) { MessageBox.Show("��ѡ�񵼳�·��"); return; }
            if (ShipBaseTokens.Count() == 0) { MessageBox.Show("��ǰ�浵û���Զ���ɴ��������޷������ɴ�"); return; }
            var firstShipBaseIndex = index;
            var firstShipBase = BaseTokens.Children().ElementAt(firstShipBaseIndex);
            var jsonString = JsonConvert.SerializeObject(firstShipBase["@ZJ"], Formatting.Indented);
            var saveFilePath = System.IO.Path.Combine(filePath, fileName + ".json");
            File.WriteAllText(saveFilePath, jsonString);
            MessageBox.Show("�ɴ��ѵ�������" + saveFilePath);
        }
    }
}
