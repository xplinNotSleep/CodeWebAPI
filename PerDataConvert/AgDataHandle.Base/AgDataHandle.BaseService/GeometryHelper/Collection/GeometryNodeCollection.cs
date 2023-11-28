using AgDataHandle.Maths.Numerics;
using AgDataHandle.BaseService.GeometryHelper.Def;
using AgDataHandle.BaseService.GeometryHelper.Param;
using AgDataHandle.BaseService.GltfSdk.Collection;
using AgDataHandle.BaseService.GltfSdk.Def;

namespace AgDataHandle.BaseService.GeometryHelper.Collection
{
    public class GeometryNodeCollection : GLTFCollection<GeometryNode>
    {
        BoundingBox _BoundingBox;
        /// <summary>
        /// 包围盒
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                if (_BoundingBox == null)
                {
                    _BoundingBox = new BoundingBox();
                    this.ForEach(x =>
                    {
                        _BoundingBox.Update(x.BoundingBox);
                    });
                }
                return _BoundingBox;
            }
        }
        /// <summary>
        /// 转成gltf
        /// </summary>
        /// <returns></returns>
        public GLTF ToGltf(GeometryConvertParam convertParam = null)
        {
            foreach(var node in this)
            {
                if(node.Children!=null)
                {
                    foreach(var child in node.Children)
                    {
                        if (child.pNode == null)
                            child.pNode = node;
                    }
                }
            }
            GeometryNodeCollection newTopNodes;
            if (Exists(n => n.pNode != null))
            {
                List<GeometryNode> hasHandleNodes = new List<GeometryNode>();
                GeometryNodeCollection topNodes = new GeometryNodeCollection();
                foreach (var node in this)
                {
                    SearchTopNodes(node, ref topNodes, ref hasHandleNodes);
                }
                newTopNodes = new GeometryNodeCollection();
                Dictionary<GeometryNode, GeometryNode> dictOldNodeToNew = new Dictionary<GeometryNode, GeometryNode>();
                foreach (var node in this)
                {
                    AddToTopNodes(node, topNodes, ref newTopNodes, ref dictOldNodeToNew, convertParam);
                }
                if (convertParam?.IsInvertYZ ?? false)
                {
                    foreach (var node in newTopNodes)
                    {
                        if (node.Matrix == null)
                        {
                            node.Matrix = GeometryModel.m_InvertYZMat;
                        }
                        else
                        {
                            node.Matrix.Multiply(GeometryModel.m_InvertYZMat);
                        }
                    }
                }
            }
            else
            {
                if (convertParam?.IsInvertYZ ?? false)
                {
                    if (Count == 1)
                    {
                        var node = this[0];
                        if (node.Matrix == null)
                        {
                            node.Matrix = GeometryModel.m_InvertYZMat;
                        }
                        else
                        {
                            node.Matrix.Multiply(GeometryModel.m_InvertYZMat);
                        }
                        newTopNodes = this;
                    }
                    else
                    {
                        GeometryNode topNode = new GeometryNode();
                        topNode.Matrix = GeometryModel.m_InvertYZMat;
                        topNode.Children = this;
                        newTopNodes = new GeometryNodeCollection
                        {
                            topNode
                        };
                    }
                }
                else
                {
                    newTopNodes = this;
                }
            }
            var gltf = GLTF.ConvertByAction(self =>
            {
                newTopNodes.ForEach(node =>
                {
                    node.AddNodeToGltf(self, convertParam);
                });
            });
            return gltf;
        }

        /// <summary>
        /// 查找所有父节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="topNodes"></param>
        /// <param name="hasHandleNodes"></param>
        void SearchTopNodes(GeometryNode node, ref GeometryNodeCollection topNodes, ref List<GeometryNode> hasHandleNodes)
        {
            if (!hasHandleNodes.Contains(node))
            {
                hasHandleNodes.Add(node);
                if (node.pNode != null)
                {
                    SearchTopNodes(node.pNode, ref topNodes, ref hasHandleNodes);
                }
                else
                {
                    topNodes.Add(node);
                }
            }
        }

        /// <summary>
        /// 根据查找到的父节点重新生成模型
        /// </summary>
        /// <param name="node"></param>
        /// <param name="topNodes"></param>
        /// <param name="NewtopNodes"></param>
        /// <param name="dictOldNodeToNew"></param>
        bool AddToTopNodes(GeometryNode node, GeometryNodeCollection topNodes, ref GeometryNodeCollection NewtopNodes, ref Dictionary<GeometryNode, GeometryNode> dictOldNodeToNew, GeometryConvertParam convertParam = null)
        {
            foreach (var topNode in topNodes)
            {
                if (topNode == node)
                {
                    NewtopNodes.Add(node);
                    return true;
                }
                if (node.pNode != null)
                {
                    if (node.pNode == topNode)
                    {
                        if (!dictOldNodeToNew.ContainsKey(topNode))
                        {
                            var newTopNode = topNode.CloneWithoutChildren(convertParam);
                            newTopNode.Children = new GeometryNodeCollection();
                            NewtopNodes.Add(newTopNode);
                            dictOldNodeToNew.Add(topNode, newTopNode);
                        }
                        dictOldNodeToNew[topNode].Children.Add(node);
                        return true;
                    }
                    else
                    {
                        if (AddToTopNodes(node.pNode, topNodes, ref NewtopNodes, ref dictOldNodeToNew))
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
