using AgDataHandle.BaseService.IOJson;
using AgDataHandle.BaseService.Utils;
using System.Text;

namespace AgDataHandle.BaseService.ExtentProperityFile
{
    /// <summary>
    /// 本版本只支持一条一条的方式
    /// </summary>
    public class BigExtentProperityFileReader : ExtentProprityFileReaderBase
    {
        private string m_extentProperityPath;
        private int m_readOutFileCount = 0;
        private Dictionary<int, List<string>> subFiles = new Dictionary<int, List<string>>();

        public override void AddExtentProperityFile(string extentProperityPath)
        {
            m_extentProperityPath = extentProperityPath;

            GetGltfSourceInfo(extentProperityPath);

            var fs = File.OpenText(extentProperityPath);
            StringBuilder sb = new StringBuilder();
            int numOfUuid = 0;
            while (fs.EndOfStream == false)
            {
                var line = fs.ReadLine();
                sb.Append(line);
                if (GltfSource.IsNullOrEmpty() && line.IndexOf("RvtConvert") != -1)
                    GltfSource = "RvtConvert";
                var t = line.IndexOf("\"uuid\"");
                if (t >= 0)
                {
                    numOfUuid++;
                }
                if (numOfUuid == 2)
                {
                    break;
                }
            }
            ReadBuildingInfo(fs);
            fs.Close();
            fs.Dispose();
            var sbStr = sb.ToString();
            var t1 = sbStr.IndexOf("\"uuid\"");
            var t2 = GetEndYH(sbStr, t1);
            var qr = sbStr.Substring(t1, t2 - t1);
            qr = "{" + qr + "}";
            SelfJSONReader jr = new SelfJSONReader(qr);
            m_extentUUIDIndex = -1;

            for (var i = 0; i < jr.Items.Count; i++)
            {
                var key = jr.Items[i].Key;
                if (key == "uuid")
                {
                    m_extentUUIDIndex = i;
                    continue;
                }
                FiledNames.Add(key);
            }
            ReadAllUuid(m_extentProperityPath);
            LoadStatue = true;
        }

        #region 存储数据
        private void ReadBuildingInfo(StreamReader fs)
        {
            var isBuilding = false;
            var sb = new StringBuilder();
            while (fs.EndOfStream == false)
            {
                var line = fs.ReadLine();
                if (isBuilding == false && line.IndexOf("\"building\"") != -1)
                    isBuilding = true;
                if (!isBuilding)
                    continue;
                sb.Append(line);
            }
            var sbStr = sb.ToString();
            var t1 = sbStr.IndexOf("\"building\"");
            var t2 = GetEndYH(sbStr, t1);
            var qr = sbStr.Substring(t1, t2 - t1);
            qr = "{" + qr + "}";
            BuildingProperities.Add("0", qr);
        }
        private void ReadAllUuid(string extentProperityPath)
        {
            var fs = File.OpenText(extentProperityPath);

            List<string> uuid = new List<string>();
            StringBuilder sb = new StringBuilder();
            int fileIndex = 0;
            var hasBegin = false;
            while (fs.EndOfStream == false)
            {
                var line = fs.ReadLine();
                if (hasBegin)
                    sb.Append(line);
                var t = line.IndexOf("\"uuid\"");
                if (t > 0)
                {
                    if (hasBegin == false)
                    {
                        sb.Append(line);
                        hasBegin = true;
                    }

                    if (uuid.Count > 2000)
                    {
                        SaveFile(fileIndex, sb.ToString(), false);
                        subFiles.Add(fileIndex++, uuid);

                        sb.Clear();
                        sb.Append(line);
                        uuid = new List<string>();
                    }
                    var lines = line.Split(':');
                    var key = lines[1].Trim(new char[] { ' ', ',', '"' });
                    if (!ExtentProperities.ContainsKey(key))
                    {
                        ExtentProperities.Add(key, "");
                    }
                    if (!uuid.Contains(key))
                    {
                        uuid.Add(key);
                    }
                }
            }
            SaveFile(fileIndex, sb.ToString(), true);
            subFiles.Add(fileIndex++, uuid);

            fs.Close();
            fs.Dispose();
        }
        private void SaveFile(int fileIndex, string results, bool isLastFile)
        {
            string tmpDir = PathLunix.Combine(Path.GetDirectoryName(m_extentProperityPath), "tempEPFR");
            if (Directory.Exists(tmpDir) == false)
            {
                Directory.CreateDirectory(tmpDir);
            }
            var result2 = "{\"items\":[{" + results;
            int t2 = GetEndYH(result2, 0);
            int t3 = result2.LastIndexOf('{');
            var t4 = result2.LastIndexOf('}');
            if (t3 > t4)
            {
                t2 = t4;
            }
            var tq = result2.Substring(0, t2);
            if (isLastFile)
            {
                var t5 = tq.LastIndexOf(']');
                tq = tq.Substring(0, t5) + "]}";
            }
            else
            {
                tq = tq + "}]}";
            }

            File.WriteAllText(PathLunix.Combine(tmpDir, fileIndex.ToString()), tq);
        }
        #endregion

        private int m_lastFileIndex = -1;
        private int m_lastRound = 0;
        private Dictionary<string, string> m_subUUIDMatch = new Dictionary<string, string>();
        private removedItem m_removedItems = new removedItem();
        /// <summary>
        /// 假设一定有
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public override string GetProperity(string uuid)
        {
            if (m_removedItems.ContainsKey(uuid))
            {
                return m_removedItems.Get(uuid);
            }
            if (m_subUUIDMatch.ContainsKey(uuid))
            {
                var t = m_subUUIDMatch[uuid];
                lock (m_removedItems)
                {
                    m_removedItems.Add(uuid, t);
                }
                lock (m_subUUIDMatch)
                {
                    m_subUUIDMatch.Remove(uuid);
                }
                return t;
            }

            foreach (var key in subFiles.Keys)
            {
                if (subFiles[key].IndexOf(uuid) >= 0)
                {
                    string tmpDir = PathLunix.Combine(Path.GetDirectoryName(m_extentProperityPath), "tempEPFR");
                    var block = File.ReadAllText(PathLunix.Combine(tmpDir, key.ToString()));
                    Console.WriteLine("开始读一次外部属性数据." + (++m_readOutFileCount));

                    if (block.IndexOf("\"building\"") != -1)
                        block = block.Substring(0, block.IndexOf("\"building\""));
                    SelfJSONReader jr = new SelfJSONReader(block);
                    var tt = jr.GetValue("items");
                    var componentsList = jr.SplitArrByFirstKey(tt);

                    for (var i = 0; i < componentsList.Count; i++)
                    {
                        int t1 = componentsList[i].IndexOf("uuid");
                        var t3 = componentsList[i].IndexOf(":", t1);
                        var t4 = componentsList[i].IndexOf("\"", t3);
                        var t2 = componentsList[i].IndexOf("\"", t4 + 2);
                        var suuid = componentsList[i].Substring(t4 + 1, t2 - t4 - 1);
                        if (m_subUUIDMatch.ContainsKey(suuid))
                            continue;
                        lock (m_subUUIDMatch)
                        {
                            m_subUUIDMatch.Add(suuid, componentsList[i]);
                        }
                    }

                    m_lastFileIndex = key;

                    lock (m_subUUIDMatch)
                    {
                        if (m_subUUIDMatch.ContainsKey(uuid))
                        {
                            var t = m_subUUIDMatch[uuid];
                            lock (m_removedItems)
                            {
                                m_removedItems.Add(uuid, t);
                            }
                            lock (m_subUUIDMatch)
                            {
                                m_subUUIDMatch.Remove(uuid);
                            }
                            return t;
                        }
                    }
                }
            }
            return "{}";
        }

        private int GetEndYH(string str, int fromIndex)
        {
            int t = 1;
            bool isInMH = false;
            for (var i = fromIndex; i < str.Length; i++)
            {
                if (str[i] == '"')
                {
                    isInMH = !isInMH;
                }
                if (isInMH == false)
                {
                    if (str[i] == '{')
                    {
                        t++;
                    }
                    if (str[i] == '}')
                    {
                        t--;
                    }
                    if (t == 0)
                        return i;
                }
            }
            return str.Length;
        }
    }
    public class removedItem
    {
        private Dictionary<string, string> cached = new Dictionary<string, string>();
        private Dictionary<string, int> keyLife = new Dictionary<string, int>();
        private int tick = 0;

        public void Add(string uuid, string t)
        {
            var minTike = int.MaxValue;
            var selKey = "";
            if (cached.Count > 100)
            {
                //foreach (var key in keyLife.Keys)
                lock (keyLife)
                {
                    var keys = keyLife.Keys.ToArray();
                    for (var i = 0; i < keys.Length; i++)
                    {
                        var key = keys[i];
                        if (string.IsNullOrEmpty(key))
                            continue;
                        if (keyLife.ContainsKey(key) && minTike > keyLife[key])
                        {
                            minTike = keyLife[key];
                            selKey = key;
                        }
                    }
                }
                lock (cached)
                {
                    cached.Remove(selKey);
                }
                lock (keyLife)
                {
                    keyLife.Remove(selKey);
                }
            }
            if (cached.ContainsKey(uuid) == false)
            {
                lock (cached)
                {
                    cached.Add(uuid, t);
                }
                lock (keyLife)
                {
                    keyLife.Add(uuid, tick);
                }
                tick++;
            }
        }

        public string Get(string uuid)
        {
            keyLife[uuid] = tick;
            return cached[uuid];
        }

        public bool ContainsKey(string uuid)
        {
            return cached.ContainsKey(uuid);
        }
    }

}
