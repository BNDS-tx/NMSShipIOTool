using libNOM.io.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class ChoiceDialog : Form
{
    private FlowLayoutPanel radioPanel;
    private Button okButton;
    private Button cancelButton;

    public int SelectedOption { get; private set; } = 0;

    private List<IContainer> saves;

    public ChoiceDialog(string title, List<IContainer> saves)
    {
        this.saves = saves;
        this.Text = title ?? "请选择一个选项";
        this.Width = 400;
        this.Height = 350;
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        radioPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 200,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };

        // 动态添加 RadioButton
        for (int i = 0; i < saves.Count; i++)
        {
            var radio = new RadioButton
            {
                Text = saves[i].ToString(),
                Tag = i, // 存储索引
                AutoSize = true,
                Padding = new Padding(10, 0, 10, 0),
            };
            radioPanel.Controls.Add(radio);
        }
        this.Controls.Add(radioPanel);

        FlowLayoutPanel panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 40
        };

        okButton = new Button { Text = "确定", Width = 80 };
        cancelButton = new Button { Text = "取消", Width = 80 };

        okButton.Click += (s, e) =>
        {
            foreach (Control ctrl in radioPanel.Controls)
            {
                if (ctrl is RadioButton rb && rb.Checked)
                {
                    SelectedOption = (int)rb.Tag;
                    this.DialogResult = DialogResult.OK;
                    return;
                }
            }
            MessageBox.Show("请先选择一个选项！");
        };

        cancelButton.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; };

        panel.Controls.Add(okButton);
        panel.Controls.Add(cancelButton);

        this.Controls.Add(panel);
    }
}

