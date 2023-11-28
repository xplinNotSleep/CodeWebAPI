using AgDataHandle.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 注意，本类部分情况下OK的，可以方向使用，但是矩阵的方位可能会在有些时候出错，谨慎使用
    /// </summary>
    public class MatrixBuilder
    {
        public Vector3d ZAxis;
        public Vector3d XAxis;
        public Vector3d YAxis;
        public Vector3d Origin=new Vector3d();
        private Matrix3x3D m_projectMatrix;
        private Matrix3x3D m_inverseMatrix;

        public MatrixBuilder() 
        {

        }

        /// <summary>
        /// 这个函数会对几个初步的矩阵进行规范化
        /// </summary>
        public void BuildMatrixFromZAxis(Vector3d ZAxis)
        {
            this.ZAxis = ZAxis;

            //XY互换
            if (Math.Abs(ZAxis.X - ZAxis.Y) > 1e-3)
            {
                XAxis = new Vector3d(ZAxis.Y, ZAxis.X, 0);
                YAxis = ZAxis.CrossToOther(XAxis);
                XAxis = YAxis.CrossToOther(ZAxis);
            }
            //XZ互换
            else if (Math.Abs(ZAxis.X - ZAxis.Z) > 1e-3)
            {
                XAxis = new Vector3d(ZAxis.Z, 0, ZAxis.X);
                YAxis = ZAxis.CrossToOther(XAxis);
                XAxis = YAxis.CrossToOther(ZAxis);
            }
            else
            {
                XAxis = new Vector3d(0, ZAxis.Z, ZAxis.Y);
                YAxis = ZAxis.CrossToOther(XAxis);
                XAxis = YAxis.CrossToOther(ZAxis);
            }
            BeutifullVector3(XAxis);
            BeutifullVector3(YAxis);
            BeutifullVector3(ZAxis);

            m_projectMatrix = new Matrix3x3D(new double[]
            {
                XAxis.X,YAxis.X,ZAxis.X,
                XAxis.Y,YAxis.Y,ZAxis.Y,
                XAxis.Z,YAxis.Z, ZAxis.Z
            });
            m_inverseMatrix = m_projectMatrix.InverseToOther<Matrix3x3D>();
        }
        public void BuildMatrixDirectly(Vector3d XAxis, Vector3d YAxis, Vector3d ZAxis)
        {
            this.XAxis = XAxis;
            this.YAxis = YAxis;
            this.ZAxis = ZAxis;

            BeutifullVector3(XAxis);
            BeutifullVector3(YAxis);
            BeutifullVector3(ZAxis);

            m_projectMatrix = new Matrix3x3D(new double[]
            {
                XAxis.X,XAxis.Y,XAxis.Z,
                YAxis.X,YAxis.Y,YAxis.Z,
                ZAxis.X,ZAxis.Y, ZAxis.Z
                /*XAxis.X,YAxis.X,ZAxis.X,
                XAxis.Y,YAxis.Y,ZAxis.Y,
                XAxis.Z,YAxis.Z, ZAxis.Z*/
            });
            m_inverseMatrix = m_projectMatrix.InverseToOther<Matrix3x3D>();
        }
        public Vector3d Project(Vector3d v)
        {
            var tx = XAxis.X * v.X + YAxis.X * v.Y + ZAxis.X * v.Z;
            var ty = XAxis.Y * v.X + YAxis.Y * v.Y + ZAxis.Y * v.Z;
            var tz = XAxis.Z * v.X + YAxis.Z * v.Y + ZAxis.Z * v.Z;

            var tx2 = XAxis.X * v.X + XAxis.Y * v.Y + XAxis.Z * v.Z;
            var ty2 = YAxis.X * v.X + YAxis.Y * v.Y + YAxis.Z * v.Z;
            var tz2 = ZAxis.X * v.X + ZAxis.Y * v.Y + ZAxis.Z * v.Z;

            var q = m_projectMatrix.Multiply(v, Origin);

            return q;// new Vector3(tx, ty, tz);
        }
        public Vector3 ProjectF(Vector3 v)
        {
            var tx = XAxis.X * v.X + YAxis.X * v.Y + ZAxis.X * v.Z;
            var ty = XAxis.Y * v.X + YAxis.Y * v.Y + ZAxis.Y * v.Z;
            var tz = XAxis.Z * v.X + YAxis.Z * v.Y + ZAxis.Z * v.Z;

            var tx2 = XAxis.X * v.X + XAxis.Y * v.Y + XAxis.Z * v.Z;
            var ty2 = YAxis.X * v.X + YAxis.Y * v.Y + YAxis.Z * v.Z;
            var tz2 = ZAxis.X * v.X + ZAxis.Y * v.Y + ZAxis.Z * v.Z;

            var q = m_projectMatrix.Multiply(new Vector3d(v.X, v.Y, v.Z), new Vector3d());

            return new Vector3(q.X, q.Y, q.Z);
        }

        internal Vector3d Unproject(Vector3d v)
        {
            var t1 = m_inverseMatrix.Multiply(v, new Vector3d());
            return t1.AddToOther(Origin);
        }
        internal Vector3 UnprojectF(Vector3 v)
        {
            var q = m_inverseMatrix.Multiply(new Vector3d(v.X, v.Y, v.Z), new Vector3d());
            return new Vector3(q.X, q.Y, q.Z);
        }

        private void BeutifullVector3(Vector3d XAxis)
        {
            XAxis.X = Math.Abs(XAxis.X) < 1e-3 ? 0 : XAxis.X;
            XAxis.Y = Math.Abs(XAxis.Y) < 1e-3 ? 0 : XAxis.Y;
            XAxis.Z = Math.Abs(XAxis.Z) < 1e-3 ? 0 : XAxis.Z;
        }
    }
}
