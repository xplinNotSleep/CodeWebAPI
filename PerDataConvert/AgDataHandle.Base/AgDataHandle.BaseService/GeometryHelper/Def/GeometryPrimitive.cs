using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.Maths;
using AgDataHandle.Maths.Numerics;
using AgDataHandle.BaseService.BufferView;
using AgDataHandle.BaseService.GeometryHelper.Param;
using AgDataHandle.BaseService.GltfSdk.Collection;
using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.GltfSdk.ENUM;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GeometryHelper.Def
{
    public enum SplitPrimitiveType
    {
        Common,
        Stream,
        Bytes
    }

    public class GeometryImage
    {
        public GLTFImage PbrImage { get; set; }
        public GLTFImage EmissiveImage { get; set; }
        public GLTFImage OcclusionImage { get; set; }
        public GLTFImage NormalImage { get; set; }
        public override int GetHashCode()
        {
            return PbrImage?.GetHashCode() ?? 0 + EmissiveImage?.GetHashCode() ?? 0 + OcclusionImage?.GetHashCode() ?? 0 + NormalImage?.GetHashCode() ?? 0;
        }
    }

    /// <summary>
    /// 图元几何信息
    /// </summary>
    public class GeometryPrimitive
    {
        public GeometryPrimitive()
        {
            PrimitiveType = SplitPrimitiveType.Common;
        }
        protected SplitPrimitiveType PrimitiveType { get; set; }
        #region 几何模型
        /// <summary>
        /// 顶点
        /// </summary>
        public virtual List<Vector3> Vertices { get; set; }
        /// <summary>
        /// 顶点索引
        /// </summary>
        public virtual List<Vector3Int> Triangles { get; set; }
        /// <summary>
        /// 向量
        /// </summary>
        public virtual List<Vector3> Normals { get; set; }
        /// <summary>
        /// uv坐标（纹理通道0）
        /// </summary>
        public virtual List<Vector2> Uvs { get; set; }
        /// <summary>
        /// uv坐标（纹理通道n）,处理uv0之外的所有uv
        /// </summary>
        public virtual Dictionary<int, List<Vector2>> UvNs { get; set; }
        /// <summary>
        /// 顶点颜色0（如果是rgb需要转换为线性色彩）
        /// </summary>
        public virtual List<Vector4> Colors { get; set; }
        /// <summary>
        /// 轮廓线
        /// </summary>
        public virtual List<Vector2Int> LineIndices { get; set; }
        /// <summary>
        /// batchId
        /// </summary>
        public virtual List<uint> BatchIds { get; set; }
        /// <summary>
        /// 图元大小
        /// </summary>
        public virtual long PrimitiveSize
        {
            get
            {
                long size = 0;
                if (Vertices != null)
                {
                    size += Vertices.Count * 12;
                }
                if (Triangles != null)
                {
                    size += Triangles.Count * 12;
                }
                if (Normals != null)
                {
                    size += Normals.Count * 12;
                }
                if (Uvs != null)
                {
                    size += Uvs.Count * 8;
                }
                if (UvNs != null)
                {
                    UvNs.ForEach(uvsKv => size += uvsKv.Value.Count * 8);
                }
                if (Colors != null)
                {
                    size += Colors.Count * 16;
                }
                if (LineIndices != null)
                {
                    size += LineIndices.Count * 8;
                }
                if (BatchIds != null)
                {
                    size += BatchIds.Count * 2;
                }
                return size;

            }
        }

        BoundingBox _BoundingBox;
        /// <summary>
        /// 包围盒
        /// </summary>
        public virtual BoundingBox BoundingBox
        {
            get
            {
                if (_BoundingBox == null)
                {
                    var minX = Vertices.Min(p2 => p2.X);
                    var minY = Vertices.Min(p2 => p2.Y);
                    var maxX = Vertices.Max(p2 => p2.X);
                    var maxY = Vertices.Max(p2 => p2.Y);
                    _BoundingBox = new BoundingBox(minX, maxX, minY, maxY);
                    if (GLRenderMode == 4)
                    {
                        _BoundingBox.MinZ = Vertices.Min(p2 => p2.Z);
                        _BoundingBox.MaxZ = Vertices.Max(p2 => p2.Z);
                    }
                }
                return _BoundingBox;
            }
        }
        #endregion
        #region 属性相关
        /// <summary>
        /// 渲染模式
        /// </summary>
        public int GLRenderMode { get; set; } = 4;
        /// <summary>
        /// 属性表
        /// </summary>
        public Dictionary<string, JObject> Extensions { get; set; }
        /// <summary>
        /// 额外属性
        /// </summary>
        public JObject Extras { get; set; }
        #endregion
        #region 材质纹理
        /// <summary>
        /// 材质id
        /// </summary>
        public int MaterialIndex { get; set; } = -1;
        /// <summary>
        /// 材质指针
        /// </summary>
        public GLTFMaterial Material { get; set; }
        /// <summary>
        /// 图片集合
        /// </summary>
        public GeometryImage Image { get; set; }
        #endregion

        #region 复制字节
        public void WriteVerticesStream(Stream bufferStream)
        {
            if (GLRenderMode == 4)
            {
                foreach (var coord in Vertices)
                {
                    bufferStream.Write(BitConverter.GetBytes(coord.X));
                    bufferStream.Write(BitConverter.GetBytes(coord.Y));
                    bufferStream.Write(BitConverter.GetBytes(coord.Z));
                }
            }
            else
            {
                foreach (var coord in Vertices)
                {
                    bufferStream.Write(BitConverter.GetBytes(coord.X));
                    bufferStream.Write(BitConverter.GetBytes(coord.Y));
                }
            }
        }

        public virtual void WriteVertices(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (Vertices == null) return;
            var bufferStream = gLTF.Buffers[bufferIndex].DataStream;
            var nCurrentByteOffset = (int)bufferStream.Position;
            WriteVerticesStream(bufferStream);
            var minX = Vertices.Min(p2 => p2.X);
            var minY = Vertices.Min(p2 => p2.Y);
            var maxX = Vertices.Max(p2 => p2.X);
            var maxY = Vertices.Max(p2 => p2.Y);
            var max = new List<float> { maxX, maxY };
            var min = new List<float> { minX, minY };
            if (GLRenderMode == 4)
            {
                var minZ = Vertices.Min(p2 => p2.Z);
                var maxZ = Vertices.Max(p2 => p2.Z);
                max.Add(maxZ);
                min.Add(minZ);
            }
            var totalLength = Vertices.Count * 4 * (GLRenderMode == 4 ? 3 : 2);
            gLTF.BufferViews.Add(BufferViewHelper.CreateBufferView(BufferViewUsage.Vertical, nCurrentByteOffset, totalLength, 12));
            var bufferViewIndex = gLTF.BufferViews.Count - 1;
            gLTF.Accessors.Add(AccessorHelper.CreateAccessor(bufferViewIndex, Vertices.Count, max, min, BufferViewUsage.Vertical));
            primitive.Attributes.Add("POSITION", bufferViewIndex);
        }

        public void WriteTrianglesStream(Stream bufferStream, bool isUseInt)
        {
            foreach (var index in Triangles)
            {
                if (isUseInt)
                {
                    bufferStream.Write(BitConverter.GetBytes(index.X));
                    bufferStream.Write(BitConverter.GetBytes(index.Y));
                    if (GLRenderMode == 4)
                    {
                        bufferStream.Write(BitConverter.GetBytes(index.Z));
                    }
                }
                else
                {
                    bufferStream.Write(BitConverter.GetBytes((ushort)index.X));
                    bufferStream.Write(BitConverter.GetBytes((ushort)index.Y));
                    if (GLRenderMode == 4)
                    {
                        bufferStream.Write(BitConverter.GetBytes((ushort)index.Z));
                    }
                }
            }
        }

        public virtual void WriteTriangles(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (Triangles == null) return;
            var bufferStream = gLTF.Buffers[bufferIndex].DataStream;
            var nCurrentByteOffset = (int)bufferStream.Position;
            var nMaxIndex = Triangles.Max(p => p.Max());
            int bytesPerComponent = nMaxIndex > 65535 ? 4 : 2;
            WriteTrianglesStream(bufferStream, nMaxIndex > 65535);
            var totalLength = Triangles.Count * bytesPerComponent * (GLRenderMode == 4 ? 3 : 2);
            if (bufferStream.Length % 4 != 0)
            {
                bufferStream.Write(new byte[2] { 0, 0 });
                totalLength += totalLength % 4;
            }
            gLTF.BufferViews.Add(BufferViewHelper.CreateBufferView(BufferViewUsage.Triangle, nCurrentByteOffset, totalLength));
            var bufferViewIndex = gLTF.BufferViews.Count - 1;
            gLTF.Accessors.Add(AccessorHelper.CreateAccessor(gLTF.BufferViews.Count - 1, Triangles.Count * 3, new List<float> { nMaxIndex }, new List<float> { 0 }, BufferViewUsage.Triangle, 0, nMaxIndex <= 65535));
            primitive.Indices = bufferViewIndex;
        }

        public void WriteNormalsStream(Stream bufferStream)
        {
            foreach (var coord in Normals)
            {
                bufferStream.Write(BitConverter.GetBytes(coord.X));
                bufferStream.Write(BitConverter.GetBytes(coord.Y));
                bufferStream.Write(BitConverter.GetBytes(coord.Z));
            }
        }

        public virtual void WriteNormals(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (Normals == null) return;
            var bufferStream = gLTF.Buffers[bufferIndex].DataStream;
            var nCurrentByteOffset = (int)bufferStream.Position;
            WriteNormalsStream(bufferStream);
            var minX = Normals.Min(p2 => p2.X);
            var minY = Normals.Min(p2 => p2.Y);
            var maxX = Normals.Max(p2 => p2.X);
            var maxY = Normals.Max(p2 => p2.Y);
            var minZ = Normals.Min(p2 => p2.Z);
            var maxZ = Normals.Max(p2 => p2.Z);
            var max = new List<float> { maxX, maxY, maxZ };
            var min = new List<float> { minX, minY, minZ };
            var totalLength = Normals.Count * 12;
            gLTF.BufferViews.Add(BufferViewHelper.CreateBufferView(BufferViewUsage.Normal, nCurrentByteOffset, totalLength, 12));
            var bufferViewIndex = gLTF.BufferViews.Count - 1;
            gLTF.Accessors.Add(AccessorHelper.CreateAccessor(gLTF.BufferViews.Count - 1, Normals.Count, max, min, BufferViewUsage.Normal));
            primitive.Attributes.Add("NORMAL", bufferViewIndex);
        }

        public void WriteUvsStream(Stream bufferStream)
        {
            foreach (var coord in Uvs)
            {
                bufferStream.Write(BitConverter.GetBytes(coord.X));
                bufferStream.Write(BitConverter.GetBytes(coord.Y));
            }
        }

        public virtual void WriteUvs(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (Uvs == null) return;
            var bufferStream = gLTF.Buffers[bufferIndex].DataStream;
            var nCurrentByteOffset = (int)bufferStream.Position;
            WriteUvsStream(bufferStream);
            var minX = Uvs.Min(p2 => p2.X);
            var minY = Uvs.Min(p2 => p2.Y);
            var maxX = Uvs.Max(p2 => p2.X);
            var maxY = Uvs.Max(p2 => p2.Y);
            var max = new List<float> { maxX, maxY };
            var min = new List<float> { minX, minY };
            var totalLength = Uvs.Count * 8;
            gLTF.BufferViews.Add(BufferViewHelper.CreateBufferView(BufferViewUsage.UV, nCurrentByteOffset, totalLength, 8));
            var bufferViewIndex = gLTF.BufferViews.Count - 1;
            gLTF.Accessors.Add(AccessorHelper.CreateAccessor(gLTF.BufferViews.Count - 1, Uvs.Count, max, min, BufferViewUsage.UV));
            primitive.Attributes.Add("TEXCOORD_0", bufferViewIndex);
        }

        public virtual void WriteUvNs(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (UvNs == null) return;
            var bufferStream = gLTF.Buffers[bufferIndex].DataStream;
            UvNs.ForEachWithIndex((uvs, index) =>
            {
                var nCurrentByteOffset = (int)bufferStream.Position;
                foreach (var coord in uvs.Value)
                {
                    bufferStream.Write(BitConverter.GetBytes(coord.X));
                    bufferStream.Write(BitConverter.GetBytes(coord.Y));
                }
                var minX = uvs.Value.Min(p2 => p2.X);
                var minY = uvs.Value.Min(p2 => p2.Y);
                var maxX = uvs.Value.Max(p2 => p2.X);
                var maxY = uvs.Value.Max(p2 => p2.Y);
                var max = new List<float> { maxX, maxY };
                var min = new List<float> { minX, minY };
                var totalLength = uvs.Value.Count * 8;
                gLTF.BufferViews.Add(BufferViewHelper.CreateBufferView(BufferViewUsage.UV, nCurrentByteOffset, totalLength, 8));
                var bufferViewIndex = gLTF.BufferViews.Count - 1;
                gLTF.Accessors.Add(AccessorHelper.CreateAccessor(gLTF.BufferViews.Count - 1, uvs.Value.Count, max, min, BufferViewUsage.UV));
                primitive.Attributes.Add($"TEXCOORD_{uvs.Key}", bufferViewIndex);
            });

        }

        public void WriteColorsStream(Stream bufferStream)
        {
            foreach (var coord in Colors)
            {
                bufferStream.Write(BitConverter.GetBytes(coord.X));
                bufferStream.Write(BitConverter.GetBytes(coord.Y));
                bufferStream.Write(BitConverter.GetBytes(coord.Z));
                bufferStream.Write(BitConverter.GetBytes(coord.W));
            }
        }

        public virtual void WriteColors(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (Colors == null) return;
            var bufferStream = gLTF.Buffers[bufferIndex].DataStream;
            var nCurrentByteOffset = (int)bufferStream.Position;
            WriteColorsStream(bufferStream);
            var minX = Colors.Min(p2 => p2.X);
            var minY = Colors.Min(p2 => p2.Y);
            var maxX = Colors.Max(p2 => p2.X);
            var maxY = Colors.Max(p2 => p2.Y);
            var minZ = Colors.Min(p2 => p2.Z);
            var maxZ = Colors.Max(p2 => p2.Z);
            var minW = Colors.Min(p2 => p2.W);
            var maxW = Colors.Max(p2 => p2.W);
            var max = new List<float> { maxX, maxY, maxZ, maxW };
            var min = new List<float> { minX, minY, minZ, minW };
            var totalLength = Colors.Count * 16;
            gLTF.BufferViews.Add(BufferViewHelper.CreateBufferView(BufferViewUsage.Colors, nCurrentByteOffset, totalLength, 16));
            var bufferViewIndex = gLTF.BufferViews.Count - 1;
            gLTF.Accessors.Add(AccessorHelper.CreateAccessor(gLTF.BufferViews.Count - 1, Colors.Count, max, min, BufferViewUsage.Colors));
            primitive.Attributes.Add("COLOR_0", bufferViewIndex);
        }

        public void WriteLineIndicesStream(Stream bufferStream, bool isUseInt)
        {
            foreach (var index in LineIndices)
            {
                if (isUseInt)
                {
                    bufferStream.Write(BitConverter.GetBytes(index.X));
                    bufferStream.Write(BitConverter.GetBytes(index.Y));
                }
                else
                {
                    bufferStream.Write(BitConverter.GetBytes((ushort)index.X));
                    bufferStream.Write(BitConverter.GetBytes((ushort)index.Y));
                }
            }
        }

        public virtual void WriteLineIndices(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (LineIndices == null) return;
            var bufferStream = gLTF.Buffers[bufferIndex].DataStream;
            var nCurrentByteOffset = (int)bufferStream.Position;
            var nMaxIndex = LineIndices.Max(p => p.Max());
            int bytesPerComponent = nMaxIndex > 65535 ? 4 : 2;
            WriteLineIndicesStream(bufferStream, nMaxIndex > 65535);
            var totalLength = LineIndices.Count * bytesPerComponent * 2;
            if (bufferStream.Length % 4 != 0)
            {
                bufferStream.Write(new byte[2] { 0, 0 });
                totalLength += totalLength % 4;
            }
            gLTF.BufferViews.Add(BufferViewHelper.CreateBufferView(BufferViewUsage.BorderLine, nCurrentByteOffset, totalLength));
            var bufferViewIndex = gLTF.BufferViews.Count - 1;
            gLTF.Accessors.Add(AccessorHelper.CreateAccessor(gLTF.BufferViews.Count - 1, LineIndices.Count * 3, new List<float> { nMaxIndex }, new List<float> { 0 }, BufferViewUsage.BorderLine, 0, nMaxIndex <= 65535));
            primitive.Extensions = new Dictionary<string, JObject>();
            //轮廓线扩展字段
            primitive.Extensions.AddValue("CESIUM_primitive_outline", "indices", bufferViewIndex);
        }

        public void WriteBatchIdsStream(Stream bufferStream, bool isUseInt)
        {
            foreach (var index in BatchIds)
            {
                if (isUseInt)
                {
                    bufferStream.Write(BitConverter.GetBytes(index));
                }
                else
                {
                    bufferStream.Write(BitConverter.GetBytes((ushort)index));
                }
            }
        }

        public virtual void WriteBatchIds(GLTF gLTF, GLTFPrimitive primitive, int bufferIndex)
        {
            if (BatchIds == null) return;
            var bufferStream = gLTF.Buffers[bufferIndex].DataStream;
            var nCurrentByteOffset = (int)bufferStream.Position;
            var nMaxIndex = BatchIds.Max();
            int bytesPerComponent = nMaxIndex > 65535 ? 4 : 2;
            WriteBatchIdsStream(bufferStream, nMaxIndex > 65535);
            var totalLength = BatchIds.Count * bytesPerComponent;
            if (bufferStream.Length % 4 != 0)
            {
                bufferStream.Write(new byte[2] { 0, 0 });
                totalLength += totalLength % 4;
            }
            gLTF.BufferViews.Add(BufferViewHelper.CreateBufferView(BufferViewUsage.BatchId, nCurrentByteOffset, totalLength));
            var bufferViewIndex = gLTF.BufferViews.Count - 1;
            gLTF.Accessors.Add(AccessorHelper.CreateAccessor(gLTF.BufferViews.Count - 1, BatchIds.Count * 3, new List<float> { nMaxIndex }, new List<float> { 0 }, BufferViewUsage.BatchId, 0, nMaxIndex <= 65535));
            primitive.Attributes.Add("_BATCHID", bufferViewIndex);
        }
        #endregion

        #region 遍历
        public virtual void VerticesLoop(Action<Vector3> action)
        {
            Vertices.ForEach(v => action(v));
        }

        public virtual void TrianglesLoop(Action<Vector3Int> action)
        {
            Triangles.ForEach(v => action(v));
        }

        public virtual void NormalsLoop(Action<Vector3> action)
        {
            Normals.ForEach(v => action(v));
        }

        public virtual void UvsLoop(Action<Vector2> action)
        {
            Uvs.ForEach(v => action(v));
        }

        public virtual void UvNsLoop(Action<Vector2> action, int nIndex)
        {
            if (UvNs.ContainsKey(nIndex))
            {
                UvNs[nIndex].ForEach(v => action(v));
            }
        }

        public virtual void ColorsLoop(Action<Vector4> action)
        {
            Colors.ForEach(v => action(v));
        }

        public virtual void LineIndicesLoop(Action<Vector2Int> action)
        {
            LineIndices.ForEach(v => action(v));
        }

        public virtual void BatchIdsLoop(Action<uint> action)
        {
            BatchIds.ForEach(v => action(v));
        }
        #endregion

        public GLTFPrimitive AddPrimitiveToGltf(GLTF gLTF, GeometryConvertParam convertParam = null, int bufferIndex = 0)
        {
            GLTFPrimitive primitive = new GLTFPrimitive();
            primitive.GLRenderMode = GLRenderMode;
            if (convertParam?.IsUseExtension ?? true)
            {
                primitive.Extensions = Extensions;
                primitive.Extras = Extras;
            }
            if (MaterialIndex != -1)
            {
                if (!gLTF.Materials.Contains(p => p.UUID == Material.UUID))
                {
                    GLTFMaterial newMaterial = Material.Clone();
                    gLTF.Materials.Add(newMaterial);
                    if (Image != null)
                    {
                        if (gLTF.Images == null)
                        {
                            gLTF.Images = new GLTFImageCollection();
                            gLTF.Textures = new GLTFTextureCollection();
                        }
                        if (Image.PbrImage != null)
                        {
                            if (!gLTF.Images.Contains(p => p.UUID == Image.PbrImage.UUID))
                            {
                                HandleImage(Image.PbrImage, gLTF);
                                newMaterial.PbrMetallicRoughness.BaseColorTexture.Index = gLTF.Images.Count - 1;
                            }
                        }
                        if (Image.EmissiveImage != null)
                        {
                            if (!gLTF.Images.Contains(p => p.UUID == Image.EmissiveImage.UUID))
                            {
                                HandleImage(Image.EmissiveImage, gLTF);
                                newMaterial.EmissiveTexture.Index = gLTF.Images.Count - 1;
                            }
                        }
                        if (Image.OcclusionImage != null)
                        {
                            if (!gLTF.Images.Contains(p => p.UUID == Image.OcclusionImage.UUID))
                            {
                                HandleImage(Image.OcclusionImage, gLTF);
                                newMaterial.OcclusionTexture.Index = gLTF.Images.Count - 1;
                            }
                        }
                        if (Image.NormalImage != null)
                        {
                            if (!gLTF.Images.Contains(p => p.UUID == Image.NormalImage.UUID))
                            {
                                HandleImage(Image.NormalImage, gLTF);
                                newMaterial.NormalTexture.Index = gLTF.Images.Count - 1;
                            }
                        }
                    }
                    primitive.MaterialIndex = gLTF.Materials.Count - 1;
                }
                else
                {
                    primitive.MaterialIndex = gLTF.Materials.IndexOf(gLTF.Materials.FirstOrDefault(p => p.UUID == Material.UUID));
                }
            }
            WriteVertices(gLTF, primitive, bufferIndex);
            WriteNormals(gLTF, primitive, bufferIndex);
            WriteTriangles(gLTF, primitive, bufferIndex);
            WriteUvs(gLTF, primitive, bufferIndex);
            WriteUvNs(gLTF, primitive, bufferIndex);
            WriteBatchIds(gLTF, primitive, bufferIndex);
            WriteLineIndices(gLTF, primitive, bufferIndex);
            WriteColors(gLTF, primitive, bufferIndex);
            return primitive;
        }

        public GLTF ToGltf(GeometryConvertParam convertParam = null)
        {
            var gltf = GLTF.ConvertByAction(self =>
            {
                self.Scenes[0].Nodes.Add(0);
                GLTFNode node = new GLTFNode();
                node.MeshIndex = 0;
                self.Nodes.Add(node);
                GLTFMesh gLTFMesh = new GLTFMesh();
                self.Meshes.Add(gLTFMesh);
                gLTFMesh.Primitives.Add(AddPrimitiveToGltf(self, convertParam));
            });
            return gltf;
        }

        void HandleImage(GLTFImage image, GLTF gLTF)
        {
            GLTFImage newImage = new GLTFImage(GLTFImageType.Bytes);
            newImage.UUID = image.UUID;
            newImage.Name = image.Name;
            newImage.BinaryData = image.GetImageData().ToList();
            newImage.MimeType = image.MimeType;
            gLTF.Images.Add(newImage);
            if (gLTF.Samplers == null)
            {
                //取样器,定义图片的采样和滤波方式
                GLTFSampler sampler = new GLTFSampler();
                sampler.MagFilter = GLTFMagFilter.Linear.GetHashCode();
                sampler.MinFilter = GLTFMinFilter.LinearMipmapLinear.GetHashCode();
                sampler.WrapS = GLTFWrappingMode.Repeat.GetHashCode();
                sampler.WrapT = GLTFWrappingMode.Repeat.GetHashCode();
                gLTF.Samplers = new GLTFSamplerCollection
                {
                    sampler
                };
            }
            //贴图信息,使用source和ssampler指向图片和采样器
            GLTFTexture texture = new GLTFTexture
            {
                ImageIndex = gLTF.Images.Count - 1,
                SamplerIndex = 0
            };
            gLTF.Textures.Add(texture);
        }
    }
}
