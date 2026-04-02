using libNOM.io.Enums;
using libNOM.io.Interfaces;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NMSShipIOTool.Model;
using NMSShipIOTool.Resources;
using NMSShipIOTool.View;
using NMSShipIOTool.Views;
using System.Globalization;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinRT.Interop;

namespace NMSShipIOTool;

public sealed partial class MainWindow : Window
{
    private string savePath = "";
    private string importPathString = "";
    private string exportPathString = "";

    private readonly SaveLoader saveLoader = new();
    private readonly DispatcherQueue _dispatcher;
    private readonly LoadTabPage LoadTab;
    private readonly ImportTabPage ImportTab;
    private readonly SeedTabPage SeedTab;

    public MainWindow()
    {
        StartupLog.LogBoot("MainWindow ctor enter");
        try
        {
            InitializeComponent();
            StartupLog.LogBoot("MainWindow after InitializeComponent");

            var exePath = Environment.ProcessPath ?? "";
            if (!string.IsNullOrEmpty(exePath))
            {
                var hIcon = ExtractIcon(IntPtr.Zero, exePath, 0);
                if (hIcon != IntPtr.Zero && hIcon != (IntPtr)1)
                    AppWindow.SetIcon(Microsoft.UI.Win32Interop.GetIconIdFromIcon(hIcon));
            }

            InitWindowSize(1200, 780);

            StartupLog.LogBoot("before new LoadTabPage()");
            LoadTab = new LoadTabPage();
            StartupLog.LogBoot("after new LoadTabPage()");

            StartupLog.LogBoot("before new ImportTabPage()");
            ImportTab = new ImportTabPage();
            StartupLog.LogBoot("after new ImportTabPage()");

            StartupLog.LogBoot("before new SeedTabPage()");
            SeedTab = new SeedTabPage();
            StartupLog.LogBoot("after new SeedTabPage()");

            NavRoot.SelectionChanged += NavRoot_SelectionChanged;
            NavRoot.SelectedItem = NavItem1;
            _dispatcher = DispatcherQueue.GetForCurrentThread();
            Closed += (_, _) => MessageNotifier.Info = null;

            ImportTab.checkBoxI.IsChecked = true;
            ImportTab.checkBoxNMSSHIP1.IsChecked = true;
            SeedTab.checkBoxS.IsChecked = true;
            SeedTab.checkBoxNMSSHIP3.IsChecked = true;

            LoadTab.buttonSelect.Click += ButtonSelect_Click;
            LoadTab.buttonLoad.Click += ButtonLoad_Click;
            ImportTab.inputImportText.TextChanged += InputImportText_TextChanged;
            ImportTab.importSelect.Click += ImportSelect_Click;
            ImportTab.buttonExport.Click += ButtonExport_Click;
            ImportTab.exportSelect.Click += ExportSelect_Click;
            ImportTab.buttonImport.Click += ButtonImport_Click;
            SeedTab.buttonSetSeed.Click += ButtonSetSeed_Click;
            ImportTab.checkBoxI.Click += CheckBoxI_Unchecked;
            SeedTab.checkBoxS.Click += CheckBoxS_Unchecked;
            ImportTab.checkBoxI.Checked += CheckBox_Checked;
            SeedTab.checkBoxS.Checked += CheckBox_Checked;
            langMenuEN.Click += (_, _) => SwitchLanguage("en-US");
            langMenuZH.Click += (_, _) => SwitchLanguage("zh-CN");
            aboutButton.Click += AboutButton_Click;
            SeedTab.checkBoxSH0.Checked += CheckBoxSH0_Checked;
            SeedTab.checkBoxNMSSHIP3.Checked += CheckBoxNMSSHIP3_Checked;
            SeedTab.buttonImportShipTech.Click += ButtonImportShipTech_Click;
            SeedTab.buttonExportShipTech.Click += ButtonExportShipTech_Click;
            ImportTab.buttonExportShipTechI.Click += ButtonExportShipTechI_Click;
            ImportTab.buttonImportShipTechI.Click += ButtonImportShipTechI_Click;
            SeedTab.buttonSeedShipImport.Click += ButtonSeedShipImport_Click;
            SeedTab.buttonSeedShipExport.Click += ButtonSeedShipExport_Click;

            Activated += MainWindow_FirstActivated;

            ApplyLanguage();
            updateTabEnabled(false);
            StartupLog.LogBoot("MainWindow ctor exit");
        }
        catch (Exception ex)
        {
            StartupLog.LogBoot($"MainWindow ctor failed: {ex}");
            throw;
        }
    }

    private void ApplyLanguage()
    {
        NavItem1.Content = Language.Form1_tabPage1_Text;
        NavItem2.Content = Language.Form1_tabPage2_Text;
        NavItem3.Content = Language.Form1_tabPage3_Text;
        Title = Language.Form1_this_Text;

        LoadTab.labelPath.Text = Language.Form1_labelPath_Text;
        LoadTab.buttonSelect.Content = Language.Form1_buttonSelect_Text;
        LoadTab.buttonLoad.Content = Language.Form1_buttonLoad_Text;
        ImportTab.shipSelectI.Text = Language.Form1_shipSelectI_Text;
        ImportTab.inputTextI.Text = Language.Form1_inputTextI_Text;
        ImportTab.pathTextI.Text = Language.Form1_pathTextI_Text;
        ImportTab.pathTextE.Text = Language.Form1_pathTextE_Text;
        ImportTab.nameTextE.Text = Language.Form1_nameTextE_Text;
        ImportTab.exportSelect.Content = Language.Form1_exportSelect_Text;
        ImportTab.buttonExport.Content = Language.Form1_buttonExport_Text;
        ImportTab.importSelect.Content = Language.Form1_importSelect_Text;
        ImportTab.buttonImport.Content = Language.Form1_buttonImport_Text;
        ImportTab.inputTextIExplanation.Text = Language.Form1_inputTextIExplanation_Text;
        ImportTab.checkBoxI.Content = Language.Form1_checkBoxI_Text;
        ImportTab.checkBoxNMSSHIP1.Content = Language.Form1_checkBoxNMSSHIP1_Text;
        ImportTab.checkBoxTechI.Content = Language.Form1_checkBoxTechI_Text;
        ImportTab.buttonExportShipTechI.Content = Language.Form1_buttonExportShipTechI_Text;
        ImportTab.buttonImportShipTechI.Content = Language.Form1_buttonImportShipTechI_Text;
        SeedTab.seedSelectText.Text = Language.Form1_seedSelectText_Text;
        SeedTab.seedText.Text = Language.Form1_seedText_Text;
        SeedTab.buttonSetSeed.Content = Language.Form1_buttonSetSeed_Text;
        SeedTab.seedShipIOText.Text = Language.Form1_seedShipIOText_Text;
        SeedTab.checkBoxNewShip.Content = Language.Form1_checkBoxNewShip_Text;
        SeedTab.buttonSeedShipImport.Content = Language.Form1_buttonSeedShipImport_Text;
        SeedTab.buttonSeedShipExport.Content = Language.Form1_buttonSeedShipExport_Text;
        SeedTab.label1.Text = Language.Form1_label1_Text;
        SeedTab.buttonExportShipTech.Content = Language.Form1_buttonExportShipTech_Text;
        SeedTab.buttonImportShipTech.Content = Language.Form1_buttonImportShipTech_Text;
        SeedTab.checkBoxS.Content = Language.Form1_checkBoxS_Text;
        SeedTab.checkBoxTech.Content = Language.Form1_checkBoxTech_Text;
        SeedTab.checkBoxNMSSHIP3.Content = Language.Form1_checkBoxNMSSHIP3_Text;
        SeedTab.checkBoxSH0.Content = Language.Form1_checkBoxSH0_Text;
        aboutButton.Content = Language.Form1_aboutButton_Text;
        langButton.Content = Language.切换语言按钮;

        var isZh = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "zh";
        langMenuEN.IsChecked = !isZh;
        langMenuZH.IsChecked = isZh;

        if (saveLoader.AllJsonNode != null)
            updateUI();
        else if (!string.IsNullOrEmpty(savePath))
            updateSaveDescription(savePath, Language.待加载);
    }

    private void SwitchLanguage(string cultureName)
    {
        if (Thread.CurrentThread.CurrentUICulture.Name.StartsWith(
                cultureName.Split('-')[0], StringComparison.OrdinalIgnoreCase)) return;
        var culture = new CultureInfo(cultureName);
        Thread.CurrentThread.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        ApplyLanguage();
    }

    private void MainWindow_FirstActivated(object sender, WindowActivatedEventArgs e)
    {
        if (e.WindowActivationState == WindowActivationState.Deactivated)
            return;
        Activated -= MainWindow_FirstActivated;
        MessageHelper.Owner = this;
        MessageNotifier.Info = msg => _dispatcher.TryEnqueue(() => { var unused = MessageHelper.InfoAsync(msg); });
    }

    private void updateTabEnabled(bool enable)
    {
        NavItem2.IsEnabled = enable;
        NavItem3.IsEnabled = enable;
    }

    private void NavRoot_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem == NavItem1) PageContent.Content = LoadTab;
        else if (args.SelectedItem == NavItem2) PageContent.Content = ImportTab;
        else if (args.SelectedItem == NavItem3) PageContent.Content = SeedTab;
    }

    private async void ButtonSelect_Click(object sender, RoutedEventArgs e)
    {
        var detectedFolders = FileOperations.GetDetectedSaveFolders();
        string path;

        if (detectedFolders.Count > 0)
        {
            var (action, selectedPath) = await ChoiceFolderDialog.ShowAsync(this, Language.请选择存档路径, detectedFolders);
            path = action switch
            {
                FolderChoiceAction.UseDetected => selectedPath ?? "",
                FolderChoiceAction.PickManually => await FileOperations.SelectSaveFileAsync(this, savePath),
                _ => "",
            };
        }
        else
        {
            path = await FileOperations.SelectSaveFileAsync(this, savePath);
        }

        if (!string.IsNullOrEmpty(path))
        {
            savePath = path;
            LoadTab.textBoxPath.Text = savePath;
            updateSaveDescription(savePath, Language.待加载);
        }
    }

    private async void ButtonLoad_Click(object sender, RoutedEventArgs e)
    {
        await LoadSaveAsync(savePath);
    }

    private void InputImportText_TextChanged(object sender, RoutedEventArgs e)
    {
        ImportTab.importPath.Text = "";
        importPathString = "";
    }

    private async void ImportSelect_Click(object sender, RoutedEventArgs e)
    {
        ImportTab.inputImportText.Text = "";
        var selectedFile = await FileOperations.FileSelectAsync(this, 1);
        if (string.IsNullOrEmpty(selectedFile)) return;
        ImportTab.importPath.Text = selectedFile;
        importPathString = selectedFile;
    }

    private async Task LoadSaveAsync(string filePath)
    {
        startLoading();

        var saves = Enumerable.Empty<IContainer>();
        try
        {
            await Task.Run(() => { saves = saveLoader.LoadPath(filePath); });
        }
        catch (Exception ex)
        {
            await MessageHelper.ErrorAsync(Language.寻找存档失败_错误信息 + ex.Message + "\n");
            updateTabEnabled(saveLoader.AllJsonNode != null);
            finishLoading();
            return;
        }

        var choose = 0;
        var saveList = saves.Select((item, idx) => new { item, idx })
            .Where(s => s.item.ActiveContext == SaveContextQueryEnum.Main || s.item.ActiveContext == SaveContextQueryEnum.Season)
            .Where(s => s.item.SaveType == SaveTypeEnum.Manual)
            .Select(s => s.idx)
            .ToList();
        if (saveList.Count == 0)
        {
            await MessageHelper.ErrorAsync(Language.没有找到有效的存档);
            updateTabEnabled(saveLoader.AllJsonNode != null);
            finishLoading();
            return;
        }
        if (saveList.Count == 1)
        {
            await MessageHelper.InfoAsync(Language.仅找到一个存档_自动选择 + saveList[0]);
        }
        else
        {
            var filtered = saves.ToList().Where(s => saveList.Contains(saves.ToList().IndexOf(s))).ToList();
            var (ok, selected) = await ChoiceSaveDialog.ShowAsync(this, Language.选择一个存档, filtered);
            if (!ok)
            {
                await MessageHelper.InfoAsync(Language.操作取消);
                updateTabEnabled(saveLoader.AllJsonNode != null);
                finishLoading();
                return;
            }
            choose = selected;
        }

        try
        {
            await Task.Run(() =>
            {
                saveLoader.LoadSave(saves.ToList()[saveList[choose]]);
                _dispatcher.TryEnqueue(() =>
                {
                    updateUI();
                    var unusedLoad = MessageHelper.InfoAsync(Language.存档加载完成);
                });
            });
        }
        catch (Exception ex)
        {
            saveLoader.uninstallSave();
            await MessageHelper.ErrorAsync(ex.Message);
            finishLoading();
            return;
        }

        updateTabEnabled(saveLoader.AllJsonNode != null);
        finishLoading();
        updateUI();
    }

    private void setAllButtonDisabled()
    {
        LoadTab.buttonSelect.IsEnabled = false;
        LoadTab.buttonLoad.IsEnabled = false;
        ImportTab.buttonImport.IsEnabled = false;
        ImportTab.buttonExport.IsEnabled = false;
        SeedTab.buttonSeedShipExport.IsEnabled = false;
        SeedTab.buttonSeedShipImport.IsEnabled = false;
        SeedTab.buttonSetSeed.IsEnabled = false;
        SeedTab.buttonImportShipTech.IsEnabled = false;
        SeedTab.buttonExportShipTech.IsEnabled = false;
        ImportTab.buttonImportShipTechI.IsEnabled = false;
        ImportTab.buttonExportShipTechI.IsEnabled = false;
    }

    private void setAllButtonEnabled()
    {
        LoadTab.buttonSelect.IsEnabled = true;
        LoadTab.buttonLoad.IsEnabled = true;
        ImportTab.buttonImport.IsEnabled = true;
        ImportTab.buttonExport.IsEnabled = true;
        SeedTab.buttonSeedShipExport.IsEnabled = true;
        SeedTab.buttonSeedShipImport.IsEnabled = true;
        SeedTab.buttonImportShipTech.IsEnabled = true;
        SeedTab.buttonExportShipTech.IsEnabled = true;
        ImportTab.buttonImportShipTechI.IsEnabled = true;
        ImportTab.buttonExportShipTechI.IsEnabled = true;
        if (GetSelectedRadioSSeed() != null &&
            GetSelectedRadioSSeed()!.Contains(Language.种子无效))
            SeedTab.buttonSetSeed.IsEnabled = true;
    }

    private void startLoading()
    {
        progressBar.Visibility = Visibility.Visible;
        setAllButtonDisabled();
    }

    private void finishLoading()
    {
        progressBar.Visibility = Visibility.Collapsed;
        setAllButtonEnabled();
    }

    private void updateUI()
    {
        LoadTab.labelShipDetected.Text = SaveHandler.getSummary(
            saveLoader.PersistentPlayerBases!,
            saveLoader.ShipOwnership!,
            saveLoader.CharacterCustomisationData!,
            saveLoader.BaseShipIndex
        );
        updateSaveDescription(savePath, saveLoader.platform?.ToString() ?? "");
        AddRadioButtons(SaveHandler.shipBaseOptions, SaveHandler.shipOptons);
    }

    private sealed class RadioSeedTag
    {
        public string Seed { get; init; } = "";
        public int TypeId { get; init; }
    }

    public void AddRadioButtons(List<string> options1, List<string> options2)
    {
        ImportTab.RadioPanelI.Children.Clear();
        for (var i = 0; i < options1.Count; i++)
        {
            var radio = new RadioButton
            {
                Content = options1[i],
                GroupName = "ShipI",
                Tag = int.Parse(options1[i].Split(Language.飞船ID_)[1].Split(Language._基地ID_)[0]),
            };
            ImportTab.RadioPanelI.Children.Add(radio);
        }

        SeedTab.RadioPanelS.Children.Clear();
        for (var i = 0; i < options2.Count; i++)
        {
            var radio = new RadioButton
            {
                Content = options2[i].Split(Language._种子_)[0],
                GroupName = "ShipS",
                Tag = new RadioSeedTag
                {
                    Seed = options2[i].Split(Language._种子_)[1],
                    TypeId = int.Parse(options2[i].Split(Language.飞船ID_)[1].Split(Language._类型_)[0]),
                },
            };
            radio.Checked += (_, _) =>
            {
                if (radio.IsChecked != true) return;
                var tag = (RadioSeedTag)radio.Tag;
                var seed = tag.Seed;
                if (seed.Contains($"（{Language.种子无效}）", StringComparison.Ordinal))
                {
                    seed = "";
                    SeedTab.buttonSetSeed.IsEnabled = false;
                }
                else
                {
                    SeedTab.buttonSetSeed.IsEnabled = true;
                }
                SeedTab.shipSeed.Text = seed;
            };
            SeedTab.RadioPanelS.Children.Add(radio);
        }
    }

    public int GetSelectedRadioI()
    {
        foreach (var child in ImportTab.RadioPanelI.Children)
        {
            if (child is RadioButton { IsChecked: true } rb && rb.Tag is int id)
                return id;
        }
        return -1;
    }

    public int? GetSelectedRadioSIndex()
    {
        foreach (var child in SeedTab.RadioPanelS.Children)
        {
            if (child is RadioButton { IsChecked: true, Tag: RadioSeedTag tag })
                return tag.TypeId;
        }
        return null;
    }

    public string? GetSelectedRadioSSeed()
    {
        foreach (var child in SeedTab.RadioPanelS.Children)
        {
            if (child is RadioButton { IsChecked: true, Tag: RadioSeedTag tag })
                return tag.Seed;
        }
        return null;
    }

    private void updateSaveDescription(string filePath, string platform)
    {
        var lastFile = Directory.GetFiles(filePath).OrderByDescending(f => File.GetLastWriteTime(f)).First();
        var fileLastWriteTime = File.GetLastWriteTime(lastFile);
        LoadTab.labelDescription.Text = $"{Language.存档平台}{platform}\n{Language.最后保存时间}{fileLastWriteTime}";
    }

    private async void ButtonExport_Click(object sender, RoutedEventArgs e)
    {
        startLoading();
        var index = GetSelectedRadioI();
        try
        {
            await saveLoader.exportShip(
                index,
                exportPathString,
                ImportTab.exportName.Text ?? "",
                ImportTab.checkBoxI.IsChecked == true,
                ImportTab.checkBoxNMSSHIP1.IsChecked != true,
                false,
                false
            );
        }
        catch (Exception ex)
        {
            await MessageHelper.ErrorAsync(ex.Message);
            finishLoading();
            return;
        }
        finishLoading();
    }

    private async void ExportSelect_Click(object sender, RoutedEventArgs e)
    {
        var selectedFile = await FileOperations.FolderSelectAsync(this);
        if (!string.IsNullOrEmpty(selectedFile))
        {
            ImportTab.exportPath.Text = selectedFile;
            exportPathString = selectedFile;
        }
    }

    private async void ButtonImport_Click(object sender, RoutedEventArgs e)
    {
        startLoading();
        var index = GetSelectedRadioI();
        if (importPathString == "" && ImportTab.inputImportText.Text == "")
        {
            await MessageHelper.ErrorAsync(Language.请先选择或输入导入内容);
            finishLoading();
            return;
        }
        if (importPathString != "" && ImportTab.inputImportText.Text != "")
        {
            await MessageHelper.ErrorAsync(Language.两个导入来源);
            finishLoading();
            return;
        }
        try
        {
            await saveLoader.importShip(
                index,
                importPathString != ""
                    ? (!File.Exists(importPathString) ? throw new Exception(Language.文件不存在) : importPathString)
                    : FileOperations.setTempFile(ImportTab.inputImportText.Text),
                ImportTab.checkBoxI.IsChecked == true,
                false
            );
        }
        catch (Exception ex)
        {
            await MessageHelper.ErrorAsync(ex.Message);
            finishLoading();
            return;
        }
        updateUI();
        finishLoading();
    }

    private async void ButtonSetSeed_Click(object sender, RoutedEventArgs e)
    {
        startLoading();
        var index = GetSelectedRadioSIndex() ?? -1;
        try
        {
            if (string.IsNullOrEmpty(SeedTab.shipSeed.Text))
                throw new Exception(Language.请输入有效种子);
            await saveLoader.setShipSeed(index, SeedTab.shipSeed.Text);
        }
        catch (Exception ex)
        {
            await MessageHelper.ErrorAsync(ex.Message);
            finishLoading();
            return;
        }
        updateUI();
        finishLoading();
    }

    private async void ButtonSeedShipImport_Click(object sender, RoutedEventArgs e)
    {
        startLoading();
        try
        {
            var tempPath = await FileOperations.FileSelectAsync(this, 2);
            if (tempPath == "")
            {
                await MessageHelper.InfoAsync(Language.操作取消);
                finishLoading();
                return;
            }
            if (!File.Exists(tempPath))
                throw new Exception(Language.文件不存在);
            var Index = GetSelectedRadioSIndex() ?? -1;
            await saveLoader.importShip(
                Index,
                tempPath,
                SeedTab.checkBoxS.IsChecked == true,
                SeedTab.checkBoxNewShip.IsChecked == true
            );
        }
        catch (Exception ex)
        {
            await MessageHelper.ErrorAsync(ex.Message);
            finishLoading();
            return;
        }
        updateUI();
        finishLoading();
    }

    private async void ButtonSeedShipExport_Click(object sender, RoutedEventArgs e)
    {
        startLoading();
        try
        {
            var tempPath = await FileOperations.FolderSelectAsync(this);
            if (tempPath == "")
            {
                await MessageHelper.InfoAsync(Language.操作取消);
                finishLoading();
                return;
            }
            var Index = GetSelectedRadioSIndex() ?? -1;
            await saveLoader.exportShip(
                Index,
                tempPath,
                SeedTab.textBoxExportName.Text,
                SeedTab.checkBoxS.IsChecked == true,
                false,
                SeedTab.checkBoxNMSSHIP3.IsChecked != true,
                SeedTab.checkBoxSH0.IsChecked == true
            );
        }
        catch (Exception ex)
        {
            await MessageHelper.ErrorAsync(ex.Message);
            finishLoading();
            return;
        }
        finishLoading();
    }

    private async void CheckBoxI_Unchecked(object sender, RoutedEventArgs e)
    {
        if (ImportTab.checkBoxI.IsChecked == true) return; 
        var ok = await MessageHelper.WarningOkCancelAsync(Language.混淆关闭警告I);
        if (!ok) ImportTab.checkBoxI.IsChecked = true;
        else SeedTab.checkBoxS.IsChecked = false;
    }

    private async void CheckBoxS_Unchecked(object sender, RoutedEventArgs e)
    {
        if (SeedTab.checkBoxS.IsChecked == true) return;
        var ok = await MessageHelper.WarningOkCancelAsync(Language.混淆关闭警告S);
        if (!ok) SeedTab.checkBoxS.IsChecked = true;
        else ImportTab.checkBoxI.IsChecked = false;
    }

    private async void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (SeedTab.checkBoxS.IsChecked != true) SeedTab.checkBoxS.IsChecked = true;
        if (ImportTab.checkBoxI.IsChecked != true) ImportTab.checkBoxI.IsChecked = true;
    }

    private async void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        await AboutDialogHelper.ShowAsync(this);
    }

    private void CheckBoxSH0_Checked(object sender, RoutedEventArgs e)
    {
        if (SeedTab.checkBoxSH0.IsChecked == true)
            SeedTab.checkBoxNMSSHIP3.IsChecked = false;
    }

    private void CheckBoxNMSSHIP3_Checked(object sender, RoutedEventArgs e)
    {
        if (SeedTab.checkBoxNMSSHIP3.IsChecked == true)
            SeedTab.checkBoxSH0.IsChecked = false;
    }

    private async void ButtonImportShipTech_Click(object sender, RoutedEventArgs e)
    {
        startLoading();
        try
        {
            var tempPath = await FileOperations.FileSelectAsync(this, 3);
            if (tempPath == "")
            {
                await MessageHelper.InfoAsync(Language.操作取消);
                finishLoading();
                return;
            }
            if (!File.Exists(tempPath))
                throw new Exception(Language.文件不存在);
            var Index = GetSelectedRadioSIndex() ?? -1;
            await saveLoader.importShipTech(Index, tempPath, SeedTab.checkBoxS.IsChecked == true);
        }
        catch (Exception ex)
        {
            await MessageHelper.ErrorAsync(ex.Message);
            finishLoading();
            return;
        }
        updateUI();
        finishLoading();
    }

    private async void ButtonExportShipTech_Click(object sender, RoutedEventArgs e)
    {
        startLoading();
        try
        {
            var tempPath = await FileOperations.FolderSelectAsync(this);
            if (tempPath == "")
            {
                await MessageHelper.InfoAsync(Language.操作取消);
                finishLoading();
                return;
            }
            var Index = GetSelectedRadioSIndex() ?? -1;
            await saveLoader.exportShipTech(
                Index,
                tempPath,
                SeedTab.textBoxExportName.Text,
                SeedTab.checkBoxS.IsChecked == true,
                SeedTab.checkBoxTech.IsChecked == true
            );
        }
        catch (Exception ex)
        {
            await MessageHelper.ErrorAsync(ex.Message);
            finishLoading();
            return;
        }
        finishLoading();
    }

    private async void ButtonExportShipTechI_Click(object sender, RoutedEventArgs e)
    {
        startLoading();
        try
        {
            var Index = GetSelectedRadioI();
            await saveLoader.exportShipTech(
                Index,
                exportPathString,
                ImportTab.exportName.Text,
                SeedTab.checkBoxS.IsChecked == true,
                ImportTab.checkBoxTechI.IsChecked == true
            );
        }
        catch (Exception ex)
        {
            await MessageHelper.ErrorAsync(ex.Message);
            finishLoading();
            return;
        }
        finishLoading();
    }

    private async void ButtonImportShipTechI_Click(object sender, RoutedEventArgs e)
    {
        startLoading();
        try
        {
            if (importPathString == "" && ImportTab.inputImportText.Text == "")
            {
                await MessageHelper.ErrorAsync(Language.请先选择要导入文件路径);
                finishLoading();
                return;
            }
            if (!File.Exists(importPathString))
                throw new Exception(Language.文件不存在);
            var Index = GetSelectedRadioI();
            await saveLoader.importShipTech(Index, importPathString, ImportTab.checkBoxI.IsChecked == true);
        }
        catch (Exception ex)
        {
            await MessageHelper.ErrorAsync(ex.Message);
            finishLoading();
            return;
        }
        updateUI();
        finishLoading();
    }

    // ── 窗口尺寸约束 ──────────────────────────────────────────────────────────

    private int _minWinWidth;
    private int _minWinHeight;
    private SubclassProc? _subclassProc;

    private void InitWindowSize(int logicalWidth, int logicalHeight)
    {
        var hwnd  = WindowNative.GetWindowHandle(this);
        var scale = GetDpiForWindow(hwnd) / 96.0;
        _minWinWidth  = (int)(logicalWidth  * scale);
        _minWinHeight = (int)(logicalHeight * scale);
        AppWindow.Resize(new SizeInt32(_minWinWidth, _minWinHeight));
        _subclassProc = WindowSubclassProc;
        SetWindowSubclass(hwnd, _subclassProc, 0, 0);
    }

    private nint WindowSubclassProc(IntPtr hWnd, uint uMsg, nint wParam, nint lParam, nuint uIdSubclass, nuint dwRefData)
    {
        if (uMsg == WM_GETMINMAXINFO)
        {
            var info = Marshal.PtrToStructure<MINMAXINFO>(lParam);
            info.ptMinTrackSize = new POINT { x = _minWinWidth, y = _minWinHeight };
            Marshal.StructureToPtr(info, lParam, false);
        }
        return DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }

    private const uint WM_GETMINMAXINFO = 0x0024;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int x, y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public POINT ptReserved, ptMaxSize, ptMaxPosition, ptMinTrackSize, ptMaxTrackSize;
    }

    private delegate nint SubclassProc(IntPtr hWnd, uint uMsg, nint wParam, nint lParam, nuint uIdSubclass, nuint dwRefData);

    [DllImport("user32.dll")]   private static extern uint GetDpiForWindow(IntPtr hwnd);
    [DllImport("comctl32.dll")] private static extern bool SetWindowSubclass(IntPtr hWnd, SubclassProc pfnSubclass, nuint uIdSubclass, nuint dwRefData);
    [DllImport("comctl32.dll")] private static extern nint DefSubclassProc(IntPtr hWnd, uint uMsg, nint wParam, nint lParam);
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);
}
