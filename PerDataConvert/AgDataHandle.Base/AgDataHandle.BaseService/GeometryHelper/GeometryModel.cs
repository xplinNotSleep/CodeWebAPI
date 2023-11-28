using AgDataHandle.Maths;
using AgDataHandle.Maths.Numerics;
using AgDataHandle.BaseService.GeometryHelper.Collection;
using AgDataHandle.BaseService.GeometryHelper.Def;
using AgDataHandle.BaseService.GeometryHelper.Param;
using AgDataHandle.BaseService.GltfSdk.Collection;
using AgDataHandle.BaseService.GltfSdk.Def;

namespace AgDataHandle.BaseService.GeometryHelper
{
    /// <summary>
    /// 分离的GLTF结果集
    /// </summary>
    public class SplitGltfResult
    {
        /// <summary>
        /// gltf实体
        /// </summary>
        public GLTF GLTF { get; set; }
        /// <summary>
        /// 包围盒
        /// </summary>
        public BoundingBox BoundingBox { get; set; }
        /// <summary>
        /// batchid对应的属性值
        /// </summary>
        public Dictionary<string, List<object>> Attrs { get; set; }
        /// <summary>
        /// batch的数量，每个mesh作为一个batch
        /// </summary>
        public int BatchLength { get; set; }
        /// <summary>
        /// 为null的时候为B3DM，非时为I3DM
        /// </summary>
        public List<IMatrix4x4> I3dmMatrixs { get; set; }
        /// <summary>
        /// I3DM的batchid
        /// </summary>
        public List<ushort> BatchIds { get; set; }
    }

    public class GeometryModel
    {
        public GeometryNodeCollection Nodes { get; set; }
        public GeometryMeshCollection Meshes { get; set; }
        /// <summary>
        /// 全部用gltf自带材质
        /// </summary>
        public GLTFMaterialCollection Materials { get; set; }
        public GLTFTextureCollection Textures { get; set; }
        public GLTFImageCollection Images { get; set; }
        /// <summary>
        /// 当前数据坐标轴
        /// </summary>
        public GLTF GLTF { get; set; }
        List<Stream> DataStreams { get; set; }
        public virtual BoundingBox BoundingBox
        {
            get
            {
                return Nodes.BoundingBox;
            }
        }

        public static Matrix4x4 m_InvertYZMat = Matrix4x4.CreateRotationX<Matrix4x4>((float)Math.PI / 2.0f);

        public GeometryModel()
        {
            Nodes = new GeometryNodeCollection();
            Meshes = new GeometryMeshCollection();
            Materials = new GLTFMaterialCollection();
            Textures = new GLTFTextureCollection();
            Images = new GLTFImageCollection();
        }

        public GeometryModel(GLTF gLTF)
        {
            GLTF = gLTF;
            DataStreams = gLTF.Buffers.Select(p => p.DataStream).ToList();
        }

        public static GeometryModel LoadFromGltf(GLTF gLTF)
        {
            GeometryModel model = new GeometryModel(gLTF);
            model.Nodes = new GeometryNodeCollection();
            model.Meshes = new GeometryMeshCollection();
            model.Materials = gLTF.Materials;
            model.Textures = gLTF.Textures;
            model.Images = gLTF.Images;
            List<int> hasHandle = new List<int>();
            gLTF.Nodes.ForEachWithIndex((node, nodeIndex) =>
            {
                if (hasHandle.Contains(nodeIndex))
                    return;
                hasHandle.Add(nodeIndex);
                if (!node.IsMeshNode && node.ChildrenID == null)
                {
                    return;
                }
                GeometryNode geometryNode = model.GLTFNodeToGeometryNode(node);
                if (geometryNode != null)
                {
                    model.Nodes.Add(geometryNode);
                    model.AddNodeMeshKey(geometryNode.MeshId, geometryNode);
                }
                if (!node.IsMeshNode)
                {
                    if (node.ChildrenID != null)
                    {
                        geometryNode.Children = new GeometryNodeCollection();
                        node.ChildrenID.ForEach(child =>
                        {
                            hasHandle.Add(child);
                            var cNode = model.GLTFNodeToGeometryNode(gLTF.Nodes[child]);
                            if (cNode != null)
                            {
                                cNode.pNode = geometryNode;
                                geometryNode.Children.Add(cNode);
                                model.AddNodeMeshKey(cNode.MeshId, cNode);
                            }
                        });
                    }
                }
            });
            return model;
        }

        /// <summary>
        /// 转成gltf
        /// </summary>
        /// <returns></returns>
        public GLTF ToGltf(GeometryConvertParam convertParam = null)
        {
            return Nodes.ToGltf(convertParam);
        }

        /// <summary>
        /// 通过图片排序生成gltf
        /// </summary>
        /// <returns></returns>
        public GLTF ToGltfByOrderImage(GeometryConvertParam convertParam = null)
        {
            GeometryNodeCollection resNodes = new GeometryNodeCollection();
            Meshes.OrderByDescending(m => m.Primitives.Sum(p => p.Image?.GetHashCode() ?? 0)).ForEach(mesh =>
            {
                GetByMesh(Nodes, mesh, ref resNodes);
            });
            return resNodes.ToGltf(convertParam);
        }

        /// <summary>
        /// 通过材质与几何大小拆分多个gltf
        /// </summary>
        /// <returns></returns>
        public List<SplitGltfResult> To3dtiles(GeometryConvertParam convertParam = null)
        {
            List<SplitGltfResult> gLTFs = new List<SplitGltfResult>();
            List<GLTFImage> images = new List<GLTFImage>();
            var tileMaxSize = (convertParam?.dMaxSize ?? 10) / 1.639 * 1024 * 1024;
            double Size = 0;
            GeometryNodeCollection mergeNodes = null;
            uint nIndex = 0;
            int nBatchLength = 0;
            Dictionary<string, List<object>> Attrs = new Dictionary<string, List<object>>();
            bool IsUseExtension = convertParam?.IsUseExtension ?? true;
            ///gltf内的属性取消
            if (convertParam != null)
            {
                convertParam.IsUseExtension = false;
            }
            Meshes.OrderByDescending(m => m.Primitives.Sum(p => p.Image?.GetHashCode() ?? 0)).ForEachWithIndex((mesh, meshId) =>
            {
                GeometryNodeCollection resNodes = new GeometryNodeCollection();
                GetByMesh(Nodes, mesh, ref resNodes);
                bool IsB3dm = resNodes.Count == 1;
                mesh.Primitives.ForEach(p =>
                {
                    if (IsB3dm)
                    {
                        Size += p.PrimitiveSize;
                        if (p.Image != null)
                        {
                            if (p.Image.PbrImage != null && !images.Contains(p.Image.PbrImage))
                            {
                                images.Add(p.Image.PbrImage);
                                Size += p.Image.PbrImage.ImageSize;
                            }
                            if (p.Image.EmissiveImage != null && !images.Contains(p.Image.EmissiveImage))
                            {
                                images.Add(p.Image.EmissiveImage);
                                Size += p.Image.EmissiveImage.ImageSize;
                            }
                            if (p.Image.OcclusionImage != null && !images.Contains(p.Image.OcclusionImage))
                            {
                                images.Add(p.Image.OcclusionImage);
                                Size += p.Image.OcclusionImage.ImageSize;
                            }
                            if (p.Image.NormalImage != null && !images.Contains(p.Image.NormalImage))
                            {
                                images.Add(p.Image.NormalImage);
                                Size += p.Image.NormalImage.ImageSize;
                            }
                        }
                        if (IsUseExtension)
                        {
                            p.BatchIds = new List<uint>();
                            for (int i = 0; i < p.Vertices.Count; i++)
                            {
                                p.BatchIds.Add(nIndex);
                            }
                        }
                    }
                });
                if (IsB3dm)
                {
                    if (IsUseExtension)
                    {
                        if (resNodes[0].Extensions != null)
                        {
                            foreach (var ext in resNodes[0].Extensions)
                            {
                                if (!Attrs.ContainsKey(ext.Key))
                                {
                                    Attrs.Add(ext.Key, new List<object>());
                                }
                            }
                        }
                        if (mesh.Extensions != null)
                        {
                            foreach (var ext in mesh.Extensions)
                            {
                                if (!Attrs.ContainsKey(ext.Key))
                                {
                                    Attrs.Add(ext.Key, new List<object>());
                                }
                            }
                        }
                        if (resNodes[0].Extensions != null)
                        {
                            foreach (var attr in Attrs)
                            {
                                if (resNodes[0].Extensions.ContainsKey(attr.Key))
                                {
                                    Attrs[attr.Key].Add(resNodes[0].Extensions.GetGLTFValue(attr.Key));
                                }
                                else
                                {
                                    Attrs[attr.Key].Add("");
                                }
                            }
                        }
                        if (mesh.Extensions != null)
                        {
                            foreach (var attr in Attrs)
                            {
                                if (mesh.Extensions.ContainsKey(attr.Key))
                                {
                                    Attrs[attr.Key].Add(mesh.Extensions.GetGLTFValue(attr.Key));
                                }
                                else
                                {
                                    Attrs[attr.Key].Add("");
                                }
                            }
                        }
                    }
                    nIndex++;
                    nBatchLength++;
                    if (mergeNodes == null)
                    {
                        mergeNodes = resNodes;
                    }
                    else
                    {
                        mergeNodes.AddRange(resNodes.ToArray());
                    }
                    if (Size > tileMaxSize)
                    {
                        gLTFs.Add(new SplitGltfResult() { GLTF = mergeNodes.ToGltf(convertParam), BoundingBox = mergeNodes.BoundingBox, Attrs = Attrs, BatchLength = nBatchLength });
                        images.Clear();
                        mergeNodes = null;
                        Size = 0;
                        nIndex = 0;
                        if (meshId != Meshes.Count - 1)
                        {
                            Attrs = new Dictionary<string, List<object>>();
                        }
                    }
                }
                else
                {
                    SplitGltfResult splitGltfResult = new SplitGltfResult();
                    splitGltfResult.BatchIds = new List<ushort>();
                    var meshBox = mesh.BoundingBox;
                    var box = new BoundingBox();
                    splitGltfResult.I3dmMatrixs = resNodes.Select(p =>
                    {
                        var matrix = p.GetMatrix();
                        if (convertParam?.IsInvertYZ ?? false)
                        {
                            matrix = matrix.Multiply<IMatrix4x4>(m_InvertYZMat);
                        }
                        var newBox = meshBox.TransformToNew(matrix);
                        box.Update(newBox);
                        return matrix;
                    }).ToList();
                    splitGltfResult.GLTF = mesh.ToGltf(convertParam);
                    if (IsUseExtension)
                    {
                        splitGltfResult.Attrs = new Dictionary<string, List<object>>();
                        splitGltfResult.BatchIds = new List<ushort>();
                        resNodes.ForEach(node =>
                        {
                            if (node.Extensions != null)
                            {
                                foreach (var ext in node.Extensions)
                                {
                                    if (!splitGltfResult.Attrs.ContainsKey(ext.Key))
                                    {
                                        splitGltfResult.Attrs.Add(ext.Key, new List<object>());
                                    }
                                }
                                foreach (var attr in splitGltfResult.Attrs)
                                {
                                    if (node.Extensions.ContainsKey(attr.Key))
                                    {
                                        splitGltfResult.Attrs[attr.Key].Add(node.Extensions.GetGLTFValue(attr.Key));
                                    }
                                    else
                                    {
                                        splitGltfResult.Attrs[attr.Key].Add("");
                                    }
                                }
                            }
                        });
                    }

                    splitGltfResult.BoundingBox = box;
                    gLTFs.Add(splitGltfResult);
                }

            });
            if (mergeNodes != null)
            {
                gLTFs.Add(new SplitGltfResult() { GLTF = mergeNodes.ToGltf(convertParam), BoundingBox = mergeNodes.BoundingBox, Attrs = Attrs, BatchLength = nBatchLength });
            }
            return gLTFs;
        }

        /// <summary>
        /// 递归查找mesh对应的所有nodes
        /// </summary>
        /// <param name="Nodes"></param>
        /// <param name="mesh"></param>
        /// <param name="resNodes"></param>
        void GetByMesh(GeometryNodeCollection Nodes, GeometryMesh mesh, ref GeometryNodeCollection resNodes)
        {
            foreach (GeometryNode node in Nodes)
            {
                if (node.Children == null)
                {
                    if (node.Mesh == mesh)
                    {
                        resNodes.Add(node);
                    }
                }
                else
                {
                    GetByMesh(node.Children, mesh, ref resNodes);
                }
            }
        }

        /// <summary>
        /// 从gltf到this
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        GeometryNode GLTFNodeToGeometryNode(GLTFNode node)
        {
            if (!node.IsMeshNode && node.ChildrenID == null)
            {
                return null;
            }
            GeometryNode geometryNode = new GeometryNode();
            geometryNode.Name = node.Name;
            geometryNode.MeshId = node.MeshIndex;
            geometryNode.Matrix = node.Matrix;
            geometryNode.Translation = node.Translation;
            geometryNode.Rotation = node.Rotation;
            geometryNode.Scale = node.Scale;
            geometryNode.Extensions = node.Extensions;
            geometryNode.Extras = node.Extras;
            var gltfmesh = node.GetMesh();
            if (gltfmesh != null)
            {
                var mesh = SearchMeshByNodes(node.MeshIndex);
                if (mesh != null)
                {
                    geometryNode.Mesh = mesh;
                }
                else
                {
                    geometryNode.Mesh = new GeometryMesh();
                    geometryNode.Mesh.Name = gltfmesh.Name;
                    geometryNode.Mesh.Extras = gltfmesh.Extras;
                    geometryNode.Mesh.Extensions = gltfmesh.Extensions;
                    gltfmesh.Primitives.ForEach(p =>
                    {
                        var primitive = new GeometryStreamPrimitive(GLTF, DataStreams);
                        primitive.Init(p);
                        geometryNode.Mesh.Primitives.Add(primitive);

                    });
                    Meshes.Add(geometryNode.Mesh);
                }
            }

            return geometryNode;
        }

        #region 搜索已有Mesh
        /**
         * 这个Region的内容是从geometryNodes找到对应的Mesh，原来采用的是 GeometryMesh SearchMeshByNodes(GeometryNodeCollection geometryNodes, int meshid)
         * 2023年8月30日  陈彪 修订为GeometryMesh SearchMeshByNodes(int meshid),利用字典提升效率。
         * **/
        Dictionary<int, GeometryNode> m_nodeMeshIdKV = new Dictionary<int, GeometryNode>();
        private void AddNodeMeshKey(int key, GeometryNode node)
        {
            if (key == -1) return;
            if (m_nodeMeshIdKV.ContainsKey(key)) return;
            m_nodeMeshIdKV.Add(key, node);
        }
        GeometryMesh SearchMeshByNodes(int meshid)
        {
            if (m_nodeMeshIdKV.ContainsKey(meshid))
            {
                return m_nodeMeshIdKV[meshid].Mesh;
            }
            return null;
        }
        /// <summary>
        /// 通过meshid查找是否生成了mesh
        /// </summary>
        /// <param name="geometryNodes"></param>
        /// <param name="meshid"></param>
        /// <returns></returns>
        GeometryMesh SearchMeshByNodes(GeometryNodeCollection geometryNodes, int meshid)
        {
            foreach (var node in geometryNodes)
            {
                if (node.Children != null)
                {
                    return SearchMeshByNodes(node.Children, meshid);
                }
                else
                {
                    if (node.MeshId == meshid)
                    {
                        return node.Mesh;
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
