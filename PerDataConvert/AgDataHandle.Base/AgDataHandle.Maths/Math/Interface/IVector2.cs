using System;
using System.Collections.Generic;

namespace AgDataHandle.Maths
{
    public interface IVector2 : IVector2<float>
    {
    }

    public interface IVector2d : IVector2<double>
    {
    }

    public interface IVector2<TNum> where TNum : struct, IEquatable<TNum>, IFormattable
    {
        TNum X { get; set; }
        TNum Y { get; set; }
        TNum this[int index] { get; set; }

        /// <summary>
        /// 强转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T To<T>() where T : class;

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
        T Normalize<T>() where T : class, IVector2<TNum>;
        /// <summary>
        /// 归一化，产生新的内存
        /// </summary>
        /// <returns></returns>
        T NormalizeToOther<T>() where T : class, IVector2<TNum>;
        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        TNum DistanceTo<T>(T center) where T : class, IVector2<TNum>;
        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        TNum DistanceTo(TNum pX, TNum pY);
        #endregion

        #region 加减乘除
        /// <summary>
        /// 向量加法，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Add<T>(T V2) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量加法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T AddToOther<T>(T V2) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量数加，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Add<T>(TNum k) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量数加,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T AddToOther<T>(TNum k) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量减法，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Minus<T>(T V2) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量减法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T MinusToOther<T>(T V2) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量数减，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Minus<T>(TNum k) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量数减,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T MinusToOther<T>(TNum k) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量乘法，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Multiply<T>(T V2) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量乘法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T MultiplyToOther<T>(T V2) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量数乘，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T Multiply<T>(TNum k) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量数乘,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T MultiplyToOther<T>(TNum k) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        T Divide<T>(T v2) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        T DivideToOther<T>(T v2) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T Divide<T>(TNum k) where T : class, IVector2<TNum>;
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector2</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T DivideToOther<T>(TNum k) where T : class, IVector2<TNum>;
        /// <summary>
        /// 取反，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Negate<T>() where T : class, IVector2<TNum>;
        /// <summary>
        /// 取反,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T NegateToOther<T>() where T : class, IVector2<TNum>;

        #endregion

        #region 点乘和叉乘
        /// <summary>
        /// 点乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector3"></param>
        /// <returns></returns>
        TNum Dot<T>(T vector3) where T : class, IVector2<TNum>;
        /// <summary>
        /// 叉乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        TNum Cross<T>(T v2) where T : class, IVector2<TNum>;
        #endregion

        #region 角度
        /// <summary>
        /// 计算角度，结果单位为度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        TNum AngleWithDegree<T>(T v2) where T : class, IVector2<TNum>;
        /// <summary>
        /// 计算角度，结果单位为弧度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        TNum Angle<T>(T v2) where T : class, IVector2<TNum>;

        #endregion

        #region 旋转
        /// <summary>
        /// 旋转一定角度,自身改变
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        T Rotate<T>(TNum angle) where T : class, IVector2<TNum>;
        /// <summary>
        /// 旋转一定角度,产生新的内存
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        T RotateToOther<T>(TNum angle) where T : class, IVector2<TNum>;
        #endregion

        #region 转换类型
        byte[] ToBytes();
        TNum[] ToArray();
        List<TNum> ToList();
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
        /// 获取唯一致
        /// </summary>
        /// <param name="errorMeter"></param>
        /// <returns></returns>
        string GetUniqueCode(TNum errorMeter);
    }
}
