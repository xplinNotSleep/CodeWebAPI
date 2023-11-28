using System;
using System.Collections.Generic;
using System.Linq;

namespace AgDataHandle.Maths
{
    public class Vector3 : IVector3
    {
        private static Vector3 m_empty = new Vector3(0, 0, 0);
        private const float FLOAD_DELTA = 1E-09f;
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public static Vector3 Empty { get { return m_empty; } }
        public float this[int index]
        {
            get
            {
                if (index == 0)
                    return X;
                else if (index == 1)
                    return Y;
                else
                    return Z;
            }
            set
            {
                if (index == 0)
                    X = value;
                else if (index == 1)
                    Y = value;
                else
                    Z = value;
            }
        }

        public static readonly Vector3 Zero = new Vector3(0f, 0f, 0f);
        public static readonly Vector3 One = new Vector3(1f, 1f, 1f);
        #region 构造函数
        public Vector3() : this(0, 0, 0) { }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3(double x, double y, double z)
        {
            X = (float)x;
            Y = (float)y;
            Z = (float)z;
        }
        public Vector3(Vector3 other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static T New<T>() where T : class, IVector3<float>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IVector3) || typeT == typeof(IVector3<float>))
            {
                model = new Vector3() as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            return model;
        }

        public static T New<T>(float x, float y, float z) where T : class, IVector3<float>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IVector3) || typeT == typeof(IVector3<float>))
            {
                model = new Vector3() as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            model.X = x;
            model.Y = y;
            model.Z = z;
            return model;
        }

        public static T New<T>(T other) where T : class, IVector3<float>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IVector3) || typeT == typeof(IVector3<float>))
            {
                model = new Vector3() as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            model.X = other.X;
            model.Y = other.Y;
            model.Z = other.Z;
            return model;
        }

        /// <summary>
        /// 强转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T To<T>() where T : class
        {
            var typeT = typeof(T);
            if (typeof(T) == GetType()) return (T)Convert.ChangeType(this, typeof(T));
            dynamic model = null;
            if (typeT == typeof(IVector3)|| typeT == typeof(IVector3<float>))
            {
                model = new Vector3();
            }
            else if (typeT == typeof(IVector3d) || typeT == typeof(IVector3<double>))
            {
                model = new Vector3d();
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            var newModelType = model.X.GetType();
            var IsSame = this.X.GetType() == newModelType;
            if (IsSame)
            {
                model.X = X; model.Y = Y; model.Z = Z;
            }
            else
            {
                if (newModelType == typeof(double))
                {
                    model.X = X; model.Y = Y; model.Z = Z;
                }
                else
                {
                    model.X = (float)X; model.Y = (float)Y; model.Z = (float)Z;
                }
            }
            return model as T;
        }

        public static T FromString<T>(string mbs, char p) where T : class, IVector3<float>
        {
            var ts = mbs.Split(p);
            return New<T>(float.Parse(ts[0]), float.Parse(ts[1]), float.Parse(ts[2])) as T;
        }
        #endregion

        #region 距离长度方向
        /// <summary>
        /// 长度
        /// </summary>
        /// <returns></returns>
        public float Length()
        {
            return (float)Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
        }
        public float LengthSquare()
        {
            return (float)(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
        }
        /// <summary>
        /// 归一化，自身改变
        /// </summary>
        /// <returns></returns>
        public T Normalize<T>() where T : class, IVector3<float>
        {
            var len = Length();
            if (len == 0f) return this as T;
            this.X /= len;
            this.Y /= len;
            this.Z /= len;
            return this as T;
        }

        /// <summary>
        /// 归一化，产生新的内存
        /// </summary>
        /// <returns></returns>
        public T NormalizeToOther<T>() where T : class, IVector3<float>
        {
            T result = New<T>(this as T);
            return result.Normalize<T>();
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float DistanceTo<T>(T v2) where T : class, IVector3<float>
        {
            return (float)Math.Sqrt((double)(Math.Pow(this.X - v2.X, 2) + Math.Pow(this.Y - v2.Y, 2) + Math.Pow(this.Z - v2.Z, 2)));
        }

        /// <summary>
        /// 距离的平方
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float DistanceTo2<T>(T v2) where T : class, IVector3<float>
        {
            return (float)(Math.Pow(this.X - v2.X, 2) + Math.Pow(this.Y - v2.Y, 2) + Math.Pow(this.Z - v2.Z, 2));
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pZ"></param>
        /// <returns></returns>
        public float DistanceTo(float pX, float pY, float pZ)
        {
            return (float)Math.Sqrt((double)(Math.Pow(this.X - pX, 2) + Math.Pow(this.Y - pY, 2) + Math.Pow(this.Z - pZ, 2)));
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <param name="ingoreZ">是否忽略Z轴坐标</param>
        /// <returns></returns>
        public float DistanceTo<T>(T v2, bool ingoreZ = false) where T : class, IVector3<float>
        {
            return (float)Math.Sqrt((double)(Math.Pow(this.X - v2.X, 2) + Math.Pow(this.Y - v2.Y, 2) + (ingoreZ ? 0f : Math.Pow(this.Z - v2.Z, 2))));
        }
        #endregion

        #region 加减乘除
        /// <summary>
        /// 向量加法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Add<T>(T v2) where T : class, IVector3<float>
        {
            this.X += v2.X;
            this.Y += v2.Y;
            this.Z += v2.Z;
            return this as T;
        }
        /// <summary>
        /// 向量加法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T AddToOther<T>(T v2) where T : class, IVector3<float>
        {
            T result = New<T>(this as T);
            return result.Add(v2);
        }
        /// <summary>
        /// 向量数加，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Add<T>(float k) where T : class, IVector3<float>
        {
            this.X += k;
            this.Y += k;
            this.Z += k;
            return this as T;
        }
        /// <summary>
        /// 向量数加,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T AddToOther<T>(float k) where T : class, IVector3<float>
        {
            T result = New<T>(this as T);
            return result.Add<T>(k);
        }
        /// <summary>
        /// 向量减法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Minus<T>(T v2) where T : class, IVector3<float>
        {
            this.X -= v2.X;
            this.Y -= v2.Y;
            this.Z -= v2.Z;
            return this as T;
        }
        /// <summary>
        /// 向量减法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MinusToOther<T>(T v2) where T : class, IVector3<float>
        {
            T result = New<T>(this as T);
            return result.Minus(v2);
        }
        /// <summary>
        /// 向量数减，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Minus<T>(float k) where T : class, IVector3<float>
        {
            this.X -= k;
            this.Y -= k;
            this.Z -= k;
            return this as T;
        }
        /// <summary>
        /// 向量数减,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MinusToOther<T>(float k) where T : class, IVector3<float>
        {
            T result = New<T>(this as T);
            return result.Minus<T>(k);
        }
        /// <summary>
        /// 向量乘法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Multiply<T>(T v2) where T : class, IVector3<float>
        {
            this.X *= v2.X;
            this.Y *= v2.Y;
            this.Z *= v2.Z;
            return this as T;
        }
        /// <summary>
        /// 向量乘法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MultiplyToOther<T>(T v2) where T : class, IVector3<float>
        {
            T result = New<T>(this as T);
            return result.Multiply<T>(v2);
        }
        /// <summary>
        /// 向量数乘，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T Multiply<T>(float k) where T : class, IVector3<float>
        {
            this.X *= k;
            this.Y *= k;
            this.Z *= k;
            return this as T;
        }
        /// <summary>
        /// 向量数乘,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T MultiplyToOther<T>(float k) where T : class, IVector3<float>
        {
            T result = New<T>(this as T);
            return result.Multiply<T>(k);
        }
        /// <summary>
        /// 向量除法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Divide<T>(T v2) where T : class, IVector3<float>
        {
            this.X /= v2.X;
            this.Y /= v2.Y;
            this.Z /= v2.Z;
            return this as T;
        }
        /// <summary>
        /// 向量除法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T DivideToOther<T>(T v2) where T : class, IVector3<float>
        {
            T result = New<T>(this as T);
            return result.Divide<T>(v2);
        }
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T Divide<T>(float k) where T : class, IVector3<float>
        {
            this.X /= k;
            this.Y /= k;
            this.Z /= k;
            return this as T;
        }
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T DivideToOther<T>(float k) where T : class, IVector3<float>
        {
            T result = New<T>(this as T);
            return result.Divide<T>(k);
        }
        /// <summary>
        /// 取反，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Negate<T>() where T : class, IVector3<float>
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;
            return this as T;
        }
        /// <summary>
        /// 取反,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NegateToOther<T>() where T : class, IVector3<float>
        {
            T result = New<T>(this as T);
            return result.Negate<T>();
        }
        #endregion

        #region 点乘和叉乘
        /// <summary>
        /// 点乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public float Dot<T>(T v2) where T : class, IVector3<float>
        {
            return X * v2.X + Y * v2.Y + Z * v2.Z;
        }

        /// <summary>
        /// 叉乘，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Cross<T>(T v2) where T : class, IVector3<float>
        {
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            this.X = y * v2.Z - z * v2.Y;
            this.Y = z * v2.X - x * v2.Z;
            this.Z = x * v2.Y - y * v2.X;
            return this as T;
        }

        /// <summary>
        /// 叉乘,产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T CrossToOther<T>(T v2) where T : class, IVector3<float>
        {
            T result = New<T>(this as T);
            return result.Cross<T>(v2);
        }
        #endregion

        #region 角度
        /// <summary>
        /// 计算角度，结果单位为度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float AngleWithDegree<T>(T v2) where T : class, IVector3<float>
        {
            var radians = Angle(v2);
            return radians * 180f / (float)Math.PI;
        }
        /// <summary>
        /// 计算角度，结果单位为弧度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float Angle<T>(T v2) where T : class, IVector3<float>
        {
            var temp1 = Dot(v2);
            var temp2 = Length() * v2.Length();
            if (temp2 == 0f)
                return 0f;
            var radian_angle = temp1 / temp2;
            if (radian_angle>1.0f)
            {
                return (float)0;
            }
            if(radian_angle<-1.0f)
            {
                return 180;
            }
            return (float)Math.Acos(radian_angle);
        }

        #endregion

        #region 旋转和矩阵
        /// <summary>
        /// 根据法向量与角度旋转点，自身改变
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T Rotate<T>(T v1, float angle) where T : class, IVector3<float>
        {
            var c = (float)Math.Cos(angle);
            var s = (float)Math.Sin(angle);
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            this.X = (v1.X * v1.X * (1 - c) + c) * x + (v1.X * v1.Y * (1 - c) - v1.Z * s) * y + (v1.X * v1.Z * (1 - c) + v1.Y * s) * z;
            this.Y = (v1.Y * v1.X * (1 - c) + v1.Z * s) * x + (v1.Y * v1.Y * (1 - c) + c) * y + (v1.Y * v1.Z * (1 - c) - v1.X * s) * z;
            this.Z = (v1.X * v1.Z * (1 - c) - v1.Y * s) * x + (v1.Y * v1.Z * (1 - c) + v1.X * s) * y + (v1.Z * v1.Z * (1 - c) + c) * z;
            return this as T;
        }

        /// <summary>
        /// 根据法向量与角度旋转点,产生新的内存
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        public T RotateToOther<T>(T v1, float angle) where T : class, IVector3<float>
        {
            T newPoint = New<T>(this as T);
            return newPoint.Rotate(v1, angle);
        }

        /// <summary>
        /// 绕X轴旋转,自身改变
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RoateX<T>(float angle) where T : class, IVector3<float>
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);
            var y = this.Y;
            var z = this.Z;
            this.Y = cos * y - sin * z;
            this.Z = sin * y + cos * z;
            return this as T;
        }

        /// <summary>
        /// 绕X轴旋转,产生新的内存
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RoateXToOther<T>(float angle) where T : class, IVector3<float>
        {
            T newPoint = New<T>(this as T);
            return newPoint.RoateX<T>(angle);
        }

        /// <summary>
        /// 绕Y轴旋转,自身改变
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RoateY<T>(float angle) where T : class, IVector3<float>
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);
            var x = this.X;
            var z = this.Z;
            this.X = sin * z + cos * x;
            this.Z = cos * z - sin * x;
            return this as T;
        }

        /// <summary>
        /// 绕Y轴旋转,产生新的内存
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RoateYToOther<T>(float angle) where T : class, IVector3<float>
        {
            T newPoint = New<T>(this as T);
            return newPoint.RoateY<T>(angle);
        }

        /// <summary>
        /// 绕Z轴旋转,自身改变
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RoateZ<T>(float angle) where T : class, IVector3<float>
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);
            var x = this.X;
            var y = this.Y;
            this.X = cos * x - sin * y;
            this.Y = sin * x + cos * y;
            return this as T;
        }

        /// <summary>
        /// 绕Z轴旋转,产生新的内存
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RoateZToOther<T>(float angle) where T : class, IVector3<float>
        {
            T newPoint = New<T>(this as T);
            return newPoint.RoateZ<T>(angle);
        }

        /// <summary>
        /// 调换YZ轴
        /// </summary>
        /// <returns></returns>
        public T InvertYZ<T>() where T : class, IVector3<float>
        {
            var temp = this.Y;
            this.Y = -this.Z;
            this.Z = temp;
            return this as T;
        }

        /// <summary>
        /// 调换ZY轴,与InvertYZ配套使用
        /// </summary>
        /// <returns></returns>
        public T InvertZY<T>() where T : class, IVector3<float>
        {
            var temp = this.Z;
            this.Z = -this.Y;
            this.Y = temp;
            return this as T;
        }

        /// <summary>
        /// 向量通过欧拉角转换，自身改变
        /// </summary>
        /// <param name="eulerAngle"></param>
        /// <returns></returns>
        public T MultiplyByEulerAngle<T>(IEulerAngle<float> eulerAngle) where T : class, IVector3<float>
        {
            float sp = (float)Math.Sin(eulerAngle.Pitch);
            float sb = (float)Math.Sin(eulerAngle.Bank);
            float sh = (float)Math.Sin(eulerAngle.Heading);
            float cp = (float)Math.Cos(eulerAngle.Pitch);
            float cb = (float)Math.Cos(eulerAngle.Bank);
            float ch = (float)Math.Cos(eulerAngle.Heading);
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            this.X = ch * cb + sh * sp * sb * x + sb * cp * y - sh * cb + ch * sp * sb * z;
            this.Y = -ch * sb + sh * sp * cb * x + cb * cp * y + sb * sh + ch * sp * cb * z;
            this.Z = sh * cp * x - sp * y + ch * cp * z;
            return this as T;
        }

        /// <summary>
        /// 点通过矩阵偏移，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByEulerAngleToOther<T>(IEulerAngle<float> eulerAngle) where T : class, IVector3<float>
        {
            T newPoint = New<T>(this as T);
            return newPoint.MultiplyByEulerAngle<T>(eulerAngle);
        }

        /// <summary>
        /// 点通过四元数转换,根据qpq-1公式,自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public T MultiplyByQuaternion<T>(IQuaternion<float> quaternion) where T : class, IVector3<float>
        {
            var point = new Quaternion(X, Y, Z, 0.0f);
            point = quaternion.CrossToOther(point).Cross(quaternion.Conjugate<Quaternion>());
            this.X = point.X;
            this.Y = point.Y;
            this.Z = point.Z;
            return this as T;
        }

        /// <summary>
        /// 点通过四元数转换,根据qpq-1公式,产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public T MultiplyByQuaternionToOther<T>(IQuaternion<float> quaternion) where T : class, IVector3<float>
        {
            T newPoint = New<T>(this as T);
            return newPoint.MultiplyByQuaternion<T>(quaternion);
        }

        #region 乘以矩阵
        /// <summary>
        /// 点通过矩阵偏移，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByMatrix<T>(IMatrix4x4<float> matrix) where T : class, IVector3<float>
        {
            if (matrix == null || matrix.IsIdentity())
            {
                return this as T;
            }
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            this.X = matrix[0, 0] * x + matrix[1, 0] * y + matrix[2, 0] * z + matrix[3, 0];
            this.Y = matrix[0, 1] * x + matrix[1, 1] * y + matrix[2, 1] * z + matrix[3, 1];
            this.Z = matrix[0, 2] * x + matrix[1, 2] * y + matrix[2, 2] * z + matrix[3, 2];
            return this as T;
        }

        /// <summary>
        /// 点通过矩阵偏移，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByMatrixToOther<T>(IMatrix4x4<float> matrix) where T : class, IVector3<float>
        {
            T newPoint = New<T>(this as T);
            return newPoint.MultiplyByMatrix<T>(matrix);
        }
        /// <summary>
        /// 点通过矩阵偏移，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 MultiplyByMatrixToOtherWithoutT(IMatrix4x4<float> matrix) 
        {
            Vector3 newPoint = new Vector3(this.X, this.Y, this.Z);
            return newPoint.MultiplyByMatrixWithoutT(matrix);
        }
        /// <summary>
        /// 点通过矩阵偏移，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 MultiplyByMatrixWithoutT(IMatrix4x4<float> matrix)
        {
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            this.X = matrix[0, 0] * x + matrix[1, 0] * y + matrix[2, 0] * z + matrix[3, 0];
            this.Y = matrix[0, 1] * x + matrix[1, 1] * y + matrix[2, 1] * z + matrix[3, 1];
            this.Z = matrix[0, 2] * x + matrix[1, 2] * y + matrix[2, 2] * z + matrix[3, 2];
            return this;
        }
        #endregion

        /// <summary>
        /// 点通过矩阵偏移，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByMatrix<T>(IMatrix3x3<float> matrix) where T : class, IVector3<float>
        {
            if (matrix == null || matrix.IsIdentity())
            {
                return this as T;
            }
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            this.X = matrix[0, 0] * x + matrix[1, 0] * y + matrix[2, 0] * z;
            this.Y = matrix[0, 1] * x + matrix[1, 1] * y + matrix[2, 1] * z;
            this.Z = matrix[0, 2] * x + matrix[1, 2] * y + matrix[2, 2] * z;
            return this as T;
        }

        /// <summary>
        /// 点通过矩阵偏移，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByMatrixToOther<T>(IMatrix3x3<float> matrix) where T : class, IVector3<float>
        {
            T newPoint = New<T>(this as T);
            return newPoint.MultiplyByMatrix<T>(matrix);
        }
        #endregion

        #region 转换
        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(X).Concat(BitConverter.GetBytes(Y)).Concat(BitConverter.GetBytes(Z)).ToArray();
        }

        public float[] ToArray()
        {
            return new[] { X, Y, Z };
        }

        public double[] ToDoubleArray(bool IsIgnoreZ = false)
        {
            return IsIgnoreZ ? new[] { (double)X, Y } : new[] { (double)X, Y, Z };
        }

        public Vertex ToVertex()
        {
            return new Vertex(X, Y, Z);
        }

        public List<float> ToList()
        {
            return new List<float> { X, Y, Z };
        }

        public float Max()
        {
            return Math.Max(Math.Max(X, Y), Z);
        }

        public float Min()
        {
            return Math.Min(Math.Min(X, Y), Z);
        }
        #endregion

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public string GetUniqueCode(float errorMeter=0.000001f, bool IsIgnoreZ = false)
        {
            string hashCode = ((long)Math.Round(X / errorMeter, 0)).ToString();
            hashCode +=","+ (long)Math.Round(Y / errorMeter, 0);
            if (!IsIgnoreZ)
            {
                hashCode += ","+(long)Math.Round(Z / errorMeter, 0);
            }
            return hashCode;
        }

        public override string ToString()
        {
            return ToString(",");
        }
        public string ToString(string p)
        {
            return X + p + Y + p + Z;
        }

        public bool Equals(IVector3 other)
        {
            return (X.Equals(other.X) || Math.Abs(X - other.X) < FLOAD_DELTA) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < FLOAD_DELTA) && (Z.Equals(other.Z) || Math.Abs(Z - other.Z) < FLOAD_DELTA);
        }

        public bool Equals<T>(T other) where T : class, IVector3<float>
        {
            return (X.Equals(other.X) || Math.Abs(X - other.X) < FLOAD_DELTA) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < FLOAD_DELTA) && (Z.Equals(other.Z) || Math.Abs(Z - other.Z) < FLOAD_DELTA);
        }

        public bool Equals<T>(T other, float buffer) where T : class, IVector3<float>
        {
            if (Math.Abs(this.X - other.X) < buffer && Math.Abs(this.Y - other.Y) < buffer && Math.Abs(this.Z - other.Z) < buffer)
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is IVector3 && Equals((IVector3)obj);
        }
        public bool Equals(Vector3 v3, double esplision = 0.01, bool ignoreZ = false)
        {
            if (ignoreZ)
            {
                if (Math.Abs(this.X - v3.X) < esplision && Math.Abs(this.Y - v3.Y) < esplision)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (Math.Abs(this.X - v3.X) < esplision && Math.Abs(this.Y - v3.Y) < esplision && Math.Abs(this.Z - v3.Z) < esplision)
                {
                    return true;
                }
                return false;
            }
        }
        #region 重写操作符
        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return left.AddToOther(right);
        }

        public static Vector3 operator -(Vector3 u)
        {
            return u.NegateToOther<Vector3>();
        }

        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return left.MinusToOther(right);
        }

        public static Vector3 operator *(float p, Vector3 u)
        {
            return u.MultiplyToOther<Vector3>(p);
        }

        public static Vector3 operator *(Vector3 u, float p)
        {
            return u.MultiplyToOther<Vector3>(p);
        }

        public static Vector3 operator *(Vector3 left, Vector3 right)
        {
            return left.MultiplyToOther(right);
        }

        public static Vector3 operator /(Vector3 left, Vector3 right)
        {
            return left.DivideToOther(right);
        }

        public static Vector3 operator /(Vector3 u, float a)
        {
            return u.DivideToOther<Vector3>(a);
        }

        public static Vector3 operator /(float a, Vector3 u)
        {
            return new Vector3(a / u.X, a / u.Y, a / u.Z);
        }
        #endregion
    }

    public class Vector3d : IVector3d
    {
        private const double DOUBLE_DELTA = 1E-15f;
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double this[int index]
        {
            get
            {
                if (index == 0)
                    return X;
                else if (index == 1)
                    return Y;
                else
                    return Z;
            }
            set
            {
                if (index == 0)
                    X = value;
                else if (index == 1)
                    Y = value;
                else
                    Z = value;
            }
        }

        public static readonly Vector3d Zero = new Vector3d(0, 0, 0);
        public static readonly Vector3d One = new Vector3d(1, 1, 1);
        #region 构造函数
        public Vector3d() : this(0, 0, 0) { }

        public Vector3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3d(Vector3d other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static T New<T>() where T : class, IVector3<double>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IVector3d) || typeT == typeof(IVector3<double>))
            {
                model = new Vector3d() as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            return model;
        }

        public static T New<T>(double x, double y, double z) where T : class, IVector3<double>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IVector3d) || typeT == typeof(IVector3<double>))
            {
                model = new Vector3d() as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            model.X = x;
            model.Y = y;
            model.Z = z;
            return model;
        }

        public static T New<T>(T other) where T : class, IVector3<double>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IVector3d) || typeT == typeof(IVector3<double>))
            {
                model = new Vector3d() as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            model.X = other.X;
            model.Y = other.Y;
            model.Z = other.Z;
            return model;
        }

        /// <summary>
        /// 强转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T To<T>() where T : class
        {
            var typeT = typeof(T);
            if (typeof(T) == GetType()) return (T)Convert.ChangeType(this, typeof(T));
            dynamic model = null;
            if (typeT == typeof(IVector3) || typeT == typeof(IVector3<float>))
            {
                model = new Vector3();
            }
            else if (typeT == typeof(IVector3d) || typeT == typeof(IVector3<double>))
            {
                model = new Vector3d();
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            var newModelType = model.X.GetType();
            var IsSame = this.X.GetType() == newModelType;
            if (IsSame)
            {
                model.X = X; model.Y = Y; model.Z = Z;
            }
            else
            {
                if (newModelType == typeof(double))
                {
                    model.X = X; model.Y = Y; model.Z = Z;
                }
                else
                {
                    model.X = (float)X; model.Y = (float)Y; model.Z = (float)Z;
                }
            }
            return model as T;
        }

        public static T FromString<T>(string mbs, char p) where T : class, IVector3<double>
        {
            var ts = mbs.Split(p);
            return New<T>(double.Parse(ts[0]), double.Parse(ts[1]), double.Parse(ts[2])) as T;
        }
        #endregion

        #region 距离长度方向
        /// <summary>
        /// 长度
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
        }
        /// <summary>
        /// 归一化，自身改变
        /// </summary>
        /// <returns></returns>
        public T Normalize<T>() where T : class, IVector3<double>
        {
            var len = Length();
            if (len == 0f) return this as T;
            this.X /= len;
            this.Y /= len;
            this.Z /= len;
            return this as T;
        }

        /// <summary>
        /// 归一化，产生新的内存
        /// </summary>
        /// <returns></returns>
        public T NormalizeToOther<T>() where T : class, IVector3<double>
        {
            T result = New<T>(this as T);
            return result.Normalize<T>();
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double DistanceTo<T>(T v2) where T : class, IVector3<double>
        {
            return Math.Sqrt((Math.Pow(this.X - v2.X, 2) + Math.Pow(this.Y - v2.Y, 2) + Math.Pow(this.Z - v2.Z, 2)));
        }

        /// <summary>
        /// 距离的平方
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double DistanceTo2<T>(T v2) where T : class, IVector3<double>
        {
            return Math.Pow(this.X - v2.X, 2) + Math.Pow(this.Y - v2.Y, 2) + Math.Pow(this.Z - v2.Z, 2);
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pZ"></param>
        /// <returns></returns>
        public double DistanceTo(double pX, double pY, double pZ)
        {
            return Math.Sqrt((Math.Pow(this.X - pX, 2) + Math.Pow(this.Y - pY, 2) + Math.Pow(this.Z - pZ, 2)));
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <param name="ingoreZ">是否忽略Z轴坐标</param>
        /// <returns></returns>
        public double DistanceTo<T>(T v2, bool ingoreZ = false) where T : class, IVector3<double>
        {
            return Math.Sqrt((Math.Pow(this.X - v2.X, 2) + Math.Pow(this.Y - v2.Y, 2) + (ingoreZ ? 0f : Math.Pow(this.Z - v2.Z, 2))));
        }
        #endregion

        #region 加减乘除
        /// <summary>
        /// 向量加法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Add<T>(T v2) where T : class, IVector3<double>
        {
            this.X += v2.X;
            this.Y += v2.Y;
            this.Z += v2.Z;
            return this as T;
        }
        /// <summary>
        /// 向量加法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T AddToOther<T>(T v2) where T : class, IVector3<double>
        {
            T result = New<T>(this as T);
            return result.Add(v2);
        }
        /// <summary>
        /// 向量数加，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Add<T>(double k) where T : class, IVector3<double>
        {
            this.X += k;
            this.Y += k;
            this.Z += k;
            return this as T;
        }
        /// <summary>
        /// 向量数加,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T AddToOther<T>(double k) where T : class, IVector3<double>
        {
            T result = New<T>(this as T);
            return result.Add<T>(k);
        }
        /// <summary>
        /// 向量减法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Minus<T>(T v2) where T : class, IVector3<double>
        {
            this.X -= v2.X;
            this.Y -= v2.Y;
            this.Z -= v2.Z;
            return this as T;
        }
        /// <summary>
        /// 向量减法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MinusToOther<T>(T v2) where T : class, IVector3<double>
        {
            T result = New<T>(this as T);
            return result.Minus(v2);
        }
        /// <summary>
        /// 向量数减，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Minus<T>(double k) where T : class, IVector3<double>
        {
            this.X -= k;
            this.Y -= k;
            this.Z -= k;
            return this as T;
        }
        /// <summary>
        /// 向量数减,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MinusToOther<T>(double k) where T : class, IVector3<double>
        {
            T result = New<T>(this as T);
            return result.Minus<T>(k);
        }
        /// <summary>
        /// 向量乘法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Multiply<T>(T v2) where T : class, IVector3<double>
        {
            this.X *= v2.X;
            this.Y *= v2.Y;
            this.Z *= v2.Z;
            return this as T;
        }
        /// <summary>
        /// 向量乘法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MultiplyToOther<T>(T v2) where T : class, IVector3<double>
        {
            T result = New<T>(this as T);
            return result.Multiply<T>(v2);
        }
        /// <summary>
        /// 向量数乘，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T Multiply<T>(double k) where T : class, IVector3<double>
        {
            this.X *= k;
            this.Y *= k;
            this.Z *= k;
            return this as T;
        }
        /// <summary>
        /// 向量数乘,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T MultiplyToOther<T>(double k) where T : class, IVector3<double>
        {
            T result = New<T>(this as T);
            return result.Multiply<T>(k);
        }
        /// <summary>
        /// 向量除法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Divide<T>(T v2) where T : class, IVector3<double>
        {
            this.X /= v2.X;
            this.Y /= v2.Y;
            this.Z /= v2.Z;
            return this as T;
        }
        /// <summary>
        /// 向量除法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T DivideToOther<T>(T v2) where T : class, IVector3<double>
        {
            T result = New<T>(this as T);
            return result.Divide<T>(v2);
        }
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T Divide<T>(double k) where T : class, IVector3<double>
        {
            this.X /= k;
            this.Y /= k;
            this.Z /= k;
            return this as T;
        }
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T DivideToOther<T>(double k) where T : class, IVector3<double>
        {
            T result = New<T>(this as T);
            return result.Divide<T>(k);
        }
        /// <summary>
        /// 取反，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Negate<T>() where T : class, IVector3<double>
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;
            return this as T;
        }
        /// <summary>
        /// 取反,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NegateToOther<T>() where T : class, IVector3<double>
        {
            T result = New<T>(this as T);
            return result.Negate<T>();
        }
        #endregion

        #region 点乘和叉乘
        /// <summary>
        /// 点乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public double Dot<T>(T v2) where T : class, IVector3<double>
        {
            return X * v2.X + Y * v2.Y + Z * v2.Z;
        }

        /// <summary>
        /// 叉乘，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Cross<T>(T v2) where T : class, IVector3<double>
        {
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            this.X = y * v2.Z - z * v2.Y;
            this.Y = z * v2.X - x * v2.Z;
            this.Z = x * v2.Y - y * v2.X;
            return this as T;
        }

        /// <summary>
        /// 叉乘,产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T CrossToOther<T>(T v2) where T : class, IVector3<double>
        {
            T result = New<T>(this as T);
            return result.Cross<T>(v2);
        }
        #endregion

        #region 角度
        /// <summary>
        /// 计算角度，结果单位为度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double AngleWithDegree<T>(T v2) where T : class, IVector3<double>
        {
            var radians = Angle(v2);
            return radians * 180f / Math.PI;
        }
        /// <summary>
        /// 计算角度，结果单位为弧度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double Angle<T>(T v2) where T : class, IVector3<double>
        {
            var temp1 = Dot(v2);
            var temp2 = Length() * v2.Length();
            if (temp2 == 0)
                return 0;
            var radian_angle = temp1 / temp2;
            if (radian_angle > 1.0)
            {
                return (float)Math.PI;
            }
            return Math.Acos(radian_angle);
        }

        #endregion

        #region 旋转和矩阵
        /// <summary>
        /// 根据法向量与角度旋转点，自身改变
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T Rotate<T>(T v1, double angle) where T : class, IVector3<double>
        {
            var c = Math.Cos(angle);
            var s = Math.Sin(angle);
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            this.X = (v1.X * v1.X * (1 - c) + c) * x + (v1.X * v1.Y * (1 - c) - v1.Z * s) * y + (v1.X * v1.Z * (1 - c) + v1.Y * s) * z;
            this.Y = (v1.Y * v1.X * (1 - c) + v1.Z * s) * x + (v1.Y * v1.Y * (1 - c) + c) * y + (v1.Y * v1.Z * (1 - c) - v1.X * s) * z;
            this.Z = (v1.X * v1.Z * (1 - c) - v1.Y * s) * x + (v1.Y * v1.Z * (1 - c) + v1.X * s) * y + (v1.Z * v1.Z * (1 - c) + c) * z;
            return this as T;
        }

        /// <summary>
        /// 根据法向量与角度旋转点,产生新的内存
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        public T RotateToOther<T>(T v1, double angle) where T : class, IVector3<double>
        {
            T newPoint = New<T>(this as T);
            return newPoint.Rotate(v1, angle);
        }

        /// <summary>
        /// 绕X轴旋转,自身改变
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RoateX<T>(double angle) where T : class, IVector3<double>
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            var y = this.Y;
            var z = this.Z;
            this.Y = cos * y - sin * z;
            this.Z = sin * y + cos * z;
            return this as T;
        }

        /// <summary>
        /// 绕X轴旋转,产生新的内存
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RoateXToOther<T>(double angle) where T : class, IVector3<double>
        {
            T newPoint = New<T>(this as T);
            return newPoint.RoateX<T>(angle);
        }

        /// <summary>
        /// 绕Y轴旋转,自身改变
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RoateY<T>(double angle) where T : class, IVector3<double>
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            var x = this.X;
            var z = this.Z;
            this.X = sin * z + cos * x;
            this.Z = cos * z - sin * x;
            return this as T;
        }

        /// <summary>
        /// 绕Y轴旋转,产生新的内存
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RoateYToOther<T>(double angle) where T : class, IVector3<double>
        {
            T newPoint = New<T>(this as T);
            return newPoint.RoateY<T>(angle);
        }

        /// <summary>
        /// 绕Z轴旋转,自身改变
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RoateZ<T>(double angle) where T : class, IVector3<double>
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            var x = this.X;
            var y = this.Y;
            this.X = cos * x - sin * y;
            this.Y = sin * x + cos * y;
            return this as T;
        }

        /// <summary>
        /// 绕Z轴旋转,产生新的内存
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RoateZToOther<T>(double angle) where T : class, IVector3<double>
        {
            T newPoint = New<T>(this as T);
            return newPoint.RoateZ<T>(angle);
        }

        /// <summary>
        /// 调换YZ轴
        /// </summary>
        /// <returns></returns>
        public T InvertYZ<T>() where T : class, IVector3<double>
        {
            var temp = this.Y;
            this.Y = -this.Z;
            this.Z = temp;
            return this as T;
        }

        /// <summary>
        /// 调换ZY轴,与InvertYZ配套使用
        /// </summary>
        /// <returns></returns>
        public T InvertZY<T>() where T : class, IVector3<double>
        {
            var temp = this.Z;
            this.Z = -this.Y;
            this.Y = temp;
            return this as T;
        }
        /// <summary>
        /// 向量通过欧拉角转换，自身改变
        /// </summary>
        /// <param name="eulerAngle"></param>
        /// <returns></returns>
        public T MultiplyByEulerAngle<T>(IEulerAngle<double> eulerAngle) where T : class, IVector3<double>
        {
            double sp = Math.Sin(eulerAngle.Pitch);
            double sb = Math.Sin(eulerAngle.Bank);
            double sh = Math.Sin(eulerAngle.Heading);
            double cp = Math.Cos(eulerAngle.Pitch);
            double cb = Math.Cos(eulerAngle.Bank);
            double ch = Math.Cos(eulerAngle.Heading);
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            this.X = ch * cb + sh * sp * sb * x + sb * cp * y - sh * cb + ch * sp * sb * z;
            this.Y = -ch * sb + sh * sp * cb * x + cb * cp * y + sb * sh + ch * sp * cb * z;
            this.Z = sh * cp * x - sp * y + ch * cp * z;
            return this as T;
        }

        /// <summary>
        /// 点通过矩阵偏移，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByEulerAngleToOther<T>(IEulerAngle<double> eulerAngle) where T : class, IVector3<double>
        {
            T newPoint = New<T>(this as T);
            return newPoint.MultiplyByEulerAngle<T>(eulerAngle);
        }

        /// <summary>
        /// 点通过四元数转换,根据qpq-1公式,自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public T MultiplyByQuaternion<T>(IQuaternion<double> quaternion) where T : class, IVector3<double>
        {
            var point = new QuaternionD(X, Y, Z, 0.0f);
            point = point.Cross(quaternion).Cross(quaternion.Conjugate<QuaternionD>());
            this.X = point.X;
            this.Y = point.Y;
            this.Z = point.Z;
            return this as T;
        }

        /// <summary>
        /// 点通过四元数转换,根据qpq-1公式,产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public T MultiplyByQuaternionToOther<T>(IQuaternion<double> quaternion) where T : class, IVector3<double>
        {
            T newPoint = New<T>(this as T);
            return newPoint.MultiplyByQuaternion<T>(quaternion);
        }

        /// <summary>
        /// 点通过矩阵偏移，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByMatrix<T>(IMatrix4x4<double> matrix) where T : class, IVector3<double>
        {
            if (matrix == null || matrix.IsIdentity())
            {
                return this as T;
            }
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            this.X = matrix[0, 0] * x + matrix[1, 0] * y + matrix[2, 0] * z + matrix[3, 0];
            this.Y = matrix[0, 1] * x + matrix[1, 1] * y + matrix[2, 1] * z + matrix[3, 1];
            this.Z = matrix[0, 2] * x + matrix[1, 2] * y + matrix[2, 2] * z + matrix[3, 2];
            return this as T;
        }

        /// <summary>
        /// 点通过矩阵偏移，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByMatrixToOther<T>(IMatrix4x4<double> matrix) where T : class, IVector3<double>
        {
            T newPoint = New<T>(this as T);
            return newPoint.MultiplyByMatrix<T>(matrix);
        }

        /// <summary>
        /// 点通过矩阵偏移，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByMatrix<T>(IMatrix3x3<double> matrix) where T : class, IVector3<double>
        {
            if (matrix == null || matrix.IsIdentity())
            {
                return this as T;
            }
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            this.X = matrix[0, 0] * x + matrix[1, 0] * y + matrix[2, 0] * z;
            this.Y = matrix[0, 1] * x + matrix[1, 1] * y + matrix[2, 1] * z;
            this.Z = matrix[0, 2] * x + matrix[1, 2] * y + matrix[2, 2] * z;
            return this as T;
        }

        /// <summary>
        /// 点通过矩阵偏移，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByMatrixToOther<T>(IMatrix3x3<double> matrix) where T : class, IVector3<double>
        {
            T newPoint = New<T>(this as T);
            return newPoint.MultiplyByMatrix<T>(matrix);
        }
        #endregion

        #region 转换
        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(X).Concat(BitConverter.GetBytes(Y)).Concat(BitConverter.GetBytes(Z)).ToArray();
        }

        public double[] ToArray()
        {
            return new[] { X, Y, Z };
        }

        public List<double> ToList()
        {
            return new List<double> { X, Y, Z };
        }

        public double Max()
        {
            return Math.Max(Math.Max(X, Y), Z);
        }

        public double Min()
        {
            return Math.Min(Math.Min(X, Y), Z);
        }
        #endregion

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }
        public string GetUniqueCode(double errorMeter, bool IsIgnoreZ = false)
        {
            string hashCode = ((int)Math.Round(X / errorMeter, 0)).ToString();
            hashCode +=","+ (int)Math.Round(Y / errorMeter, 0);
            if (!IsIgnoreZ)
            {
                hashCode += ","+(int)Math.Round(Z / errorMeter, 0);
            }
            return hashCode;
        }
        public override string ToString()
        {
            return ToString(",");
        }
        public string ToString(string p)
        {
            return X + p + Y + p + Z;
        }

        public bool Equals(IVector3d other)
        {
            return (X.Equals(other.X) || Math.Abs(X - other.X) < DOUBLE_DELTA) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < DOUBLE_DELTA) && (Z.Equals(other.Z) || Math.Abs(Z - other.Z) < DOUBLE_DELTA);
        }

        public bool Equals<T>(T other) where T : class, IVector3<double>
        {
            return (X.Equals(other.X) || Math.Abs(X - other.X) < DOUBLE_DELTA) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < DOUBLE_DELTA) && (Z.Equals(other.Z) || Math.Abs(Z - other.Z) < DOUBLE_DELTA);
        }

        public bool Equals<T>(T other, double buffer) where T : class, IVector3<double>
        {
            if (Math.Abs(this.X - other.X) < buffer && Math.Abs(this.Y - other.Y) < buffer && Math.Abs(this.Z - other.Z) < buffer)
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is IVector3d && Equals((IVector3d)obj);
        }
        #region 重写操作符
        public static Vector3d operator +(Vector3d left, Vector3d right)
        {
            return left.AddToOther(right);
        }

        public static Vector3d operator -(Vector3d u)
        {
            return u.NegateToOther<Vector3d>();
        }

        public static Vector3d operator -(Vector3d left, Vector3d right)
        {
            return left.MinusToOther(right);
        }

        public static Vector3d operator *(double p, Vector3d u)
        {
            return u.MultiplyToOther<Vector3d>(p);
        }

        public static Vector3d operator *(Vector3d u, double p)
        {
            return u.MultiplyToOther<Vector3d>(p);
        }

        public static Vector3d operator *(Vector3d left, Vector3d right)
        {
            return left.MultiplyToOther(right);
        }

        public static Vector3d operator /(Vector3d left, Vector3d right)
        {
            return left.DivideToOther(right);
        }

        public static Vector3d operator /(Vector3d u, double a)
        {
            return u.DivideToOther<Vector3d>(a);
        }

        public static Vector3d operator /(double a, Vector3d u)
        {
            return new Vector3d(a / u.X, a / u.Y, a / u.Z);
        }
        #endregion
    }

    public class Vector3Int
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int this[int index]
        {
            get
            {
                if (index == 0)
                    return X;
                else if (index == 1)
                    return Y;
                else
                    return Z;
            }
            set
            {
                if (index == 0)
                    X = value;
                else if (index == 1)
                    Y = value;
                else
                    Z = value;
            }
        }

        public static readonly Vector3Int Zero = new Vector3Int(0, 0, 0);
        public static readonly Vector3Int One = new Vector3Int(1, 1, 1);
        #region 构造函数
        public Vector3Int() : this(0, 0, 0) { }

        public Vector3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3Int(Vector3Int other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static Vector3Int FromString(string mbs, char p)
        {
            var ts = mbs.Split(p);
            return new Vector3Int(int.Parse(ts[0]), int.Parse(ts[1]), int.Parse(ts[2]));
        }
        #endregion



        #region 加减乘除
        /// <summary>
        /// 向量加法，自身改变
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector3Int Add(Vector3Int v2)
        {
            this.X += v2.X;
            this.Y += v2.Y;
            this.Z += v2.Z;
            return this;
        }
        /// <summary>
        /// 向量加法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector3Int AddToOther(Vector3Int v2)
        {
            Vector3Int result = new Vector3Int(this);
            return result.Add(v2);
        }
        /// <summary>
        /// 向量数加，自身改变
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector3Int Add(int k)
        {
            this.X += k;
            this.Y += k;
            this.Z += k;
            return this;
        }
        /// <summary>
        /// 向量数加,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector3Int AddToOther(int k)
        {
            Vector3Int result = new Vector3Int(this);
            return result.Add(k);
        }
        /// <summary>
        /// 向量减法，自身改变
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector3Int Minus(Vector3Int v2)
        {
            this.X -= v2.X;
            this.Y -= v2.Y;
            this.Z -= v2.Z;
            return this;
        }
        /// <summary>
        /// 向量减法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector3Int MinusToOther(Vector3Int v2)
        {
            Vector3Int result = new Vector3Int(this);
            return result.Minus(v2);
        }
        /// <summary>
        /// 向量数减，自身改变
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector3Int Minus(int k)
        {
            this.X -= k;
            this.Y -= k;
            this.Z -= k;
            return this;
        }
        /// <summary>
        /// 向量数减,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector3Int MinusToOther(int k)
        {
            Vector3Int result = new Vector3Int(this);
            return result.Minus(k);
        }
        /// <summary>
        /// 向量乘法，自身改变
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector3Int Multiply(Vector3Int v2)
        {
            this.X *= v2.X;
            this.Y *= v2.Y;
            this.Z *= v2.Z;
            return this;
        }
        /// <summary>
        /// 向量乘法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector3Int MultiplyToOther(Vector3Int v2)
        {
            Vector3Int result = new Vector3Int(this);
            return result.Multiply(v2);
        }
        /// <summary>
        /// 向量数乘，自身改变
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector3Int Multiply(int k)
        {
            this.X *= k;
            this.Y *= k;
            this.Z *= k;
            return this;
        }
        /// <summary>
        /// 向量数乘,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector3Int MultiplyToOther(int k)
        {
            Vector3Int result = new Vector3Int(this);
            return result.Multiply(k);
        }
        /// <summary>
        /// 向量除法，自身改变
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector3Int Divide(Vector3Int v2)
        {
            this.X /= v2.X;
            this.Y /= v2.Y;
            this.Z /= v2.Z;
            return this;
        }
        /// <summary>
        /// 向量除法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector3Int DivideToOther(Vector3Int v2)
        {
            Vector3Int result = new Vector3Int(this);
            return result.Divide(v2);
        }
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector3Int Divide(int k)
        {
            this.X /= k;
            this.Y /= k;
            this.Z /= k;
            return this;
        }
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector3Int">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector3Int DivideToOther(int k)
        {
            Vector3Int result = new Vector3Int(this);
            return result.Divide(k);
        }
        /// <summary>
        /// 取反，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Vector3Int Negate()
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;
            return this;
        }
        /// <summary>
        /// 取反,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Vector3Int NegateToOther()
        {
            var result = new Vector3Int();
            return result.Negate();
        }
        #endregion

        #region 点乘和叉乘
        /// <summary>
        /// 点乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public int Dot(Vector3Int v2)
        {
            return X * v2.X + Y * v2.Y + Z * v2.Z;
        }
        #endregion

        #region 转换
        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(X).Concat(BitConverter.GetBytes(Y)).Concat(BitConverter.GetBytes(Z)).ToArray();
        }

        public int[] ToArray()
        {
            return new[] { X, Y, Z };
        }

        public List<int> ToList()
        {
            return new List<int> { X, Y, Z };
        }

        public int Max()
        {
            return Math.Max(Math.Max(X, Y), Z);
        }

        public int Min()
        {
            return Math.Min(Math.Min(X, Y), Z);
        }
        #endregion

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return ToString(",");
        }
        public string ToString(string p)
        {
            return X + p + Y + p + Z;
        }

        public bool Equals(IVector3<int> other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public bool Equals<T>(T other) where T : class, IVector3<int>
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is IVector3<int> && Equals((IVector3<int>)obj);
        }

        #region 重写操作符
        public static Vector3Int operator +(Vector3Int left, Vector3Int right)
        {
            return left.AddToOther(right);
        }

        public static Vector3Int operator -(Vector3Int u)
        {
            return u.NegateToOther();
        }

        public static Vector3Int operator -(Vector3Int left, Vector3Int right)
        {
            return left.MinusToOther(right);
        }

        public static Vector3Int operator *(int p, Vector3Int u)
        {
            return u.MultiplyToOther(p);
        }

        public static Vector3Int operator *(Vector3Int u, int p)
        {
            return u.MultiplyToOther(p);
        }

        public static Vector3Int operator *(Vector3Int left, Vector3Int right)
        {
            return left.MultiplyToOther(right);
        }

        public static Vector3Int operator /(Vector3Int left, Vector3Int right)
        {
            return left.DivideToOther(right);
        }

        public static Vector3Int operator /(Vector3Int u, int a)
        {
            return u.DivideToOther(a);
        }

        public static Vector3Int operator /(int a, Vector3Int u)
        {
            return new Vector3Int(a / u.X, a / u.Y, a / u.Z);
        }
        #endregion
    }
}