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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            title = new Label();
            info = new Label();
            SuspendLayout();
            // 
            // title
            // 
            resources.ApplyResources(title, "title");
            title.Name = "title";
            // 
            // info
            // 
            resources.ApplyResources(info, "info");
            info.Name = "info";
            // 
            // AboutDialog
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(title);
            Controls.Add(info);
            Name = "AboutDialog";
            ResumeLayout(false);

        }

        #endregion
    }
}