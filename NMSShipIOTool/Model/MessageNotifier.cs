namespace NMSShipIOTool.Model;

/// <summary>
/// 由 UI 层注入，供 Model（如 SaveLoader）在操作成功后提示用户，避免 Model 依赖具体 UI 框架。
/// </summary>
public static class MessageNotifier
{
    public static Action<string>? Info { get; set; }

    public static void NotifyInfo(string message) => Info?.Invoke(message);
}
