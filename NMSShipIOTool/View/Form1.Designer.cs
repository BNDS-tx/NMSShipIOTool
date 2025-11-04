using NMSShipIOTool.Resources;
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
            checkBoxTechI = new CheckBox();
            buttonImportShipTechI = new Button();
            buttonExportShipTechI = new Button();
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
            checkBoxTech = new CheckBox();
            buttonImportShipTech = new Button();
            buttonExportShipTech = new Button();
            label1 = new Label();
            textBoxExportName = new TextBox();
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
            tabControl1.Size = new Size(976, 610);
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
            tabPage1.Size = new Size(968, 580);
            tabPage1.TabIndex = 0;
            tabPage1.Text = Language.Form1_tabPage1_Text;
            // 
            // labelPath
            // 
            labelPath.Location = new Point(12, 391);
            labelPath.Name = "labelPath";
            labelPath.Size = new Size(60, 23);
            labelPath.TabIndex = 0;
            labelPath.Text = Language.Form1_labelPath_Text;
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
            labelShipDetected.Size = new Size(612, 319);
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
            buttonSelect.Text = Language.Form1_buttonSelect_Text;
            buttonSelect.Click += buttonSelect_Click;
            // 
            // buttonLoad
            // 
            buttonLoad.Location = new Point(231, 430);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(100, 33);
            buttonLoad.TabIndex = 4;
            buttonLoad.Text = Language.Form1_buttonLoad_Text;
            buttonLoad.Click += buttonLoad_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(checkBoxTechI);
            tabPage2.Controls.Add(buttonImportShipTechI);
            tabPage2.Controls.Add(buttonExportShipTechI);
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
            tabPage2.Size = new Size(968, 580);
            tabPage2.TabIndex = 1;
            tabPage2.Text = Language.Form1_tabPage2_Text;
            // 
            // checkBoxTechI
            // 
            checkBoxTechI.Location = new Point(287, 480);
            checkBoxTechI.Name = "checkBoxTechI";
            checkBoxTechI.Size = new Size(211, 24);
            checkBoxTechI.TabIndex = 21;
            checkBoxTechI.Text = Language.Form1_checkBoxTechI_Text;
            // 
            // buttonImportShipTechI
            // 
            buttonImportShipTechI.Location = new Point(736, 430);
            buttonImportShipTechI.Name = "buttonImportShipTechI";
            buttonImportShipTechI.Size = new Size(189, 33);
            buttonImportShipTechI.TabIndex = 20;
            buttonImportShipTechI.Text = Language.Form1_buttonImportShipTechI_Text;
            buttonImportShipTechI.Click += buttonImportShipTechI_Click;
            // 
            // buttonExportShipTechI
            // 
            buttonExportShipTechI.Location = new Point(287, 430);
            buttonExportShipTechI.Name = "buttonExportShipTechI";
            buttonExportShipTechI.Size = new Size(189, 33);
            buttonExportShipTechI.TabIndex = 19;
            buttonExportShipTechI.Text = Language.Form1_buttonExportShipTechI_Text;
            buttonExportShipTechI.Click += buttonExportShipTechI_Click;
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
            pathTextE.Text = Language.Form1_pathTextE_Text;
            // 
            // nameTextE
            // 
            nameTextE.Location = new Point(12, 391);
            nameTextE.Name = "nameTextE";
            nameTextE.Size = new Size(100, 23);
            nameTextE.TabIndex = 14;
            nameTextE.Text = Language.Form1_nameTextE_Text;
            // 
            // exportSelect
            // 
            exportSelect.Location = new Point(58, 430);
            exportSelect.Name = "exportSelect";
            exportSelect.Size = new Size(100, 33);
            exportSelect.TabIndex = 15;
            exportSelect.Text = Language.Form1_exportSelect_Text;
            exportSelect.Click += exportSelect_Click;
            // 
            // buttonExport
            // 
            buttonExport.Location = new Point(173, 430);
            buttonExport.Name = "buttonExport";
            buttonExport.Size = new Size(100, 33);
            buttonExport.TabIndex = 16;
            buttonExport.Text = Language.Form1_buttonExport_Text;
            buttonExport.Click += buttonExport_Click;
            // 
            // checkBoxNMSSHIP1
            // 
            checkBoxNMSSHIP1.Checked = true;
            checkBoxNMSSHIP1.CheckState = CheckState.Checked;
            checkBoxNMSSHIP1.Location = new Point(173, 480);
            checkBoxNMSSHIP1.Name = "checkBoxNMSSHIP1";
            checkBoxNMSSHIP1.Size = new Size(115, 24);
            checkBoxNMSSHIP1.TabIndex = 10;
            checkBoxNMSSHIP1.Text = Language.Form1_checkBoxNMSSHIP1_Text;
            // 
            // importPath
            // 
            importPath.Location = new Point(506, 388);
            importPath.Name = "importPath";
            importPath.ReadOnly = true;
            importPath.Size = new Size(419, 23);
            importPath.TabIndex = 0;
            // 
            // inputImportText
            // 
            inputImportText.Location = new Point(506, 61);
            inputImportText.Multiline = true;
            inputImportText.Name = "inputImportText";
            inputImportText.ScrollBars = ScrollBars.Both;
            inputImportText.Size = new Size(419, 271);
            inputImportText.TabIndex = 1;
            inputImportText.TextChanged += inputImportText_TextChanged;
            // 
            // importSelect
            // 
            importSelect.Location = new Point(506, 430);
            importSelect.Name = "importSelect";
            importSelect.Size = new Size(100, 33);
            importSelect.TabIndex = 2;
            importSelect.Text = Language.Form1_importSelect_Text;
            importSelect.Click += impoerSelect_Click;
            // 
            // shipSelectI
            // 
            shipSelectI.Location = new Point(12, 35);
            shipSelectI.Name = "shipSelectI";
            shipSelectI.Size = new Size(464, 23);
            shipSelectI.TabIndex = 5;
            shipSelectI.Text = Language.Form1_shipSelectI_Text;
            // 
            // pathTextI
            // 
            pathTextI.Location = new Point(506, 354);
            pathTextI.Name = "pathTextI";
            pathTextI.Size = new Size(98, 23);
            pathTextI.TabIndex = 3;
            pathTextI.Text = Language.Form1_pathTextI_Text;
            // 
            // inputTextI
            // 
            inputTextI.Location = new Point(506, 35);
            inputTextI.Name = "inputTextI";
            inputTextI.Size = new Size(419, 23);
            inputTextI.TabIndex = 4;
            inputTextI.Text = Language.Form1_inputTextI_Text;
            // 
            // inputTextIExplanation
            // 
            inputTextIExplanation.Location = new Point(506, 480);
            inputTextIExplanation.Name = "inputTextIExplanation";
            inputTextIExplanation.Size = new Size(390, 23);
            inputTextIExplanation.TabIndex = 4;
            inputTextIExplanation.Text = Language.Form1_inputTextIExplanation_Text;
            inputTextIExplanation.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonImport
            // 
            buttonImport.Location = new Point(621, 430);
            buttonImport.Name = "buttonImport";
            buttonImport.Size = new Size(100, 33);
            buttonImport.TabIndex = 6;
            buttonImport.Text = Language.Form1_buttonImport_Text;
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
            checkBoxI.Location = new Point(58, 480);
            checkBoxI.Name = "checkBoxI";
            checkBoxI.Size = new Size(98, 24);
            checkBoxI.TabIndex = 9;
            checkBoxI.Text = Language.Form1_checkBoxI_Text;
            checkBoxI.CheckedChanged += checkBoxI_CheckedChanged;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(checkBoxTech);
            tabPage3.Controls.Add(buttonImportShipTech);
            tabPage3.Controls.Add(buttonExportShipTech);
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
            tabPage3.Size = new Size(968, 580);
            tabPage3.TabIndex = 3;
            tabPage3.Text = Language.Form1_tabPage3_Text;
            // 
            // checkBoxTech
            // 
            checkBoxTech.Location = new Point(620, 390);
            checkBoxTech.Name = "checkBoxTech";
            checkBoxTech.Size = new Size(211, 24);
            checkBoxTech.TabIndex = 18;
            checkBoxTech.Text = Language.Form1_checkBoxTech_Text;
            // 
            // buttonImportShipTech
            // 
            buttonImportShipTech.Location = new Point(620, 207);
            buttonImportShipTech.Name = "buttonImportShipTech";
            buttonImportShipTech.Size = new Size(211, 33);
            buttonImportShipTech.TabIndex = 17;
            buttonImportShipTech.Text = Language.Form1_buttonImportShipTech_Text;
            buttonImportShipTech.Click += buttonImportShipTech_Click;
            // 
            // buttonExportShipTech
            // 
            buttonExportShipTech.Location = new Point(620, 332);
            buttonExportShipTech.Name = "buttonExportShipTech";
            buttonExportShipTech.Size = new Size(211, 33);
            buttonExportShipTech.TabIndex = 16;
            buttonExportShipTech.Text = Language.Form1_buttonExportShipTech_Text;
            buttonExportShipTech.Click += buttonExportShipTech_Click;
            // 
            // label1
            // 
            label1.Location = new Point(506, 264);
            label1.Name = "label1";
            label1.Size = new Size(100, 23);
            label1.TabIndex = 15;
            label1.Text = Language.Form1_label1_Text;
            // 
            // textBoxExportName
            // 
            textBoxExportName.Location = new Point(506, 290);
            textBoxExportName.Name = "textBoxExportName";
            textBoxExportName.Size = new Size(419, 23);
            textBoxExportName.TabIndex = 14;
            // 
            // checkBoxNMSSHIP3
            // 
            checkBoxNMSSHIP3.Checked = true;
            checkBoxNMSSHIP3.CheckState = CheckState.Checked;
            checkBoxNMSSHIP3.Location = new Point(506, 420);
            checkBoxNMSSHIP3.Name = "checkBoxNMSSHIP3";
            checkBoxNMSSHIP3.Size = new Size(112, 24);
            checkBoxNMSSHIP3.TabIndex = 11;
            checkBoxNMSSHIP3.Text = Language.Form1_checkBoxNMSSHIP3_Text;
            checkBoxNMSSHIP3.CheckedChanged += checkBoxNMSSHIP3_CheckedChanged;
            // 
            // checkBoxNewShip
            // 
            checkBoxNewShip.Location = new Point(506, 177);
            checkBoxNewShip.Name = "checkBoxNewShip";
            checkBoxNewShip.Size = new Size(112, 24);
            checkBoxNewShip.TabIndex = 13;
            checkBoxNewShip.Text = Language.Form1_checkBoxNewShip_Text;
            // 
            // seedShipIOText
            // 
            seedShipIOText.Location = new Point(506, 151);
            seedShipIOText.Name = "seedShipIOText";
            seedShipIOText.Size = new Size(190, 23);
            seedShipIOText.TabIndex = 12;
            seedShipIOText.Text = Language.Form1_seedShipIOText_Text;
            // 
            // checkBoxSH0
            // 
            checkBoxSH0.Location = new Point(506, 450);
            checkBoxSH0.Name = "checkBoxSH0";
            checkBoxSH0.Size = new Size(100, 24);
            checkBoxSH0.TabIndex = 11;
            checkBoxSH0.Text = Language.Form1_checkBoxSH0_Text;
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
            checkBoxS.Text = Language.Form1_checkBoxS_Text;
            checkBoxS.CheckedChanged += checkBoxS_CheckedChanged;
            // 
            // buttonSeedShipExport
            // 
            buttonSeedShipExport.Location = new Point(506, 332);
            buttonSeedShipExport.Name = "buttonSeedShipExport";
            buttonSeedShipExport.Size = new Size(100, 33);
            buttonSeedShipExport.TabIndex = 1;
            buttonSeedShipExport.Text = Language.Form1_buttonSeedShipExport_Text;
            buttonSeedShipExport.Click += buttonSeedShipExport_Click;
            // 
            // buttonSeedShipImport
            // 
            buttonSeedShipImport.Location = new Point(506, 207);
            buttonSeedShipImport.Name = "buttonSeedShipImport";
            buttonSeedShipImport.Size = new Size(100, 33);
            buttonSeedShipImport.TabIndex = 10;
            buttonSeedShipImport.Text = Language.Form1_buttonSeedShipImport_Text;
            buttonSeedShipImport.Click += buttonSeedShipImport_Click;
            // 
            // shipSeed
            // 
            shipSeed.Location = new Point(506, 61);
            shipSeed.Name = "shipSeed";
            shipSeed.Size = new Size(419, 23);
            shipSeed.TabIndex = 0;
            // 
            // seedSelectText
            // 
            seedSelectText.Location = new Point(12, 35);
            seedSelectText.Name = "seedSelectText";
            seedSelectText.Size = new Size(464, 23);
            seedSelectText.TabIndex = 0;
            seedSelectText.Text = Language.Form1_seedSelectText_Text;
            // 
            // seedText
            // 
            seedText.Location = new Point(506, 35);
            seedText.Name = "seedText";
            seedText.Size = new Size(100, 23);
            seedText.TabIndex = 0;
            seedText.Text = Language.Form1_seedText_Text;
            // 
            // buttonSetSeed
            // 
            buttonSetSeed.Location = new Point(506, 102);
            buttonSetSeed.Name = "buttonSetSeed";
            buttonSetSeed.Size = new Size(100, 33);
            buttonSetSeed.TabIndex = 0;
            buttonSetSeed.Text = Language.Form1_buttonSetSeed_Text;
            buttonSetSeed.Click += buttonSetSeed_Click;
            // 
            // radioPanelS
            // 
            radioPanelS.AutoScroll = true;
            radioPanelS.Location = new Point(50, 61);
            radioPanelS.Name = "radioPanelS";
            radioPanelS.Size = new Size(426, 413);
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
            aboutButton.Location = new Point(867, 560);
            aboutButton.Name = "aboutButton";
            aboutButton.Size = new Size(100, 40);
            aboutButton.TabIndex = 0;
            aboutButton.Text = Language.Form1_aboutButton_Text;
            aboutButton.Click += aboutButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 633);
            Controls.Add(aboutButton);
            Controls.Add(progressBar);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = Language.Form1_this_Text;
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
        private Button buttonImportShipTech;
        private Button buttonExportShipTech;
        private CheckBox checkBoxTech;
        private CheckBox checkBoxTechI;
        private Button buttonImportShipTechI;
        private Button buttonExportShipTechI;
    }
}
