using NMSShipIOTool.Resources;

namespace NMSShipIOTool
{
    partial class AboutDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Label info;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            title = new Label();
            info = new Label();
            SuspendLayout();
            // 
            // title
            // 
            title.Font = new Font("微软雅黑", 20F, FontStyle.Bold);
            title.Location = new Point(0, 0);
            title.Name = "title";
            title.Size = new Size(600, 200);
            title.TabIndex = 0;
            title.Text = Language.AboutDialog_title_Text;
            title.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // info
            // 
            info.Font = new Font("微软雅黑", 10F);
            info.Location = new Point(0, 200);
            info.Name = "info";
            info.Size = new Size(600, 200);
            info.TabIndex = 1;
            info.Text = Language.AboutDialog_info_Text;
            info.TextAlign = ContentAlignment.TopCenter;
            // 
            // AboutDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 400);
            Controls.Add(title);
            Controls.Add(info);
            Name = "AboutDialog";
            Text = Language.AboutDialog_this_Text;
            ResumeLayout(false);

        }

        #endregion
    }
}