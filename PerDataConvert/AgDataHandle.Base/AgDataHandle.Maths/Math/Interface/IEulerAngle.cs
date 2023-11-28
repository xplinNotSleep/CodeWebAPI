using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public interface IEulerAngle : IEulerAngle<float>
    {
    }

    public interface IEulerAngleD : IEulerAngle<double>
    {
    }

    public interface IEulerAngle<T1> where T1 : struct, IEquatable<T1>, IFormattable
    {
        /// <summary>
        /// 强转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T To<T>() where T : class;
        /// <summary>
        /// 绕物体Y轴的旋转量，左手左边系，+X为右
        /// </summary>
        T1 Heading { get; set; }
        /// <summary>
        /// 绕物体X轴的旋转量，+Y为上
        /// </summary>
        T1 Pitch { get; set; }
        /// <summary>
        /// 绕物体Z轴的旋转量，+Z向前
        /// </summary>
        T1 Bank { get; set; }
        /// <summary>
        /// 置0
        /// </summary>
        void ToIdentity();
        /// <summary>
        /// 变换为“限制集”欧拉角
        /// </summary>
        void Canonize();
        /// <summary>
        /// 转成四元数，构造执行物体-惯性旋转的四元数
        /// </summary>
        /// <returns></returns>
        T ToQuaternion<T>() where T:class, IQuaternion<T1>;
        /// <summary>
        /// 转成旋转矩阵
        /// </summary>
        /// <returns></returns>
        T ToMatrix<T>() where T : class, IMatrix3x3<T1>;
    }
}
