using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 当前矩阵默认为4x4齐次矩阵
    /// </summary>
    public interface IMatrix4x4 : IMatrixNxN, IMatrix4x4<float>
    {
    }

    public interface IMatrix4x4D : IMatrixNxND, IMatrix4x4<double>
    {
    }

    public interface IMatrix4x4<TNum> : IMatrixNxN<TNum> where TNum : struct, IEquatable<TNum>, IFormattable
    {
        /// <summary>
        /// 通过向量缩放,自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        T ScaleByVector<T>(IVector3<TNum> vector) where T : class, IMatrix4x4<TNum>;
        /// <summary>
        /// 通过向量缩放，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        T ScaleByVectorToOther<T>(IVector3<TNum> vector) where T : class, IMatrix4x4<TNum>;
        /// <summary>
        /// 通过向量平移,自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        T TranslationByVector<T>(IVector3<TNum> vector) where T : class, IMatrix4x4<TNum>;
        /// <summary>
        /// 通过向量平移，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        T TranslationByVectorToOther<T>(IVector3<TNum> vector) where T : class, IMatrix4x4<TNum>;
        /// <summary>
        /// 齐次矩阵求逆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T HomogeneousInverse<T>() where T : class, IMatrix4x4<TNum>;
    }
}
