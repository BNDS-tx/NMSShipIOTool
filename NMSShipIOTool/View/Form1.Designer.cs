using System.Globalization;
using System.Resources;

namespace NMSShipIOTool
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl1;

        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.TextBox importPath;
        private System.Windows.Forms.TextBox inputImportText;
        private System.Windows.Forms.TextBox shipSeed;
        private System.Windows.Forms.TextBox labelShipDetected;

        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label shipSelectI;
        private System.Windows.Forms.Label pathTextI;
        private System.Windows.Forms.Label inputTextI;
        private System.Windows.Forms.Label inputTextIExplanation;
        private System.Windows.Forms.Label seedSelectText;
        private System.Windows.Forms.Label seedText;

        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button importSelect;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button aboutButton;
        private System.Windows.Forms.Button buttonSetSeed;

        private System.Windows.Forms.CheckBox checkBoxI;

        private System.Windows.Forms.ProgressBar progressBar;

        private System.Windows.Forms.FlowLayoutPanel radioPanelI;
        private System.Windows.Forms.FlowLayoutPanel radioPanelS;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            labelPath = new Label();
            labelDescription = new Label();
            labelShipDetected = new TextBox();
            textBoxPath = new TextBox();
            buttonSelect = new Button();
            buttonLoad = new Button();
            tabPage2 = new TabPage();
            exportPath = new TextBox();
            exportName = new TextBox();
            pathTextE = new Label();
            nameTextE = new Label();
            exportSelect = new Button();
            buttonExport = new Button();
            checkBoxNMSSHIP1 = new CheckBox();
            importPath = new TextBox();
            inputImportText = new TextBox();
            importSelect = new Button();
            shipSelectI = new Label();
            pathTextI = new Label();
            inputTextI = new Label();
            inputTextIExplanation = new Label();
            buttonImport = new Button();
            radioPanelI = new FlowLayoutPanel();
            checkBoxI = new CheckBox();
            tabPage3 = new TabPage();
            checkBoxNMSSHIP3 = new CheckBox();
            checkBoxNewShip = new CheckBox();
            seedShipIOText = new Label();
            checkBoxSH0 = new CheckBox();
            checkBoxS = new CheckBox();
            buttonSeedShipExport = new Button();
            buttonSeedShipImport = new Button();
            shipSeed = new TextBox();
            seedSelectText = new Label();
            seedText = new Label();
            buttonSetSeed = new Button();
            radioPanelS = new FlowLayoutPanel();
            progressBar = new ProgressBar();
            aboutButton = new Button();
            textBoxExportName = new TextBox();
            label1 = new Label();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(945, 610);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(labelPath);
            tabPage1.Controls.Add(labelDescription);
            tabPage1.Controls.Add(labelShipDetected);
            tabPage1.Controls.Add(textBoxPath);
            tabPage1.Controls.Add(buttonSelect);
            tabPage1.Controls.Add(buttonLoad);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(937, 580);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "游戏存档";
            // 
            // labelPath
            // 
            labelPath.Location = new Point(12, 391);
            labelPath.Name = "labelPath";
            labelPath.Size = new Size(60, 23);
            labelPath.TabIndex = 0;
            labelPath.Text = "存档路径:";
            // 
            // labelDescription
            // 
            labelDescription.Location = new Point(12, 41);
            labelDescription.Name = "labelDescription";
            labelDescription.Size = new Size(286, 249);
            labelDescription.TabIndex = 1;
            // 
            // labelShipDetected
            // 
            labelShipDetected.Location = new Point(319, 38);
            labelShipDetected.Multiline = true;
            labelShipDetected.Name = "labelShipDetected";
            labelShipDetected.ReadOnly = true;
            labelShipDetected.ScrollBars = ScrollBars.Both;
            labelShipDetected.Size = new Size(600, 319);
            labelShipDetected.TabIndex = 2;
            // 
            // textBoxPath
            // 
            textBoxPath.Location = new Point(116, 388);
            textBoxPath.Name = "textBoxPath";
            textBoxPath.ReadOnly = true;
            textBoxPath.Size = new Size(511, 23);
            textBoxPath.TabIndex = 2;
            // 
            // buttonSelect
            // 
            buttonSelect.Location = new Point(116, 430);
            buttonSelect.Name = "buttonSelect";
            buttonSelect.Size = new Size(100, 33);
            buttonSelect.TabIndex = 3;
            buttonSelect.Text = "选择路径";
            buttonSelect.Click += buttonSelect_Click;
            // 
            // buttonLoad
            // 
            buttonLoad.Location = new Point(231, 430);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(100, 33);
            buttonLoad.TabIndex = 4;
            buttonLoad.Text = "加载存档";
            buttonLoad.Click += buttonLoad_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(exportPath);
            tabPage2.Controls.Add(exportName);
            tabPage2.Controls.Add(pathTextE);
            tabPage2.Controls.Add(nameTextE);
            tabPage2.Controls.Add(exportSelect);
            tabPage2.Controls.Add(buttonExport);
            tabPage2.Controls.Add(checkBoxNMSSHIP1);
            tabPage2.Controls.Add(importPath);
            tabPage2.Controls.Add(inputImportText);
            tabPage2.Controls.Add(importSelect);
            tabPage2.Controls.Add(shipSelectI);
            tabPage2.Controls.Add(pathTextI);
            tabPage2.Controls.Add(inputTextI);
            tabPage2.Controls.Add(inputTextIExplanation);
            tabPage2.Controls.Add(buttonImport);
            tabPage2.Controls.Add(radioPanelI);
            tabPage2.Controls.Add(checkBoxI);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(937, 580);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "自定义飞船";
            // 
            // exportPath
            // 
            exportPath.Location = new Point(118, 351);
            exportPath.Name = "exportPath";
            exportPath.ReadOnly = true;
            exportPath.Size = new Size(358, 23);
            exportPath.TabIndex = 11;
            // 
            // exportName
            // 
            exportName.Location = new Point(118, 388);
            exportName.Name = "exportName";
            exportName.Size = new Size(358, 23);
            exportName.TabIndex = 12;
            // 
            // pathTextE
            // 
            pathTextE.Location = new Point(12, 354);
            pathTextE.Name = "pathTextE";
            pathTextE.Size = new Size(100, 23);
            pathTextE.TabIndex = 13;
            pathTextE.Text = "导出文件地址：";
            // 
            // nameTextE
            // 
            nameTextE.Location = new Point(12, 391);
            nameTextE.Name = "nameTextE";
            nameTextE.Size = new Size(100, 23);
            nameTextE.TabIndex = 14;
            nameTextE.Text = "导出文件名：";
            // 
            // exportSelect
            // 
            exportSelect.Location = new Point(116, 430);
            exportSelect.Name = "exportSelect";
            exportSelect.Size = new Size(100, 33);
            exportSelect.TabIndex = 15;
            exportSelect.Text = "选择保存地址";
            exportSelect.Click += exportSelect_Click;
            // 
            // buttonExport
            // 
            buttonExport.Location = new Point(231, 430);
            buttonExport.Name = "buttonExport";
            buttonExport.Size = new Size(100, 33);
            buttonExport.TabIndex = 16;
            buttonExport.Text = "导出飞船";
            buttonExport.Click += buttonExport_Click;
            // 
            // checkBoxNMSSHIP1
            // 
            checkBoxNMSSHIP1.Checked = true;
            checkBoxNMSSHIP1.CheckState = CheckState.Checked;
            checkBoxNMSSHIP1.Location = new Point(231, 480);
            checkBoxNMSSHIP1.Name = "checkBoxNMSSHIP1";
            checkBoxNMSSHIP1.Size = new Size(245, 24);
            checkBoxNMSSHIP1.TabIndex = 10;
            checkBoxNMSSHIP1.Text = "启用完整包";
            // 
            // importPath
            // 
            importPath.Location = new Point(506, 388);
            importPath.Name = "importPath";
            importPath.ReadOnly = true;
            importPath.Size = new Size(390, 23);
            importPath.TabIndex = 0;
            // 
            // inputImportText
            // 
            inputImportText.Location = new Point(506, 61);
            inputImportText.Multiline = true;
            inputImportText.Name = "inputImportText";
            inputImportText.ScrollBars = ScrollBars.Both;
            inputImportText.Size = new Size(390, 271);
            inputImportText.TabIndex = 1;
            inputImportText.TextChanged += inputImportText_TextChanged;
            // 
            // importSelect
            // 
            importSelect.Location = new Point(506, 430);
            importSelect.Name = "importSelect";
            importSelect.Size = new Size(100, 33);
            importSelect.TabIndex = 2;
            importSelect.Text = "选择导入文件";
            importSelect.Click += impoerSelect_Click;
            // 
            // shipSelectI
            // 
            shipSelectI.Location = new Point(12, 35);
            shipSelectI.Name = "shipSelectI";
            shipSelectI.Size = new Size(128, 23);
            shipSelectI.TabIndex = 5;
            shipSelectI.Text = "选择你要覆盖的飞船:";
            // 
            // pathTextI
            // 
            pathTextI.Location = new Point(506, 354);
            pathTextI.Name = "pathTextI";
            pathTextI.Size = new Size(98, 23);
            pathTextI.TabIndex = 3;
            pathTextI.Text = "导入文件目录：";
            // 
            // inputTextI
            // 
            inputTextI.Location = new Point(506, 35);
            inputTextI.Name = "inputTextI";
            inputTextI.Size = new Size(212, 23);
            inputTextI.TabIndex = 4;
            inputTextI.Text = "或者你可以在此处手动输入导入内容：";
            // 
            // inputTextIExplanation
            // 
            inputTextIExplanation.Location = new Point(506, 480);
            inputTextIExplanation.Name = "inputTextIExplanation";
            inputTextIExplanation.Size = new Size(390, 23);
            inputTextIExplanation.TabIndex = 4;
            inputTextIExplanation.Text = "请遵守 JSON 格式，并确保在最外围包含中括号”[]“";
            inputTextIExplanation.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonImport
            // 
            buttonImport.Location = new Point(621, 430);
            buttonImport.Name = "buttonImport";
            buttonImport.Size = new Size(100, 33);
            buttonImport.TabIndex = 6;
            buttonImport.Text = "导入飞船";
            buttonImport.Click += buttonImport_Click;
            // 
            // radioPanelI
            // 
            radioPanelI.AutoScroll = true;
            radioPanelI.Location = new Point(50, 61);
            radioPanelI.Name = "radioPanelI";
            radioPanelI.Size = new Size(426, 271);
            radioPanelI.TabIndex = 8;
            // 
            // checkBoxI
            // 
            checkBoxI.Checked = true;
            checkBoxI.CheckState = CheckState.Checked;
            checkBoxI.Location = new Point(118, 480);
            checkBoxI.Name = "checkBoxI";
            checkBoxI.Size = new Size(98, 24);
            checkBoxI.TabIndex = 9;
            checkBoxI.Text = "启用混淆";
            checkBoxI.CheckedChanged += checkBoxI_CheckedChanged;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(label1);
            tabPage3.Controls.Add(textBoxExportName);
            tabPage3.Controls.Add(checkBoxNMSSHIP3);
            tabPage3.Controls.Add(checkBoxNewShip);
            tabPage3.Controls.Add(seedShipIOText);
            tabPage3.Controls.Add(checkBoxSH0);
            tabPage3.Controls.Add(checkBoxS);
            tabPage3.Controls.Add(buttonSeedShipExport);
            tabPage3.Controls.Add(buttonSeedShipImport);
            tabPage3.Controls.Add(shipSeed);
            tabPage3.Controls.Add(seedSelectText);
            tabPage3.Controls.Add(seedText);
            tabPage3.Controls.Add(buttonSetSeed);
            tabPage3.Controls.Add(radioPanelS);
            tabPage3.Location = new Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(937, 580);
            tabPage3.TabIndex = 3;
            tabPage3.Text = "常规飞船";
            // 
            // checkBoxNMSSHIP3
            // 
            checkBoxNMSSHIP3.Checked = true;
            checkBoxNMSSHIP3.CheckState = CheckState.Checked;
            checkBoxNMSSHIP3.Location = new Point(633, 337);
            checkBoxNMSSHIP3.Name = "checkBoxNMSSHIP3";
            checkBoxNMSSHIP3.Size = new Size(100, 24);
            checkBoxNMSSHIP3.TabIndex = 11;
            checkBoxNMSSHIP3.Text = "启用完整包";
            checkBoxNMSSHIP3.CheckedChanged += checkBoxNMSSHIP3_CheckedChanged;
            // 
            // checkBoxNewShip
            // 
            checkBoxNewShip.Location = new Point(506, 177);
            checkBoxNewShip.Name = "checkBoxNewShip";
            checkBoxNewShip.Size = new Size(112, 24);
            checkBoxNewShip.TabIndex = 13;
            checkBoxNewShip.Text = "作为新飞船导入";
            // 
            // seedShipIOText
            // 
            seedShipIOText.Location = new Point(506, 151);
            seedShipIOText.Name = "seedShipIOText";
            seedShipIOText.Size = new Size(190, 23);
            seedShipIOText.TabIndex = 12;
            seedShipIOText.Text = "你可在此处导出/入飞船文件：";
            // 
            // checkBoxSH0
            // 
            checkBoxSH0.Location = new Point(739, 337);
            checkBoxSH0.Name = "checkBoxSH0";
            checkBoxSH0.Size = new Size(100, 24);
            checkBoxSH0.TabIndex = 11;
            checkBoxSH0.Text = "保存为 .sh0";
            checkBoxSH0.CheckedChanged += checkBoxSH0_CheckedChanged;
            // 
            // checkBoxS
            // 
            checkBoxS.Checked = true;
            checkBoxS.CheckState = CheckState.Checked;
            checkBoxS.Location = new Point(506, 390);
            checkBoxS.Name = "checkBoxS";
            checkBoxS.Size = new Size(100, 24);
            checkBoxS.TabIndex = 4;
            checkBoxS.Text = "启用混淆";
            checkBoxS.CheckedChanged += checkBoxS_CheckedChanged;
            // 
            // buttonSeedShipExport
            // 
            buttonSeedShipExport.Location = new Point(506, 332);
            buttonSeedShipExport.Name = "buttonSeedShipExport";
            buttonSeedShipExport.Size = new Size(100, 33);
            buttonSeedShipExport.TabIndex = 1;
            buttonSeedShipExport.Text = "导出飞船";
            buttonSeedShipExport.Click += buttonSeedShipExport_Click;
            // 
            // buttonSeedShipImport
            // 
            buttonSeedShipImport.Location = new Point(506, 207);
            buttonSeedShipImport.Name = "buttonSeedShipImport";
            buttonSeedShipImport.Size = new Size(100, 33);
            buttonSeedShipImport.TabIndex = 10;
            buttonSeedShipImport.Text = "导入飞船";
            buttonSeedShipImport.Click += buttonSeedShipImport_Click;
            // 
            // shipSeed
            // 
            shipSeed.Location = new Point(506, 61);
            shipSeed.Name = "shipSeed";
            shipSeed.Size = new Size(358, 23);
            shipSeed.TabIndex = 0;
            // 
            // seedSelectText
            // 
            seedSelectText.Location = new Point(12, 35);
            seedSelectText.Name = "seedSelectText";
            seedSelectText.Size = new Size(184, 23);
            seedSelectText.TabIndex = 0;
            seedSelectText.Text = "选择你要导入/导出的常规飞船：";
            // 
            // seedText
            // 
            seedText.Location = new Point(506, 35);
            seedText.Name = "seedText";
            seedText.Size = new Size(100, 23);
            seedText.TabIndex = 0;
            seedText.Text = "飞船种子：";
            // 
            // buttonSetSeed
            // 
            buttonSetSeed.Location = new Point(506, 102);
            buttonSetSeed.Name = "buttonSetSeed";
            buttonSetSeed.Size = new Size(100, 33);
            buttonSetSeed.TabIndex = 0;
            buttonSetSeed.Text = "导入种子";
            buttonSetSeed.Click += buttonSetSeed_Click;
            // 
            // radioPanelS
            // 
            radioPanelS.AutoScroll = true;
            radioPanelS.Location = new Point(50, 61);
            radioPanelS.Name = "radioPanelS";
            radioPanelS.Size = new Size(426, 353);
            radioPanelS.TabIndex = 2;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(670, 560);
            progressBar.MarqueeAnimationSpeed = 30;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(145, 40);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.TabIndex = 1;
            progressBar.Visible = false;
            // 
            // aboutButton
            // 
            aboutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            aboutButton.Location = new Point(835, 560);
            aboutButton.Name = "aboutButton";
            aboutButton.Size = new Size(100, 40);
            aboutButton.TabIndex = 0;
            aboutButton.Text = "关于";
            aboutButton.Click += aboutButton_Click;
            // 
            // textBoxExportName
            // 
            textBoxExportName.Location = new Point(506, 290);
            textBoxExportName.Name = "textBox1";
            textBoxExportName.Size = new Size(358, 23);
            textBoxExportName.TabIndex = 14;
            // 
            // label1
            // 
            label1.Location = new Point(506, 264);
            label1.Name = "label1";
            label1.Size = new Size(100, 23);
            label1.TabIndex = 15;
            label1.Text = "保存文件名：";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(968, 633);
            Controls.Add(aboutButton);
            Controls.Add(progressBar);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "无人深空飞船导入与导出工具";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private Button buttonSeedShipImport;
        private Button buttonSeedShipExport;
        private CheckBox checkBoxS;
        private CheckBox checkBoxSH0;
        private Label seedShipIOText;
        private CheckBox checkBoxNewShip;
        private CheckBox checkBoxNMSSHIP1;
        private CheckBox checkBoxNMSSHIP3;
        private TextBox exportPath;
        private TextBox exportName;
        private Label pathTextE;
        private Label nameTextE;
        private Button exportSelect;
        private Button buttonExport;
        private Label label1;
        private TextBox textBoxExportName;
    }
}
