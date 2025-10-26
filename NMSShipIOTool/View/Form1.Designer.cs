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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            resources.ApplyResources(tabControl1, "tabControl1");
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(labelPath);
            tabPage1.Controls.Add(labelDescription);
            tabPage1.Controls.Add(labelShipDetected);
            tabPage1.Controls.Add(textBoxPath);
            tabPage1.Controls.Add(buttonSelect);
            tabPage1.Controls.Add(buttonLoad);
            resources.ApplyResources(tabPage1, "tabPage1");
            tabPage1.Name = "tabPage1";
            // 
            // labelPath
            // 
            resources.ApplyResources(labelPath, "labelPath");
            labelPath.Name = "labelPath";
            // 
            // labelDescription
            // 
            resources.ApplyResources(labelDescription, "labelDescription");
            labelDescription.Name = "labelDescription";
            // 
            // labelShipDetected
            // 
            resources.ApplyResources(labelShipDetected, "labelShipDetected");
            labelShipDetected.Name = "labelShipDetected";
            labelShipDetected.ReadOnly = true;
            // 
            // textBoxPath
            // 
            resources.ApplyResources(textBoxPath, "textBoxPath");
            textBoxPath.Name = "textBoxPath";
            textBoxPath.ReadOnly = true;
            // 
            // buttonSelect
            // 
            resources.ApplyResources(buttonSelect, "buttonSelect");
            buttonSelect.Name = "buttonSelect";
            buttonSelect.Click += buttonSelect_Click;
            // 
            // buttonLoad
            // 
            resources.ApplyResources(buttonLoad, "buttonLoad");
            buttonLoad.Name = "buttonLoad";
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
            resources.ApplyResources(tabPage2, "tabPage2");
            tabPage2.Name = "tabPage2";
            // 
            // checkBoxTechI
            // 
            resources.ApplyResources(checkBoxTechI, "checkBoxTechI");
            checkBoxTechI.Name = "checkBoxTechI";
            // 
            // buttonImportShipTechI
            // 
            resources.ApplyResources(buttonImportShipTechI, "buttonImportShipTechI");
            buttonImportShipTechI.Name = "buttonImportShipTechI";
            buttonImportShipTechI.Click += buttonImportShipTechI_Click;
            // 
            // buttonExportShipTechI
            // 
            resources.ApplyResources(buttonExportShipTechI, "buttonExportShipTechI");
            buttonExportShipTechI.Name = "buttonExportShipTechI";
            buttonExportShipTechI.Click += buttonExportShipTechI_Click;
            // 
            // exportPath
            // 
            resources.ApplyResources(exportPath, "exportPath");
            exportPath.Name = "exportPath";
            exportPath.ReadOnly = true;
            // 
            // exportName
            // 
            resources.ApplyResources(exportName, "exportName");
            exportName.Name = "exportName";
            // 
            // pathTextE
            // 
            resources.ApplyResources(pathTextE, "pathTextE");
            pathTextE.Name = "pathTextE";
            // 
            // nameTextE
            // 
            resources.ApplyResources(nameTextE, "nameTextE");
            nameTextE.Name = "nameTextE";
            // 
            // exportSelect
            // 
            resources.ApplyResources(exportSelect, "exportSelect");
            exportSelect.Name = "exportSelect";
            exportSelect.Click += exportSelect_Click;
            // 
            // buttonExport
            // 
            resources.ApplyResources(buttonExport, "buttonExport");
            buttonExport.Name = "buttonExport";
            buttonExport.Click += buttonExport_Click;
            // 
            // checkBoxNMSSHIP1
            // 
            checkBoxNMSSHIP1.Checked = true;
            checkBoxNMSSHIP1.CheckState = CheckState.Checked;
            resources.ApplyResources(checkBoxNMSSHIP1, "checkBoxNMSSHIP1");
            checkBoxNMSSHIP1.Name = "checkBoxNMSSHIP1";
            // 
            // importPath
            // 
            resources.ApplyResources(importPath, "importPath");
            importPath.Name = "importPath";
            importPath.ReadOnly = true;
            // 
            // inputImportText
            // 
            resources.ApplyResources(inputImportText, "inputImportText");
            inputImportText.Name = "inputImportText";
            inputImportText.TextChanged += inputImportText_TextChanged;
            // 
            // importSelect
            // 
            resources.ApplyResources(importSelect, "importSelect");
            importSelect.Name = "importSelect";
            importSelect.Click += impoerSelect_Click;
            // 
            // shipSelectI
            // 
            resources.ApplyResources(shipSelectI, "shipSelectI");
            shipSelectI.Name = "shipSelectI";
            // 
            // pathTextI
            // 
            resources.ApplyResources(pathTextI, "pathTextI");
            pathTextI.Name = "pathTextI";
            // 
            // inputTextI
            // 
            resources.ApplyResources(inputTextI, "inputTextI");
            inputTextI.Name = "inputTextI";
            // 
            // inputTextIExplanation
            // 
            resources.ApplyResources(inputTextIExplanation, "inputTextIExplanation");
            inputTextIExplanation.Name = "inputTextIExplanation";
            // 
            // buttonImport
            // 
            resources.ApplyResources(buttonImport, "buttonImport");
            buttonImport.Name = "buttonImport";
            buttonImport.Click += buttonImport_Click;
            // 
            // radioPanelI
            // 
            resources.ApplyResources(radioPanelI, "radioPanelI");
            radioPanelI.Name = "radioPanelI";
            // 
            // checkBoxI
            // 
            checkBoxI.Checked = true;
            checkBoxI.CheckState = CheckState.Checked;
            resources.ApplyResources(checkBoxI, "checkBoxI");
            checkBoxI.Name = "checkBoxI";
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
            resources.ApplyResources(tabPage3, "tabPage3");
            tabPage3.Name = "tabPage3";
            // 
            // checkBoxTech
            // 
            resources.ApplyResources(checkBoxTech, "checkBoxTech");
            checkBoxTech.Name = "checkBoxTech";
            // 
            // buttonImportShipTech
            // 
            resources.ApplyResources(buttonImportShipTech, "buttonImportShipTech");
            buttonImportShipTech.Name = "buttonImportShipTech";
            buttonImportShipTech.Click += buttonImportShipTech_Click;
            // 
            // buttonExportShipTech
            // 
            resources.ApplyResources(buttonExportShipTech, "buttonExportShipTech");
            buttonExportShipTech.Name = "buttonExportShipTech";
            buttonExportShipTech.Click += buttonExportShipTech_Click;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // textBoxExportName
            // 
            resources.ApplyResources(textBoxExportName, "textBoxExportName");
            textBoxExportName.Name = "textBoxExportName";
            // 
            // checkBoxNMSSHIP3
            // 
            checkBoxNMSSHIP3.Checked = true;
            checkBoxNMSSHIP3.CheckState = CheckState.Checked;
            resources.ApplyResources(checkBoxNMSSHIP3, "checkBoxNMSSHIP3");
            checkBoxNMSSHIP3.Name = "checkBoxNMSSHIP3";
            checkBoxNMSSHIP3.CheckedChanged += checkBoxNMSSHIP3_CheckedChanged;
            // 
            // checkBoxNewShip
            // 
            resources.ApplyResources(checkBoxNewShip, "checkBoxNewShip");
            checkBoxNewShip.Name = "checkBoxNewShip";
            // 
            // seedShipIOText
            // 
            resources.ApplyResources(seedShipIOText, "seedShipIOText");
            seedShipIOText.Name = "seedShipIOText";
            // 
            // checkBoxSH0
            // 
            resources.ApplyResources(checkBoxSH0, "checkBoxSH0");
            checkBoxSH0.Name = "checkBoxSH0";
            checkBoxSH0.CheckedChanged += checkBoxSH0_CheckedChanged;
            // 
            // checkBoxS
            // 
            checkBoxS.Checked = true;
            checkBoxS.CheckState = CheckState.Checked;
            resources.ApplyResources(checkBoxS, "checkBoxS");
            checkBoxS.Name = "checkBoxS";
            checkBoxS.CheckedChanged += checkBoxS_CheckedChanged;
            // 
            // buttonSeedShipExport
            // 
            resources.ApplyResources(buttonSeedShipExport, "buttonSeedShipExport");
            buttonSeedShipExport.Name = "buttonSeedShipExport";
            buttonSeedShipExport.Click += buttonSeedShipExport_Click;
            // 
            // buttonSeedShipImport
            // 
            resources.ApplyResources(buttonSeedShipImport, "buttonSeedShipImport");
            buttonSeedShipImport.Name = "buttonSeedShipImport";
            buttonSeedShipImport.Click += buttonSeedShipImport_Click;
            // 
            // shipSeed
            // 
            resources.ApplyResources(shipSeed, "shipSeed");
            shipSeed.Name = "shipSeed";
            // 
            // seedSelectText
            // 
            resources.ApplyResources(seedSelectText, "seedSelectText");
            seedSelectText.Name = "seedSelectText";
            // 
            // seedText
            // 
            resources.ApplyResources(seedText, "seedText");
            seedText.Name = "seedText";
            // 
            // buttonSetSeed
            // 
            resources.ApplyResources(buttonSetSeed, "buttonSetSeed");
            buttonSetSeed.Name = "buttonSetSeed";
            buttonSetSeed.Click += buttonSetSeed_Click;
            // 
            // radioPanelS
            // 
            resources.ApplyResources(radioPanelS, "radioPanelS");
            radioPanelS.Name = "radioPanelS";
            // 
            // progressBar
            // 
            resources.ApplyResources(progressBar, "progressBar");
            progressBar.MarqueeAnimationSpeed = 30;
            progressBar.Name = "progressBar";
            progressBar.Style = ProgressBarStyle.Marquee;
            // 
            // aboutButton
            // 
            resources.ApplyResources(aboutButton, "aboutButton");
            aboutButton.Name = "aboutButton";
            aboutButton.Click += aboutButton_Click;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(aboutButton);
            Controls.Add(progressBar);
            Controls.Add(tabControl1);
            Name = "Form1";
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
