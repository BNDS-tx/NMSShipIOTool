using NMSShipIOTool.Resources;
using System.Text.Json.Nodes;

namespace NMSShipIOTool.Model
{
    public static class SaveHandler
    {
        public static List<string> baseOptions = new List<string>();
        public static List<string> shipBaseOptions = new List<string>();
        public static List<string> shipOptons = new List<string>();

        public static String getSummary(
            JsonNode PersistentPlayerBases,
            JsonNode ShipOwnership,
            JsonNode CharacterCustomisationData,
            List<int> BaseShipIndex
            )
        {
            baseOptions.Clear();
            shipBaseOptions.Clear();
            shipOptons.Clear();

            try
            {
                var baseCount = PersistentPlayerBases.AsArray().Count();
                var shipBaseCount = BaseShipIndex.Count();
                var shipCount = ShipOwnership.AsArray().Count() - shipBaseCount;

                var allShipDetected = Language.普通飞船_ + Environment.NewLine + Environment.NewLine;
                if (shipCount > 0)
                {
                    foreach (var t in ShipOwnership.AsArray().ToList())
                    {
                        string fileName = t?["NTx"]?["93M"]?.ToString() ?? "";
                        if (fileName == "" || fileName == null) { continue; }
                        int shipID = ShipOwnership.AsArray().ToList().IndexOf(t);
                        string shipName = t?["NKm"]?.ToString() ?? "";
                        string shipSeed = t?["NTx"]?["@EL"]?[1]?.ToString() ?? "";
                        string shipType = ContentHelper.checkType(fileName);
                        int slot = -1;
                        foreach (var item in ContentHelper.SlotTrack)
                        { if (item.Key == shipID) { slot = item.Value; break; } }
                        var ccdToken = CharacterCustomisationData.AsArray().ElementAt(slot);
                        if (ccdToken != null && ccdToken?["wnR"]?["SMP"]?.AsArray().ToList().Count > 0)
                        {
                            shipType = $"（{Language.拼接}）" + shipType;
                            shipSeed = $"（{Language.种子无效}）";
                        }
                        if (shipType == Language.特殊船) { shipSeed = $"（{Language.种子无效}）"; }
                        if (shipType == Language.自定义护卫舰) { continue; }
                        string option = Language.飞船ID_ + shipID + Language._类型_ + shipType + Language._飞船名_ + shipName + Language._种子_ + shipSeed;
                        shipOptons.Add(option);
                        allShipDetected = allShipDetected + option + Environment.NewLine;
                    }
                    allShipDetected += Environment.NewLine + Environment.NewLine;
                }
                else { allShipDetected += Environment.NewLine; }

                var shipBaseDetected = Language.自定义护卫舰_ + Environment.NewLine + Environment.NewLine;
                if (shipBaseCount > 0)
                {
                    foreach (int t in BaseShipIndex)
                    {
                        int shipID = int.Parse((PersistentPlayerBases.AsArray().ElementAt(t)?["CVX"] ?? "-1").ToString());
                        string shipName = (ShipOwnership.AsArray().ElementAt(shipID)?["NKm"] ?? "").ToString();
                        string option = Language.飞船ID_ + shipID + Language._基地ID_ + t.ToString() + Language._飞船名_ + shipName;
                        shipBaseOptions.Add(option);
                        shipBaseDetected = shipBaseDetected + option + Environment.NewLine;
                    }
                    shipBaseDetected += Environment.NewLine + Environment.NewLine;
                }
                else { shipBaseDetected += Environment.NewLine; }

                return allShipDetected + shipBaseDetected;
            }
            catch (Exception ex)
            {
                string errorInfo = "读取数据时发生错误：" + ex.Message + "\n\n您的存档版本过低或疑似发生损坏，请登录游戏刷新存档或使用其他工具检查。";
                //MessageClass.ShowError(errorInfo);
                //return errorInfo;
                throw new Exception(errorInfo);
            }
        }
    }
}
