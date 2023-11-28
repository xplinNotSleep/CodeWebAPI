using AgDataHandle.Maths;
using AgDataHandle.BaseService.GltfSdk.Def;

namespace AgDataHandle.BaseService.GltfSdk.Spilter
{
    /// <summary>
    /// #如果是存在Children的，那么直接打散，不保留。
    /// </summary>
    public class MeshOrderTableGenerator
    {

        #region 获得模型

        private static bool[] m_searchedNodeIndex; // 记录已经搜索过的 node 的 id

        public static List<MeshInfo> LoadNodeMesh(GLTF model, ref List<string> MeshTypeList)
        {
            //记录每个node的 meshindex，nodeindex，matrix，type，
            MeshTypeList = new List<string>();
            var nodes = model.Nodes;
            List<MeshInfo> MeshInfos = new List<MeshInfo>();
            m_searchedNodeIndex = new bool[nodes.Count];
            if (nodes == null || nodes.Count == 0)
                throw new Exception("模型有问题！");
            //#nodes的数量是直接读取的，所以数量是完全的，
            //#因为是跳跃式访问的，所以记录哪些对象是读取的
            for (int i = 0; i < nodes.Count; i++)
            {
                if (m_searchedNodeIndex[i]) // 意味着已经搜索过此node
                    continue;
                var nd = nodes[i];
                if (nd.IsMeshNode)
                {
                    AddNodeInfo(MeshInfos, ref MeshTypeList, nd, nd.Matrix, i);
                }
                else
                {
                    if (nd.ChildrenID.IsNullOrEmpty())
                        continue;
                    m_searchedNodeIndex[i] = true;
                    AddChildNodeInfo(MeshInfos, ref MeshTypeList, nd, nd.Matrix);
                }
            }
            //MeshInfos.Sort(new MeshInfoCompare());

            return ComputeSimilar(MeshInfos);
        }

        public static List<MeshInfo> ComputeSimilar(List<MeshInfo> Meshinfos)
        {
            //根据meshindex分组，划分相似构件。结果为相似性构件分类集合。
            return Meshinfos.GroupBy(p => p.MeshId).ToList(p =>
            {
                var meshInfo = new MeshInfo();
                meshInfo.MeshId = p.Key;
                meshInfo.MeshType = p.First().MeshType;
                if (p.Count() > 1)
                {
                    meshInfo.NodeIds = p.Select(p1 => p1.NodeId).ToList();
                    meshInfo.SimilarMatrixs = p.Select(p1 => p1.Matrix).ToList();
                    meshInfo.IsSimilar = true;
                }
                else
                {
                    meshInfo.NodeId = p.First().NodeId;
                    meshInfo.Matrix = p.First().Matrix;
                    meshInfo.IsSimilar = false;
                }
                return meshInfo;
            });
        }

        private static void AddNodeInfo(List<MeshInfo> MeshInfos, ref List<string> MeshTypeList, GLTFNode nd, IMatrix4x4 computedMatrix, int nodeIndex)
        {
            var info = LoadFromNode(nd, ref MeshTypeList, computedMatrix);
            if (info.NodeId != nodeIndex)
            {
                throw new Exception("序号不对");
            }
            MeshInfos.Add(info);
            //MeshInfos[info.NodeId]= info;
        }

        /// <summary>
        /// 2021年1月21日 陈彪 南京，关于左乘还是右乘，张彬振修改了
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="computedMatrix"></param>
        private static void AddChildNodeInfo(List<MeshInfo> MeshInfos, ref List<string> MeshTypeList, GLTFNode parentNode, IMatrix4x4 computedMatrix)
        {
            var children = parentNode.ChildrenID;
            var nodes = parentNode.GLTF.Nodes;

            for (int i = 0; i < children.Count; i++)
            {
                var childNodeId = children[i];
                var childNode = nodes[childNodeId];
                IMatrix4x4 childComputeMatrix = null;
                if (childNode.Matrix == null)
                {
                    childComputeMatrix = computedMatrix;
                }
                else if (computedMatrix == null)
                {
                    childComputeMatrix = childNode.Matrix;
                }
                else
                {
                    childComputeMatrix = childNode.Matrix.Multiply(computedMatrix);
                }

                if (childNode.IsMeshNode)
                {
                    m_searchedNodeIndex[childNodeId] = true;
                    AddNodeInfo(MeshInfos, ref MeshTypeList, childNode, childComputeMatrix, childNodeId);
                }
                else
                {
                    if (childNode.ChildrenID.IsNullOrEmpty())
                        continue;
                    m_searchedNodeIndex[childNodeId] = true;
                    // 递归
                    AddChildNodeInfo(MeshInfos, ref MeshTypeList, childNode, childComputeMatrix);
                }
            }
        }
        #endregion

        #region 添加阶段
        private static MeshInfo LoadFromNode(GLTFNode nd, ref List<string> MeshTypeList, IMatrix4x4 computedMatrixStr)
        {
            var mesh = nd.GetMesh();
            if (mesh == null)
                return null;

            MeshInfo info = new MeshInfo();
            info.Matrix = GetNodeMatrix(nd, computedMatrixStr, info);
            info.MeshId = nd.MeshIndex;
            info.NodeId = nd.GLTF.Nodes.IndexOf(nd);
            var nodeExt = nd.Extensions;
            string meshType = null;
            if (nodeExt != null && nodeExt.ContainsKey("catagory"))
                meshType = nodeExt["catagory"].Value<string>("Value");
            if (nodeExt != null && nodeExt.ContainsKey("category"))
                meshType = nodeExt["category"].Value<string>("Value");
            if (meshType == null)
            {
                meshType = "GenericModel";
            }
            info.MeshType = meshType;
            if (!MeshTypeList.Contains(meshType))
            {
                MeshTypeList.Add(meshType);
            }
            return info;
        }

        /// <summary>
        /// 获取当前节点的矩阵信息
        /// </summary>
        /// <param name="nd"></param>
        /// <param name="computedMatrixStr"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private static IMatrix4x4 GetNodeMatrix(GLTFNode nd, IMatrix4x4 computedMatrixStr, MeshInfo info)
        {
            // 获取node节点中的矩阵信息
            IMatrix4x4 matrix = Matrix4x4.Identity;
            if (computedMatrixStr != null)
            {
                matrix = computedMatrixStr;
            }
            else
            {
                matrix = nd.GetMatrix();
            }

            return matrix;
        }
        #endregion
    }
}
