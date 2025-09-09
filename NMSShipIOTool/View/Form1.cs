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
                updateSaveDescription(savePath, "待加载");
            }
        }

        private void updateSaveDescription(String filePath, string platform)
        {
            var lastFile = Directory.GetFiles(filePath).OrderByDescending(f => File.GetLastWriteTime(f)).First();
            DateTime fileLastWriteTime = File.GetLastWriteTime(lastFile);

            labelDescription.Text = "存档平台：" + platform + "\n最后保存时间：" + fileLastWriteTime;
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
                MessageClass.ErrorMessageBox("寻找存档失败，错误信息：" + ex.Message + "\n");
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
                MessageClass.ErrorMessageBox("没有找到有效的存档");
                finishLoading();
                return;
            }
            else if (saveList.Count() == 1)
            {
                MessageClass.InfoMessageBox("仅找到一个存档，自动选择 " + saveList.ToList()[0].ToString());
            }
            else
            {
                using var dialog = new ChoiceDialog("选择一个存档", saves.ToList().Where(s => saveList.Contains(saves.ToList().IndexOf(s))).ToList());
                {
                    dialog.ShowDialog();
                    if (dialog.DialogResult == DialogResult.OK)
                    { choose = dialog.SelectedOption; }
                    else { MessageClass.InfoMessageBox("选择取消，默认 " + saves.ToList()[saveList[choose]].ToString()); }
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
                        MessageClass.InfoMessageBox("存档加载完成。      ");
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
                GetSelectedRadioSSeed()!.Contains("种子无效"))
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
                    Text = options2[i].Split("，种子：")[0],
                    Name = options2[i].Split("，种子：")[1],
                    AutoSize = true,
                    Tag = int.Parse(options2[i].Split("飞船 ID：")[1].Split("，类型：")[0])
                };
                radio.CheckedChanged += (s, e) =>
                {
                    if (((RadioButton)s!).Checked)
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
                    return (int)(rb.Tag ?? -1); // 或者返回索引、其他参数
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
                    return (int)(rb.Tag ?? -1); // 或者返回索引、其他参数
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
            // 在这里编写按钮点击后的逻辑
            selectFile();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            // 在这里编写按钮点击后的逻辑
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
                    // 这里可以处理选中的文件路径，例如显示到文本框
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
                MessageClass.ErrorMessageBox("请先选择或输入要导入的文件路径！");
                finishLoading();
                return;
            }
            if (importPathString != "" && inputImportText.Text != "")
            {
                MessageClass.ErrorMessageBox("你设置了两个导入来源，请选择一个导入！！");
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
                    throw new Exception("请输入有效的种子！");
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
                    MessageClass.InfoMessageBox("操作取消！");
                    finishLoading();
                    return;
                }

                if (!File.Exists(tempPath))
                {
                    throw new Exception("文件不存在！");
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
                    MessageClass.InfoMessageBox("操作取消！");
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
                    MessageClass.InfoMessageBox("操作取消！");
                    finishLoading();
                    return;
                }

                if (!File.Exists(tempPath))
                {
                    throw new Exception("文件不存在！");
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
                    MessageClass.InfoMessageBox("操作取消！");
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
