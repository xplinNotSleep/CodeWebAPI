using System;
using System.Collections.Generic;

namespace AgDataHandle.Maths
{
    public interface IVector3 : IVector3<float>
    {
    }

    public interface IVector3d : IVector3<double>
    {
    }

    public interface IVector3<TNum> where TNum : struct, IEquatable<TNum>, IFormattable
    {
        TNum X { get; set; }
        TNum Y { get; set; }
        TNum Z { get; set; }
        TNum this[int index] { get; set; }

        #region 距离长度方向
        /// <summary>
        /// 距离
        /// </summary>
        /// <returns></returns>
        TNum Length();
        /// <summary>
        /// 归一化，自身改变
        /// </summary>
        /// <returns></returns>
        T Normalize<T>() where T : class, IVector3<TNum>;
        /// <summary>
        /// 归一化，产生新的内存
        /// </summary>
        /// <returns></returns>
        T NormalizeToOther<T>() where T : class, IVector3<TNum>;
        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        TNum DistanceTo<T>(T center) where T : class, IVector3<TNum>;
        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pZ"></param>
        /// <returns></returns>
        TNum DistanceTo(TNum pX, TNum pY, TNum pZ);
        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <param name="ingoreZ">是否忽略Z轴坐标</param>
        /// <returns></returns>
        TNum DistanceTo<T>(T v2, bool ingoreZ = false) where T : class, IVector3<TNum>;
        #endregion

        #region 加减乘除
        /// <summary>
        /// 向量加法，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Add<T>(T V2) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量加法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T AddToOther<T>(T V2) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量数加，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Add<T>(TNum k) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量数加,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T AddToOther<T>(TNum k) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量减法，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Minus<T>(T V2) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量减法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T MinusToOther<T>(T V2) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量数减，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Minus<T>(TNum k) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量数减,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T MinusToOther<T>(TNum k) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量乘法，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T Multiply<T>(T V2) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量乘法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="V2"></param>
        /// <returns></returns>
        T MultiplyToOther<T>(T V2) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量数乘，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T Multiply<T>(TNum k) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量数乘,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T MultiplyToOther<T>(TNum k) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        T Divide<T>(T v2) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        T DivideToOther<T>(T v2) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T Divide<T>(TNum k) where T : class, IVector3<TNum>;
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, IVector3</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        T DivideToOther<T>(TNum k) where T : class, IVector3<TNum>;
        /// <summary>
        /// 取反，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Negate<T>() where T : class, IVector3<TNum>;
        /// <summary>
        /// 取反,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T NegateToOther<T>() where T : class, IVector3<TNum>;

        #endregion

        #region 点乘和叉乘
        /// <summary>
        /// 点乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector3"></param>
        /// <returns></returns>
        TNum Dot<T>(T vector3) where T : class, IVector3<TNum>;
        /// <summary>
        /// 叉乘，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        T Cross<T>(T v2) where T : class, IVector3<TNum>;
        /// <summary>
        /// 叉乘,产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        T CrossToOther<T>(T v2) where T : class, IVector3<TNum>;
        #endregion

        #region 角度
        /// <summary>
        /// 计算角度，结果单位为度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        TNum AngleWithDegree<T>(T v2) where T : class, IVector3<TNum>;
        /// <summary>
        /// 计算角度，结果单位为弧度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        TNum Angle<T>(T v2) where T : class, IVector3<TNum>;

        #endregion

        #region 调整坐标轴
        /// <summary>
        /// 调换YZ轴
        /// </summary>
        /// <returns></returns>
        T InvertYZ<T>() where T : class, IVector3<TNum>;

        /// <summary>
        /// 调换ZY轴,与InvertYZ配套使用
        /// </summary>
        /// <returns></returns>
        T InvertZY<T>() where T : class, IVector3<TNum>;
        #endregion

        #region 旋转和向量
        /// <summary>
        /// 根据法向量与角度旋转点,自身改变
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        T Rotate<T>(T v1, TNum angle) where T : class, IVector3<TNum>;
        /// <summary>
        /// 根据法向量与角度旋转点,产生新的内存
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        T RotateToOther<T>(T v1, TNum angle) where T : class, IVector3<TNum>;

        /// <summary>
        /// 绕X轴旋转,自身改变
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        T RoateX<T>(TNum angle) where T : class, IVector3<TNum>;

        /// <summary>
        /// 绕Y轴旋转,自身改变
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        T RoateY<T>(TNum angle) where T : class, IVector3<TNum>;

        /// <summary>
        /// 绕Z轴旋转,自身改变
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        T RoateZ<T>(TNum angle) where T : class, IVector3<TNum>;

        /// <summary>
        /// 绕X轴旋转,产生新的内存
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        T RoateXToOther<T>(TNum angle) where T : class, IVector3<TNum>;

        /// <summary>
        /// 绕Y轴旋转,产生新的内存
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        T RoateYToOther<T>(TNum angle) where T : class, IVector3<TNum>;

        /// <summary>
        /// 绕Z轴旋转,产生新的内存
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        T RoateZToOther<T>(TNum angle) where T : class, IVector3<TNum>;
        #endregion

        #region 点偏移旋转

        /// <summary>
        /// 点通过欧拉角转换，自身改变
        /// </summary>
        /// <param name="eulerAngle"></param>
        /// <returns></returns>
        T MultiplyByEulerAngle<T>(IEulerAngle<TNum> eulerAngle) where T : class, IVector3<TNum>;
        /// <summary>
        /// 点通过欧拉角转换，产生新的内存
        /// </summary>
        /// <param name="eulerAngle"></param>
        /// <returns></returns>
        T MultiplyByEulerAngleToOther<T>(IEulerAngle<TNum> eulerAngle) where T : class, IVector3<TNum>;
        /// <summary>
        /// 点通过四元数转换，自身改变
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        T MultiplyByQuaternion<T>(IQuaternion<TNum> quaternion) where T : class, IVector3<TNum>;
        /// <summary>
        /// 点通过四元数转换,产生新的内存
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        T MultiplyByQuaternionToOther<T>(IQuaternion<TNum> quaternion) where T : class, IVector3<TNum>;
        /// <summary>
        /// 点通过矩阵偏移，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix">4x4矩阵</param>
        /// <returns></returns>
        T MultiplyByMatrix<T>(IMatrix3x3<TNum> matrix) where T : class, IVector3<TNum>;
        /// <summary>
        /// 点通过矩阵偏移，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix">4x4矩阵</param>
        /// <returns></returns>
        T MultiplyByMatrixToOther<T>(IMatrix3x3<TNum> matrix) where T : class, IVector3<TNum>;
        /// <summary>
        /// 点通过矩阵偏移，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix">4x4矩阵</param>
        /// <returns></returns>
        T MultiplyByMatrix<T>(IMatrix4x4<TNum> matrix) where T : class, IVector3<TNum>;
        /// <summary>
        /// 点通过矩阵偏移，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix">4x4矩阵</param>
        /// <returns></returns>
        T MultiplyByMatrixToOther<T>(IMatrix4x4<TNum> matrix) where T : class, IVector3<TNum>;
        #endregion

        #region 转换类型
        /// <summary>
        /// 转成字节
        /// </summary>
        /// <returns></returns>
        byte[] ToBytes();
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
        #region 最大最小
        /// <summary>
        /// XYZ最大
        /// </summary>
        /// <returns></returns>
        TNum Max();
        /// <summary>
        /// XYZ最小
        /// </summary>
        /// <returns></returns>
        TNum Min();
        #endregion
        /// <summary>
        /// 获取唯一，排重用
        /// </summary>
        /// <param name="errorMeter">误差</param>
        /// <param name="IsIgnoreZ">是否忽略Z轴</param>
        /// <returns></returns>
        string GetUniqueCode(TNum errorMeter,bool IsIgnoreZ=false);
    }
}
