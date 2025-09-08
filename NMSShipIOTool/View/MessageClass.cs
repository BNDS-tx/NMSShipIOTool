
namespace NMSShipIOTool.View
{
    public class MessageClass
    {
        public static void ErrorMessageBox(string text) 
        { 
            MessageBox.Show(text, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void InfoMessageBox(string text) 
        {
            MessageBox.Show(text, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult WarningMessageBox(string text)
        {
            return MessageBox.Show(text, "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        }
    }
}
