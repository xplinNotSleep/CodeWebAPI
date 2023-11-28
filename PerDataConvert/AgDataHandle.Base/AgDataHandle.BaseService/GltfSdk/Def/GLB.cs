using AgDataHandle.BaseService.CommonHelper;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    public class GLB
    {
        private const string JSON_MATCH_STRING = "JSON{";

        public GLB()
        {
        }

        #region Properties
        private GLTF _GLTF { get; set; }
        public GLTF GLTF
        {
            get
            {
                if (_GLTF != null)
                {
                    return _GLTF;
                }
                _GLTF = LoadFromGLBStream(BinaryDataStream, JSONStartPosition, JSONByteLength);
                return _GLTF;
            }
        }
        public int JSONStartPosition { get; set; }
        public int JSONByteLength { get; set; }
        public int BinaryDataStartPosition { get; set; }

        public GLBType Type { get; set; }

        public Stream BinaryDataStream
        {
            get;
            set;
        }

        #endregion

        #region Methods
        public static GLB LoadFromGLTF(GLTF model)
        {
            GLB glb = new GLB
            {
                _GLTF = model
            };

            model.Buffers.MergeBufferIntoOne(model.DirectoryName);
            glb.BinaryDataStream = model.Buffers[0].DataStream;
            glb.JSONByteLength = EncodingHelper.StringToBytes(model.GetJSONStr(true)).Length;
            glb.BinaryDataStartPosition = 28 + glb.JSONByteLength;
            glb.Type = GLBType.FromGLTF;
            return glb;
        }

        public static GLB LoadFromStream(Stream stream)
        {
            GLB glb = new GLB() { Type = GLBType.FromStream };
            glb.BinaryDataStream = new MemoryStream();
            stream.CopyTo(glb.BinaryDataStream);
            glb.BinaryDataStream.Position = 8;
            byte[] num = new byte[8];
            glb.BinaryDataStream.Read(num, 0, 8);
            glb.JSONByteLength = (int)BitConverter.ToUInt32(num, 4);

            glb.BinaryDataStream.Position = 0;

            glb.JSONStartPosition = ComputeJsonTextPosition(glb.BinaryDataStream);
            glb.BinaryDataStream.Position = 0;
            glb.BinaryDataStartPosition = glb.JSONStartPosition + glb.JSONByteLength;
            return glb;
        }

        public static GLB LoadFromFile(string filePath)
        {
            #region 兼容gltf文件
            if (Path.GetExtension(filePath).ToLower() == ".gltf")
            {
                return LoadFromGLTF(GLTF.LoadFromFile(filePath));
            }
            #endregion
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                GLB glb = LoadFromStream(fs);
                glb.Type = GLBType.FromFile;
                return glb;
            }
        }

        public byte[] GetAllBytes()
        {

            // 拼凑 chunk2.data
            var binaryData = GLTF.Buffers[0].ReadData();
            // 拼凑 chunk1.data
            byte[] jsonInBytes = EncodingHelper.StringToBytes(GLTF.GetJSONStr(true));
            jsonInBytes = EncodingHelper.FillBlank(jsonInBytes);

            int togetherLength = jsonInBytes.Length + binaryData.Length + 28;
            byte[] together = ArrayHelper.MergeArray(
                GLBSchemaDef.GLBHeaderMagic, BitConverter.GetBytes((uint)2), BitConverter.GetBytes((uint)togetherLength),
                BitConverter.GetBytes((uint)jsonInBytes.Length), GLBSchemaDef.GLBJsonChunkType, jsonInBytes,
                BitConverter.GetBytes((uint)binaryData.Length), GLBSchemaDef.GLBBinaryChunkType, binaryData
                );
            return together;
        }

        public void Save(string path)
        {
            var all = GetAllBytes();
            File.WriteAllBytes(path, all);
        }

        #endregion

        #region 解析glb的stream为GLTF
        static GLTF LoadFromGLBStream(Stream stream, int start, int length)
        {
            if (length == -1)
                length = (int)stream.Length - start;
            if (start == 0)
            {
                start = ComputeJsonTextPosition(stream);
            }
            stream.Position = start;

            int readLength = (int)(stream.Length - start);
            byte[] jsonStrInBytes = new byte[readLength];
            stream.Read(jsonStrInBytes, 0, readLength);
            Stream dataStream = new MemoryStream(jsonStrInBytes);

            TextReader sr = new StreamReader(dataStream);
            JsonReader jr = new JsonTextReader(sr);
            var model = GLTF.LoadFromJsonReader(jr);
            byte[] num = new byte[4];
            stream.Position = start + length;
            stream.Read(num, 0, 4);
            int binCount = (int)BitConverter.ToUInt32(num, 0);
            model.Buffers[0].DataStream = new MemoryStream();
            stream.Position = start + length + 8;
            stream.CopyTo(model.Buffers[0].DataStream, binCount);
            model.BindGLTF(); // 重要
            return model;
        }

        private static int ComputeJsonTextPosition(Stream stream)
        {
            byte[] buffer = stream.ReadAllBytes();
            byte[] strArr = JSON_MATCH_STRING.ToByteArray();

            int index = buffer.FindArrayInArray(strArr);

            return index + 4;
        }

        #endregion
    }

    public enum GLBType
    {
        FromFile,
        FromStream,
        FromGLTF,
        FromB3dmStream
    }

    public static class GLBSchemaDef
    {
        public static byte[] GLBJsonChunkType
        {
            get { return BitConverter.GetBytes(0x4E4F534A); }
        }
        public static byte[] GLBBinaryChunkType
        {
            get { return BitConverter.GetBytes(0x004E4942); }
        }
        public static byte[] GLBHeaderMagic
        {
            get { return BitConverter.GetBytes(0x46546C67); }
        }
    }
}
