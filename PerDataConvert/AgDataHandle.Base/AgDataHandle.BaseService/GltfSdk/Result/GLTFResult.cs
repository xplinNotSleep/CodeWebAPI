using AgDataHandle.Maths.Numerics;
using AgDataHandle.BaseService.GltfSdk.Append;
using AgDataHandle.BaseService.GltfSdk.Collection;
using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.GltfSdk.ENUM;

namespace AgDataHandle.BaseService.GltfSdk.Result
{
    /// <summary>
    /// 每个gltf包含的几何结果集
    /// </summary>
    public class GLTFResult
    {
        #region 属性-范围
        public double MinX { get { return GeometryResults.Min(p => p.MinX); } }
        public double MaxX { get { return GeometryResults.Max(p => p.MaxX); } }
        public double MinY { get { return GeometryResults.Min(p => p.MinY); } }
        public double MaxY { get { return GeometryResults.Max(p => p.MaxY); } }
        public double MinZ { get { return GeometryResults.Min(p => p.MinZ); } }
        public double MaxZ { get { return GeometryResults.Max(p => p.MaxZ); } }

        public BoundingBox M_BoundingBox { get; set; } = new BoundingBox();
        #endregion

        #region 属性-几何材质纹理
        /// <summary>
        /// 所有的几何结果
        /// </summary>
        public GLTFNodeResultCollection GeometryResults { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public GLTFMaterialCollection Materials { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public List<byte[]> ImageDatas { get; set; }
        public GLTFImageCollection Images { get; set; }
        #endregion

        #region 属性-字节流
        public GLTFBufferType BufferType { get; set; }
        public MemoryStream DataStream { get; set; }
        public List<byte> BinaryData { get; set; }
        public int ByteLength
        {
            get
            {
                if (BufferType == GLTFBufferType.BinaryData)
                {
                    return BinaryData.Count;
                }
                else
                {
                    return (int)DataStream.Length;
                }
            }
        }
        #endregion

        #region 支持扩展
        public bool HasLineIndex { get; set; } = false;
        #endregion

        #region 构造函数
        public GLTFResult(GLTFBufferType bufferType = GLTFBufferType.Memory)
        {
            if (bufferType == GLTFBufferType.Memory)
            {
                DataStream = new MemoryStream();
            }
            else
            {
                BinaryData = new List<byte>();
            }
            BufferType = bufferType;
            GeometryResults = new GLTFNodeResultCollection();
        }
        #endregion

        #region 添加获得字节流
        /// <summary>
        /// 增加字节
        /// </summary>
        /// <param name="b"></param>
        public void Add(byte b)
        {
            if (BufferType == GLTFBufferType.BinaryData)
            {
                BinaryData.Add(b);
            }
            else
            {
                DataStream.WriteByte(b);
            }
        }

        /// <summary>
        /// 增加字节数组
        /// </summary>
        /// <param name="bs"></param>
        public void AddRange(byte[] bs)
        {
            if (BufferType == GLTFBufferType.BinaryData)
            {
                BinaryData.AddRange(bs);
            }
            else
            {
                DataStream.Write(bs, 0, bs.Length);
            }
        }

        /// <summary>
        /// 增加字节列表
        /// </summary>
        /// <param name="bs"></param>
        public void AddRange(List<byte> bs)
        {
            if (BufferType == GLTFBufferType.BinaryData)
            {
                BinaryData.AddRange(bs);
            }
            else
            {
                DataStream.Write(bs.ToArray(), 0, bs.Count);
            }
        }

        /// <summary>
        /// 获取所有字节
        /// </summary>
        /// <returns></returns>
        public List<byte> GetBufferBytes()
        {
            if (BufferType == GLTFBufferType.BinaryData)
            {
                return BinaryData;
            }
            else
            {
                byte[] bs = new byte[DataStream.Length];
                DataStream.Read(bs, 0, bs.Length);
                // 设置当前流的位置为流的开始
                DataStream.Seek(0, SeekOrigin.Begin);
                return bs.ToList();
            }

        }
        #endregion

        #region 材质和纹理
        private Dictionary<int, int> dictMaterialIndex { get; set; } = new Dictionary<int, int>();
        private Dictionary<int, int> dictImageIndex { get; set; } = new Dictionary<int, int>();
        /// <summary>
        /// 是否包含材质
        /// </summary>
        /// <param name="MaterialIndex"></param>
        /// <returns></returns>
        public bool ContainsMaterial(int MaterialIndex)
        {
            if (!dictMaterialIndex.ContainsKey(MaterialIndex))
            {
                dictMaterialIndex.Add(MaterialIndex, dictMaterialIndex.Values.Count > 0 ? dictMaterialIndex.Values.Last() + 1 : 0);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取新材质id
        /// </summary>
        /// <param name="MaterialIndex"></param>
        /// <returns></returns>
        public int GetNewMaterialIndex(int MaterialIndex)
        {
            return dictMaterialIndex[MaterialIndex];
        }


        /// <summary>
        /// 是否包含图片
        /// </summary>
        /// <param name="ImageIndex"></param>
        /// <returns></returns>
        public bool ContainsImage(int ImageIndex)
        {
            if (!dictImageIndex.ContainsKey(ImageIndex))
            {
                dictImageIndex.Add(ImageIndex, dictImageIndex.Values.Count > 0 ? dictImageIndex.Values.Last() + 1 : 0);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取新图片id
        /// </summary>
        /// <param name="MaterialIndex"></param>
        /// <returns></returns>
        public int GetNewImageIndex(int ImageIndex)
        {
            return dictImageIndex[ImageIndex];
        }
        #endregion

        #region 导出Gltf和B3DM
        /// <summary>
        /// 结果集快速转成gltf
        /// </summary>
        /// <param name="sourceDirectoryName"></param>
        /// <returns></returns>
        public GLTF ToGltf()
        {
            GLTF glTF = new GLTF();
            if (HasLineIndex)
            {
                glTF.ExtensionsUsed = new List<string>() { "CESIUM_primitive_outline" };
            }

            var buffer = new GLTFBuffer();
            buffer.DataStream = DataStream;
            buffer.ByteLength = ByteLength;
            glTF.Buffers.Add(buffer);
            glTF.Scenes.Add(new GLTFScene());
            glTF.Scenes[0].Nodes.Add(0);
            GLTFNode firstNode = new GLTFNode();
            firstNode.ChildrenID = new List<int>();
            glTF.Nodes.Add(firstNode);
            for (int i = 0; i < GeometryResults.Count; i++)
            {
                GLTFNode gLTFNode = new GLTFNode();
                glTF.Nodes.Add(gLTFNode);
                GLTFMesh gLTFMesh = new GLTFMesh();
                glTF.Meshes.Add(gLTFMesh);
                gLTFNode.MeshIndex = glTF.Meshes.Count - 1;
                gLTFNode.Extensions = GeometryResults[i].Extensions;
                firstNode.ChildrenID.Add(glTF.Nodes.Count - 1);
                gLTFMesh.Primitives.AddRange(GeometryResults[i].gLTFPrimitives.ToArray());
                glTF.Accessors.AddRange(GeometryResults[i].gLTFAccessors.ToArray());
                glTF.BufferViews.AddRange(GeometryResults[i].gLTFBufferViews.ToArray());
            }
            if (Materials != null)
            {
                glTF.Materials.AddRange(Materials.ToArray());
            }
            if (Images != null)
            {
                new ImageToGLTFBuffer().AppendTo(glTF, buffer, Images);
            }
            if (glTF.Samplers == null)
            {
                //取样器,定义图片的采样和滤波方式
                GLTFSampler sampler = new GLTFSampler();
                sampler.MagFilter = GLTFMagFilter.Linear.GetHashCode();
                sampler.MinFilter = GLTFMinFilter.LinearMipmapLinear.GetHashCode();
                sampler.WrapS = GLTFWrappingMode.Repeat.GetHashCode();
                sampler.WrapT = GLTFWrappingMode.Repeat.GetHashCode();
                glTF.Samplers = new GLTFSamplerCollection();
                glTF.Samplers.Add(sampler);
            }
            glTF.BindGLTF();
            return glTF;
        }

        #endregion
    }
}
