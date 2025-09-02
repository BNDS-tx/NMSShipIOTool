using libNOM.map;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace NMSShipIOTool
{
    internal class Obfuscation
    {
        public static JToken Obfuscate(JToken token) 
        {
            return  ReplacePlainKeys(token, ReverseKey);
        }

        public static JToken Deobfuscate(JToken token) 
        {
            return ReplaceObfuscatedKeys(token, Key);
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

        private static Dictionary<string, string> Key = new Dictionary<string, string>
        {
            {"wMC","Position"},
            {"b1:","Timestamp"},
            {"aNu","At"},
            {"wJ0","Up"},
            {"r<7","ObjectID"},
            {"CVX","UserData"}
        };

        private static Dictionary<string, string> ReverseKey = Key.ToDictionary(kv => kv.Value, kv => kv.Key);
    }
}
