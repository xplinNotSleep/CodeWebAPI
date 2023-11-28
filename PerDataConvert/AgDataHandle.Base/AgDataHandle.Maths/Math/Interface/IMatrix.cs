using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public interface IMatrix : IMatrix<float>
    {
    }

    public interface IMatrixD : IMatrix<double>
    {
    }

    public interface IMatrix<TNum>:IEquatable<IMatrix<TNum>> where TNum : struct, IEquatable<TNum>, IFormattable
    {
        #region 基本信息
        /// <summary>
        /// 通过行列获取数据
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnu"></param>
        /// <returns></returns>
        TNum this[int row, int columnu] { get; set; }
        /// <summary>
        /// 根据下标获取数组，行优先
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        TNum this[int index] { get; set; }
        /// <summary>
        /// 行
        /// </summary>
        int row { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        int column { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        TNum[,] Mat { get; set; }
        #endregion
        #region 数据转换
        /// <summary>
        /// 强转矩阵
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T To<T>() where T : class;
        /// <summary>
        /// 转换成数组
        /// </summary>
        /// <returns></returns>
        TNum[] ToArray();
        /// <summary>
        /// 转换成列表
        /// </summary>
        /// <returns></returns>
        List<TNum> ToList();
        /// <summary>
        /// 转换成浮点数组
        /// </summary>
        /// <returns></returns>
        float[] ToFloatArray();
        /// <summary>
        /// 转换成双精度数组
        /// </summary>
        /// <returns></returns>
        double[] ToDoubleArray();
        /// <summary>
        /// 转换成浮点列表
        /// </summary>
        /// <returns></returns>
        List<float> ToFloatList();
        /// <summary>
        /// 转换成双进度列表
        /// </summary>
        /// <returns></returns>
        List<double> ToDoubleList();
        #endregion
        #region 加减乘除
        /// <summary>
        /// 矩阵加法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        T Add<T>(T Mb) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵加法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        T AddToOther<T>(T Mb) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵数加，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        T Add<T>(TNum k) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵数加,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        T AddToOther<T>(TNum k) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵减法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        T Minus<T>(T Mb) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵减法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        T MinusToOther<T>(T Mb) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵数减，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        T Minus<T>(TNum k) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵数减,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        T MinusToOther<T>(TNum k) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵乘法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        T Multiply<T>(T Mb) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵乘法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        T MultiplyToOther<T>(T Mb) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵数乘，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T Multiply<T>(TNum k) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵数乘,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T MultiplyToOther<T>(TNum k) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T Divide<T>(TNum k) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T DivideToOther<T>(TNum k) where T : class, IMatrix<TNum>;
        /// <summary>
        /// 取反，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Negate<T>() where T : class, IMatrix<TNum>;
        /// <summary>
        /// 取反,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T NegateToOther<T>() where T : class, IMatrix<TNum>;
        #endregion
        #region 转置
        /// <summary>
        /// 矩阵转置，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <returns></returns>
        T Transpose<T>() where T : class, IMatrix<TNum>;
        /// <summary>
        /// 矩阵转置,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <returns></returns>
        T TransposeToOther<T>() where T : class, IMatrix<TNum>;
        #endregion
    }
}
