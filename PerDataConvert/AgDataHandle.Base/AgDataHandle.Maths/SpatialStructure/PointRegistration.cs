using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace AgDataHandle.Maths.SpatialStructure
{
    /// <summary>
    /// 点集配准,使用icp算法，考虑旋转和缩放，目前计算出来的矩阵信息还存在一定的误差
    /// 算法主要步骤如下
    ///（1）点集移动到中心点
    ///（2）点集配对
    ///（3）计算协方差矩阵，奇异值分解（SVD），得到旋转矩阵
    ///（4）根据步骤3得到的矩阵更新点集，计算偏差值，重复步骤2，直到满足条件
    /// </summary>
    public class PointRegistration
    {
        /// <summary>
        /// 原始点集中心点
        /// </summary>
        private Vector3 m_sCenter;
        /// <summary>
        /// 目标点集中心点
        /// </summary>
        private Vector3 m_tCenter;
        public Vector3 GridCenter { get; set; }
        public float GridSize { get; set; }
        /// <summary>
        /// 是否需要配对点集
        /// </summary>
        public bool NeedPairPoints { get; set; }
        public PointRegistration()
        {
            GridCenter = Vector3.Zero;
            GridSize = 0.4f;
        }


        /// <summary>
        /// 计算矩阵变换信息，包括旋转、缩放、偏移
        /// </summary>
        /// <param name="sourcePs">原始点集</param>
        /// <param name="targetPs">目标点集</param>
        /// <param name="max">最大迭代次数，迭代次数越多，效果越好</param>
        /// <param name="offset">点集允许最大偏差</param>
        /// <returns></returns>
        public MatrixInfo ComputeRST(List<Vertex> sourcePs, List<Vertex> targetPs, int max = 5, float offset = 0.005f)
        {
            //将点移动到中心
            var mInfo = new MatrixInfo();
            var sourcePList = ToVector3List(sourcePs);
            var targetPList = ToVector3List(targetPs);
            m_sCenter = MoveToCenter(sourcePList);
            m_tCenter = MoveToCenter(targetPList);

            //计算初始旋转矩阵
            var rInfo = ComputeInitRotation(sourcePList, targetPList);
            var rM = rInfo.Item1;

            //更新并矫准两个点集
            Update(sourcePList, rM);
            if (NeedPairPoints)
            {
                targetPList = PairPoints(sourcePList, targetPList);
            }


            //迭代计算最佳旋转矩阵
            for (int i = 0; i < max; i++)
            {
                var mat = ComputeRMatrixByICP(targetPList, targetPList);

                rM = rM.Multiply(mat);
                foreach (var item in sourcePList)
                {
                    item.MultiplyByMatrix<Vector3>(mat);
                }
                if (NeedPairPoints)
                {
                    targetPList = PairPoints(sourcePList, targetPList);
                }

            }

            //计算缩放、旋转、偏移矩阵
            var scale = ((targetPList[0] / sourcePList[0]));
            var sourceTM = Matrix4x4.FromTranslationQuaternionRotationScale<Matrix4x4>(-1 * m_sCenter,
              Matrix3x3.Identity.ToQuaternion<Quaternion>(), Vector3.One);
            var sM = Matrix4x4.FromTranslationQuaternionRotationScale<Matrix4x4>(Vector3.Zero,
              Matrix3x3.Identity.ToQuaternion<Quaternion>(), scale);
            var rsM = rM.Multiply(sM);
            var targetTM = Matrix4x4.FromTranslationQuaternionRotationScale<Matrix4x4>(m_tCenter,
              Matrix3x3.Identity.ToQuaternion<Quaternion>(), Vector3.One);
            var rstM = sourceTM.Multiply(rsM).Multiply(targetTM);

            var flag = true;
            var used = new bool[sourcePs.Count];
            for (int i = 0; i < sourcePs.Count; i++)
            {
                var re = sourcePs[i].MultiplyByMatrixToOther<Vector3>(rstM);
                var index = FindNearestP(re, targetPs, used);
                used[index] = true;
                var cOffset = re - targetPs[index];
                if (Math.Abs(cOffset.X) >= offset || Math.Abs(cOffset.Y) >= offset || Math.Abs(cOffset.Z) >= offset)
                {
                    flag = false;
                    break;
                }
            }

            mInfo.Scale = scale;
            mInfo.Rotation = rM.RotateMatrix.ToQuaternion<Quaternion>();
            mInfo.Translation = m_tCenter - m_sCenter;
            mInfo.Matrix = rstM;
            if (flag)
                return mInfo;
            return null;
        }

        /// <summary>
        /// 计算偏移矩阵
        /// </summary>
        /// <param name="sourcePs"></param>
        /// <param name="targetPs"></param>
        /// <param name="max"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public MatrixInfo ComputeT(List<Vertex> sourcePs, List<Vertex> targetPs, float offset = 0.005f)
        {
            //将点移动到中心
            var mInfo = new MatrixInfo();
            var sourcePList = ToVector3List(sourcePs);
            var targetPList = ToVector3List(targetPs);
            m_sCenter = MoveToCenter(sourcePList);
            m_tCenter = MoveToCenter(targetPList);

            //更新并矫准两个点集
            if (NeedPairPoints)
            {
                targetPList = PairPoints(sourcePList, targetPList);
            }

            //偏移矩阵
            var tM = Matrix4x4.FromTranslationQuaternionRotationScale<Matrix4x4>(-1 * m_sCenter + m_tCenter,
              Matrix3x3.Identity.ToQuaternion<Quaternion>(), Vector3.One);

            var flag = true;
            var used = new bool[sourcePs.Count];
            for (int i = 0; i < sourcePs.Count; i++)
            {
                var re = sourcePs[i].MultiplyByMatrixToOther<Vector3>(tM);
                var index = FindNearestP(re, targetPs, used);
                used[index] = true;
                var cOffset = re - targetPs[index];
                if (Math.Abs(cOffset.X) >= offset || Math.Abs(cOffset.Y) >= offset || Math.Abs(cOffset.Z) >= offset)
                {
                    flag = false;
                    break;
                }
            }

            mInfo.Scale = Vector3.One;
            mInfo.Rotation = Quaternion.Identity;
            mInfo.Translation = m_tCenter - m_sCenter;
            mInfo.Matrix = tM;
            if (flag)
                return mInfo;
            return null;
        }

        /// <summary>
        /// 计算初始的旋转参数
        /// </summary>
        /// <param name="sourcePs"></param>
        /// <param name="targetPs"></param>
        /// <returns></returns>
        private Tuple<Matrix4x4, float> ComputeInitRotation(List<Vector3> sourcePs, List<Vector3> targetPs)
        {
            RotateBoundingBox sBox = new RotateBoundingBox(sourcePs);
            RotateBoundingBox tBox = new RotateBoundingBox(targetPs);

            var sF = sBox.DistinBoundingBox.Width > sBox.DistinBoundingBox.Height;
            var tF = tBox.DistinBoundingBox.Width > tBox.DistinBoundingBox.Height;
            sBox.MinTh = sF ? -1 * sBox.MinTh : 90 - sBox.MinTh;
            tBox.MinTh = tF ? -1 * tBox.MinTh : 90 - tBox.MinTh;
            var offset = tBox.MinTh - sBox.MinTh;
            var rM3 = Matrix3x3.CreateRotationZ<Matrix3x3>((float)(-offset * Math.PI / 180f));
            var rM4 = Matrix4x4.FromTranslationQuaternionRotationScale<Matrix4x4>(Vector3.Zero,
              rM3.ToQuaternion<Quaternion>(), Vector3.One);
            return new Tuple<Matrix4x4, float>(rM4, (float)offset);
        }

        /// <summary>
        /// 使用ICP算法计算旋转矩阵
        /// </summary>
        /// <param name="sourcePs"></param>
        /// <param name="targetPs"></param>
        /// <returns></returns>
        private Matrix4x4 ComputeRMatrixByICP(List<Vector3> sourcePs, List<Vector3> targetPs)
        {
            double[,] sArray = new double[sourcePs.Count, 3];
            double[,] tArray = new double[sourcePs.Count, 3];
            for (int i = 0; i < sourcePs.Count; i++)
            {
                var a = sourcePs[i];
                var b = targetPs[i];
                sArray[i, 0] = a.X;
                sArray[i, 1] = a.Y;
                sArray[i, 2] = a.Z;
                tArray[i, 0] = b.X;
                tArray[i, 1] = b.Y;
                tArray[i, 2] = b.Z;
            }
            var matrixBuild = Matrix<double>.Build;
            var sourceM = matrixBuild.DenseOfArray(sArray);
            var targetM = matrixBuild.DenseOfArray(tArray);

            var H = (sourceM).Transpose() * targetM;

            var U = H.Svd().U; //左奇异向量
            var vt = H.Svd().VT;//右奇异向量转置
            var R = vt.Transpose() * U.Transpose();

            var roationM = ToMatrix3x3(R.Transpose());

            var resultM = Matrix4x4.FromTranslationQuaternionRotationScale<Matrix4x4>(Vector3.Zero,
               roationM.ToQuaternion<Quaternion>(), Vector3.One);

            return resultM;
        }

        #region 点集变换
        /// <summary>
        /// 转Vector3
        /// </summary>
        /// <param name="sourcePs"></param>
        /// <returns></returns>
        private List<Vector3> ToVector3List(List<Vertex> sourcePs)
        {
            var list = new List<Vector3>();
            foreach (var item in sourcePs)
            {
                list.Add(new Vector3(item.X, item.Y, item.Z));
            }
            return list;
        }
        /// <summary>
        /// 计算点集中心点，并将点集移动到中心点
        /// </summary>
        /// <param name="vector3s"></param>
        /// <returns></returns>
        private Vector3 MoveToCenter(List<Vector3> vector3s)
        {
            var box = new BoundingBox();
            box.Update(vector3s);
            var center = box.Center();
            for (int i = 0; i < vector3s.Count; i++)
            {
                vector3s[i] -= center;
            }
            return center;
        }
        /// <summary>
        /// 更新点集
        /// </summary>
        /// <param name="sourcePs"></param>
        /// <param name="matrix4X4"></param>
        private void Update(List<Vector3> sourcePs, Matrix4x4 matrix4X4)
        {
            foreach (var item in sourcePs)
            {
                item.MultiplyByMatrix<Vector3>(matrix4X4);
            }
        }
        #endregion

        #region 点集配对
        /// <summary>
        /// 寻找最近的点,配对点集
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private List<Vector3> PairPoints(List<Vector3> source, List<Vector3> target)
        {
            var used = new bool[source.Count];
            var reList = new List<Vector3>();
            var box = new BoundingBox();
            box.Update(source);
            box.Update(target);
            var targetG = Grouping(target, box);
            for (int i = 0; i < source.Count; i++)
            {
                var index = FindNearestP(source[i], targetG, used);
                used[index] = true;
                reList.Add(new Vector3(target[index].X, target[index].Y, target[index].Z));
            }

            return reList;
        }

        private int m_xSize;
        private int m_ySize;
        private int m_zSize;
        /// <summary>
        /// 对点集进行网格划分
        /// </summary>
        /// <param name="source"></param>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        private PointSet[,,] Grouping(List<Vector3> source, BoundingBox box)
        {
            GridCenter = box.Min;
            m_xSize = (int)Math.Round(box.Width / GridSize) + 1;
            m_ySize = (int)Math.Round(box.Height / GridSize) + 1;
            m_zSize = (int)Math.Round(box.ZWidth / GridSize) + 1;
            var pointsSets = new PointSet[m_xSize, m_ySize, m_zSize];
            for (int i = 0; i < source.Count; i++)
            {
                var xIndex = (int)((source[i].X - GridCenter.X) / GridSize);
                var yIndex = (int)((source[i].Y - GridCenter.Y) / GridSize);
                var zIndex = (int)((source[i].Z - GridCenter.Z) / GridSize);
                if (pointsSets[xIndex, yIndex, zIndex].Points == null)
                {
                    pointsSets[xIndex, yIndex, zIndex].Points = new List<PointInfo>();
                }
                pointsSets[xIndex, yIndex, zIndex].Points.Add(new PointInfo(i, source[i]));
            }

            return pointsSets;
        }

        /// <summary>
        /// 寻找最近点
        /// </summary>
        /// <param name="sp">起点</param>
        /// <param name="vertices">目标点集</param>
        /// <param name="uesd"></param>
        /// <returns></returns>
        private int FindNearestP(Vector3 sp, List<Vertex> vertices, bool[] uesd)
        {
            var index = 0;
            var min = float.MaxValue;
            for (int i = 0; i < vertices.Count; i++)
            {
                if (uesd[i])
                    continue;
                var dis = sp.DistanceTo2<Vector3>(vertices[i]);
                if (dis < min)
                {
                    min = dis;
                    index = i;
                }
                if (min <= 0.000001f)
                    break;
            }
            return index;
        }
        private int FindNearestP(Vector3 sp, List<Vector3> vertices, bool[] uesd)
        {
            var index = 0;
            var min = float.MaxValue;
            for (int i = 0; i < vertices.Count; i++)
            {
                if (uesd[i])
                    continue;
                var dis = sp.DistanceTo2<Vector3>(vertices[i]);
                if (dis < min)
                {
                    min = dis;
                    index = i;
                }
                if (min <= 0.000001f)
                    break;
            }
            return index;
        }
        private int FindNearestP(Vector3 sp, PointSet[,,] vertices, bool[] uesd)
        {
            var index = 0;
            var min = float.MaxValue;
            var xIndex = (int)((sp.X - GridCenter.X) / GridSize);
            var yIndex = (int)((sp.Y - GridCenter.Y) / GridSize);
            var zIndex = (int)((sp.Z - GridCenter.Z) / GridSize);
            for (int x = xIndex - 1; x <= xIndex + 1; x++)
            {
                if (x < 0 || x >= m_xSize)
                    continue;
                for (int y = yIndex - 1; y <= yIndex + 1; y++)
                {
                    if (y < 0 || y >= m_ySize)
                        continue;
                    for (int z = zIndex - 1; z <= zIndex + 1; z++)
                    {
                        if (z < 0 || z >= m_zSize)
                            continue;
                        if (vertices[x, y, z].Points == null)
                            continue;
                        var ps = vertices[x, y, z].Points;
                        foreach (var item in ps)
                        {
                            if (uesd[item.ID])
                                continue;
                            var dis = sp.DistanceTo2<Vector3>(item.Position);
                            if (dis < min)
                            {
                                min = dis;
                                index = item.ID;
                            }
                            if (min <= 0.000001f)
                                break;
                        }
                    }
                }
            }
            return index;
        }
        #endregion

        private Matrix3x3 ToMatrix3x3(Matrix<double> R)
        {
            var reslut = Matrix3x3.Identity;
            reslut[0, 0] = (float)R[0, 0];
            reslut[0, 1] = (float)R[0, 1];
            reslut[0, 2] = (float)R[0, 2];
            reslut[1, 0] = (float)R[1, 0];
            reslut[1, 1] = (float)R[1, 1];
            reslut[1, 2] = (float)R[1, 2];
            reslut[2, 0] = (float)R[2, 0];
            reslut[2, 1] = (float)R[2, 1];
            reslut[2, 2] = (float)R[2, 2];
            return reslut;
        }

    }

    /// <summary>
    /// 矩阵变换信息
    /// </summary>
    public class MatrixInfo
    {
        /// <summary>
        /// 变换矩阵
        /// </summary>
        public Matrix4x4 Matrix { get; set; } = Matrix4x4.Identity;

        /// <summary>
        /// 偏移
        /// </summary>
        public Vector3 Translation { get; set; }
        /// <summary>
        /// 旋转
        /// </summary>
        public Quaternion Rotation { get; set; }
        /// <summary>
        /// 缩放
        /// </summary>
        public Vector3 Scale { get; set; }
    }

    struct PointSet
    {
        public List<PointInfo> Points { get; set; }

    }
    struct PointInfo
    {
        public int ID { get; set; }
        public Vector3 Position { get; set; }

        public PointInfo(int id, Vector3 position)
        {
            ID = id;
            Position = position;
        }
    }
}
