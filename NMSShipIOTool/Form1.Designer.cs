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
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.Button buttonLoad;

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
            textBoxPath = new TextBox();
            buttonSelect = new Button();
            buttonLoad = new Button();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
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
            tabControl1.Size = new Size(944, 609);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(labelPath);
            tabPage1.Controls.Add(labelDescription);
            tabPage1.Controls.Add(textBoxPath);
            tabPage1.Controls.Add(buttonSelect);
            tabPage1.Controls.Add(buttonLoad);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(936, 579);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "游戏存档";
            // 
            // labelPath
            // 
            labelPath.Location = new Point(12, 35);
            labelPath.Name = "labelPath";
            labelPath.Size = new Size(60, 23);
            labelPath.TabIndex = 0;
            labelPath.Text = "存档路径:";
            // 
            // labelDescription
            // 
            labelDescription.Location = new Point(78, 108);
            labelDescription.Name = "labelDescription";
            labelDescription.Size = new Size(750, 83);
            labelDescription.TabIndex = 1;
            // 
            // textBoxPath
            // 
            textBoxPath.Location = new Point(78, 32);
            textBoxPath.Name = "textBoxPath";
            textBoxPath.ReadOnly = true;
            textBoxPath.Size = new Size(750, 23);
            textBoxPath.TabIndex = 2;
            // 
            // buttonSelect
            // 
            buttonSelect.Location = new Point(78, 67);
            buttonSelect.Name = "buttonSelect";
            buttonSelect.Size = new Size(100, 33);
            buttonSelect.TabIndex = 3;
            buttonSelect.Text = "选择路径";
            buttonSelect.Click += buttonSelect_Click;
            // 
            // buttonLoad
            // 
            buttonLoad.Location = new Point(201, 67);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(100, 33);
            buttonLoad.TabIndex = 3;
            buttonLoad.Text = "加载存档";
            buttonLoad.Click += buttonLoad_Click;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(936, 579);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "自定义飞船导入";
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(936, 579);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "自定义飞船导出";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(968, 633);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
    }
}
