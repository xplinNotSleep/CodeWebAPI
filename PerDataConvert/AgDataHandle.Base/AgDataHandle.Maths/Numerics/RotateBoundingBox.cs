using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.Numerics
{

    /// <summary>
    /// 旋转包围盒
    /// </summary>
    public class RotateBoundingBox
    {
        private Vector3 m_center;
        private double minTh;
        private BoundingBox m_distinBoundingBox;
        private double m_minSize = 0;
        private double m_sMinSize = 0;
        private delegate double RotateABoundingBoxDelegate(List<Vector3> vts, double th, out BoundingBox boundingBox2);

        #region 属性
        public Vector3 Center { get => m_center; }
        public double MinTh { get => minTh; set => minTh = value; }
        public BoundingBox DistinBoundingBox { get => m_distinBoundingBox; }
        /// <summary>
        /// 是否使用更高的精度来计算
        /// </summary>
        public bool HighPrecision { get; set; } = false;
        #endregion
        public RotateBoundingBox(List<Vector3> vts,bool highPrecision=false)
        {
            HighPrecision = highPrecision;
            var boundingBox = new BoundingBox();
            for (var i = 0; i < vts.Count; i++)
            {
                boundingBox.Update(vts[i]);
            }
            m_minSize = (boundingBox.MaxX - boundingBox.MinX) * (boundingBox.MaxY - boundingBox.MinY);
            m_center = new Vector3((float)(boundingBox.MaxX + boundingBox.MinX) / 2, (float)(boundingBox.MaxY + boundingBox.MinY) / 2, (float)(boundingBox.MaxZ + boundingBox.MinZ) / 2);
            m_distinBoundingBox = boundingBox;
            m_sMinSize = m_minSize;

            MinTh = 0.0;
            RotateABoundingBoxDelegate rotateABoundingBoxDelegate = null;
            if (HighPrecision)
                rotateABoundingBoxDelegate = RotateABoundingBoxWithHighPrecision;
            else
                rotateABoundingBoxDelegate = RotateABoundingBox;

            for (double i = 0; i < 360; i += 1)
            {
                var d = rotateABoundingBoxDelegate(vts, i, out BoundingBox boundingBox2);
                if (d < m_minSize)
                {
                    m_minSize = d;
                    m_distinBoundingBox = boundingBox2;
                    MinTh = i;
                }
            }
            if(HighPrecision)
            {
                var min = MinTh - 1;
                var max = MinTh + 1;
                for (double i = min; i <= max; i += 0.01)
                {
                    var d = rotateABoundingBoxDelegate(vts, i, out BoundingBox boundingBox2);
                    if (d < m_minSize)
                    {
                        m_minSize = d;
                        m_distinBoundingBox = boundingBox2;
                        MinTh = i;
                    }
                }
            }
        }
        /// <summary>
        /// 计算旋转包围盒面积
        /// </summary>
        /// <param name="vts"></param>
        /// <param name="th"></param>
        /// <param name="boundingBox2"></param>
        /// <returns></returns>
        private double RotateABoundingBox(List<Vector3> vts, double th, out BoundingBox boundingBox2)
        {
            th = th / 180 * Math.PI;
            var costh = Math.Cos(th);
            var sinth = Math.Sin(th);

            boundingBox2 = new BoundingBox();
            for (var j = 0; j < 10; j++)
            {
                for (var i = j; i < vts.Count; i += 10)
                {
                    var px = vts[i].X - Center.X;
                    var py = vts[i].Y - Center.Y;
                    var rx = px * costh - py * sinth + Center.X;
                    var ry = px * sinth + py * costh + Center.Y;
                    boundingBox2.Update((float)rx, (float)ry, 0);
                    var size3 = (boundingBox2.MaxX - boundingBox2.MinX) * (boundingBox2.MaxY - boundingBox2.MinY);
                    if (size3 > m_minSize)
                        break;
                }
            }

            var size2 = (boundingBox2.MaxX - boundingBox2.MinX) * (boundingBox2.MaxY - boundingBox2.MinY);
            return size2;
        }

        /// <summary>
        /// 计算旋转包围盒面积，具有更高的精度，因为所有的点都参与的运算
        /// </summary>
        /// <param name="vts"></param>
        /// <param name="th"></param>
        /// <param name="boundingBox2"></param>
        /// <returns></returns>
        private double RotateABoundingBoxWithHighPrecision(List<Vector3> vts, double th, out BoundingBox boundingBox2)
        {
            th = th / 180 * Math.PI;
            var costh = Math.Cos(th);
            var sinth = Math.Sin(th);

            boundingBox2 = new BoundingBox();
            for (var i = 0; i < vts.Count; i ++)
            {
                var px = vts[i].X - Center.X;
                var py = vts[i].Y - Center.Y;
                var rx = px * costh - py * sinth + Center.X;
                var ry = px * sinth + py * costh + Center.Y;
                boundingBox2.Update((float)rx, (float)ry, 0);
                var size3 = (boundingBox2.MaxX - boundingBox2.MinX) * (boundingBox2.MaxY - boundingBox2.MinY);
                if (size3 > m_minSize)
                    break;
            }

            var size2 = (boundingBox2.MaxX - boundingBox2.MinX) * (boundingBox2.MaxY - boundingBox2.MinY);
            return size2;
        }

    }

}
