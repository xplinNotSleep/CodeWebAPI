using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public interface IMatrix3x3 : IMatrixNxN, IMatrix3x3<float>
    {
    }

    public interface IMatrix3x3D : IMatrixNxND, IMatrix3x3<double>
    {
    }

    public interface IMatrix3x3<TNum> : IMatrixNxN<TNum> where TNum : struct, IEquatable<TNum>, IFormattable
    {
        /// <summary>
        /// 通过向量缩放，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        T ScaleByVector<T>(IVector3<TNum> vector) where T : class, IMatrix3x3<TNum>;
        /// <summary>
        /// 通过向量缩放，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        T ScaleByVectorToOther<T>(IVector3<TNum> vector) where T : class, IMatrix3x3<TNum>;
        /// <summary>
        /// 转成欧拉角
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T ToEulerAngle<T>() where T : class, IEulerAngle<TNum>;
        /// <summary>
        /// 转成四元数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>

        T ToQuaternion<T>() where T:class, IQuaternion<TNum>;
        /// <summary>
        /// 转成四元数(Cesium)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>

        T ToQuaternionByCesium<T>() where T : class, IQuaternion<TNum>;

        /// <summary>
        /// 计算特征变量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Tuple<T, T> ComputeEigenDecomposition<T>() where T : class, IMatrix3x3<TNum>;
    }
}
