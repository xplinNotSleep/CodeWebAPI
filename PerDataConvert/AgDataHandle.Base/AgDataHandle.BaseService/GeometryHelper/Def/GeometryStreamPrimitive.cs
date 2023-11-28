using AgDataHandle.Maths;
using AgDataHandle.Maths.Numerics;
using AgDataHandle.BaseService.BufferView;
using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.GltfSdk.ENUM;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GeometryHelper.Def
{
    public class PrimitiveBufferView
    {
        /// <summary>
        /// 使用的buffer的下标
        /// </summary>
        public int BufferIndex { get; set; }
        /// <summary>
        /// 在当前buffer的偏移位，等于总体的偏移量+bufferview内的偏移量
        /// </summary>
        public int ByteOffset { get; set; }
        /// <summary>
        /// 数组中每个变量的长度
        /// </summary>
        public int ByteStride { get => AccPerLength * ComponentLength; }
        /// <summary>
        /// 数组总字节长度
        /// </summary>
        public int ByteLength { get => ByteStride * Count; }
        /// <summary>
        /// 顶点数据还是索引数据
        /// </summary>
        public GLTFTargets Target { get; set; }
        /// <summary>
        /// 变量数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 变量类型
        /// </summary>
        public GLTFAccessorComponentType ComponentType { get; set; }
        /// <summary>
        /// 数组的数据类型
        /// </summary>
        public GLTFAccessorType AccessorType { get; set; }
        /// <summary>
        /// 每个数组的纬度长度
        /// </summary>
        public int AccPerLength { get; set; }
        /// <summary>
        /// 变量类型长度
        /// </summary>
        public int ComponentLength { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public List<float> Min { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public List<float> Max { get; set; }
    }

    public class GeometryStreamPrimitive : GeometryPrimitive
    {
        GLTF GLTF { get; set; }
        private List<Stream> DataStreams { get; set; }
        public GeometryStreamPrimitive(GLTF gLTF, List<Stream> streams)
        {
            PrimitiveType = SplitPrimitiveType.Stream;
            GLTF = gLTF;
            DataStreams = streams;
        }

        #region 图元buffer,提供流的方式
        /// <summary>
        /// 顶点
        /// </summary>
        private PrimitiveBufferView VerticesBuffer { get; set; }
        /// <summary>
        /// 顶点索引
        /// </summary>
        private PrimitiveBufferView TrianglesBuffer { get; set; }
        /// <summary>
        /// 法线
        /// </summary>
        private PrimitiveBufferView NormalsBuffer { get; set; }
        /// <summary>
        /// uv坐标（纹理通道0）
        /// </summary>
        private PrimitiveBufferView UvsBuffer { get; set; }
        /// <summary>
        /// uv坐标（纹理通道n）,处理uv0之外的所有uv
        /// </summary>
        private Dictionary<int, PrimitiveBufferView> UvNsBuffers { get; set; }
        /// <summary>
        /// 顶点颜色0（如果是rgb需要转换为线性色彩）
        /// </summary>
        private PrimitiveBufferView ColorsBuffer { get; set; }
        /// <summary>
        /// 轮廓线
        /// </summary>
        private PrimitiveBufferView LineIndicesBuffer { get; set; }
        /// <summary>
        /// batchId
        /// </summary>
        private PrimitiveBufferView BatchIdsBuffer { get; set; }
        /// <summary>
        /// 图元大小
        /// </summary>
        public override long PrimitiveSize
        {
            get
            {
                long size = 0;
                if (VerticesBuffer != null)
                {
                    size += VerticesBuffer.ByteLength;
                }
                if (TrianglesBuffer != null)
                {
                    size += TrianglesBuffer.ByteLength;
                }
                if (NormalsBuffer != null)
                {
                    size += NormalsBuffer.ByteLength;
                }
                if (UvsBuffer != null)
                {
                    size += UvsBuffer.ByteLength;
                }
                if (UvNsBuffers != null)
                {
                    UvNsBuffers.ForEach(uvsKv => size += uvsKv.Value.ByteLength);
                }
                if (ColorsBuffer != null)
                {
                    size += ColorsBuffer.ByteLength;
                }
                if (LineIndicesBuffer != null)
                {
                    size += LineIndicesBuffer.ByteLength;
                }
                if (BatchIdsBuffer != null)
                {
                    size += BatchIdsBuffer.ByteLength;
                }
                return size;

            }
        }

        BoundingBox _BoundingBox;
        /// <summary>
        /// 包围盒
        /// </summary>
        public override BoundingBox BoundingBox
        {
            get
            {
                if (_BoundingBox == null)
                {
                    var minX = VerticesBuffer.Min[0];
                    var minY = VerticesBuffer.Min[1];
                    var maxX = VerticesBuffer.Max[0];
                    var maxY = VerticesBuffer.Max[1];
                    _BoundingBox = new BoundingBox(minX, maxX, minY, maxY);
                    if (GLRenderMode == 4)
                    {
                        _BoundingBox.MinZ = VerticesBuffer.Min[2];
                        _BoundingBox.MaxZ = VerticesBuffer.Max[2];
                    }
                }
                return _BoundingBox;
            }
        }
        /// <summary>
        /// 通过GLTF的图元转换成当前图元
        /// </summary>
        /// <param name="p"></param>
        public void Init(GLTFPrimitive p)
        {
            if (p.Attributes.ContainsKey("POSITION"))
                VerticesBuffer = GetPrimitiveBufferView(p.Attributes["POSITION"]);
            if (p.Indices >= 0)
                TrianglesBuffer = GetPrimitiveBufferView(p.Indices);
            if (p.Attributes.ContainsKey("NORMAL"))
                NormalsBuffer = GetPrimitiveBufferView(p.Attributes["NORMAL"]);
            if (p.Attributes.ContainsKey("TEXCOORD_0"))
                UvsBuffer = GetPrimitiveBufferView(p.Attributes["TEXCOORD_0"]);
            if (p.Attributes.Keys.Any(k => k != "TEXCOORD_0" && k.StartsWith("TEXCOORD_")))
            {
                UvNsBuffers = new Dictionary<int, PrimitiveBufferView>();
                p.Attributes.Keys.Where(k => k != "TEXCOORD_0" && k.StartsWith("TEXCOORD_")).ForEach(k =>
                {
                    int key = int.Parse(k.Substring(9));
                    UvNsBuffers.Add(key, GetPrimitiveBufferView(p.Attributes[k]));
                });
            }
            if (p.Attributes.ContainsKey("COLOR_0"))
                ColorsBuffer = GetPrimitiveBufferView(p.Attributes["COLOR_0"]);
            if (p.Extensions != null && p.Extensions.ContainsKey("CESIUM_primitive_outline"))
                LineIndicesBuffer = GetPrimitiveBufferView(Convert.ToInt32(p.Extensions.GetValue("CESIUM_primitive_outline", "indices")));
            if (p.Attributes.ContainsKey("_BATCHID"))
                BatchIdsBuffer = GetPrimitiveBufferView(p.Attributes["_BATCHID"]);
            GLRenderMode = p.GLRenderMode;
            Extensions = p.Extensions;
            Extras = p.Extras;
            if (p.MaterialIndex != -1)
            {
                MaterialIndex = p.MaterialIndex;
                var material = p.GetMaterial(); ;
                Material = material;
                if (material.ImageIndex != -1
                || material.EmissiveTexture?.Index != null && material.EmissiveTexture?.Index >= 0 && GLTF.Textures[material.EmissiveTexture.Index].ImageIndex >= 0
                || material.OcclusionTexture?.Index != null && material.OcclusionTexture?.Index >= 0 && GLTF.Textures[material.OcclusionTexture.Index].ImageIndex >= 0
                || material.NormalTexture?.Index != null && material.NormalTexture?.Index >= 0 && GLTF.Textures[material.NormalTexture.Index].ImageIndex >= 0)
                {
                    Image = new GeometryImage();
                    if (material.ImageIndex != -1)
                    {
                        Image.PbrImage = material.Image;
                    }
                    if (material.EmissiveTexture?.Index != null && material.EmissiveTexture?.Index >= 0 && GLTF.Textures[material.EmissiveTexture.Index].ImageIndex >= 0)
                    {
                        Image.EmissiveImage = GLTF.Images[GLTF.Textures[material.EmissiveTexture.Index].ImageIndex];
                    }
                    if (material.OcclusionTexture?.Index != null && material.OcclusionTexture?.Index >= 0 && GLTF.Textures[material.OcclusionTexture.Index].ImageIndex >= 0)
                    {
                        Image.OcclusionImage = GLTF.Images[GLTF.Textures[material.OcclusionTexture.Index].ImageIndex];
                    }
                    if (material.NormalTexture?.Index != null && material.NormalTexture?.Index >= 0 && GLTF.Textures[material.NormalTexture.Index].ImageIndex >= 0)
                    {
                        Image.NormalImage = GLTF.Images[GLTF.Textures[material.NormalTexture.Index].ImageIndex];
                    }
                }
            }
        }

        PrimitiveBufferView GetPrimitiveBufferView(int AccIndex)
        {
            var Buffer = new PrimitiveBufferView();
            var acc = GLTF.Accessors[AccIndex];
            int bvIndex = acc.BufferViewIndex != null ? acc.BufferViewIndex.Value : 0;
            int accOffset = acc.ByteOffset == null && acc.BufferViewIndex == null ? ComputeByteOffsetWhenNull(AccIndex) : acc.ByteOffset == null ? 0 : acc.ByteOffset.Value;
            var bv = GLTF.BufferViews[bvIndex];
            Buffer.BufferIndex = bv.BufferIndex;
            Buffer.ByteOffset = bv.ByteOffset + accOffset;
            Buffer.Count = acc.Count;
            Buffer.ComponentType = (GLTFAccessorComponentType)acc.ComponentType;
            Buffer.AccPerLength = ComputeAccPerLength(acc.Type);
            Buffer.ComponentLength = ComputeComponentLength(Buffer.ComponentType);
            Buffer.AccessorType = (GLTFAccessorType)Enum.Parse(typeof(GLTFAccessorType), acc.Type);
            Buffer.Target = (GLTFTargets)bv.Target;
            Buffer.Max = acc.Max;
            Buffer.Min = acc.Min;
            return Buffer;
        }

        int ComputeAccPerLength(string accessorType)
        {
            switch (accessorType)
            {
                case "SCALAR":
                    return 1;
                case "VEC2":
                    return 2;
                case "VEC3":
                    return 3;
                case "VEC4":
                    return 4;
            }
            return 1;
        }

        int ComputeComponentLength(GLTFAccessorComponentType componentType)
        {
            switch (componentType)
            {
                case GLTFAccessorComponentType.BYTE:
                case GLTFAccessorComponentType.UNSIGNED_BYTE:
                    return 1;
                case GLTFAccessorComponentType.SHORT:
                case GLTFAccessorComponentType.UNSIGNED_SHORT:
                    return 2;
                case GLTFAccessorComponentType.UNSIGNED_INT:
                case GLTFAccessorComponentType.FLOAT:
                    return 4;
            }
            return 1;
        }

        int ComputeByteOffsetWhenNull(int accIndex)
        {
            int btyeOffset = 0;
            for (int i = 0; i < accIndex; i++)
            {
                var acc = GLTF.Accessors[i];
                int accItemSize = 1;
                if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_INT.GetHashCode() || acc.ComponentType == GLTFAccessorComponentType.FLOAT.GetHashCode())
                {
                    accItemSize = 4;
                }
                else if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_SHORT.GetHashCode() || acc.ComponentType == GLTFAccessorComponentType.SHORT.GetHashCode())
                {
                    accItemSize = 2;
                }
                else if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_BYTE.GetHashCode() || acc.ComponentType == GLTFAccessorComponentType.BYTE.GetHashCode())
                {
                    accItemSize = 1;
                }
                int accTime = 1;
                if (acc.Type == GLTFAccessorType.SCALAR.ToString())
                {
                    accTime = 1;
                }
                else if (acc.Type == GLTFAccessorType.VEC2.ToString())
                {
                    accTime = 2;
                }
                else if (acc.Type == GLTFAccessorType.VEC3.ToString())
                {
                    accTime = 3;
                }
                else if (acc.Type == GLTFAccessorType.VEC4.ToString())
                {
                    accTime = 4;
                }
                btyeOffset += accItemSize * accTime * acc.Count;
            }
            return btyeOffset;
        }
        #endregion

        #region 几何模型真实坐标获取
        private List<Vector3> _Vertices;
        /// <summary>
        /// 顶点
        /// </summary>
        public override List<Vector3> Vertices
        {
            get
            {
                if (_Vertices == null)
                {
                    _Vertices = new List<Vector3>();
                    VerticesLoop(v => _Vertices.Add(v));
                }
                return _Vertices;
            }
            set => _Vertices = value;
        }
        private List<Vector3Int> _Triangles;
        /// <summary>
        /// 顶点索引
        /// </summary>
        public override List<Vector3Int> Triangles
        {
            get
            {
                if (_Triangles == null)
                {
                    _Triangles = new List<Vector3Int>();
                    TrianglesLoop(p => _Triangles.Add(p));
                }
                return _Triangles;
            }
            set => _Triangles = value;
        }
        private List<Vector3> _Normals;
        /// <summary>
        /// 向量
        /// </summary>
        public override List<Vector3> Normals
        {
            get
            {
                if (_Normals == null)
                {
                    _Normals = new List<Vector3>();
                    NormalsLoop(n => _Normals.Add(n));
                }
                return _Normals;
            }
            set => _Normals = value;
        }
        private List<Vector2> _UVs;
        /// <summary>
        /// uv坐标（纹理通道0）
        /// </summary>
        public override List<Vector2> Uvs
        {
            get
            {
                if (_UVs == null)
                {
                    _UVs = new List<Vector2>();
                    UvsLoop(uv => _UVs.Add(uv));
                }
                return _UVs;
            }
            set => _UVs = value;
        }

        private Dictionary<int, List<Vector2>> _UvNs;
        /// <summary>
        /// uv坐标（纹理通道0）
        /// </summary>
        public override Dictionary<int, List<Vector2>> UvNs
        {
            get
            {
                if (_UvNs == null && UvNsBuffers != null)
                {
                    _UvNs = new Dictionary<int, List<Vector2>>();
                    UvNsBuffers.Keys.ForEach(key =>
                    {
                        List<Vector2> uvs = new List<Vector2>();
                        UvNsLoop(uv => uvs.Add(uv), key);
                        _UvNs.Add(key, uvs);
                    });
                }
                return _UvNs;
            }
            set => _UvNs = value;
        }

        private List<Vector4> _Colors;
        /// <summary>
        /// 顶点颜色0（如果是rgb需要转换为线性色彩）
        /// </summary>
        public override List<Vector4> Colors
        {
            get
            {
                if (_Colors == null)
                {
                    _Colors = new List<Vector4>();
                    ColorsLoop(color => _Colors.Add(color));
                }
                return _Colors;
            }
            set => _Colors = value;
        }

        /// <summary>
        /// 待优化调整代码
        /// </summary>
        /// <param name="num"></param>
        /// <param name="accItemSize"></param>
        /// <param name="hasAlpha"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
        private Vector4 GetRGBA(byte[] num, int accItemSize, bool hasAlpha, int componentType)
        {
            Vector4 p = Vector4.Zero;
            if (accItemSize == 1)
            {
                // 错误代码，待更新
                p.X = num[0];
                p.Y = num[1];
                p.Z = num[2];
                if (!hasAlpha)
                {
                    p.W = 0;
                }
                else
                {
                    p.W = num[3];
                }
            }
            else if (accItemSize == 2)
            {
                if (componentType == GLTFAccessorComponentType.UNSIGNED_SHORT.GetHashCode())
                {
                    p.X = BitConverter.ToUInt16(num, 0);
                    p.Y = BitConverter.ToUInt16(num, accItemSize);
                    p.Z = BitConverter.ToUInt16(num, accItemSize * 2);
                    if (!hasAlpha)
                    {
                        p.W = 0;
                    }
                    else
                    {
                        p.W = BitConverter.ToUInt16(num, accItemSize * 3);
                    }
                }
                else
                {
                    p.X = BitConverter.ToInt16(num, 0);
                    p.Y = BitConverter.ToInt16(num, accItemSize);
                    p.Z = BitConverter.ToInt16(num, accItemSize * 2);
                    if (!hasAlpha)
                    {
                        p.W = 0;
                    }
                    else
                    {
                        p.W = BitConverter.ToInt16(num, accItemSize * 3);
                    }
                }
            }
            else if (accItemSize == 4)
            {
                p.X = BitConverter.ToSingle(num, 0);
                p.Y = BitConverter.ToSingle(num, accItemSize);
                p.Z = BitConverter.ToSingle(num, accItemSize * 2);
                if (!hasAlpha)
                {
                    p.W = 0;
                }
                else
                {
                    p.W = BitConverter.ToSingle(num, accItemSize * 3);
                }
            }

            return p;
        }

        private List<Vector2Int> _LineIndices;
        /// <summary>
        /// 轮廓线
        /// </summary>
        public override List<Vector2Int> LineIndices
        {
            get
            {
                if (_LineIndices == null)
                {
                    _LineIndices = new List<Vector2Int>();
                    LineIndicesLoop(line => _LineIndices.Add(line));
                }
                return _LineIndices;
            }
            set => _LineIndices = value;
        }
        private List<uint> _BatchIds;
        /// <summary>
        /// batchId
        /// </summary>
        public override List<uint> BatchIds
        {
            get
            {
                if (_BatchIds == null)
                {
                    _BatchIds = new List<uint>();
                    BatchIdsLoop(batch => _BatchIds.Add(batch));
                }
                return _BatchIds;
            }
            set => _BatchIds = value;
        }
        #endregion

        #region 复制字节
        /// <summary>
        /// 写顶点
        /// </summary>
        /// <param name="destinatio"></param>
        public override void WriteVertices(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (VerticesBuffer != null)
            {
                WriteBuffer(gLTF, bufferIndex, VerticesBuffer);
                primitive.Attributes.Add("POSITION", gLTF.BufferViews.Count - 1);
            }
        }

        /// <summary>
        /// 写三角网
        /// </summary>
        /// <param name="destinatio"></param>
        public override void WriteTriangles(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (TrianglesBuffer != null)
            {
                WriteBuffer(gLTF, bufferIndex, TrianglesBuffer);
                primitive.Indices = gLTF.BufferViews.Count - 1;
            }
        }

        /// <summary>
        /// 写法线
        /// </summary>
        /// <param name="destinatio"></param>
        public override void WriteNormals(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (NormalsBuffer != null)
            {
                WriteBuffer(gLTF, bufferIndex, NormalsBuffer);
                primitive.Attributes.Add("NORMAL", gLTF.BufferViews.Count - 1);
            }
        }

        /// <summary>
        /// 写UV
        /// </summary>
        /// <param name="destinatio"></param>
        public override void WriteUvs(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (UvsBuffer != null)
            {
                WriteBuffer(gLTF, bufferIndex, UvsBuffer);
                primitive.Attributes.Add("TEXCOORD_0", gLTF.BufferViews.Count - 1);
            }
        }

        /// <summary>
        /// 写其他UV
        /// </summary>
        /// <param name="gLTF"></param>
        /// <param name="primitive"></param>
        /// <param name="bufferIndex"></param>
        public override void WriteUvNs(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (UvNsBuffers != null)
            {
                UvNsBuffers.ForEachWithIndex((uvbuffer, index) =>
                {
                    WriteBuffer(gLTF, bufferIndex, uvbuffer.Value);
                    primitive.Attributes.Add($"TEXCOORD_{uvbuffer.Key}", gLTF.BufferViews.Count - 1);
                });

            }
        }

        /// <summary>
        /// 写顶点颜色0（如果是rgb需要转换为线性色彩）
        /// </summary>
        /// <param name="destinatio"></param>
        public override void WriteColors(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (ColorsBuffer != null)
            {
                WriteBuffer(gLTF, bufferIndex, ColorsBuffer);
                primitive.Attributes.Add("COLOR_0", gLTF.BufferViews.Count - 1);
            }
        }

        /// <summary>
        /// 写轮廓线
        /// </summary>
        /// <param name="destinatio"></param>
        public override void WriteLineIndices(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (LineIndicesBuffer != null)
            {
                WriteBuffer(gLTF, bufferIndex, LineIndicesBuffer);
                primitive.Extensions = new Dictionary<string, JObject>();
                //轮廓线扩展字段
                primitive.Extensions.AddValue("CESIUM_primitive_outline", "indices", gLTF.BufferViews.Count - 1);

            }
        }

        /// <summary>
        /// 写batchId
        /// </summary>
        /// <param name="destinatio"></param>
        public override void WriteBatchIds(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (BatchIdsBuffer != null)
            {
                WriteBuffer(gLTF, bufferIndex, BatchIdsBuffer);
                primitive.Attributes.Add("_BATCHID", gLTF.BufferViews.Count - 1);
            }
        }

        /// <summary>
        /// 直接复制内存
        /// </summary>
        /// <param name="gLTF"></param>
        /// <param name="bufferIndex"></param>
        /// <param name="bufferView"></param>
        private void WriteBuffer(GLTF gLTF, int bufferIndex, PrimitiveBufferView bufferView)
        {
            var bufferStream = gLTF.Buffers[bufferIndex].DataStream;
            var nCurrentByteOffset = (int)bufferStream.Position;
            bufferStream.Write(GetBytesByBuffer(bufferView));
            gLTF.BufferViews.Add(BufferViewHelper.CreateBufferView(bufferView.Target, nCurrentByteOffset, bufferView.ByteLength, bufferView.ByteStride));
            gLTF.Accessors.Add(AccessorHelper.CreateAccessor(gLTF.BufferViews.Count - 1, bufferView.Count, bufferView.Max, bufferView.Min, bufferView.ComponentType, bufferView.AccessorType));
        }

        byte[] GetBytesByBuffer(PrimitiveBufferView bufferView)
        {
            byte[] bytes = new byte[bufferView.ByteLength];
            DataStreams[bufferView.BufferIndex].Position = bufferView.ByteOffset;
            DataStreams[bufferView.BufferIndex].Read(bytes, 0, bufferView.ByteLength);
            return bytes;
        }
        #endregion

        #region 遍历，通过流的方式
        public override void VerticesLoop(Action<Vector3> action)
        {
            if (VerticesBuffer != null)
            {
                DataStreams[VerticesBuffer.BufferIndex].Position = VerticesBuffer.ByteOffset;
                byte[] num = new byte[VerticesBuffer.ByteStride];
                for (int i = 0; i < VerticesBuffer.Count; i++)
                {
                    var p = new Vector3();
                    DataStreams[VerticesBuffer.BufferIndex].Read(num, 0, VerticesBuffer.ByteStride);
                    p.X = BitConverter.ToSingle(num, 0);
                    p.Y = BitConverter.ToSingle(num, 4);
                    p.Z = BitConverter.ToSingle(num, 8);
                    action(p);
                }
            }
        }

        public override void TrianglesLoop(Action<Vector3Int> action)
        {
            if (TrianglesBuffer != null)
            {
                DataStreams[TrianglesBuffer.BufferIndex].Position = TrianglesBuffer.ByteOffset;
                var indexCount = GLRenderMode == 4 ? 3 : 2;
                for (int i = 0; i < TrianglesBuffer.Count / indexCount; i++)
                {
                    var p = new Vector3Int();
                    byte[] num = new byte[TrianglesBuffer.ByteStride];
                    for (int j = 0; j < indexCount; j++)
                    {
                        DataStreams[TrianglesBuffer.BufferIndex].Read(num, 0, TrianglesBuffer.ComponentLength);
                        if (TrianglesBuffer.ComponentLength == 4)
                        {
                            p[j] = (int)BitConverter.ToUInt32(num, 0);
                        }
                        else
                        {
                            p[j] = BitConverter.ToUInt16(num, 0);
                        }
                    }
                    action(p);
                }
            }
        }

        public override void NormalsLoop(Action<Vector3> action)
        {
            if (NormalsBuffer != null)
            {
                DataStreams[NormalsBuffer.BufferIndex].Position = NormalsBuffer.ByteOffset;
                byte[] num = new byte[NormalsBuffer.ByteStride];
                for (int i = 0; i < NormalsBuffer.Count; i++)
                {
                    var p = new Vector3();
                    DataStreams[NormalsBuffer.BufferIndex].Read(num, 0, NormalsBuffer.ByteStride);
                    p.X = BitConverter.ToSingle(num, 0);
                    p.Y = BitConverter.ToSingle(num, 4);
                    p.Z = BitConverter.ToSingle(num, 8);
                    action(p);
                }
            }
        }

        public override void UvsLoop(Action<Vector2> action)
        {
            if (UvsBuffer != null)
            {
                DataStreams[UvsBuffer.BufferIndex].Position = UvsBuffer.ByteOffset;
                byte[] num = new byte[UvsBuffer.ByteStride];
                for (int i = 0; i < UvsBuffer.Count; i++)
                {
                    var p = new Vector2();
                    DataStreams[UvsBuffer.BufferIndex].Read(num, 0, UvsBuffer.ByteStride);
                    p.X = BitConverter.ToSingle(num, 0);
                    p.Y = BitConverter.ToSingle(num, 4);
                    action(p);
                }
            }
        }

        public override void UvNsLoop(Action<Vector2> action, int nIndex)
        {
            if (UvNsBuffers != null && UvNsBuffers.ContainsKey(nIndex))
            {
                var uvbuffer = UvNsBuffers[nIndex];
                DataStreams[uvbuffer.BufferIndex].Position = uvbuffer.ByteOffset;
                byte[] num = new byte[uvbuffer.ByteStride];
                for (int i = 0; i < uvbuffer.Count; i++)
                {
                    var p = new Vector2();
                    DataStreams[uvbuffer.BufferIndex].Read(num, 0, uvbuffer.ByteStride);
                    p.X = BitConverter.ToSingle(num, 0);
                    p.Y = BitConverter.ToSingle(num, 4);
                    action(p);
                }
            }
        }

        public override void ColorsLoop(Action<Vector4> action)
        {
            if (ColorsBuffer != null)
            {
                DataStreams[ColorsBuffer.BufferIndex].Position = ColorsBuffer.ByteOffset;
                byte[] num = new byte[ColorsBuffer.ByteStride];
                for (int i = 0; i < ColorsBuffer.Count; i++)
                {
                    DataStreams[ColorsBuffer.BufferIndex].Read(num, 0, ColorsBuffer.ByteStride);
                    var p = GetRGBA(num, ColorsBuffer.ComponentLength, false, ColorsBuffer.ComponentType.GetHashCode());
                    action(p);
                }
            }
        }

        public override void LineIndicesLoop(Action<Vector2Int> action)
        {
            if (LineIndicesBuffer != null)
            {
                DataStreams[LineIndicesBuffer.BufferIndex].Position = LineIndicesBuffer.ByteOffset;
                for (int i = 0; i < LineIndicesBuffer.Count / 2; i++)
                {
                    var p = new Vector2Int();
                    byte[] num = new byte[LineIndicesBuffer.ByteStride];
                    for (int j = 0; j < 2; j++)
                    {
                        DataStreams[LineIndicesBuffer.BufferIndex].Read(num, 0, LineIndicesBuffer.ComponentLength);
                        if (LineIndicesBuffer.ComponentLength == 4)
                        {
                            p[j] = (int)BitConverter.ToUInt32(num, 0);
                        }
                        else
                        {
                            p[j] = BitConverter.ToUInt16(num, 0);
                        }
                    }
                    action(p);
                }
            }
        }

        public override void BatchIdsLoop(Action<uint> action)
        {
            if (BatchIdsBuffer != null)
            {
                DataStreams[BatchIdsBuffer.BufferIndex].Position = BatchIdsBuffer.ByteOffset;
                byte[] num = new byte[BatchIdsBuffer.ByteStride];
                for (int i = 0; i < BatchIdsBuffer.Count; i++)
                {
                    DataStreams[BatchIdsBuffer.BufferIndex].Read(num, 0, BatchIdsBuffer.ByteStride);
                    if (BatchIdsBuffer.ComponentType == GLTFAccessorComponentType.UNSIGNED_INT)
                    {
                        action(BitConverter.ToUInt32(num, 0));
                    }
                    else
                    {
                        action(BitConverter.ToUInt16(num, 0));
                    }
                }
            }
        }
        #endregion
    }
}
