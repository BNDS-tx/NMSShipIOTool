namespace NMSShipIOTool
{
    using libNOM.io.Enums;
    using libNOM.io.Interfaces;
    using NMSModelIOTool.Model;
    using NMSShipIOTool.Model;
    using NMSShipIOTool.View;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public partial class Form1 : Form
    {
        private String savePath = "";
        private String importPathString = "";
        private String exportPathString = "";

        private SaveLoader saveLoader = new SaveLoader();

        public Form1()
        {
            InitializeComponent();
            updateTabEnabled(false);
        }

        #region Page Scripts

        private void updateTabEnabled(bool enable)
        {
            tabControl1.TabPages[1].Enabled = enable;
            tabControl1.TabPages[2].Enabled = enable;
        }

        private void selectFile()
        {
            string path = FileOperations.selectSaveFile(savePath);
            if (path != null && path != "")
            {
                savePath = path;
                textBoxPath.Text = savePath;
                updateSaveDescription(savePath, "������");
            }
        }

        private void updateSaveDescription(String filePath, string platform)
        {
            var lastFile = Directory.GetFiles(filePath).OrderByDescending(f => File.GetLastWriteTime(f)).First();
            DateTime fileLastWriteTime = File.GetLastWriteTime(lastFile);

            labelDescription.Text = "�浵ƽ̨��" + platform + "\n��󱣴�ʱ�䣺" + fileLastWriteTime;
        }

        public async void loadSave(String filePath)
        {
            startLoading();

            var saves = Enumerable.Empty<IContainer>();
            try
            {
                await Task.Run(() =>
                {
                    saves = saveLoader.LoadPath(filePath);
                });
            }
            catch (Exception ex)
            {
                MessageClass.ErrorMessageBox("Ѱ�Ҵ浵ʧ�ܣ�������Ϣ��" + ex.Message + "\n");
                finishLoading();
                return;
            }

            var choose = 0;
            var saveList = saves.Select((item, idx) => new { item, idx })
                .Where(s => s.item.ActiveContext == SaveContextQueryEnum.Main)
                .Select(s => s.idx)
                .ToList();
            if (saveList == null || saveList.Count() == 0)
            {
                MessageClass.ErrorMessageBox("û���ҵ���Ч�Ĵ浵");
                finishLoading();
                return;
            }
            else if (saveList.Count() == 1)
            {
                MessageClass.InfoMessageBox("���ҵ�һ���浵���Զ�ѡ�� " + saveList.ToList()[0].ToString());
            }
            else
            {
                using var dialog = new ChoiceDialog("ѡ��һ���浵", saves.ToList().Where(s => saveList.Contains(saves.ToList().IndexOf(s))).ToList());
                {
                    dialog.ShowDialog();
                    if (dialog.DialogResult == DialogResult.OK)
                    { choose = dialog.SelectedOption; }
                    else { MessageClass.InfoMessageBox("ѡ��ȡ����Ĭ�� " + saves.ToList()[saveList[choose]].ToString()); }
                }
            }

            try
            {
                await Task.Run(() =>
                {
                    saveLoader.LoadSave(saves.ToList()[saveList[choose]]);

                    this.Invoke((MethodInvoker)delegate
                    {
                        updateUI();
                        MessageClass.InfoMessageBox("�浵������ɡ�      ");
                    });
                });
            }
            catch (Exception ex)
            {
                saveLoader.uninstallSave();
                MessageClass.ErrorMessageBox(ex.Message);
                finishLoading();
                return;
            }

            updateTabEnabled(saveLoader.AllJsonNode != null);
            finishLoading();
            updateUI();
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
                GetSelectedRadioSSeed()!.Contains("������Ч"))
                buttonSetSeed.Enabled = true;
        }

        private void startLoading()
        {
            progressBar.Visible = true;
            setAllButtonDisabled();
        }

        private void finishLoading()
        {
            progressBar.Visible = false;
            setAllButtonEnabled();
        }

        private void updateUI()
        {
            labelShipDetected.Text = SaveHandler.getSummary(
                        saveLoader.PersistentPlayerBases!,
                        saveLoader.ShipOwnership!,
                        saveLoader.CharacterCustomisationData!,
                        saveLoader.BaseShipIndex
                        );
            updateSaveDescription(savePath, saveLoader.platform?.ToString() ?? "");
            AddRadioButtons(
                SaveHandler.shipBaseOptions,
                SaveHandler.shipOptons
                );
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
                    if (((RadioButton)s!).Checked)
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
                    return (int)(rb.Tag ?? -1); // ���߷�����������������
                }
            }
            return -1;
        }

        public int? GetSelectedRadioSIndex()
        {
            foreach (Control ctrl in radioPanelS.Controls)
            {
                if (ctrl is RadioButton rb && rb.Checked)
                {
                    return (int)(rb.Tag ?? -1); // ���߷�����������������
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

        #endregion

        #region UI Events

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            // �������д��ť�������߼�
            selectFile();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            // �������д��ť�������߼�
            loadSave(savePath);
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

        private async void buttonExport_Click(object sender, EventArgs e)
        {
            startLoading();
            var index = GetSelectedRadioI();
            try
            {
                await saveLoader.exportShip(
                    index,
                    exportPathString,
                    exportName.Text ?? "",
                    checkBoxI.Checked,
                    !checkBoxNMSSHIP1.Checked,
                    false,
                    false
                    );
            }
            catch (Exception ex)
            {
                MessageClass.ErrorMessageBox($"{ex.Message}");
                finishLoading();
                return;
            }
            finishLoading();
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

        private async void buttonImport_Click(object sender, EventArgs e)
        {
            startLoading();
            var index = GetSelectedRadioI();
            if (importPathString == "" && inputImportText.Text == "")
            {
                MessageClass.ErrorMessageBox("����ѡ�������Ҫ������ļ�·����");
                finishLoading();
                return;
            }
            if (importPathString != "" && inputImportText.Text != "")
            {
                MessageClass.ErrorMessageBox("������������������Դ����ѡ��һ�����룡��");
                finishLoading();
                return;
            }
            try
            {
                await saveLoader.importShip(
                    index,
                    (importPathString != ""
                        ? importPathString
                        : FileOperations.setTempFile(inputImportText.Text)
                    ),
                    checkBoxI.Checked,
                    false
                    );
            }
            catch (Exception ex)
            {
                MessageClass.ErrorMessageBox($"{ex.Message}");
                finishLoading();
                return;
            }
            updateUI();
            finishLoading();
        }

        private async void buttonSetSeed_Click(object sender, EventArgs e)
        {
            startLoading();
            var index = GetSelectedRadioSIndex() ?? -1;
            try
            {
                if (seedText.Text == "")
                {
                    throw new Exception("��������Ч�����ӣ�");
                }
                await saveLoader.setShipSeed(
                    index,
                    seedText.Text
                    );
            }
            catch (Exception ex)
            {
                MessageClass.ErrorMessageBox($"{ex.Message}");
                finishLoading();
                return;
            }
            updateUI();
            finishLoading();
        }

        private async void buttonSeedShipImport_Click(object sender, EventArgs e)
        {
            startLoading();
            var tempPath = "";
            try
            {
                tempPath = FileOperations.fileSelect();
                if (tempPath == "")
                {
                    MessageClass.InfoMessageBox("����ȡ����");
                    finishLoading();
                    return;
                }

                if (!File.Exists(tempPath))
                {
                    throw new Exception("�ļ������ڣ�");
                }
                var Index = GetSelectedRadioSIndex() ?? -1;
                await saveLoader.importShip(
                    Index,
                    tempPath,
                    checkBoxS.Checked,
                    checkBoxNewShip.Checked
                    );
            }
            catch (System.Exception ex)
            {
                MessageClass.ErrorMessageBox(ex.Message);
                finishLoading();
                return;
            }
            updateUI();
            finishLoading();
        }

        private async void buttonSeedShipExport_Click(object sender, EventArgs e)
        {
            startLoading();
            var tempPath = "";
            try
            {
                tempPath = FileOperations.folderSelect();
                if (tempPath == "")
                {
                    MessageClass.InfoMessageBox("����ȡ����");
                    finishLoading();
                    return;
                }
                var Index = GetSelectedRadioSIndex() ?? -1;
                await saveLoader.exportShip(
                    Index,
                    tempPath,
                    textBoxExportName.Text,
                    checkBoxS.Checked,
                    false,
                    !checkBoxNMSSHIP3.Checked,
                    checkBoxSH0.Checked);
            }
            catch (Exception ex)
            {
                MessageClass.ErrorMessageBox($"{ex.Message}");
                finishLoading();
                return;
            }
            finishLoading();
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

        #endregion

        private async void buttonImportShipTech_Click(object sender, EventArgs e)
        {
            startLoading();
            var tempPath = "";
            try
            {
                tempPath = FileOperations.fileSelect();
                if (tempPath == "")
                {
                    MessageClass.InfoMessageBox("����ȡ����");
                    finishLoading();
                    return;
                }

                if (!File.Exists(tempPath))
                {
                    throw new Exception("�ļ������ڣ�");
                }
                var Index = GetSelectedRadioSIndex() ?? -1;
                await saveLoader.importShipTech(
                    Index,
                    tempPath,
                    checkBoxS.Checked
                    );
            }
            catch (System.Exception ex)
            {
                MessageClass.ErrorMessageBox(ex.Message);
                finishLoading();
                return;
            }
            updateUI();
            finishLoading();
        }

        private async void buttonExportShipTech_Click(object sender, EventArgs e)
        {
            startLoading();
            var tempPath = "";
            try
            {
                tempPath = FileOperations.folderSelect();
                if (tempPath == "")
                {
                    MessageClass.InfoMessageBox("����ȡ����");
                    finishLoading();
                    return;
                }
                var Index = GetSelectedRadioSIndex() ?? -1;
                await saveLoader.exportShipTech(
                    Index,
                    tempPath,
                    textBoxExportName.Text,
                    checkBoxS.Checked,
                    checkBoxTech.Checked);
            }
            catch (Exception ex)
            {
                MessageClass.ErrorMessageBox($"{ex.Message}");
                finishLoading();
                return;
            }
            finishLoading();
        }
    }
}
