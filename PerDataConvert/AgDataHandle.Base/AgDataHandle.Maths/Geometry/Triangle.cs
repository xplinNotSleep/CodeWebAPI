using AgDataHandle.Maths;
using System;
using System.Collections.Generic;

namespace AgDataHandle.Maths.Geometry
{
    public class Triangle
    {
        public Vector3 p1 { get; set; }
        public Vector3 p2 { get; set; }
        public Vector3 p3 { get; set; }

        public Vector3 V1 { get; set; }
        public Vector3 V2 { get; set; }

        #region 属性
        public Vector3 this[int index]
        {
            get
            {
                if (index == 0)
                    return p1;
                if (index == 1)
                    return p2;
                return p3;
            }
        }
        public double MinZ
        {
            get
            {
                var min = double.MaxValue;
                var vs = new Vector3[] { p1, p2, p3 };
                for (var i = 0; i < vs.Length; i++)
                {
                    min = min > vs[i].Z ? vs[i].Z : min;
                }
                return min;
            }
        }
        public double MaxZ
        {
            get
            {
                var max = double.MinValue;
                var vs = new Vector3[] { p1, p2, p3 };
                for (var i = 0; i < vs.Length; i++)
                {
                    max = max < vs[i].Z ? vs[i].Z : max;
                }
                return max;
            }
        }
        #endregion

        #region 构造函数
        public Triangle(Vector3[] ps)
        {
            p1 = ps[0];
            p2 = ps[1];
            p3 = ps[2];
        }
        public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
        #endregion

        #region 计算向量
        public void ComputeVector()
        {
            V1 = p1 - p2;
            V2 = p1 - p3;
        }
        public Vector3 ComputeNormal()
        {
            if (V1 == null)
            {
                ComputeVector();
            }

            return V1.CrossToOther(V2).Normalize<Vector3>();
        }
        #endregion

        public Vector3 GetPoint(int i)
        {
            return i == 0 ? p1 : (i == 1 ? p2 : (i == 2 ? p3 : null));
        }

        public LineV[] GetLines()
        {
            LineV[] lines = new LineV[3];
            lines[0] = new LineV(p1, p2);
            lines[1] = new LineV(p2, p3);
            lines[2] = new LineV(p3, p1);
            return lines;
        }

        public Triangle ExtentLength(float p)
        {
            if (V1 == null)
            {
                ComputeVector();
            }
            Triangle t = new Triangle(new Vector3[] { this.p1, (this.V1*2).Add(this.p1), (this.V2*2).Add(this.p1) });
            return t;
        }

        public List<Vector3> ToList()
        {
            return new List<Vector3> { this.p1, this.p2, this.p3 };
        }
    }
}
