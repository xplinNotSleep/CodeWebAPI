using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public interface IVector4 : IVector4<float>
    {
    }

    public interface IVector4d : IVector4<double>
    {
    }

    public interface IVector4<TNum> where TNum : struct, IEquatable<TNum>, IFormattable
    {
        TNum X { get; set; }
        TNum Y { get; set; }
        TNum Z { get; set; }
        TNum W { get; set; }
        TNum this[int index] { get; set; }

        #region 距离长度方向
        /// <summary>
        /// 长度
        /// </summary>
        /// <returns></returns>
        TNum Length();
        /// <summary>
        /// 归一化，自身改变
        /// </summary>
        /// <returns></returns>
        T Normalize<T>() where T : class, IVector4<TNum>;
        /// <summary>
        /// 归一化，产生新的内存
        /// </summary>
        /// <returns></returns>
        T NormalizeToOther<T>() where T : class, IVector4<TNum>;
        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        TNum DistanceTo<T>(T center) where T : class, IVector4<TNum>;
        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pZ"></param>
        /// <param name="pW"></param>
        /// <returns></returns>
        TNum DistanceTo(TNum pX, TNum pY, TNum pZ, TNum pW);
        #endregion

        #region 加减乘除
        /// <summary>
        /// 向量加法，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Add<T>(T V2) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量加法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T AddToOther<T>(T V2) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量数加，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Add<T>(TNum k) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量数加,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T AddToOther<T>(TNum k) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量减法，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Minus<T>(T V2) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量减法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T MinusToOther<T>(T V2) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量数减，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Minus<T>(TNum k) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量数减,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T MinusToOther<T>(TNum k) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量乘法，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Multiply<T>(T V2) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量乘法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T MultiplyToOther<T>(T V2) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量数乘，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T Multiply<T>(TNum k) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量数乘,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T MultiplyToOther<T>(TNum k) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        T Divide<T>(T v2) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        T DivideToOther<T>(T v2) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T Divide<T>(TNum k) where T : class, IVector4<TNum>;
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector4</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T DivideToOther<T>(TNum k) where T : class, IVector4<TNum>;
        /// <summary>
        /// 取反，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Negate<T>() where T : class, IVector4<TNum>;
        /// <summary>
        /// 取反,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T NegateToOther<T>() where T : class, IVector4<TNum>;

        #endregion

        #region 点乘和叉乘
        /// <summary>
        /// 点乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector4"></param>
        /// <returns></returns>
        TNum Dot<T>(T vector4) where T : class, IVector4<TNum>;

        /// <summary>
        /// 叉乘，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="right"></param>
        /// <returns></returns>
        T Cross<T>(T right) where T : class, IVector4<TNum>;

        /// <summary>
        /// 叉乘，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="right"></param>
        /// <returns></returns>
        T CrossToOther<T>(T right) where T : class, IVector4<TNum>;
        #endregion

        #region 角度
        /// <summary>
        /// 计算角度，结果单位为度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        TNum AngleWithDegree<T>(T v2) where T : class, IVector4<TNum>;
        /// <summary>
        /// 计算角度，结果单位为弧度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        TNum Angle<T>(T v2) where T : class, IVector4<TNum>;

        #endregion

        #region 转换
        /// <summary>
        /// 点通过矩阵偏移，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix">4x4矩阵</param>
        /// <returns></returns>
        T MultiplyByMatrix<T>(IMatrix4x4<TNum> matrix) where T : class, IVector4<TNum>;
        /// <summary>
        /// 点通过矩阵偏移，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix">4x4矩阵</param>
        /// <returns></returns>
        T MultiplyByMatrixToOther<T>(IMatrix4x4<TNum> matrix) where T : class, IVector4<TNum>;
        /// <summary>
        /// 点通过四元数转换，自身改变
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        T MultiplyByQuaternion<T>(IQuaternion<TNum> quaternion) where T : class, IVector4<TNum>;
        /// <summary>
        /// 点通过四元数转换,产生新的内存
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        T MultiplyByQuaternionToOther<T>(IQuaternion<TNum> quaternion) where T : class, IVector4<TNum>;
        #endregion

        #region 转换类型
        /// <summary>
        /// 转成字节
        /// </summary>
        /// <returns></returns>
        byte[] ToBytes();
        /// <summary>
        /// 转成数组
        /// </summary>
        /// <returns></returns>
        TNum[] ToArray();
        /// <summary>
        /// 转成列表
        /// </summary>
        /// <returns></returns>
        List<TNum> ToList();
        /// <summary>
        /// 强转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T To<T>() where T : class;
        #endregion
        /// <summary>
        /// 最大
        /// </summary>
        /// <returns></returns>
        TNum Max();
        /// <summary>
        /// 最小
        /// </summary>
        /// <returns></returns>
        TNum Min();
        /// <summary>
        /// 获取唯一值，用来排重
        /// </summary>
        /// <param name="errorMeter"></param>
        /// <returns></returns>
        string GetUniqueCode(TNum errorMeter);
    }
}
