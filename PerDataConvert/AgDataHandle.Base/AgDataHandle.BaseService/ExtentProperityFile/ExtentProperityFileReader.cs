using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.ExtentProperityFile
{
    public class ExtentProperityFileReader
    {
        /// <summary>
        /// uuid所在的列的序号
        /// </summary>
        public ExtentProprityFileReaderBase m_extentPFR;
        private List<string> m_filedNames = new List<string>();
        private Dictionary<string, string> nodeProperities = new Dictionary<string, string>();

        #region 读取附加的文件
        /// <summary>
        /// Link_Part001-A_1#.json
        /// </summary>
        /// <param name="extentProperityPath"></param>
        public void AddExtentProperityFile(string extentProperityPath)
        {
            if (File.Exists(extentProperityPath) == false)
                return;
            if (new FileInfo(extentProperityPath).Length > 1024 * 1024 * 1024 * 1.5)
            {
                m_extentPFR = new BigExtentProperityFileReader();
            }
            else
            {
                m_extentPFR = new SmallExtentProprityFileReader();
            }
            m_extentPFR.FiledNames = m_filedNames;
            m_extentPFR.AddExtentProperityFile(extentProperityPath);
        }
        #endregion

        #region 计算属性
        public string GetAttribute(string objectId)
        {
            if (nodeProperities.ContainsKey(objectId))
            {
                var res = nodeProperities[objectId];
                nodeProperities.Remove(objectId);
                return res;
            }
            string extentStr = "";
            if (m_extentPFR != null)
            {
                extentStr = m_extentPFR.GetProperity(objectId);
            }
            return extentStr;
        }

        public string GetAttributeByObjectId(string objectId, string attrName)
        {
            var extentStr = m_extentPFR?.GetProperity(objectId);
            string attrValue = null;
            if (extentStr != null)
            {
                var bb = (JObject)JsonConvert.DeserializeObject(extentStr);
                if (bb != null)
                {
                    attrValue = bb[attrName]?.ToString().Trim();
                }
            }
            return attrValue;
        }

        public bool isextentPFR()
        {
            return m_extentPFR == null ? false : true;
        }
        public bool IsBuilding()
        {
            return m_extentPFR.BuildingProperities.Count > 0 ? true : false;
        }
        public string GetBuilding()
        {
            string value;
            m_extentPFR.BuildingProperities.TryGetValue("0", out value);
            return value;
        }
        public int GetComponentCount()
        {
            return m_extentPFR.M_ComponentCount;
        }
        public string GetGltfSouce()
        {
            return m_extentPFR.GltfSource;
        }
        #endregion
    }
}
