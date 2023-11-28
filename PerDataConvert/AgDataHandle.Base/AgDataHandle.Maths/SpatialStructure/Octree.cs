using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialStructure
{
    /// <summary>
    /// 八叉树
    /// </summary>
    public class Octree
    {
        public Octree Parent { get; set; }
        public List<Octree> Children { get; set; }

        public BoundingBox Box { get; set; }
        public Object Content { get; set; }

        public Octree(BoundingBox boundingBox)
        {
            Box = boundingBox;
        }

        #region 创建拥有指定层级的八叉树
        /// <summary>
        /// 创建八叉树
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        public static Octree CreateOctreeTree(BoundingBox boundingBox, int maxLevel)
        {
            var root = new Octree(boundingBox);
            root.SplitByLevel(root, maxLevel - 1);
            return root;
        }

        /// <summary>
        /// 通过层级分割
        /// </summary>
        /// <param name="octree"></param>
        /// <param name="level"></param>
        private void SplitByLevel(Octree octree, int level)
        {
            if (level == 0)
                return;
            octree.Split();
            foreach (var item in octree.Children)
            {
                SplitByLevel(item, level - 1);
            }
        }
        #endregion

        /// <summary>
        /// 分割
        /// </summary>
        /// <returns></returns>
        public List<Octree> Split()
        {
            Children = new List<Octree>();
            for (int i = 0; i < 8; i++)
            {
                var p = Box.Corner(i);
                var box = new BoundingBox();
                box.Update(Box.Center());
                box.Update(p);
                var node = new Octree(box);
                node.Parent = this;
                Children.Add(node);
            }
            return Children;
        }

        /// <summary>
        /// 获取树的所有叶子节点
        /// </summary>
        /// <returns></returns>
        public static void GetLeafNode(Octree octree,ref List<Octree> leafNodes)
        {
            if (octree.Children == null || octree.Children.Count == 0)
            {
                leafNodes.Add(octree);
                return;
            }
            foreach (var item in octree.Children)
            {
                GetLeafNode(item,ref leafNodes);
            }
        }
        /// <summary>
        /// 是否是叶子节点
        /// </summary>
        /// <returns></returns>
        public bool IsLeafNode()
        {
            return Children == null || Children.Count == 0;
        }
    }
}
