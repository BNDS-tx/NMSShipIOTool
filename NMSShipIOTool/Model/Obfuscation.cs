using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text.Json.Nodes;

namespace NMSShipIOTool.Model
{
    internal class Obfuscation
    {

        public static Dictionary<string, string> CommonKey { get; set; } = new();
        public static Dictionary<string, string> AccountKey { get; set; } = new();

        private static string mappingJson;

        public static void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "NMSShipIOTool.Resources.mapping.json";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"找不到嵌入资源: {resourceName}");

                using var reader = new StreamReader(stream);
                mappingJson = reader.ReadToEnd();
            }

            var node = JsonNode.Parse(mappingJson);
            var mapping = node?["Mapping"]?.AsArray();
            if (mapping == null) return;

            // 将 JsonArray 转换为 KeyValuePair 集合
            var mappingData = mapping
                .Where(item => item != null)
                .Select(item => new KeyValuePair<string, string>(
                    item["Key"]?.ToString() ?? string.Empty,
                    item["Value"]?.ToString() ?? string.Empty
                ))
                .Where(kvp => !string.IsNullOrEmpty(kvp.Key))
                .ToList();

            // 查找 "UserSettingsData" 的索引
            var splitIndex = mappingData.FindIndex(kvp => kvp.Value.Equals("UserSettingsData"));

            if (splitIndex >= 0)
            {
                // 找到分割点：前半部分存入 CommonKey，后半部分存入 AccountKey
                for (int i = 0; i < splitIndex; i++)
                {
                    var kvp = mappingData[i];
                    CommonKey[kvp.Key] = kvp.Value;
                }

                for (int i = splitIndex; i < mappingData.Count; i++)
                {
                    var kvp = mappingData[i];
                    AccountKey[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                throw new Exception("Mapping data does not contain 'UserSettingsData' key.");
            }
        }

        public static JToken JTObfuscate(JToken token)
        {
            if (CommonKey == null || CommonKey.Count() == 0) Initialize();
            if (CommonKey == null || CommonKey.Count() == 0) throw new Exception("初始化混淆密钥失败！");
            return ReplacePlainKeys(token, CommonKey.ToDictionary(kv => kv.Value, kv => kv.Key));
        }

        public static JToken JTDeobfuscate(JToken token)
        {
            if (CommonKey == null || CommonKey.Count() == 0) Initialize();
            if (CommonKey == null || CommonKey.Count() == 0) throw new Exception("初始化混淆密钥失败！");
            return ReplaceObfuscatedKeys(token, CommonKey);
        }

        private static JToken ReplaceObfuscatedKeys(JToken token, Dictionary<string, string> keyMap)
        {
            if (token is JObject obj)
            {
                var newObj = new JObject();
                foreach (var prop in obj.Properties())
                {
                    string newKey = keyMap.ContainsKey(prop.Name) ? keyMap[prop.Name] : prop.Name;
                    newObj[newKey] = ReplaceObfuscatedKeys(prop.Value, keyMap);
                }
                return newObj;
            }
            else if (token is JArray arr)
            {
                var newArr = new JArray();
                foreach (var item in arr)
                {
                    newArr.Add(ReplaceObfuscatedKeys(item, keyMap));
                }
                return newArr;
            }
            else
            {
                return token.DeepClone();
            }
        }

        private static JToken ReplacePlainKeys(JToken token, Dictionary<string, string> reverseMap)
        {
            if (token is JObject obj)
            {
                var newObj = new JObject();
                foreach (var prop in obj.Properties())
                {
                    string newKey = reverseMap.ContainsKey(prop.Name) ? reverseMap[prop.Name] : prop.Name;
                    newObj[newKey] = ReplacePlainKeys(prop.Value, reverseMap);
                }
                return newObj;
            }
            else if (token is JArray arr)
            {
                var newArr = new JArray();
                foreach (var item in arr)
                {
                    newArr.Add(ReplacePlainKeys(item, reverseMap));
                }
                return newArr;
            }
            else
            {
                return token.DeepClone();
            }
        }

        public static String StrObfuscate(string plainWord)
        {
            if (CommonKey == null || CommonKey.Count() == 0) Initialize();
            if (CommonKey == null || CommonKey.Count() == 0) throw new Exception("初始化混淆密钥失败！");
            return CommonKey.ContainsValue(plainWord) ? CommonKey.FirstOrDefault(x => x.Value == plainWord).Key : plainWord;
        }

        public static String StrDeobfuscate(string obfusWord)
        {
            if (CommonKey == null || CommonKey.Count() == 0) Initialize();
            if (CommonKey == null || CommonKey.Count() == 0) throw new Exception("初始化混淆密钥失败！");
            return CommonKey.ContainsKey(obfusWord) ? CommonKey.FirstOrDefault(x => x.Key == obfusWord).Key : obfusWord;
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
