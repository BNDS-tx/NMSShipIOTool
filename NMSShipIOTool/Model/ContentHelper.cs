using NMSShipIOTool.Resources;

namespace NMSShipIOTool.Model
{
    internal static class ContentHelper
    {
        public static string checkType(string input)
        {
            var parts = input.Split('/');
            if (parts[parts.Count() - 1].Split('_')[0].Equals("FIGHTER")) { return Language.战斗; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("SENTINELSHIP")) { return Language.护卫; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("BIOSHIP")) { return Language.生物; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("SHUTTLE")) { return Language.飞艇; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("SAILSHIP")) { return Language.太阳帆; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("S-CLASS")) { return Language.异星; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("SCIENTIFIC")) { return Language.探险家; }
            else if (parts[parts.Count() - 1].Split('_')[0].Equals("DROPSHIP")) { return Language.托运; }
            else if (parts[parts.Count() - 1].Split('.')[0].Equals("BIGGS")) { return Language.自定义护卫舰; }
            else { return Language.特殊船; }
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
