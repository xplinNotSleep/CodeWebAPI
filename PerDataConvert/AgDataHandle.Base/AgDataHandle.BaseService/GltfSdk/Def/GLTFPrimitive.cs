using AgDataHandle.Maths;
using AgDataHandle.BaseService.CommonHelper;
using AgDataHandle.BaseService.GltfSdk.ENUM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    /*
     * 反序列化/序列化测试通过 2020年8月7日
     */

    public class GLTFPrimitive : GLTFElement
    {
        public GLTFPrimitive()
        {
            GLRenderMode = 4;
            MaterialIndex = -1;
            Indices = -1;
            Attributes = new Dictionary<string, int>();
        }

        #region Properties

        [JsonProperty(
            "attributes",
            Required = Required.Always)]
        public Dictionary<string, int> Attributes { get; set; }

        [JsonProperty(
            "indices",
            Required = Required.DisallowNull,
            NullValueHandling = NullValueHandling.Ignore)]
        public int Indices { get; set; }

        [JsonProperty(
            "material",
            Required = Required.DisallowNull,
            NullValueHandling = NullValueHandling.Ignore)]
        public int MaterialIndex { get; set; }

        [JsonProperty(
            "mode",
            Required = Required.DisallowNull,
            NullValueHandling = NullValueHandling.Ignore)]
        public int GLRenderMode { get; set; }

        [JsonIgnore]
        public int VertexCount
        {
            get
            {
                var acc = GLTF.Accessors[Attributes["POSITION"]];
                return acc.Count;
            }
        }

        [JsonIgnore]
        public int VerticelCount
        {
            get
            {
                var acc = GLTF.Accessors[Indices];
                return acc.Count;
            }
        }

        [JsonProperty(
            "extensions",
            Required = Required.AllowNull,
            NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, JObject> Extensions { get; set; }

        [JsonProperty(
            "extras",
            Required = Required.DisallowNull,
            NullValueHandling = NullValueHandling.Ignore)]
        public JObject Extras { get; set; }

        #endregion

        #region Methods
        public int ComputeByteOffsetWhenNull(int accIndex)
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
        /// <summary>
        /// 获取图元顶点
        /// </summary>
        /// <param name="ModelUnitChange">模型单位变换</param>
        /// <param name="matrix4X4">变换矩阵，默认为空</param>
        /// <returns></returns>
        public List<Vector3> GetPositions(float ModelUnitChange = 1, IMatrix4x4 matrix4X4 = null)
        {

            List<Vector3> ls = new List<Vector3>();
            int accIndex = Attributes["POSITION"];

            var acc = GLTF.Accessors[accIndex];
            int bvIndex = acc.BufferViewIndex != null ? acc.BufferViewIndex.Value : 0;
            var bv = GLTF.BufferViews[bvIndex];
            var bf = GLTF.Buffers[bv.BufferIndex];
            lock (bf)
            {
                // 判断是否是数据交错型
                #region 数据交错判断

                int accOffset = acc.ByteOffset == null && acc.BufferViewIndex == null ? ComputeByteOffsetWhenNull(accIndex) : acc.ByteOffset == null ? 0 : acc.ByteOffset.Value;
                int byteStride = bv.ByteStride > 0 ? bv.ByteStride : 0;
                int accItemSize = byteStride == 0 ? 12 : byteStride;

                #endregion

                int count = acc.Count;

                if (bf.BufferType != GLTFBufferType.BinaryData)
                {
                    if (!bf.DataStream.CanRead)
                        return null;
                    bf.ResetPosition();
                    bf.DataStream.Seek(bv.ByteOffset + accOffset, SeekOrigin.Current);

                    byte[] num = new byte[accItemSize];
                    for (int i = 0; i < count; i++)
                    {
                        var p = new Vector3(0, 0, 0);
                        bf.DataStream.Read(num, 0, accItemSize);
                        p.X = BitConverter.ToSingle(num, 0) * ModelUnitChange;
                        p.Y = BitConverter.ToSingle(num, 4) * ModelUnitChange;
                        p.Z = BitConverter.ToSingle(num, 8) * ModelUnitChange;
                        ls.Add(p.MultiplyByMatrix<Vector3>(matrix4X4));
                    }
                    bf.ResetPosition();
                }
                else
                {
                    var offset = bv.ByteOffset + accOffset;
                    byte[] allnum = bf.BinaryData.Skip(offset).Take(accItemSize * count).ToArray();
                    offset = 0;
                    for (int i = 0; i < count; i++)
                    {
                        byte[] num = allnum.Skip(offset).Take(accItemSize).ToArray();
                        var p = new Vector3(0, 0, 0);
                        p.X = BitConverter.ToSingle(num, 0);
                        p.Y = BitConverter.ToSingle(num, 4);
                        p.Z = BitConverter.ToSingle(num, 8);
                        ls.Add(p.MultiplyByMatrix<Vector3>(matrix4X4));
                        offset += accItemSize;
                    }
                }
                return ls;
            }

        }

        // 可以用反序列化优化读取
        public List<Vector3> GetNormals()
        {
            List<Vector3> ls = new List<Vector3>();

            if (!Attributes.ContainsKey("NORMAL"))
                return null;

            int accIndex = Attributes["NORMAL"];
            var acc = GLTF.Accessors[accIndex];
            int bvIndex = acc.BufferViewIndex != null ? acc.BufferViewIndex.Value : 0;
            var bv = GLTF.BufferViews[bvIndex];
            var bf = GLTF.Buffers[bv.BufferIndex];
            lock (bf)
            {
                #region 数据交错判断

                int accOffset = acc.ByteOffset == null && acc.BufferViewIndex == null ? ComputeByteOffsetWhenNull(accIndex) : acc.ByteOffset == null ? 0 : acc.ByteOffset.Value;
                int byteStride = bv.ByteStride > 0 ? bv.ByteStride : 0;
                int accItemSize = byteStride == 0 ? 12 : byteStride;

                #endregion

                int count = acc.Count;
                if (bf.BufferType != GLTFBufferType.BinaryData)
                {
                    if (!bf.DataStream.CanRead)
                        return null;
                    bf.ResetPosition();
                    bf.DataStream.Position = bv.ByteOffset;
                    bf.DataStream.Seek(accOffset, SeekOrigin.Current);

                    byte[] num = new byte[accItemSize];
                    for (int i = 0; i < count; i++)
                    {
                        var p = new Vector3(0, 0, 0);
                        bf.DataStream.Read(num, 0, accItemSize);
                        p.X = BitConverter.ToSingle(num, 0);
                        p.Y = BitConverter.ToSingle(num, 4);
                        p.Z = BitConverter.ToSingle(num, 8);
                        ls.Add(p);
                    }
                    bf.ResetPosition();
                }
                else
                {
                    var offset = bv.ByteOffset + accOffset;
                    byte[] allnum = bf.BinaryData.Skip(offset).Take(accItemSize * count).ToArray();
                    offset = 0;
                    for (int i = 0; i < count; i++)
                    {
                        byte[] num = allnum.Skip(offset).Take(accItemSize).ToArray();
                        var p = new Vector3(0, 0, 0);
                        p.X = BitConverter.ToSingle(num, 0);
                        p.Y = BitConverter.ToSingle(num, 4);
                        p.Z = BitConverter.ToSingle(num, 8);
                        ls.Add(p);
                        offset += accItemSize;
                    }
                }
                return ls;
            }

        }

        public List<Vector2> GetUV0(string texcoord = "TEXCOORD_0")
        {
            if (!Attributes.ContainsKey(texcoord))
                return null;
            List<Vector2> ls = new List<Vector2>();
            var accIndex = Attributes[texcoord];
            var acc = GLTF.Accessors[accIndex];
            int bvIndex = acc.BufferViewIndex != null ? acc.BufferViewIndex.Value : 0;
            var bv = GLTF.BufferViews[bvIndex];
            var count = acc.Count;
            var bf = GLTF.Buffers[bv.BufferIndex];

            lock (bf)
            {
                #region 数据交错判断

                int accOffset = acc.ByteOffset == null && acc.BufferViewIndex == null ? ComputeByteOffsetWhenNull(accIndex) : acc.ByteOffset == null ? 0 : acc.ByteOffset.Value;
                int byteStride = bv.ByteStride > 0 ? bv.ByteStride : 0;
                int accItemSize = byteStride == 0 ? 8 : byteStride;

                #endregion
                if (bf.BufferType != GLTFBufferType.BinaryData)
                {
                    if (!bf.DataStream.CanRead)
                        return null;
                    bf.ResetPosition();
                    bf.DataStream.Position = bv.ByteOffset;
                    bf.DataStream.Seek(accOffset, SeekOrigin.Current);

                    byte[] num = new byte[accItemSize];
                    for (int i = 0; i < count; i++)
                    {
                        var p = new Vector2(0, 0);
                        bf.DataStream.Read(num, 0, accItemSize);
                        p.X = BitConverter.ToSingle(num, 0);
                        p.Y = BitConverter.ToSingle(num, 4);
                        ls.Add(p);
                    }
                    bf.ResetPosition();
                }
                else
                {
                    var offset = bv.ByteOffset + accOffset;
                    byte[] allnum = bf.BinaryData.Skip(offset).Take(accItemSize * count).ToArray();
                    offset = 0;
                    for (int i = 0; i < count; i++)
                    {
                        byte[] num = allnum.Skip(offset).Take(accItemSize).ToArray();
                        var p = new Vector2(0, 0);
                        p.X = BitConverter.ToSingle(num, 0);
                        p.Y = BitConverter.ToSingle(num, 4);
                        ls.Add(p);
                        offset += accItemSize;
                    }
                }
                return ls;
            }
        }

        public List<Vector4> GetCOLOR0(string color = "COLOR_0")
        {
            if (!Attributes.ContainsKey(color))
                return null;
            List<Vector4> ls = new List<Vector4>();
            int accIndex = Attributes[color];

            var acc = GLTF.Accessors[accIndex];
            int bvIndex = acc.BufferViewIndex != null ? acc.BufferViewIndex.Value : 0;
            var bv = GLTF.BufferViews[bvIndex];
            var bf = GLTF.Buffers[bv.BufferIndex];

            int accItemSize = 0;
            if (acc.ComponentType == GLTFAccessorComponentType.FLOAT.GetHashCode())
            {
                accItemSize = 4;
            }
            else if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_SHORT.GetHashCode())
            {
                accItemSize = 2;
            }
            else if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_BYTE.GetHashCode())
            {
                accItemSize = 1;
            }

            if (acc.Type == "Vec3")
            {
                lock (bf)
                {
                    // 判断是否是数据交错型
                    #region 数据交错判断

                    int accOffset = acc.ByteOffset == null && acc.BufferViewIndex == null ? ComputeByteOffsetWhenNull(accIndex) : acc.ByteOffset == null ? 0 : acc.ByteOffset.Value;
                    int byteStride = bv.ByteStride > 0 ? bv.ByteStride : 0;
                    int vectorSize = accItemSize * 3;

                    #endregion

                    int count = acc.Count;

                    if (bf.BufferType != GLTFBufferType.BinaryData)
                    {
                        if (!bf.DataStream.CanRead)
                            return null;
                        bf.ResetPosition();
                        bf.DataStream.Seek(bv.ByteOffset + accOffset, SeekOrigin.Current);

                        byte[] num = new byte[vectorSize];
                        for (int i = 0; i < count; i++)
                        {
                            bf.DataStream.Read(num, 0, vectorSize);
                            ls.Add(GetRGBA(num, accItemSize, false, acc.ComponentType));
                        }
                        bf.ResetPosition();
                    }
                    else
                    {
                        var offset = bv.ByteOffset + accOffset;
                        byte[] allnum = bf.BinaryData.Skip(offset).Take(vectorSize * count).ToArray();
                        offset = 0;
                        for (int i = 0; i < count; i++)
                        {
                            byte[] num = allnum.Skip(offset).Take(vectorSize).ToArray();
                            ls.Add(GetRGBA(num, accItemSize, false, acc.ComponentType));
                            offset += vectorSize;
                        }
                    }
                    return ls;
                }
            }
            else
            {
                lock (bf)
                {
                    // 判断是否是数据交错型
                    #region 数据交错判断

                    int accOffset = acc.ByteOffset == null && acc.BufferViewIndex == null ? ComputeByteOffsetWhenNull(accIndex) : acc.ByteOffset == null ? 0 : acc.ByteOffset.Value;
                    int byteStride = bv.ByteStride > 0 ? bv.ByteStride : 0;
                    int vectorSize = accItemSize * 4;

                    #endregion

                    int count = acc.Count;

                    if (bf.BufferType != GLTFBufferType.BinaryData)
                    {
                        if (!bf.DataStream.CanRead)
                            return null;

                        bf.ResetPosition();
                        bf.DataStream.Seek(bv.ByteOffset + accOffset, SeekOrigin.Current);

                        byte[] num = new byte[vectorSize];
                        for (int i = 0; i < count; i++)
                        {
                            bf.DataStream.Read(num, 0, vectorSize);
                            ls.Add(GetRGBA(num, accItemSize, true, acc.ComponentType));
                        }
                        bf.ResetPosition();
                    }
                    else
                    {
                        var offset = bv.ByteOffset + accOffset;
                        byte[] allnum = bf.BinaryData.Skip(offset).Take(vectorSize * count).ToArray();
                        offset = 0;
                        for (int i = 0; i < count; i++)
                        {
                            byte[] num = allnum.Skip(offset).Take(vectorSize).ToArray();
                            ls.Add(GetRGBA(num, accItemSize, true, acc.ComponentType));
                            offset += vectorSize;
                        }
                    }
                    return ls;
                }
            }
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

        public List<int> GetIndices()
        {
            if (Indices < 0)
                return null;
            List<int> inds = new List<int>();
            var acc = GLTF.Accessors[Indices];
            int bvIndex = acc.BufferViewIndex != null ? acc.BufferViewIndex.Value : 0;
            var bv = GLTF.BufferViews[bvIndex];
            var bf = GLTF.Buffers[bv.BufferIndex];

            int accOffset = acc.ByteOffset == null && acc.BufferViewIndex == null ? ComputeByteOffsetWhenNull(Indices) : acc.ByteOffset == null ? 0 : acc.ByteOffset.Value;
            int count = acc.Count;
            int accItemSize = 0;
            if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_INT.GetHashCode())
            {
                accItemSize = 4;
            }
            else if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_SHORT.GetHashCode())
            {
                accItemSize = 2;
            }
            else if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_BYTE.GetHashCode())
            {
                accItemSize = 1;
            }

            lock (bf)
            {
                if (bf.BufferType != GLTFBufferType.BinaryData)
                {
                    if (!bf.DataStream.CanRead)
                        return null;
                    bf.ResetPosition();
                    bf.DataStream.Seek(bv.ByteOffset + accOffset, SeekOrigin.Current);

                    for (int i = 0; i < count; i++)
                    {
                        int id = 0;
                        if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_INT.GetHashCode())
                        {
                            byte[] num = new byte[accItemSize];
                            bf.DataStream.Read(num, 0, accItemSize);
                            id = (int)BitConverter.ToUInt32(num, 0);
                        }
                        else if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_SHORT.GetHashCode())
                        {
                            byte[] num = new byte[accItemSize];
                            bf.DataStream.Read(num, 0, accItemSize);
                            id = BitConverter.ToUInt16(num, 0);
                        }
                        else if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_BYTE.GetHashCode())
                        {
                            byte[] num = new byte[accItemSize];
                            bf.DataStream.Read(num, 0, accItemSize);
                            id = num[0];
                        }
                        inds.Add(id);
                    }
                    bf.ResetPosition();
                }
                else
                {
                    var offset = bv.ByteOffset + accOffset;
                    byte[] allnum = bf.BinaryData.Skip(offset).Take(accItemSize * count).ToArray();
                    offset = 0;
                    for (int i = 0; i < count; i++)
                    {
                        int id = 0;
                        if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_INT.GetHashCode())
                        {
                            byte[] num = allnum.Skip(offset).Take(accItemSize).ToArray();
                            id = (int)BitConverter.ToUInt32(num, 0);
                        }
                        else if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_SHORT.GetHashCode())
                        {
                            byte[] num = allnum.Skip(offset).Take(accItemSize).ToArray();
                            id = BitConverter.ToUInt16(num, 0);
                        }
                        else if (acc.ComponentType == GLTFAccessorComponentType.UNSIGNED_BYTE.GetHashCode())
                        {
                            byte[] num = allnum.Skip(offset).Take(accItemSize).ToArray();
                            id = num[0];
                        }
                        offset += accItemSize;
                        inds.Add(id);
                    }
                }
                return inds;
            }
        }

        public List<Vector2Int> GetLineIndex()
        {
            if (Extensions == null || !Extensions.ContainsKey("CESIUM_primitive_outline"))
                return null;
            List<Vector2Int> ls = new List<Vector2Int>();
            var accIndex = Convert.ToInt32(Extensions.GetValue("CESIUM_primitive_outline", "indices"));
            GLTFAccessor liAccessor = GLTF.Accessors[Convert.ToInt32(Extensions.GetValue("CESIUM_primitive_outline", "indices"))];
            int bvIndex = liAccessor.BufferViewIndex != null ? liAccessor.BufferViewIndex.Value : 0;
            GLTFBufferView bv = GLTF.BufferViews[bvIndex];
            //LineIndex在Accessor中类型为标量"SCALAR"，所以读取到Vector2Int时需要除以2
            int count = liAccessor.Count / 2;
            int accItemSize = liAccessor.ComponentType == GLTFAccessorComponentType.UNSIGNED_SHORT.GetHashCode() ? 2 : 4;
            GLTFBuffer bf = GLTF.Buffers[bv.BufferIndex];

            lock (bf)
            {
                int accOffset = liAccessor.ByteOffset == null && liAccessor.BufferViewIndex == null ? ComputeByteOffsetWhenNull(accIndex) : liAccessor.ByteOffset == null ? 0 : liAccessor.ByteOffset.Value;

                if (bf.BufferType != GLTFBufferType.BinaryData)
                {
                    if (!bf.DataStream.CanRead)
                        return null;

                    bf.ResetPosition();
                    bf.DataStream.Seek(bv.ByteOffset + accOffset, SeekOrigin.Current);

                    byte[] num = new byte[accItemSize * 2];
                    for (int i = 0; i < count; i++)
                    {
                        Vector2Int p = new Vector2Int(0, 0);
                        bf.DataStream.Read(num, 0, accItemSize * 2);
                        if (liAccessor.ComponentType == GLTFAccessorComponentType.UNSIGNED_SHORT.GetHashCode())
                        {
                            p.X = BitConverter.ToUInt16(num, 0);
                            p.Y = BitConverter.ToUInt16(num, accItemSize);
                        }
                        else
                        {
                            p.X = BitConverter.ToInt32(num, 0);
                            p.Y = BitConverter.ToInt32(num, accItemSize);
                        }
                        ls.Add(p);
                    }
                    bf.ResetPosition();
                }
                else
                {
                    int offset = bv.ByteOffset + accOffset;
                    byte[] allnum = bf.BinaryData.Skip(offset).Take(accItemSize * 2 * count).ToArray();
                    offset = 0;
                    for (int i = 0; i < count; i++)
                    {
                        byte[] num = allnum.Skip(offset).Take(accItemSize).ToArray();
                        Vector2Int p = new Vector2Int(0, 0);
                        if (liAccessor.ComponentType == GLTFAccessorComponentType.UNSIGNED_SHORT.GetHashCode())
                        {
                            p.X = BitConverter.ToUInt16(num, 0);
                            p.Y = BitConverter.ToUInt16(num, offset);
                        }
                        else
                        {
                            p.X = BitConverter.ToInt32(num, 0);
                            p.Y = BitConverter.ToInt32(num, offset);
                        }
                        ls.Add(p);
                        offset += accItemSize;
                    }
                }
                return ls;
            }
        }

        public GLTFImage GetImage()
        {
            if (MaterialIndex < 0)
                return null;
            var material = GLTF.Materials[MaterialIndex];
            if (material.PbrMetallicRoughness == null)
                return null;
            if (material.PbrMetallicRoughness.BaseColorTexture == null)
                return null;
            var baseTexIndex = material.PbrMetallicRoughness.BaseColorTexture.Index;
            var baseTex = GLTF.Textures[baseTexIndex];
            if (baseTex == null)
                return null;
            var img = GLTF.Images[baseTex.ImageIndex];
            return img;
        }

        public byte[] GetImageData()
        {
            var img = GetImage();
            if (img != null)
                return img.GetImageData();
            else
                return null;
        }

        public MemoryStream GetImageDataStream()
        {
            var img = GetImage();
            var bv = img.GetBufferView();
            var data = new MemoryStream();
            int bt;
            int flag = 0;
            img.ResetPosition();
            while ((bt = img.DataStream.ReadByte()) != -1 && flag < bv.ByteLength)
            {
                data.WriteByte((byte)bt);
                flag++;
            }
            img.ResetPosition();
            return data;
        }

        public List<float> GetVetexAttributes(string attributeName)
        {
            if (!Attributes.ContainsKey(attributeName))
                return null;

            int accIndex = Attributes[attributeName];
            var acc = GLTF.Accessors[accIndex];
            int bvIndex = acc.BufferViewIndex != null ? acc.BufferViewIndex.Value : 0;
            var bv = GLTF.BufferViews[bvIndex];
            var bf = GLTF.Buffers[bv.BufferIndex];

            int count = acc.Count;
            List<float> ls = new List<float>(count);

            if (!bf.DataStream.CanRead)
                throw new Exception("数据不可读，请检查文件");
            if (acc.ByteOffset != null || bvIndex == 0)
            {
                bf.ResetPosition();
                bf.DataStream.Position += bv.ByteOffset;
            }

            for (int i = 0; i < count; i++)
            {
                byte[] num = new byte[4];
                var v = BitConverter.ToSingle(num, 0);
                ls.Add(v);
            }

            if (acc.ByteOffset != null)
            {
                bf.ResetPosition();
            }

            return ls;
        }

        public GLTFMaterial GetMaterial()
        {
            if (MaterialIndex != -1)
                return GLTF.Materials[MaterialIndex];
            else
                return null;
        }

        public GLTFPrimitive Clone()
        {
            GLTFPrimitive pri = new GLTFPrimitive() { GLTF = null };

            pri.Indices = Indices;
            pri.MaterialIndex = MaterialIndex;
            pri.GLRenderMode = GLRenderMode;
            pri.Attributes = ArrayHelper.CloneDict(Attributes);

            return pri;
        }

        internal static GLTFPrimitive Deserialize(JsonReader reader, GLTF model)
        {
            if (reader.TokenType == JsonToken.StartArray)
                reader.Read();
            GLTFPrimitive pri = new GLTFPrimitive();

            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                var curProp = reader.Value as string;
                switch (curProp)
                {
                    case "attributes":
                        reader.Read(); // token -> startObject
                        while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                        {
                            string attrName = reader.Value.ToString();
                            int attrIndex = (int)reader.ReadAsInt32();
                            pri.Attributes.Add(attrName, attrIndex);
                        }
                        break;
                    case "indices":
                        pri.Indices = (int)reader.ReadAsInt32();
                        break;
                    case "material":
                        pri.MaterialIndex = (int)reader.ReadAsInt32();
                        break;
                    case "mode":
                        pri.GLRenderMode = (int)reader.ReadAsInt32();
                        break;
                    case "extensions":
                        if (GLTF.IgnoreExtAndExtras)
                        {
                            reader.Skip();
                            break;
                        }
                        var jpExts = JToken.ReadFrom(reader) as JProperty;
                        pri.Extensions = pri.Extensions ?? new Dictionary<string, JObject>();
                        pri.LoadExtension(jpExts, pri.Extensions);
                        break;
                    case "extras":
                        if (GLTF.IgnoreExtAndExtras)
                        {
                            reader.Skip();
                            break;
                        }
                        var jpExtras = JToken.ReadFrom(reader) as JObject;
                        pri.Extras = jpExtras;
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            if (reader.TokenType == JsonToken.EndObject)
                reader.Read();


            pri.GLTF = model;
            return pri;
        }

        #endregion
    }
}
