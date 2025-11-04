
namespace NMSShipIOTool.Model
{
    internal static class ContentHelper
    {
        public static string checkType(string input)
        {
            var parts = input.Split('/');
            if (parts[parts.Count() - 1].Split('_')[0].Equals("FIGHTER")) { return "战斗"; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("SENTINELSHIP")) { return "护卫"; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("BIOSHIP")) { return "生物"; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("SHUTTLE")) { return "飞艇"; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("SAILSHIP")) { return "太阳帆"; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("S-CLASS")) { return "异星"; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("SCIENTIFIC")) { return "探险家"; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("DROPSHIP")) { return "托运"; }
            else if (parts[parts.Count() - 1].Split('.')[0].Equals("BIGGS")) { return "自定义"; }
            else { return "特殊船"; }
        }

        public static string selectBetween(string pos1, string pos2, string target)
        {
            int idx1 = target.IndexOf(pos1);
            int idx2 = target.IndexOf(pos2, idx1 + pos1.Length);
            return (idx1 >= 0 && idx2 > idx1)
                ? target.Substring(idx1 + pos1.Length, idx2 - (idx1 + pos1.Length))
                : "";
        }

        public static string selectAfter(string pos, string target)
        {
            int idx = target.IndexOf(pos);
            return (idx >= 0)
                ? target.Substring(idx + pos.Length)
                : "";
        }

        public static string selectBefore(string pos, string target)
        {
            int idx = target.IndexOf(pos);
            return (idx >= 0)
                ? target.Substring(0, idx)
                : "";
        }

        public static Dictionary<int, int> SlotTrack = new Dictionary<int, int>
        {
            {0,3},
            {1,4},
            {2,5},
            {3,6},
            {4,7},
            {5,8},
            {6,17},
            {7,18},
            {8,19},
            {9,20},
            {10,21},
            {11,22}
        };
    }
}
