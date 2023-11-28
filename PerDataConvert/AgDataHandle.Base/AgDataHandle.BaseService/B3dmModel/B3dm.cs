using AgDataHandle.BaseService.GltfSdk.Def;
using System.Text;

namespace AgDataHandle.BaseService.B3dmModel
{
    public class B3dm : BI3dmBase
    {
        private string m_path;
        //private byte[] allBytes;

        #region 构造函数
        public B3dm() : base("b3dm") { }

        /// <summary>
        /// 根据B3DM文件构造B3DM
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="Exception"></exception>
        public B3dm(string path) : base("b3dm")
        {
            m_path = path;
            if (!File.Exists(m_path))
                throw new Exception("文件不存在：" + m_path);

            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(fs))
                {
                    var b3dmHeader = BI3dmHelper.ReadHead(reader);
                    var list = new List<byte>();

                    NavigateToKuoHaoSign(reader);

                    var featureJson = reader.ReadBytes(b3dmHeader.featureTableJSONByteLength);
                    list.AddRange(featureJson.ToList());
                    FeatureTableJson = Encoding.UTF8.GetString(featureJson);
                    FeatureTableBinary = reader.ReadBytes(b3dmHeader.featureTableBinaryByteLength);
                    list.AddRange(FeatureTableBinary.ToList());
                    var batchJson = reader.ReadBytes(b3dmHeader.batchTableJSONByteLength);
                    list.AddRange(batchJson.ToList());
                    BatchTableJson = Encoding.UTF8.GetString(batchJson);
                    BatchTableBinary = reader.ReadBytes(b3dmHeader.batchTableBinaryByteLength);
                    list.AddRange(BatchTableBinary.ToList());
                    var glbLength = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                    glbData = reader.ReadBytes(glbLength);
                    if (glbData[0] != 103 && glbData[1] != 108 && glbData[2] != 84 && glbData[3] != 70)
                    {
                        glbData = NavigateToGltfSign(glbData);
                    }
                    head = b3dmHeader;
                    featureAndBatchData = list.ToArray();
                }
            }
        }

        /// <summary>
        /// 根据GLB文件构件B3DM
        /// </summary>
        /// <param name="gLB"></param>
        public B3dm(GLB gLB) : base("b3dm")
        {
            glbData = gLB.GetAllBytes();
        }

        /// <summary>
        /// 根据GLTF文件构造B3DM
        /// </summary>
        /// <param name="gLTF"></param>
        public B3dm(GLTF gLTF) : base("b3dm")
        {
            GLB gLB = GLB.LoadFromGLTF(gLTF);
            glbData = gLB.GetAllBytes();
        }

        /// <summary>
        /// 根据GLB字节构造B3DM
        /// </summary>
        /// <param name="glbbyte"></param>
        public B3dm(byte[] glbbyte) : base("b3dm")
        {
            glbData = glbbyte;
        }

        public B3dm(BinaryReader reader, BI3dmHeader b3dmHeader) : base(reader, b3dmHeader)
        {
        }
        #endregion

        #region 导出
        public override void Export(string path)
        {
            var bytes = GetBytes();
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllBytes(path, bytes);
        }
        #endregion


    }

}
