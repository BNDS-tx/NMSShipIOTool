using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NMSShipIOTool
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();

            // 禁止调整窗口大小
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 设置窗口固定大小（例如 800x600，可根据实际需求调整）
            this.MaximumSize = new Size(600, 400);
            this.MinimumSize = new Size(600, 400);

        }
    }
}
