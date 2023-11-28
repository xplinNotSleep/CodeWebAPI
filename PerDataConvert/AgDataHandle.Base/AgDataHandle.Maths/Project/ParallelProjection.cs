using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.Project
{
    /// <summary>
    /// 平行投影
    /// </summary>
    public class ParallelProjection
    {
        /// <summary>
        /// 平面法向量
        /// </summary>
        public Vector3d PlanNor { get; set; }
        /// <summary>
        /// 平面所过某点
        /// </summary>
        public Vector3d PlanOneP { get; set; }
        /// <summary>
        ///投影方向向量
        /// </summary>
        public Vector3d ProjLine { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fw_angle">方位角,弧度制</param>
        /// <param name="gd_angle">高度角，弧度制</param>
        /// <param name="planN">平面法向量</param>
        /// <param name="planP">平面点</param>
        public ParallelProjection(double fw_angle,double gd_angle,Vector3d planN,Vector3d planP) 
        {
            PlanNor = planN;
            PlanOneP = planP;
            ProjLine = new Vector3d(Math.Cos(fw_angle), Math.Sin(fw_angle), Math.Tan(gd_angle));
            //ProjLine = new Vector3d(Math.Sin(fw_angle) * Math.Cos(gd_angle), Math.Cos(fw_angle) * Math.Cos(gd_angle), Math.Sin(gd_angle));
        }
        /// <summary>
        /// 获取平行投影矩阵
        /// </summary>
        /// <returns></returns>
        public Matrix4x4D GetProjectMat() 
        {
            var identity = Matrix3x3D.Identity;
            var tempMat = GetMatrix3X3ByVec(PlanNor, ProjLine).MultiplyToOther<Matrix3x3D>(1 / ProjLine.Dot(PlanNor));
            var rotateMat = identity.MinusToOther<Matrix3x3D>(tempMat);

            var translation = ProjLine.MultiplyToOther<Vector3d>(PlanOneP.Dot(PlanNor) / ProjLine.Dot(PlanNor));
            Matrix4x4D projMat = new Matrix4x4D(
                 rotateMat[0, 0], rotateMat[0, 1], rotateMat[0, 2], 0,
                 rotateMat[1, 0], rotateMat[1, 1], rotateMat[1, 2], 0,
                 rotateMat[2, 0], rotateMat[2, 1], rotateMat[2, 2], 0,
                 translation.X, translation.Y, translation.Z, 1
                );
            return projMat;
        }

        /// <summary>
        /// 向量转置与向量相乘得到的矩阵
        /// </summary>
        /// <param name="trs_V"></param>
        /// <param name="vector"></param>
        private Matrix3x3D GetMatrix3X3ByVec(Vector3d trs_V, Vector3d vector) 
        {
            Matrix3x3D Mat = Matrix3x3D.Identity;
            Mat[0, 0] = trs_V.X * vector.X;
            Mat[1, 0] = trs_V.Y * vector.X;
            Mat[2, 0] = trs_V.Z * vector.X;
            Mat[0, 1] = trs_V.X * vector.Y;
            Mat[1, 1] = trs_V.Y * vector.Y;
            Mat[2, 1] = trs_V.Z * vector.Y;
            Mat[0, 2] = trs_V.X * vector.Z;
            Mat[1, 2] = trs_V.Y * vector.Z;
            Mat[2, 2] = trs_V.Z * vector.Z;
            return Mat;
        }
    }
}
