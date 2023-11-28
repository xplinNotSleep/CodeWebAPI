using AgDataHandle.BaseService.CmptModel;
using AgDataHandle.BaseService.GltfSdk.Def;
using System.Text;

namespace AgDataHandle.BaseService.B3dmModel
{
    public abstract class BI3dmBase : IDisposable
    {
        #region 公共参数
        public BI3dmHeader head { get; protected set; }
        public string FeatureTableJson { get; set; }
        public byte[] FeatureTableBinary { get; set; }
        public string BatchTableJson { get; set; }
        public byte[] BatchTableBinary { get; set; }
        public byte[] glbData { get; set; }
        #endregion

        public int featureTableJsonCount { get; set; } = 0;
        public byte[] featureAndBatchData { get; set; }

        public BI3dmBase(string magic)
        {
            head = new BI3dmHeader(magic);
        }

        #region 计算头部和获得数据流
        /// <summary>
        /// 计算头部的比特位信息
        /// </summary>
        public void ComputeHeader()
        {
            ComputeHeader(out byte[] featureTableJsonT, out byte[] batchTableJsonT, out byte[] featureTableBinaryT, out byte[] batchTableBinaryT);
        }
        private void ComputeHeader(out byte[] featureTableJsonT, out byte[] batchTableJsonT, out byte[] featureTableBinaryT, out byte[] batchTableBinaryT)
        {
            if (FeatureTableJson.IsNullOrEmpty())
            {
                FeatureTableJson = $"{{\"BATCH_LENGTH\":{featureTableJsonCount}}}  ";
            }
            if (BatchTableJson.IsNullOrEmpty())
            {
                BatchTableJson = string.Empty;
            }
            featureTableJsonT = Encoding.UTF8.GetBytes(BufferPadding.AddPadding(FeatureTableJson));
            batchTableJsonT = Encoding.UTF8.GetBytes(BufferPadding.AddPadding(BatchTableJson));
            featureTableBinaryT = BufferPadding.AddPadding(FeatureTableBinary);
            batchTableBinaryT = BufferPadding.AddPadding(BatchTableBinary);

            head.byteLength = glbData.Length + head.Length + featureTableJsonT.Length + batchTableJsonT.Length + batchTableBinaryT.Length + featureTableBinaryT.Length;
            head.featureTableJSONByteLength = featureTableJsonT.Length;
            head.batchTableJSONByteLength = batchTableJsonT.Length;
            head.featureTableBinaryByteLength = featureTableBinaryT.Length;
            head.batchTableBinaryByteLength = batchTableBinaryT.Length;
        }
        public byte[] GetBytes()
        {
            ComputeHeader(out byte[] featureTableJsonT, out byte[] batchTableJsonT, out byte[] featureTableBinaryT, out byte[] batchTableBinaryT);

            var memoryStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(head.GetHeadBytes());
            binaryWriter.Write(featureTableJsonT);
            if (featureTableBinaryT != null)
            {
                binaryWriter.Write(featureTableBinaryT);
            }
            binaryWriter.Write(batchTableJsonT);
            if (batchTableBinaryT != null)
            {
                binaryWriter.Write(batchTableBinaryT);
            }
            binaryWriter.Write(glbData);
            binaryWriter.Flush();
            binaryWriter.Close();
            return memoryStream.ToArray();
        }
        #endregion


        #region 其他
        //public BI3dmBase(string path) { }

        //public BI3dmBase(byte[] glbbyte) { }

        public BI3dmBase(BinaryReader reader, BI3dmHeader b3dmHeader)
        {
            var list = new List<byte>();
            head = b3dmHeader;
            long t1 = reader.BaseStream.Position;
            NavigateToKuoHaoSign(reader);
            long t2 = reader.BaseStream.Position;
            if (t1 != t2)
            {
                reader.BaseStream.Position = t1;
                head.EndBytes = reader.ReadBytes((int)(t2 - t1));
            }

            var featureJson = reader.ReadBytes(head.featureTableJSONByteLength);
            list.AddRange(featureJson.ToList());
            FeatureTableJson = Encoding.UTF8.GetString(featureJson);

            FeatureTableBinary = reader.ReadBytes(head.featureTableBinaryByteLength);
            list.AddRange(FeatureTableBinary.ToList());

            var batchJson = reader.ReadBytes(head.batchTableJSONByteLength);
            list.AddRange(batchJson.ToList());

            BatchTableJson = Encoding.UTF8.GetString(batchJson);
            BatchTableBinary = reader.ReadBytes(head.batchTableBinaryByteLength);
            list.AddRange(BatchTableBinary.ToList());

            var glbLength = head.byteLength - 28 - head.featureTableJSONByteLength - head.featureTableBinaryByteLength
                - head.batchTableJSONByteLength - head.batchTableBinaryByteLength;
            if (glbLength < 0)
            {

            }
            if (head.magic == "i3dm")
            {
                glbLength -= 4;
            }

            if (head.gltfFormat == 0)
            {
                var byteStr = reader.ReadBytes(glbLength);
                var modelUrl = Encoding.UTF8.GetString(byteStr);

                //读取文件为glb
                var dir = Path.GetDirectoryName((reader.BaseStream as FileStream).Name);
                var glbPath = Path.Combine(dir, modelUrl);
                if (glbPath.Contains(".glb"))
                {
                    var glbModel = GLB.LoadFromFile(glbPath);
                    glbData = glbModel.GetAllBytes();
                }
                else
                {
                    var gltf = GLTF.LoadFromFile(glbPath);
                    glbData = GLB.LoadFromGLTF(gltf).GetAllBytes();
                }

            }
            else
            {

                glbData = reader.ReadBytes(glbLength);
                if (glbData[0] != 103 && glbData[1] != 108 && glbData[2] != 84 && glbData[3] != 70)
                {
                    glbData = NavigateToGltfSign(glbData);
                }
                //head = b3dmHeader;
                featureAndBatchData = list.ToArray();

                if (glbData[glbData.Length - 4] == 105 && glbData[glbData.Length - 3] == 51 && glbData[glbData.Length - 2] == 100 && glbData[glbData.Length - 1] == 109)
                {

                }
            }
        }

        #region 获得内容
        public GLB GetGlb()
        {
            return GLB.LoadFromStream(new MemoryStream(glbData));
        }
        #endregion

        #region
        public abstract void Export(string path);
        #endregion

        protected void NavigateToKuoHaoSign(BinaryReader reader)
        {
            var t = reader.PeekChar();
            while (t != 123)
            {
                reader.ReadByte();
                t = reader.PeekChar();
            }
        }
        protected byte[] NavigateToGltfSign(byte[] glbData)
        {
            var t = glbData[0] != 103 && glbData[1] != 108 && glbData[2] != 84 && glbData[3] != 70;
            var index = 0;
            while (t)
            {
                index++;
                t = glbData[index + 0] != 103 && glbData[index + 1] != 108 && glbData[index + 2] != 84 && glbData[index + 3] != 70;
            }
            if (index != 0 && index < 5)
            {
                byte[] copyGlbData = new byte[glbData.Length - index];
                for (var i = 0; i < copyGlbData.Length; i++)
                {
                    copyGlbData[i] = glbData[index + i];
                }
                return copyGlbData;
            }
            return glbData;
        }

        #endregion

        public void Dispose()
        {
            if (FeatureTableBinary != null)
            {
                FeatureTableBinary.Clear(0, FeatureTableBinary.Length);
                FeatureTableBinary = null;
            }
            if (BatchTableBinary != null)
            {
                BatchTableBinary.Clear(0, BatchTableBinary.Length);
                BatchTableBinary = null;
            }
            if (glbData != null)
            {
                glbData.Clear(0, glbData.Length);
                glbData = null;
            }
            if (featureAndBatchData != null)
            {
                featureAndBatchData.Clear(0, featureAndBatchData.Length);
                featureAndBatchData = null;
            }
        }
    }
}
