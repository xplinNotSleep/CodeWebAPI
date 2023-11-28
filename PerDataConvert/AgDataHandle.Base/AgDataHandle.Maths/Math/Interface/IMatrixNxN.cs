using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public interface IMatrixNxN : IMatrix, IMatrixNxN<float>
    {

    }
    public interface IMatrixNxND : IMatrixD, IMatrixNxN<double>
    {

    }

    public interface IMatrixNxN<TNum> : IMatrix<TNum> where TNum : struct, IEquatable<TNum>, IFormattable
    {
        /// <summary>
        /// 是否正交矩阵
        /// </summary>
        /// <returns></returns>
        bool IsOrthogonal();
        /// <summary>
        /// 是否可转换为正交矩阵
        /// </summary>
        /// <returns></returns>
        bool IsCanOrthogonal();
        /// <summary>
        /// 是否单位矩阵
        /// </summary>
        /// <returns></returns>
        bool IsIdentity();
        /// <summary>
        /// 矩阵除法，自身改变
        /// </summary>
        /// <param name="Mb"></param>
        /// <returns></returns>
        T Divide<T>(T Mb) where T : class, IMatrixNxN<TNum>;
        /// <summary>
        /// 矩阵除法,自身不变，产生新内存保存
        /// </summary>
        /// <param name="Mb"></param>
        /// <returns></returns>
        T DivideToOther<T>(T Mb) where T : class, IMatrixNxN<TNum>;
        /// <summary>
        /// 矩阵求逆（伴随矩阵法），自身改变
        /// </summary>
        /// <typeparam name="T">class, IMatrix</typeparam>
        /// <returns></returns>
        T Inverse<T>() where T : class, IMatrixNxN<TNum>;
        /// <summary>
        /// 矩阵求逆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TryInverseToOther<T>(out T result) where T : class, IMatrixNxN<TNum>;
        /// <summary>
        /// 矩阵求逆（伴随矩阵法）,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IMatrix</typeparam>
        /// <returns></returns>
        T InverseToOther<T>() where T : class, IMatrixNxN<TNum>;
        /// <summary>
        /// 矩阵的行列式,矩阵必须是方阵
        /// </summary>
        /// <typeparam name="T">class, IMatrix</typeparam>
        /// <returns></returns>
        TNum GetDeterminant<T>() where T : class, IMatrixNxN<TNum>;
        /// <summary>
        /// 矩阵的伴随矩阵
        /// </summary>
        /// <param name="Ma"></param>
        /// <typeparam name="T">class, IMatrix</typeparam>
        /// <returns></returns>
        T GetAdjoint<T>() where T : class, IMatrixNxN<TNum>;
        /// <summary>
        /// 对应行列式的代数余子式矩阵
        /// </summary>
        /// <param name="ai"></param>
        /// <param name="aj"></param>
        /// <typeparam name="T">class, IMatrix</typeparam>
        /// <returns></returns>
        T GetCofactor<T>(int ai, int aj) where T : class, IMatrixNxN<TNum>;
    }
}
