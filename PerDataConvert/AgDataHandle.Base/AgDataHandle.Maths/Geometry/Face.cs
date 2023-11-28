using AgDataHandle.Maths;
using AgDataHandle.Maths.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace AgDataHandle.Maths.Geometry
{
    /// <summary>
    /// 面 一般不存储Verticals和_vertices
    /// </summary>
    public class Face
    {
        protected List<Vector3> _vertices = null;
        protected List<int> _verticesIndex = null;

        public int MaterialIndex { get; set; } = -1;
        public List<int[]> VTNs { get; set; }
        public List<int> VerticesIndex
        {
            get
            {
                if (_verticesIndex == null)
                {
                    _verticesIndex = new List<int>();
                }
                return _verticesIndex;
            }
            set { _verticesIndex = value; }
        }

        public List<Vector3> Vertices
        {
            get
            {
                if (_vertices == null)
                {
                    _vertices = new List<Vector3>();
                }
                return _vertices;
            }
            set { _vertices = value; }
        }
        public Vector3 Normal { get; set; }

        public float Area { get; set; } = -1;

        #region 构造函数
        public Face()
        {
            Init();
        }
        public Face(int v0Index, int v1Index, int v2Index)
        {
            Init();
            VTNs.Add(new int[] {v0Index,-1,-1 });
            VTNs.Add(new int[] { v1Index, -1, -1 });
            VTNs.Add(new int[] { v2Index, -1, -1 });
        }
        public Face(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            Init();
            if (_vertices == null)
            {
                _vertices = new List<Vector3>();
            }
            _vertices.Add(v0);
            _vertices.Add(v1);
            _vertices.Add(v2);
        }
        public Face(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Init();
            if (_vertices == null)
            {
                _vertices = new List<Vector3>();
            }
            _vertices.Add(v0);
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
        }
        protected virtual void Init()
        {
            VTNs = new List<int[]>();
        }
        #endregion


        public void AddVertex(Vertex vertex)
        {
            _vertices.Add(vertex);
        }

        public Vector3 this[int i]
        {
            get { return _vertices[i]; }
        }

        public int Count
        {
            get { return _vertices.Count; }
        }
        public int GetMaxNormalIndex()
        {
            var index = 0;
            var max = float.MinValue;
            for (var i = 0; i < 3; i++)
            {
                if (Math.Abs(Normal[i]) > max)
                {
                    index = i;
                    max = Math.Abs(Normal[i]);
                }

            }
            return index;
        }

        #region 计算法相
        public Vector3 GetNormal(bool clockwise = true)
        {
            if (Normal == null)
            {
                Vector3 v1 = this.Vertices[0];
                Vector3 v2 = this.Vertices[1];
                Vector3 v3 = this.Vertices[2];
                Normal = clockwise ? MathAlgorithm.ComputeNormalByClockwise(new Vector3[] { v1, v2, v3 }) : MathAlgorithm.ComputeNormalByAnticlockwise(new Vector3[] { v1, v2, v3 });
                Normal.Normalize<Vector3>();
            }
            return Normal;
        }
        /// <summary>
        /// 适用于FBX中的法线计算
        /// 不要更改此方法会影响FBX转换
        /// </summary>
        /// <returns></returns>
        public Vector3 GetNormal_v1()
        {
            if (Normal == null)
            {
                Vector3 v1 = this.Vertices[0];
                Vector3 v2 = this.Vertices[1];
                Vector3 v3 = this.Vertices[2];
                Normal = MathAlgorithm.ComputeNormalByAnticlockwise(new Vector3[] { v1, v2, v3 });
                Normal.Normalize<Vector3>();
            }
            return Normal;
        }
        public float GetArea()
        {
            if (Area == -1)
            {
                Vector3 v1 = this.Vertices[0];
                Vector3 v2 = this.Vertices[1];
                Vector3 v3 = this.Vertices[2];
                Area = (float)MathAlgorithm.GetTriangleArea(new List<Vector3>() { v1, v2, v3 });
            }
            return Area;
        }
        #endregion

        #region 复制
        /// <summary>
        /// 复制索引
        /// </summary>
        /// <returns></returns>
        public List<int[]> CopyVNT()
        {
            var result = new List<int[]>();
            for (var i = 0; i < VTNs.Count; i++)
            {
                var array = new int[VTNs[i].Length];
                for (var j = 0; j < VTNs[i].Length; j++)
                {
                    array[j] = VTNs[i][j];
                }
                result.Add(array);
            }
            return result;
        }
        #endregion

        public void FlippingFace()
        {
            var t = VTNs[1][0];
            VTNs[1][0] = VTNs[2][0];
            VTNs[2][0] = t;

            t = VTNs[1][1];
            VTNs[1][1] = VTNs[2][1];
            VTNs[2][1] = t;

            t = VTNs[1][2];
            VTNs[1][2] = VTNs[2][2];
            VTNs[2][2] = t;
        }
    }

    public class FaceWithVertical: Face
    {
        protected override void Init()
        {
            this._verticesIndex = new List<int>();
            this._vertices = new List<Vector3>();
            base.Init();
        }
    }
}
