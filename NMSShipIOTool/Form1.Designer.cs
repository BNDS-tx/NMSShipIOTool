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
        private System.Windows.Forms.TextBox exportPath;
        private System.Windows.Forms.TextBox exportName;
        private System.Windows.Forms.TextBox importPath;
        private System.Windows.Forms.TextBox inputImportText;
        private System.Windows.Forms.TextBox shipSeed;
        private System.Windows.Forms.TextBox labelShipDetected;

        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label shipSelectE;
        private System.Windows.Forms.Label shipSelectI;
        private System.Windows.Forms.Label pathTextE;
        private System.Windows.Forms.Label nameTextE;
        private System.Windows.Forms.Label pathTextI;
        private System.Windows.Forms.Label inputTextI;
        private System.Windows.Forms.Label inputTextIExplanation;
        private System.Windows.Forms.Label seedSelectText;
        private System.Windows.Forms.Label seedText;

        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button exportSelect;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button importSelect;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button aboutButton;
        private System.Windows.Forms.Button buttonSetSeed;

        private System.Windows.Forms.CheckBox checkBoxI;
        private System.Windows.Forms.CheckBox checkBoxE;

        private System.Windows.Forms.Label progressBar1;
        private System.Windows.Forms.Label progressBar2;
        private System.Windows.Forms.Label progressBar3;
        private System.Windows.Forms.Label progressBar4;

        private System.Windows.Forms.FlowLayoutPanel radioPanelI;
        private System.Windows.Forms.FlowLayoutPanel radioPanelE;
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
            progressBar1 = new Label();
            tabPage2 = new TabPage();
            importPath = new TextBox();
            inputImportText = new TextBox();
            importSelect = new Button();
            shipSelectI = new Label();
            pathTextI = new Label();
            inputTextI = new Label();
            inputTextIExplanation = new Label();
            buttonImport = new Button();
            progressBar2 = new Label();
            radioPanelI = new FlowLayoutPanel();
            checkBoxI = new CheckBox();
            tabPage3 = new TabPage();
            exportPath = new TextBox();
            exportName = new TextBox();
            shipSelectE = new Label();
            pathTextE = new Label();
            nameTextE = new Label();
            exportSelect = new Button();
            buttonExport = new Button();
            progressBar3 = new Label();
            radioPanelE = new FlowLayoutPanel();
            checkBoxE = new CheckBox();
            tabPage4 = new TabPage();
            shipSeed = new TextBox();
            seedSelectText = new Label();
            seedText = new Label();
            buttonSetSeed = new Button();
            progressBar4 = new Label();
            radioPanelS = new FlowLayoutPanel();
            aboutButton = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage4.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
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
            tabPage1.Controls.Add(progressBar1);
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
            // progressBar1
            // 
            progressBar1.Location = new Point(350, 438);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(279, 23);
            progressBar1.TabIndex = 5;
            progressBar1.Text = "正在加载，请稍候...";
            progressBar1.Visible = false;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(importPath);
            tabPage2.Controls.Add(inputImportText);
            tabPage2.Controls.Add(importSelect);
            tabPage2.Controls.Add(shipSelectI);
            tabPage2.Controls.Add(pathTextI);
            tabPage2.Controls.Add(inputTextI);
            tabPage2.Controls.Add(inputTextIExplanation);
            tabPage2.Controls.Add(buttonImport);
            tabPage2.Controls.Add(progressBar2);
            tabPage2.Controls.Add(radioPanelI);
            tabPage2.Controls.Add(checkBoxI);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(937, 580);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "自定义飞船导入";
            // 
            // importPath
            // 
            importPath.Location = new Point(116, 388);
            importPath.Name = "importPath";
            importPath.ReadOnly = true;
            importPath.Size = new Size(360, 23);
            importPath.TabIndex = 0;
            // 
            // inputImportText
            // 
            inputImportText.Location = new Point(506, 78);
            inputImportText.Multiline = true;
            inputImportText.Name = "inputImportText";
            inputImportText.ScrollBars = ScrollBars.Both;
            inputImportText.Size = new Size(390, 333);
            inputImportText.TabIndex = 1;
            inputImportText.TextChanged += inputImportText_TextChanged;
            // 
            // importSelect
            // 
            importSelect.Location = new Point(116, 430);
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
            pathTextI.Location = new Point(12, 391);
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
            inputTextIExplanation.Location = new Point(506, 435);
            inputTextIExplanation.Name = "inputTextIExplanation";
            inputTextIExplanation.Size = new Size(390, 23);
            inputTextIExplanation.TabIndex = 4;
            inputTextIExplanation.Text = "请遵守 JSON 格式，并确保在最外围包含中括号”[]“";
            inputTextIExplanation.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonImport
            // 
            buttonImport.Location = new Point(231, 430);
            buttonImport.Name = "buttonImport";
            buttonImport.Size = new Size(100, 33);
            buttonImport.TabIndex = 6;
            buttonImport.Text = "导入飞船";
            buttonImport.Click += buttonImport_Click;
            // 
            // progressBar2
            // 
            progressBar2.Location = new Point(350, 438);
            progressBar2.Name = "progressBar2";
            progressBar2.Size = new Size(279, 23);
            progressBar2.TabIndex = 7;
            progressBar2.Text = "正在导入，请稍候...";
            progressBar2.Visible = false;
            // 
            // radioPanelI
            // 
            radioPanelI.AutoScroll = true;
            radioPanelI.Location = new Point(50, 78);
            radioPanelI.Name = "radioPanelI";
            radioPanelI.Size = new Size(426, 286);
            radioPanelI.TabIndex = 8;
            // 
            // checkBoxI
            // 
            checkBoxI.Checked = true;
            checkBoxI.CheckState = CheckState.Checked;
            checkBoxI.Location = new Point(118, 480);
            checkBoxI.Name = "checkBoxI";
            checkBoxI.Size = new Size(360, 24);
            checkBoxI.TabIndex = 9;
            checkBoxI.Text = "启用混淆";
            checkBoxI.CheckedChanged += checkBoxI_CheckedChanged;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(exportPath);
            tabPage3.Controls.Add(exportName);
            tabPage3.Controls.Add(shipSelectE);
            tabPage3.Controls.Add(pathTextE);
            tabPage3.Controls.Add(nameTextE);
            tabPage3.Controls.Add(exportSelect);
            tabPage3.Controls.Add(buttonExport);
            tabPage3.Controls.Add(progressBar3);
            tabPage3.Controls.Add(radioPanelE);
            tabPage3.Controls.Add(checkBoxE);
            tabPage3.Location = new Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(937, 580);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "自定义飞船导出";
            // 
            // exportPath
            // 
            exportPath.Location = new Point(118, 351);
            exportPath.Name = "exportPath";
            exportPath.ReadOnly = true;
            exportPath.Size = new Size(511, 23);
            exportPath.TabIndex = 0;
            // 
            // exportName
            // 
            exportName.Location = new Point(118, 388);
            exportName.Name = "exportName";
            exportName.Size = new Size(511, 23);
            exportName.TabIndex = 0;
            // 
            // shipSelectE
            // 
            shipSelectE.Location = new Point(12, 35);
            shipSelectE.Name = "shipSelectE";
            shipSelectE.Size = new Size(133, 23);
            shipSelectE.TabIndex = 0;
            shipSelectE.Text = "选择你要导出的飞船：";
            // 
            // pathTextE
            // 
            pathTextE.Location = new Point(12, 354);
            pathTextE.Name = "pathTextE";
            pathTextE.Size = new Size(100, 23);
            pathTextE.TabIndex = 0;
            pathTextE.Text = "导出文件地址：";
            // 
            // nameTextE
            // 
            nameTextE.Location = new Point(12, 391);
            nameTextE.Name = "nameTextE";
            nameTextE.Size = new Size(100, 23);
            nameTextE.TabIndex = 0;
            nameTextE.Text = "导出文件名：";
            // 
            // exportSelect
            // 
            exportSelect.Location = new Point(116, 430);
            exportSelect.Name = "exportSelect";
            exportSelect.Size = new Size(100, 33);
            exportSelect.TabIndex = 0;
            exportSelect.Text = "选择保存地址";
            exportSelect.Click += exportSelect_Click;
            // 
            // buttonExport
            // 
            buttonExport.Location = new Point(231, 430);
            buttonExport.Name = "buttonExport";
            buttonExport.Size = new Size(100, 33);
            buttonExport.TabIndex = 0;
            buttonExport.Text = "导出飞船";
            buttonExport.Click += buttonExport_Click;
            // 
            // progressBar3
            // 
            progressBar3.Location = new Point(350, 438);
            progressBar3.Name = "progressBar3";
            progressBar3.Size = new Size(279, 23);
            progressBar3.TabIndex = 1;
            progressBar3.Text = "正在导出，请稍候...";
            progressBar3.Visible = false;
            // 
            // radioPanelE
            // 
            radioPanelE.AutoScroll = true;
            radioPanelE.Location = new Point(50, 78);
            radioPanelE.Name = "radioPanelE";
            radioPanelE.Size = new Size(426, 248);
            radioPanelE.TabIndex = 2;
            // 
            // checkBoxE
            // 
            checkBoxE.Checked = true;
            checkBoxE.CheckState = CheckState.Checked;
            checkBoxE.Location = new Point(118, 480);
            checkBoxE.Name = "checkBoxE";
            checkBoxE.Size = new Size(360, 24);
            checkBoxE.TabIndex = 3;
            checkBoxE.Text = "启用反混淆";
            checkBoxE.CheckedChanged += checkBoxE_CheckedChanged;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(shipSeed);
            tabPage4.Controls.Add(seedSelectText);
            tabPage4.Controls.Add(seedText);
            tabPage4.Controls.Add(buttonSetSeed);
            tabPage4.Controls.Add(progressBar4);
            tabPage4.Controls.Add(radioPanelS);
            tabPage4.Location = new Point(4, 26);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(937, 580);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "种子飞船";
            // 
            // shipSeed
            // 
            shipSeed.Location = new Point(118, 388);
            shipSeed.Name = "shipSeed";
            shipSeed.Size = new Size(511, 23);
            shipSeed.TabIndex = 0;
            shipSeed.TextChanged += shipSeed_TextChanged;
            // 
            // seedSelectText
            // 
            seedSelectText.Location = new Point(12, 35);
            seedSelectText.Name = "seedSelectText";
            seedSelectText.Size = new Size(184, 23);
            seedSelectText.TabIndex = 0;
            seedSelectText.Text = "选择你要导入/导出种子的飞船：";
            // 
            // seedText
            // 
            seedText.Location = new Point(12, 391);
            seedText.Name = "seedText";
            seedText.Size = new Size(100, 23);
            seedText.TabIndex = 0;
            seedText.Text = "飞船种子：";
            // 
            // buttonSetSeed
            // 
            buttonSetSeed.Location = new Point(116, 430);
            buttonSetSeed.Name = "buttonSetSeed";
            buttonSetSeed.Size = new Size(100, 33);
            buttonSetSeed.TabIndex = 0;
            buttonSetSeed.Text = "导入种子";
            buttonSetSeed.Click += buttonSetSeed_Click;
            // 
            // progressBar4
            // 
            progressBar4.Location = new Point(350, 438);
            progressBar4.Name = "progressBar4";
            progressBar4.Size = new Size(279, 23);
            progressBar4.TabIndex = 1;
            progressBar4.Text = "正在导入，请稍候...";
            progressBar4.Visible = false;
            // 
            // radioPanelS
            // 
            radioPanelS.AutoScroll = true;
            radioPanelS.Location = new Point(50, 78);
            radioPanelS.Name = "radioPanelS";
            radioPanelS.Size = new Size(579, 286);
            radioPanelS.TabIndex = 2;
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(968, 633);
            Controls.Add(aboutButton);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "无人深空自定义飞船导入与导出工具";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
    }
}
