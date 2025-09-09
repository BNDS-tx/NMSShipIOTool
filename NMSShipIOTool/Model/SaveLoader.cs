using libNOM.io;
using libNOM.io.Enums;
using libNOM.io.Settings;
using Newtonsoft.Json.Linq;
using NMSShipIOTool.Model;
using NMSShipIOTool.View;
using Octokit;
using System.Text.Json.Nodes;

namespace NMSModelIOTool.Model
{

    public class SaveLoader
    {
        private String JsonPathLayer1 = "vLc";
        private String JsonPathLayer2 = "6f=";
        private String basePath = "F?0";
        private String shipPath = "@Cs";
        private String ccdPath = "l:j";
        private String defaultExportNameString = "导出飞船";

        public libNOM.io.Interfaces.IPlatform? platform;
        private int saveSlot = 0;
        public libNOM.io.Interfaces.IContainer? currentSave;

        public JsonNode? AllJsonNode;
        public JsonNode? PersistentPlayerBases;
        public JsonNode? ShipOwnership;
        public JsonNode? CharacterCustomisationData;
        public List<int> BaseShipIndex = new List<int>();

        #region SaveFile

        public IEnumerable<libNOM.io.Interfaces.IContainer> LoadPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("路径不能为空。", nameof(path));
            }
            else if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("指定的路径不存在或无效。");
            }

            var settings = new PlatformSettings { LoadingStrategy = LoadingStrategyEnum.Current };

            var collection = new PlatformCollection();
            var platforms = collection.AnalyzePath(path, settings);

            platform = platforms.First();

            var account = platform.GetAccountContainer();
            return platform.GetSaveContainers();
        }

        public void LoadSave(libNOM.io.Interfaces.IContainer save)
        {
            platform!.Load(save);
            UpdateSave(save);
            currentSave = save;
            saveSlot = platform.GetSaveContainers().ToList().IndexOf(save);
        }

        public void SaveSave(libNOM.io.Interfaces.IContainer save)
        {
            platform!.Write(save);
            UpdateSave(save);
            currentSave = save;
            saveSlot = platform.GetSaveContainers().ToList().IndexOf(save);
        }

        public void UpdateSave(libNOM.io.Interfaces.IContainer save)
        {
            var rawData = save.GetJsonObject();

            var jN = JsonNode.Parse(rawData.ToString());

            AllJsonNode = jN;
            PersistentPlayerBases = jN?[JsonPathLayer1]?[JsonPathLayer2]?[basePath];
            ShipOwnership = jN?[JsonPathLayer1]?[JsonPathLayer2]?[shipPath];
            CharacterCustomisationData = jN?[JsonPathLayer1]?[JsonPathLayer2]?[ccdPath];

            BaseShipIndex.Clear();
            if (PersistentPlayerBases != null && PersistentPlayerBases.AsArray().Count > 0)
            {
                for (int i = 0; i < PersistentPlayerBases.AsArray().Count; i++)
                {
                    var baseItem = PersistentPlayerBases[i];
                    if (baseItem != null)
                    {
                        var baseType = baseItem["peI"]?["DPp"]?.GetValue<string>();
                        if (!string.IsNullOrEmpty(baseType) && baseType == "PlayerShipBase")
                        {
                            BaseShipIndex.Add(i);
                        }
                    }
                }
            }
        }

        public void uninstallSave()
        {
            PersistentPlayerBases = null;
            ShipOwnership = null;
            CharacterCustomisationData = null;
            AllJsonNode = null;
            currentSave = null;
            platform = null;
            saveSlot = -1;
            BaseShipIndex.Clear();
            SaveHandler.baseOptions.Clear();
            SaveHandler.shipBaseOptions.Clear();
            SaveHandler.shipOptons.Clear();
        }

        #endregion

        #region SaveContent

        private int findAvailableShipID()
        {
            int shipID = -1;
            foreach (var ship in ShipOwnership?.AsArray()?.ToList() ?? new List<JsonNode?>())
            {
                string fileName = ship?["NTx"]?["93M"]?.ToString() ?? "";
                if (fileName == "") { shipID = ShipOwnership?.AsArray()?.ToList().IndexOf(ship) ?? -1; }
            }
            return shipID;
        }

        // 写入基地/货船基地/自定义飞船数据
        private void writeBaseObject(JsonNode Base, int baseID, int shipID, bool isO)
        {
            var save = currentSave!;
            var newBase = Base.DeepClone();

            string path1 = JsonPathLayer1;
            string path2 = JsonPathLayer2;
            string path3 = basePath;
            string path4 = "@ZJ";

            if (baseID != -1 && shipID == -1)
            {
                JToken objectsJToken;
                if (isO) objectsJToken = Obfuscation.JTObfuscate(JToken.Parse(newBase.ToJsonString())); 
                else objectsJToken = JToken.Parse(newBase.ToJsonString());

                string jsonPath = $"$.{path1}.{path2}.{path3}[{baseID}].{path4}";
                save.SetJsonValue(objectsJToken, jsonPath);
            }
            else if (baseID == -1 && shipID != -1)
            {
                JToken objectsJToken;
                if (isO) objectsJToken = Obfuscation.JTObfuscate(JToken.Parse(newBase.ToJsonString())); 
                else objectsJToken = JToken.Parse(newBase.ToJsonString());
                var index = -1;
                foreach (int i in BaseShipIndex)
                {
                    if (PersistentPlayerBases?.AsArray()?.ElementAt(i)?["CVX"]?.ToString() == shipID.ToString())
                    { index = i; break; }
                }
                if (index != -1)
                {
                    string jsonPath = $"$.{path1}.{path2}.{path3}[{index}].{path4}";
                    save.SetJsonValue(objectsJToken, jsonPath);
                }
                else
                {
                    throw new Exception("自定义飞船的 ShipID 未找到对应基地！");
                }
            }
            else
            {
                throw new Exception("写入 Objects 时传参混乱！");
            }
            currentSave = save;
        }

        // 写入拼接飞船自定义部件数据
        private void writeCCD(JsonNode CCD, int shipID, bool isO)
        {
            var save = currentSave!;
            var newCCD = CCD.DeepClone();
            JToken ccdJToken;
            if (isO) { ccdJToken = Obfuscation.JTObfuscate(JToken.Parse(newCCD.ToJsonString())); }
            ccdJToken = JToken.Parse(newCCD.ToJsonString());
            int slot = -1;
            foreach (var item in Obfuscation.SlotTrack)
            { if (item.Key == shipID) { slot = item.Value; break; } }
            if (slot == -1) { throw new Exception("找不到 ShipID 对应的部件参数！"); }

            string path1 = JsonPathLayer1;
            string path2 = JsonPathLayer2;
            string path3 = ccdPath;
            string jsonPath = $"$.{path1}.{path2}.{path3}[{slot}]";

            save.SetJsonValue(ccdJToken, jsonPath);
            currentSave = save;
        }

        // 写入飞船所有权数据
        private void writeSO(JsonNode SO, int shipID, bool isO)
        {
            var save = currentSave!;
            var newSO = SO.DeepClone();
            if (shipID == -1) { throw new Exception("写入飞船所有权时 ShipID 不能为 -1！"); }
            JToken soJToken;
            if (isO)  soJToken = Obfuscation.JTObfuscate(JToken.Parse(newSO.ToJsonString())); 
            else soJToken = JToken.Parse(newSO.ToJsonString());
            
            string path1 = JsonPathLayer1;
            string path2 = JsonPathLayer2;
            string path3 = shipPath;

            string jsonPath = $"$.{path1}.{path2}.{path3}[{shipID}]";
            save.SetJsonValue(soJToken, jsonPath);
            currentSave = save;
        }

        private void writeSeed(string seed, int shipID)
        {
            var save = currentSave!;
            if (shipID == -1) { throw new Exception("写入种子时 ShipID 不能为 -1！"); }
            if (seed == null || seed == "") { throw new Exception("种子为空！"); }

            string path1 = JsonPathLayer1;
            string path2 = JsonPathLayer2;
            string path3 = shipPath;
            string path4 = "NTx";
            string path5 = "@EL";
            string jsonPath = $"$.{path1}.{path2}.{path3}[{shipID}].{path4}.{path5}[1]";

            save.SetJsonValue(seed, jsonPath);
            currentSave = save;
        }

        // 写入飞船技术、模块与库存数据
        private void writeTech(JsonNode Tech, int shipID, bool isO)
        {
            var save = currentSave!;
            var newTech = Tech.DeepClone();
            if (shipID == -1) { throw new Exception("写入飞船技术、模块与库存时 ShipID 不能为 -1！"); }
            string path1 = JsonPathLayer1;
            string path2 = JsonPathLayer2;
            string path3 = shipPath;
            string path4_1 = Obfuscation.StrObfuscate("Inventory");
            string path4_2 = Obfuscation.StrObfuscate("Inventory_Cargo");
            string path4_3 = Obfuscation.StrObfuscate("Inventory_TechOnly");
            string path4_4 = Obfuscation.StrObfuscate("InventoryLayout");
            JToken techJToken;
            if (isO) techJToken = Obfuscation.JTObfuscate(JToken.Parse(newTech.ToJsonString())); 
            else techJToken = JToken.Parse(newTech.ToJsonString());
            var techArray = techJToken as JArray;
            if (techArray == null || techArray.Count != 4) { throw new Exception("飞船技术、模块与库存数据异常！"); }
            string jsonPath1 = $"$.{path1}.{path2}.{path3}[{shipID}].{path4_1}";
            string jsonPath2 = $"$.{path1}.{path2}.{path3}[{shipID}].{path4_2}";
            string jsonPath3 = $"$.{path1}.{path2}.{path3}[{shipID}].{path4_3}";
            string jsonPath4 = $"$.{path1}.{path2}.{path3}[{shipID}].{path4_4}";
            save.SetJsonValue(techArray[0], jsonPath1);
            save.SetJsonValue(techArray[1], jsonPath2);
            save.SetJsonValue(techArray[2], jsonPath3);
            save.SetJsonValue(techArray[3], jsonPath4);
            currentSave = save;
        }

        // 读取基地/货船基地/自定义飞船数据
        private string readBaseObject(int baseID, int shipID, bool isO)
        {
            if (baseID == -1 && (!PersistentPlayerBases?.AsArray()?.Any(s => s?["CVX"]?.ToString() == shipID.ToString()) ?? true))
                return "";
            var baseJN = JsonNode.Parse(PersistentPlayerBases?.AsArray()?.ElementAt(
                (baseID != -1 && shipID == -1) ? baseID 
                : (baseID == -1 && shipID != -1) 
                    ? PersistentPlayerBases?.AsArray()?.ToList().Where(s => s?["CVX"]?.ToString() == shipID.ToString()).Select(s => PersistentPlayerBases.AsArray().ToList().IndexOf(s)).FirstOrDefault() 
                    ?? throw new Exception("自定义飞船的 ShipID 未找到对应基地！")
                : throw new Exception("读取 Objects 时传参混乱!")
                )?.ToString() ?? "");
            string jsonString;
            if (baseJN == null || baseJN["@ZJ"] == null) { jsonString = ""; }
            else
            {
                jsonString = isO == true
                    ? Obfuscation.JTDeobfuscate(JToken.Parse(baseJN["@ZJ"]!.ToJsonString())).ToString()
                    : baseJN["@ZJ"]?.ToJsonString() ?? "";
            }
            return jsonString;
        }

        // 读取拼接飞船自定义部件数据
        private string readCCD(int shipID, bool isO)
        {
            int slot = -1;
            foreach (var item in Obfuscation.SlotTrack)
            { if (item.Key == shipID) { slot = item.Value; break; } }
            if (slot == -1) throw new Exception("找不到 ShipID 对应的部件参数！");
            var ccdJN = JsonNode.Parse(CharacterCustomisationData?.AsArray()?.ElementAt(slot)?.ToString() ?? "");
            string jsonString;
            if (ccdJN == null) { jsonString = ""; }
            else
            {
                jsonString = isO == true
                    ? Obfuscation.JTDeobfuscate(JToken.Parse(ccdJN.ToJsonString())).ToString()
                    : ccdJN.ToJsonString();
            }
            return jsonString;
        }

        // 读取飞船所有权数据
        private string readSO(int shipID, bool isO)
        {
            if (shipID == -1) throw new Exception("读取飞船所有权时 ShipID 不能为 -1！");
            var shipJN = JsonNode.Parse(ShipOwnership?.AsArray()?.ElementAt(shipID)?.ToString() ?? "");
            string jsonString;
            if (shipJN == null) { jsonString = ""; }
            else
            {
                jsonString = isO == true
                    ? Obfuscation.JTDeobfuscate(JToken.Parse(shipJN.ToJsonString())).ToString()
                    : shipJN.ToJsonString();
            }
            return jsonString;
        }

        // 读取飞船技术、模块与库存数据
        private string readTech(int shipID, bool isO)
        {
            if (shipID == -1) throw new Exception("读取飞船技术、模块与库存时 ShipID 不能为 -1！");
            var shipJN = JsonNode.Parse(ShipOwnership?.AsArray()?.ElementAt(shipID)?.ToString() ?? "");

            var shipInventory = shipJN?[Obfuscation.StrObfuscate("Inventory")]?.DeepClone() ?? new JsonObject();
            var shipInventory_Cargo = shipJN?[Obfuscation.StrObfuscate("Inventory_Cargo")]?.DeepClone() ?? new JsonObject();
            var shipInventory_TechOnly = shipJN?[Obfuscation.StrObfuscate("Inventory_TechOnly")]?.DeepClone() ?? new JsonObject();
            var shipInventoryLayout = shipJN?[Obfuscation.StrObfuscate("InventoryLayout")]?.DeepClone() ?? new JsonObject();

            var shipTech = new JsonArray();
            shipTech.Add(shipInventory);
            shipTech.Add(shipInventory_Cargo);
            shipTech.Add(shipInventory_TechOnly);
            shipTech.Add(shipInventoryLayout);

            string jsonString;
            if (shipTech == null) { jsonString = ""; }
            else
            {
                jsonString = isO == true
                    ? Obfuscation.JTDeobfuscate(JToken.Parse(shipTech.ToJsonString())).ToString()
                    : shipTech.ToJsonString();
            }
            return jsonString;
        }



        public async Task importShip(
            int index,
            string importPath,
            bool isOb,
            bool isNew)
        {
            if (index == -1)
            {
                isNew = true;
            }
            if (importPath.Split('.').Last() == "nmsship")
            {
                var tempDir = Path.Combine(System.IO.Path.GetTempPath(), "NMSMIOT_Temp");
                ShipPackage.Unpack(importPath, tempDir);
                var objectsPath = Path.Combine(tempDir, "objects.json");
                var objectsString = File.ReadAllText(objectsPath);
                var ccdPath = Path.Combine(tempDir, "ccd.json");
                var ccdString = File.ReadAllText(ccdPath);
                var soPath = Path.Combine(tempDir, "so.json");
                var soString = File.ReadAllText(soPath);

                if (objectsString != "")
                {
                    JsonNode? objects = JsonNode.Parse(objectsString);
                    JsonNode? ccd = JsonNode.Parse(ccdString);
                    JsonNode? so = JsonNode.Parse(soString);

                    if (index == -1 || isNew) { throw new Exception("暂不支持添加新自定义飞船。"); }
                    await Task.Run(() => { writeBaseObject(objects!, -1, index, isOb); });
                    await Task.Run(() => { writeCCD(ccd!, index, isOb); });
                    await Task.Run(() => { writeSO(so!, index, isOb); });
                }
                else
                {
                    JsonNode? ccd = JsonNode.Parse(ccdString);
                    JsonNode? so = JsonNode.Parse(soString);

                    var baseID = PersistentPlayerBases?.AsArray()?.ToList()
                        .Where(s => s?["CVX"]?.ToString() == index.ToString())
                        .Select(s => PersistentPlayerBases.AsArray().ToList().IndexOf(s))
                        .FirstOrDefault() ?? -1;
                    if (BaseShipIndex.Contains(baseID)) throw new Exception("无法导入普通船到自定义飞船！\n请导入完整的自定义飞船 .nmsship 文件，或选择一个非自定义飞船的飞船槽位。");
                    if (isNew) { index = findAvailableShipID(); }
                    if (index == -1) { throw new Exception("无法添加新飞船！\n你的飞船已经满了。"); }
                    await Task.Run(() => { writeCCD(ccd!, index, isOb); });
                    await Task.Run(() => { writeSO(so!, index, isOb); });
                }
            }
            else if (importPath.Split('.').Last() == "json")
            {
                var jsonString = File.ReadAllText(importPath);
                JsonNode? json = JsonNode.Parse(jsonString);
                if (json == null) { throw new Exception("文件内容无法解析！"); }
                if (json is JsonObject)
                    await Task.Run(() => { writeSO(json, index, isOb); });
                else await Task.Run(() => { writeBaseObject(json, -1, index, isOb); });

            }
            else if (importPath.Split('.').Last() == "sh0")
            {
                var jsonString = File.ReadAllText(importPath);
                JsonNode? json = JsonNode.Parse(jsonString);
                if (json == null) { throw new Exception("文件内容无法解析！"); }
                await Task.Run(() => { writeSO(json, index, isOb); });

            }
            else
            {
                throw new Exception("未选择文件或不受支持的文件格式！");
            }

            await Task.Run(() => { SaveSave(currentSave!); });
            MessageClass.InfoMessageBox("导入成功！");
        }

        public async Task importShipTech(
            int index,
            string importPath,
            bool isOb
            )
        {
            if (index < 0 && index > 11) throw new Exception("传入的飞船 ID 异常！");
            if (importPath.Split('.').Last() != "json" && importPath.Split('.').Last() != "tech")
                throw new Exception("未选择文件或不受支持的文件格式！");
            var jsonString = File.ReadAllText(importPath);
            JsonNode? json = JsonNode.Parse(jsonString);
            if (json == null) { throw new Exception("文件内容无法解析！"); }
            await Task.Run(() => { writeTech(json, index, isOb); });
            await Task.Run(() => { SaveSave(currentSave!); });
            MessageClass.InfoMessageBox("导入成功！");
        }

        public async Task setShipSeed(
            int shipID,
            string seed
            )
        {
            if (shipID == -1) throw new Exception("尚未选择飞船！请先选择你要设置的飞船。");
            if (seed == null) throw new Exception("尚未输入种子！请输入种子以设置飞船。");

            await Task.Run(() => { writeSeed(seed, shipID); });
            await Task.Run(() => { SaveSave(currentSave!); });
            MessageClass.InfoMessageBox("设置种子成功：" + seed);
        }

        public async Task exportShip(
            int index,
            string exportPath,
            string fileName,
            bool isOb,
            bool isOOnly,
            bool isSOOnly,
            bool isSH0
            )
        {
            var saveFilePath = "";
            if (!isOOnly && !isSOOnly)
            {
                if (index == -1) throw new Exception("尚未选择要导出的飞船！");

                if (exportPath == "" || !Path.Exists(exportPath)) throw new Exception("未选择导出路径或路径无效！");

                var objectsString = readBaseObject(-1, index, isOb);
                var ccdString = readCCD(index, isOb);
                var soString = readSO(index, isOb);
                
                if (objectsString == "" && (ccdString == "" || soString == "")) throw new Exception("飞船数据异常，无法导出！");
                
                var tempDir = Path.Combine(System.IO.Path.GetTempPath(), "NMSMIOT_Temp");
                if (Directory.Exists(tempDir)) { Directory.Delete(tempDir, true); }
                Directory.CreateDirectory(tempDir);
                
                var objectsPath = Path.Combine(tempDir, "objects.json");
                File.WriteAllText(objectsPath, objectsString);
                var ccdPath = Path.Combine(tempDir, "ccd.json");
                File.WriteAllText(ccdPath, ccdString);
                var soPath = Path.Combine(tempDir, "so.json");
                File.WriteAllText(soPath, soString);

                if (fileName == "")
                {
                    if (!string.IsNullOrEmpty(ShipOwnership?.AsArray()?.ElementAt(index)?["NKm"]?.ToString()))
                        fileName = ShipOwnership?.AsArray()?.ElementAt(index)?["NKm"]?.ToString()
                        ?? defaultExportNameString;
                    else fileName = defaultExportNameString;
                }
                saveFilePath = Path.Combine(exportPath, fileName + ".nmsship");
                ShipPackage.Pack([objectsPath, ccdPath, soPath], saveFilePath);
                Directory.Delete(tempDir, true);
            }
            else {
                if (isSOOnly && isOOnly) throw new Exception("不得同时设置仅限 Object 和 SO 数据！");
                if (index == -1) throw new Exception("尚未选择要导出的飞船！");
                var stringJN = isOOnly ? readBaseObject(-1, index, isOb) :readSO(index, isOb);
                if (stringJN == "") throw new Exception("飞船数据为空，无法导出！");

                if (fileName == "")
                {
                    if (!string.IsNullOrEmpty(ShipOwnership?.AsArray()?.ElementAt(index)?["NKm"]?.ToString()))
                        fileName = ShipOwnership?.AsArray()?.ElementAt(index)?["NKm"]?.ToString()
                        ?? defaultExportNameString;
                    else fileName = defaultExportNameString;
                }
                saveFilePath = Path.Combine(exportPath, fileName + (isSH0 ? ".sh0" : ".json"));
                File.WriteAllText(saveFilePath, stringJN);
            }
            MessageClass.InfoMessageBox("导出文件成功：" + saveFilePath);
        }

        public async Task exportShipTech(
            int index,
            string exportPath,
            string fileName,
            bool isO,
            bool isTech
            )
        {
            if (index == -1) throw new Exception("尚未选择要导出的飞船！");
            var stringJN = readTech(index, isO);
            if (stringJN == "" || stringJN.Count() < 4) throw new Exception("飞船技术、模块与库存数据异常，无法导出！");

            if (fileName == "")
            {
                if (!string.IsNullOrEmpty(ShipOwnership?.AsArray()?.ElementAt(index)?["NKm"]?.ToString()))
                    fileName = ShipOwnership?.AsArray()?.ElementAt(index)?["NKm"]?.ToString()
                    ?? defaultExportNameString + "_技术、模块与库存";
                else fileName = defaultExportNameString + "_技术、模块与库存";
            }
            var saveFilePath = Path.Combine(exportPath, fileName + (isTech ? ".tech" : ".json"));
            File.WriteAllText(saveFilePath, stringJN);
            MessageClass.InfoMessageBox("导出文件成功：" + saveFilePath);
        }

        #endregion
    }
}