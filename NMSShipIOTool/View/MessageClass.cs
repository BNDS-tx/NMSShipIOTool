using NMSShipIOTool.Resources;

namespace NMSShipIOTool.View
{
    public class MessageClass
    {
        public static void ErrorMessageBox(string text)
        {
            MessageBox.Show(text, Language.错误, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void InfoMessageBox(string text)
        {
            MessageBox.Show(text, Language.提示, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult WarningMessageBox(string text)
        {
            return MessageBox.Show(text, Language.警告, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        }
    }
}
