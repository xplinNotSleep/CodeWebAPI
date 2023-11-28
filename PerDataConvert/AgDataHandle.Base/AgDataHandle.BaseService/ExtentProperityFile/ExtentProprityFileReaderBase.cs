namespace AgDataHandle.BaseService.ExtentProperityFile
{
    public abstract class ExtentProprityFileReaderBase
    {
        public bool LoadStatue = false;
        protected int m_extentUUIDIndex;
        private List<string> m_filedNames = new List<string>();
        // protected int m_extentFieldCount;
        private int m_componentCount = 0;
        private string gltfSource = null;
        private Dictionary<string, string> extentProperities = new Dictionary<string, string>();
        private Dictionary<string, string> buildingproperities = new Dictionary<string, string>();

        #region 属性
        public int ExtentUUIDIndex
        {
            get { return m_extentUUIDIndex; }
        }
        public string GltfSource
        {
            get { return gltfSource; }
            set { gltfSource = value; }
        }
        public List<string> FiledNames
        {
            get { return m_filedNames; }
            set { m_filedNames = value; }
        }
        public Dictionary<string, string> ExtentProperities
        {
            get { return extentProperities; }
            set { extentProperities = value; }
        }
        public Dictionary<string, string> BuildingProperities
        {
            get { return buildingproperities; }
            set { buildingproperities = value; }
        }
        public int M_ComponentCount
        {
            get { return m_componentCount; }
            set { m_componentCount = value; }
        }
        #endregion

        public abstract void AddExtentProperityFile(string extentProperityPath);

        public abstract string GetProperity(string uuid);

        protected void GetGltfSourceInfo(string extentProperityPath)
        {
            var fs = File.OpenText(extentProperityPath);
            GltfSource = null;
            while (fs.EndOfStream == false)
            {
                var line = fs.ReadLine();
                if (line.IndexOf("gltfsorce") != -1)
                {
                    var t = line.IndexOf(',');
                    var subStr = line.Substring(0, t);
                    GltfSource = subStr.Trim().Split(':')[1].Trim(new char[] { ',' });
                    fs.Close();
                    break;
                }
            }
            fs.Close();
            return;
        }
    }
}