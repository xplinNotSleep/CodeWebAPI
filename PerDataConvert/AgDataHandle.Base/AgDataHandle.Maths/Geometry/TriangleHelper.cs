using AgDataHandle.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.Geometry
{
    /// <summary>
    /// 对不规则多边形做三角剖分处理
    /// </summary>
    public class TriangleHelper
    {
        /// <summary>
        /// 顶点序列
        /// </summary>
        private List<int> m_VertexsSequence = new List<int>();

        /// <summary>
        /// 节点管理器
        /// </summary>
        private NodeManager m_NodeManager = new NodeManager();

        /// <summary>
        /// 当前面片法向量
        /// </summary>
        private Vector3 m_Nomal;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="polygonVertexs">多边形顶点</param>
        public TriangleHelper(List<Vector3> polygonVertexs)
        {
            m_NodeManager.Init(polygonVertexs);

            m_Nomal = MathAlgorithm.ComputeNormalByAnticlockwise(new Vector3[] { polygonVertexs[0], polygonVertexs[1], polygonVertexs[2] });
            m_Nomal.Normalize<Vector3>();
        }

        /// <summary>
        /// 获取三角形的顶点序列
        /// </summary>
        public int[] GetTriangles()
        {
            while (m_NodeManager.LinkedListLength >= 3)
            {
                if (!CutPolygon())
                {
                    return new int[0];
                }
            }
            return m_VertexsSequence.ToArray();
        }

        /// <summary>
        /// 切割多边形
        /// </summary>
        private bool CutPolygon()
        {                
            List<Node> raisedVertices = new List<Node>();
            List<Node> concaveVertices = new List<Node>();
            List<Node> earTips = new List<Node>();

            Node currentNode = m_NodeManager.FirstNode;
            for (int i = 0; i < m_NodeManager.LinkedListLength; i++)
            {
                if (IsRaisedVertex(currentNode.lastNode, currentNode, currentNode.nextNode))
                {
                    raisedVertices.Add(currentNode);
                }
                else
                {
                    concaveVertices.Add(currentNode);
                }

                currentNode = currentNode.nextNode;
            }

            ReversePoints(raisedVertices, concaveVertices);
            for (int i = 0; i < raisedVertices.Count; i++)
            {
                currentNode = raisedVertices[i];
                if (IsEarTip(currentNode.lastNode, currentNode, currentNode.nextNode))
                {
                    earTips.Add(currentNode);
                }
            }

            if (earTips.Count == 0)
            {
                return false;
            }

            m_VertexsSequence.Add(earTips[0].lastNode.id);
            m_VertexsSequence.Add(earTips[0].id);
            m_VertexsSequence.Add(earTips[0].nextNode.id);
            m_NodeManager.RemoveNode(earTips[0]);

            return true;
        }

        /// <summary>
        /// 反转凹凸点集合
        /// </summary>
        /// <param name="raisedVertices"></param>
        /// <param name="concaveVertices"></param>
        private void ReversePoints(List<Node> raisedVertices, List<Node> concaveVertices)
        {
            if (concaveVertices.Count == 0)
            {
                return;
            }
            if (raisedVertices.Count == 0)
            {
                concaveVertices.ForEach(s => raisedVertices.Add(s));
                concaveVertices.Clear();
            }
            else
            {
                Node minNode = GetMinNode();
                if (!raisedVertices.Contains(minNode))
                {
                    List<Node> temp = new List<Node>();
                    raisedVertices.ForEach(s => temp.Add(s));
                    raisedVertices.Clear();
                    concaveVertices.ForEach(s => raisedVertices.Add(s));
                    concaveVertices.Clear();
                    temp.ForEach(s => concaveVertices.Add(s));
                }
            }
        }

        /// <summary>
        /// 获取边缘节点
        /// </summary>
        /// <returns></returns>
        private Node GetMinNode()
        {
            List<Node> nodes = m_NodeManager.AllNodes;
            List<Node> minNodes = GetMinNode(0, nodes);
            if (minNodes.Count != 1)
            {
                minNodes = GetMinNode(1, minNodes);
            }
            if (minNodes.Count != 1)
            {
                minNodes = GetMinNode(2, minNodes);
            }

            return minNodes[0];
        }

        private List<Node> GetMinNode(int vIndex, List<Node> nodes)
        {            
            Node minNode = nodes[0];
            double minValue = minNode.vertex[vIndex];
            List<Node> minNodes = new List<Node>() { minNode };
            for (int i = 1; i < nodes.Count; i++)
            {
                Vector3 currentVector = nodes[i].vertex;
                double currentValue = currentVector[vIndex];
                if (currentValue < minValue)
                {
                    minValue = currentValue;
                    minNode = nodes[i];

                    minNodes.Clear();
                    minNodes.Add(minNode);
                }
                else if (currentValue == minValue)
                {
                    minNodes.Add(nodes[i]);
                }
            }

            return minNodes;
        }        

        private bool IsEarTip(Node previousNode, Node currentNode, Node nextNode)
        {
            bool flag = true;
            Node node = m_NodeManager.FirstNode;
            for (int i = 0; i < m_NodeManager.LinkedListLength; i++)
            {
                if (node != currentNode && node != previousNode && node != nextNode)
                {
                    if (MathAlgorithm.IsInTriangle(previousNode.vertex, currentNode.vertex, nextNode.vertex, node.vertex))
                    {
                        flag = false;
                        break;
                    }
                }
                node = node.nextNode;
            }
            return flag;
        }

        /// <summary>
        /// 判断当前顶点是否凸点
        /// </summary>
        /// <param name="previousNode"></param>
        /// <param name="currentNode"></param>
        /// <param name="nextNode"></param>
        /// <returns></returns>
        private bool IsRaisedVertex(Node previousNode, Node currentNode, Node nextNode)
        {
            Vector3 first = previousNode.vertex;
            Vector3 center = currentNode.vertex;
            Vector3 second = nextNode.vertex;

            Vector3 one = center.MinusToOther(first);
            Vector3 two = second.MinusToOther(center);

            double angle = Math.Atan2(m_Nomal.Dot(one.Cross(two)), one.Dot(two)) * 57.29578F;
            if (angle > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }        

        /// <summary>
        /// 管理多边形 构成一个双向链表
        /// </summary>
        public class NodeManager
        {

            private List<Node> _nodeList = new List<Node>();

            public int LinkedListLength
            {
                get { return _nodeList.Count; }
            }

            public Node FirstNode
            {
                get { return _nodeList[0]; }
            }

            public List<Node> AllNodes
            {
                get
                {
                    return _nodeList;
                }
            }

            public void Init(List<Vector3> vertexs)
            {

                for (int i = 0; i < vertexs.Count; i++)
                {
                    Node node = new Node(i, vertexs[i]);
                    _nodeList.Add(node);
                }

                for (int i = 0; i < LinkedListLength; i++)
                {
                    if (i == 0)
                    {
                        _nodeList[i].lastNode = _nodeList[LinkedListLength - 1];
                        _nodeList[i].nextNode = _nodeList[1];
                    }
                    else if (i == LinkedListLength - 1)
                    {
                        _nodeList[i].lastNode = _nodeList[LinkedListLength - 2];
                        _nodeList[i].nextNode = _nodeList[0];
                    }
                    else
                    {
                        _nodeList[i].lastNode = _nodeList[i - 1];
                        _nodeList[i].nextNode = _nodeList[i + 1];
                    }
                }
            }

            public void RemoveNode(Node node)
            {
                _nodeList.Remove(node);
                node.lastNode.nextNode = node.nextNode;
                node.nextNode.lastNode = node.lastNode;
            }
        }
    }

    public class Node
    {

        public int id;
        public Vector3 vertex;
        public Node lastNode;
        public Node nextNode;

        public Node(int id, Vector3 vertex)
        {
            this.id = id;
            this.vertex = vertex;
        }
    }
}
