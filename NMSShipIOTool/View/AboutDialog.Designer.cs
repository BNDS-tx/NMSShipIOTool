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
            title.Location = new Point(0, 0);
            title.Name = "title";
            title.Size = new Size(600, 200);
            title.TabIndex = 0;
            title.Text = "NMS Ship IO Tool\n无人深空飞船导入导出工具";
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Font = new Font("Microsoft YaHei", 20, FontStyle.Bold);
            // 
            // info
            // 
            info.Location = new Point(0, 200);
            info.Name = "info";
            info.Size = new Size(600, 200);
            info.TabIndex = 1;
            info.Text = "版本 1.1.0\n作者：蓝夜深空（BNDS-tx）\n\n\n\n本工具仅供学习和交流使用。\nNMS Ship IO Tool  Copyright (C) 2025  徐腾（蓝夜深空）";
            info.Font = new Font("Microsoft YaHei", 10);
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
            Text = "关于";
            ResumeLayout(false);

        }

        #endregion
    }
}