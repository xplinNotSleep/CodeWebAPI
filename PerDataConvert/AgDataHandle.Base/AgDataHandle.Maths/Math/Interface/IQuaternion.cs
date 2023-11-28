using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public interface IQuaternion : IVector4<float>, IQuaternion<float>
    {
    }

    public interface IQuaternionD : IVector4<double>, IQuaternion<double>
    {
    }

    public interface IQuaternion<TNum> : IVector4<TNum> where TNum : struct, IEquatable<TNum>, IFormattable
    {
        /// <summary>
        /// 置为单位四元数
        /// </summary>
        void ToIdentity();
        /// <summary>
        /// 返回旋转轴
        /// </summary>
        /// <returns></returns>
        IVector3<TNum> GetRotationAxis();
        /// <summary>
        /// 返回旋转角
        /// </summary>
        /// <returns></returns>
        TNum GetRotationAngle();
        /// <summary>
        /// 四元数转欧拉角
        /// </summary>
        /// <returns></returns>
        T ToEulerAngle<T>() where T : class, IEulerAngle<TNum>;
        /// <summary>
        /// 四元数转3X3旋转矩阵
        /// </summary>
        /// <returns></returns>
        T ToMatrix<T>() where T : class, IMatrix3x3<TNum>;
        /// <summary>
        /// 球面线性插值，单位四元数才能用，假如不是就先规则化
        /// </summary>
        /// <param name="q1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        T Slerp<T>(T q1, TNum t) where T : class, IQuaternion<TNum>;
        /// <summary>
        /// 四元数共轭，即与四元数旋转方向相反的四元数
        /// </summary>
        /// <returns></returns>
        T Conjugate<T>() where T : class, IQuaternion<TNum>;
        /// <summary>
        /// 四元数幂运算
        /// </summary>
        /// <param name="exponent"></param>
        /// <returns></returns>
        T Pow<T>(TNum exponent) where T : class, IQuaternion<TNum>;
    }
}
