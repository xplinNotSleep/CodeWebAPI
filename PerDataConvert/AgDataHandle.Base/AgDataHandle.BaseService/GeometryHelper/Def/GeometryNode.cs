using AgDataHandle.Maths;
using AgDataHandle.Maths.Numerics;
using AgDataHandle.BaseService.GeometryHelper.Collection;
using AgDataHandle.BaseService.GeometryHelper.Param;
using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GeometryHelper.Def
{
    public class GeometryNode
    {
        public GeometryNode()
        {
            Mesh = new GeometryMesh();
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 只用来排重
        /// </summary>
        public int MeshId { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public GeometryNode pNode { get; set; }
        /// <summary>
        /// mesh的实际指针
        /// </summary>
        public GeometryMesh Mesh { get; set; }
        /// <summary>
        /// 其次矩阵
        /// </summary>
        public IMatrix4x4 Matrix { get; set; }
        /// <summary>
        /// 旋转
        /// </summary>
        public float[] Rotation { get; set; }
        /// <summary>
        /// 放大缩小
        /// </summary>
        public float[] Scale { get; set; }
        /// <summary>
        /// 偏移
        /// </summary>
        public float[] Translation { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public Dictionary<string, JObject> Extensions { get; set; }
        /// <summary>
        /// 额外
        /// </summary>
        public JObject Extras { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public GeometryNodeCollection Children { get; set; }
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
                    if (Mesh == null)
                    {
                        return Children?.BoundingBox;
                    }
                    else
                    {
                        _BoundingBox = Mesh.BoundingBox;
                        _BoundingBox.Transform2(GetMatrix());
                    }
                }
                return _BoundingBox;
            }
        }

        public IMatrix4x4 GetMatrix()
        {
            //存在变换矩阵则直接返回
            if (Matrix != null)
                return Matrix;

            Vector3 T = new Vector3(0, 0, 0);
            var Q = Quaternion.Identity;
            Vector3 S = Vector3.One;

            if (Translation != null && Translation.Length == 3)
            {
                T = new Vector3(Translation[0], Translation[1], Translation[2]);

            }
            if (Rotation != null && Rotation.Length == 4)
            {
                Q = new Quaternion(Rotation[0], Rotation[1], Rotation[2], Rotation[3]);
            }
            if (Scale != null && Scale.Length == 3)
            {
                S = new Vector3(Scale[0], Scale[1], Scale[2]);
            }

            return Matrix4x4.FromTranslationQuaternionRotationScale<Matrix4x4>(T, Q, S);
        }
        /// <summary>
        /// 把节点转成gltfnode并且加到集合中
        /// </summary>
        /// <param name="gLTF"></param>
        /// <param name="ppNode"></param>
        /// <returns></returns>
        public GLTFNode AddNodeToGltf(GLTF gLTF, GeometryConvertParam convertParam = null, GeometryNode ppNode = null)
        {
            if (ppNode != null)
            {
                AddParentNodeToGltf(gLTF, ppNode, convertParam);
            }
            var node = ToGLTFNode(convertParam);
            gLTF.Nodes.Add(node);
            if (pNode == null)
            {
                gLTF.Scenes.First().Nodes.Add(gLTF.Nodes.Count - 1);
            }
            if (Children != null)
            {
                node.ChildrenID = new List<int>();
                foreach (var child in Children)
                {
                    child.AddNodeToGltf(gLTF, convertParam);
                    node.ChildrenID.Add(gLTF.Nodes.Count - 1);
                }
            }
            else
            {
                if (gLTF.Meshes.Contains(p => p.UUID == MeshId.ToString()))
                {
                    node.MeshIndex = gLTF.Meshes.IndexOf(gLTF.Meshes.FirstOrDefault(p => p.UUID == MeshId.ToString()));
                }
                else
                {
                    var mesh = Mesh.AddMeshToGltf(gLTF);
                    if (mesh != null)
                    {
                        node.MeshIndex = gLTF.Meshes.Count - 1;
                        mesh.UUID = MeshId.ToString();
                    }
                }
            }
            return node;
        }

        public GLTFNode ToGLTFNode(GeometryConvertParam convertParam = null)
        {
            GLTFNode node = new GLTFNode();
            node.Name = Name;
            node.Matrix = Matrix;
            node.Rotation = Rotation;
            node.Scale = Scale;
            node.Translation = Translation;
            if (convertParam?.IsUseExtension ?? true)
            {
                node.Extensions = Extensions;
                node.Extras = Extras;
            }
            return node;
        }

        /// <summary>
        /// 处理当前node的父节点
        /// </summary>
        /// <param name="gLTF"></param>
        /// <param name="ppNode"></param>
        /// <returns></returns>
        public GLTFNode AddParentNodeToGltf(GLTF gLTF, GeometryNode ppNode, GeometryConvertParam convertParam = null)
        {
            if (ppNode.pNode != null)
            {
                AddParentNodeToGltf(gLTF, ppNode.pNode, convertParam);
            }
            GLTFNode node = new GLTFNode();
            node.Name = ppNode.Name;
            node.Matrix = ppNode.Matrix;
            if (convertParam?.IsUseExtension ?? true)
            {
                node.Extensions = ppNode.Extensions;
                node.Extras = ppNode.Extras;
            }
            node.Rotation = ppNode.Rotation;
            node.Scale = ppNode.Scale;
            node.Translation = ppNode.Translation;
            gLTF.Nodes.Add(node);
            if (ppNode.pNode == null)
            {
                gLTF.Scenes.First().Nodes.Add(gLTF.Nodes.Count - 1);
            }
            node.ChildrenID = new List<int> { gLTF.Nodes.Count };
            return node;
        }

        /// <summary>
        /// 转成gltf
        /// </summary>
        /// <returns></returns>
        public GLTF ToGltf(GeometryConvertParam convertParam = null)
        {
            var gltf = GLTF.ConvertByAction(self =>
            {
                AddNodeToGltf(self, convertParam, pNode);
            });
            return gltf;
        }

        public GeometryNode CloneWithoutChildren(GeometryConvertParam convertParam = null)
        {
            var newNode = new GeometryNode();
            newNode.pNode = pNode;
            newNode.MeshId = MeshId;
            newNode.Mesh = Mesh;
            newNode.Name = Name;
            newNode.Matrix = Matrix;
            newNode.Rotation = Rotation;
            newNode.Scale = Scale;
            newNode.Translation = Translation;
            if (convertParam?.IsUseExtension ?? true)
            {
                newNode.Extensions = Extensions;
                newNode.Extras = Extras;
            }
            return newNode;
        }
    }
}
