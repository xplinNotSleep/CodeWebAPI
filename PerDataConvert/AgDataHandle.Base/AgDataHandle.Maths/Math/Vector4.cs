using System;
using System.Collections.Generic;
using System.Linq;

namespace AgDataHandle.Maths
{
    public class Vector4 : IVector4
    {
        private const float FLOAD_DELTA = 1E-09f;
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public static readonly Vector4 Zero = new Vector4(0f, 0f, 0f,0f);
        public static readonly Vector4 One = new Vector4(1f, 1f, 1f, 1f);
        #region 构造函数
        public Vector4() : this(0, 0, 0,0) { }

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W=w;
        }
        public Vector4(Vector4 other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
            W=other.W;
        }

        public static T New<T>() where T : class, IVector4<float>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IVector4)|| typeT == typeof(IVector4<float>))
            {
                model = new Vector4() as T;
            }
            else if (typeT == typeof(IQuaternion) || typeT == typeof(IQuaternion<float>))
            {
                model = new Quaternion() as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            return model;
        }

        public static T New<T>(float x, float y, float z, float w) where T : class, IVector4<float>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IVector4) || typeT == typeof(IVector4<float>))
            {
                model = new Vector4() as T;
            }
            else if (typeT == typeof(IQuaternion) || typeT == typeof(IQuaternion<float>))
            {
                model = new Quaternion() as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            model.X = x;
            model.Y = y;
            model.Z = z;
            model.W = w;
            return model;
        }

        public static T New<T>(T other) where T : class, IVector4<float>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IVector4) || typeT == typeof(IVector4<float>))
            {
                model = new Vector4() as T;
            }
            else if (typeT == typeof(IQuaternion) || typeT == typeof(IQuaternion<float>))
            {
                model = new Quaternion() as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            model.X = other.X;
            model.Y = other.Y;
            model.Z = other.Z;
            model.W = other.W;
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
            if (typeT == typeof(IQuaternion)|| typeT == typeof(IQuaternion<float>))
            {
                model = new Quaternion();
            }
            else if (typeT == typeof(IQuaternionD) || typeT == typeof(IQuaternion<double>))
            {
                model = new QuaternionD();
            }
            else if (typeT == typeof(IVector4) || typeT == typeof(IVector4<float>))
            {
                model = new Vector4();
            }
            else if (typeT == typeof(IVector4d) || typeT == typeof(IVector4<double>))
            {
                model = new Vector4d();
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            var newModelType = model.X.GetType();
            var IsSame = this.X.GetType() == newModelType;
            if (IsSame)
            {
                model.X = X; model.Y = Y; model.Z = Z; model.W = W;
            }
            else
            {
                if (newModelType == typeof(double))
                {
                    model.X = X; model.Y = Y; model.Z = Z; model.W = W;
                }
                else
                {
                    model.X = (float)X; model.Y = (float)Y; model.Z = (float)Z; model.W = (float)W;
                }
            }
            return model as T;
        }
        #endregion

        public float this[int index]
        {
            get
            {
                if (index == 0)
                    return X;
                else if (index == 1)
                    return Y;
                else if (index == 2)
                    return Z;
                else
                    return W;
            }
            set
            {
                if (index == 0)
                    X = value;
                else if (index == 1)
                    Y = value;
                else if (index == 2)
                    Z = value;
                else
                    W = value;
            }
        }

        #region 距离长度方向
        /// <summary>
        /// 长度
        /// </summary>
        /// <returns></returns>
        public float Length()
        {
            return (float)Math.Sqrt(this.X * this.X+this.Y * this.Y+ this.Z * this.Z+this.W * this.W);
        }
        /// <summary>
        /// 归一化，自身改变
        /// </summary>
        /// <returns></returns>
        public T Normalize<T>() where T : class, IVector4<float>
        {
            var len = Length();
            if (len == 0f) return this as T;
            this.X /=  len;
            this.Y /=  len;
            this.Z /=  len;
            this.W /=  len;
            return this as T;
        }

        /// <summary>
        /// 归一化，产生新的内存
        /// </summary>
        /// <returns></returns>
        public T NormalizeToOther<T>() where T : class, IVector4<float>
        {
            T result = New(this as T);
            return result.Normalize<T>();
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float DistanceTo<T>(T v2) where T : class, IVector4<float>
        {
            return (float)Math.Sqrt((double)(Math.Pow(this.X- v2.X,2)+ Math.Pow(this.Y - v2.Y, 2)+ Math.Pow(this.Z - v2.Z, 2)+ Math.Pow(this.W - v2.W, 2)));
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pZ"></param>
        /// <param name="pW"></param>
        /// <returns></returns>
        public float DistanceTo(float pX, float pY, float pZ, float pW)
        {
            return (float)Math.Sqrt((double)(Math.Pow(this.X - pX, 2) + Math.Pow(this.Y - pY, 2) + Math.Pow(this.Z - pZ, 2) + Math.Pow(this.W - pW, 2)));
        }
        #endregion

        #region 加减乘除
        /// <summary>
        /// 向量加法，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Add<T>(T v2) where T : class, IVector4<float>
        {
            this.X += v2.X;
            this.Y += v2.Y;
            this.Z += v2.Z;
            this.W += v2.W;
            return this as T;
        }
        /// <summary>
        /// 向量加法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T AddToOther<T>(T v2) where T : class, IVector4<float>
        {
            T result = New(this as T);
            return result.Add(v2);
        }
        /// <summary>
        /// 向量数加，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Add<T>(float k) where T : class, IVector4<float>
        {
            this.X +=  k;
            this.Y +=  k;
            this.Z +=  k;
            this.W +=  k;
            return this as T;
        }
        /// <summary>
        /// 向量数加,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T AddToOther<T>(float k) where T : class, IVector4<float>
        {
            T result = New(this as T);
            return result.Add<T>(k);
        }
        /// <summary>
        /// 向量减法，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Minus<T>(T v2) where T : class, IVector4<float>
        {
            this.X -=v2.X;
            this.Y -=v2.Y;
            this.Z -=v2.Z;
            this.W -=v2.W;
            return this as T;
        }
        /// <summary>
        /// 向量减法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MinusToOther<T>(T v2) where T : class, IVector4<float>
        {
            T result = New(this as T);
            return result.Minus(v2);
        }
        /// <summary>
        /// 向量数减，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Minus<T>(float k) where T : class, IVector4<float>
        {
            this.X -=  k;
            this.Y -=  k;
            this.Z -=  k;
            this.W -=  k;
            return this as T;
        }
        /// <summary>
        /// 向量数减,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MinusToOther<T>(float k) where T : class, IVector4<float>
        {
            T result = New(this as T);
            return result.Minus<T>(k);
        }
        /// <summary>
        /// 向量乘法，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Multiply<T>(T v2) where T : class, IVector4<float>
        {
            this.X *= v2.X;
            this.Y *= v2.Y;
            this.Z *= v2.Z;
            this.W *= v2.W;
            return this as T;
        }
        /// <summary>
        /// 向量乘法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MultiplyToOther<T>(T v2) where T : class, IVector4<float>
        {
            T result = New(this as T);
            return result.Multiply<T>(v2);
        }
        /// <summary>
        /// 向量数乘，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T Multiply<T>(float k) where T : class, IVector4<float>
        {
            this.X *=k;
            this.Y *=k;
            this.Z *=k;
            this.W *=k;
            return this as T;
        }
        /// <summary>
        /// 向量数乘,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T MultiplyToOther<T>(float k) where T : class, IVector4<float>
        {
            T result = New(this as T);
            return result.Multiply<T>(k);
        }
        /// <summary>
        /// 向量除法，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Divide<T>(T v2) where T : class, IVector4<float>
        {
            this.X /=v2.X;
            this.Y /=v2.Y;
            this.Z /=v2.Z;
            this.W /=v2.W;
            return this as T;
        }
        /// <summary>
        /// 向量除法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T DivideToOther<T>(T v2) where T : class, IVector4<float>
        {
            T result = New(this as T);
            return result.Divide<T>(v2);
        }
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T Divide<T>(float k) where T : class, IVector4<float>
        {
            this.X/=  k;
            this.Y/=  k;
            this.Z/=  k;
            this.W/=  k;
            return this as T;
        }
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T DivideToOther<T>(float k) where T : class, IVector4<float>
        {
            T result = New(this as T);
            return result.Divide<T>(k);
        }
        /// <summary>
        /// 取反，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Negate<T>() where T : class, IVector4<float>
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;
            this.W = -this.W;
            return this as T;
        }
        /// <summary>
        /// 取反,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NegateToOther<T>() where T : class, IVector4<float>
        {
            T result = New(this as T);
            return result.Negate<T>();
        }
        #endregion

        #region 点乘和叉乘
        /// <summary>
        /// 点乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Vector4"></param>
        /// <returns></returns>
        public float Dot<T>(T v2) where T : class, IVector4<float>
        {
            return X * v2.X + Y * v2.Y + Z * v2.Z + W * v2.W;
        }

        /// <summary>
        /// 叉乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector4"></param>
        /// <returns></returns>
        public T Cross<T>(T right) where T : class, IVector4<float>
        {
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            var w = this.W;
            this.W = w * right.W - x * right.X - y * right.Y - z * right.Z;
            this.X = w * right.X + x * right.W +y * right.Z - Z * right.Y;
            this.Y = w * right.Y + y * right.W + z * right.X - x * right.Z;
            this.Z = w * right.Z + z * right.W + x * right.Y - y * right.X;
            return this as T;
        }

        /// <summary>
        /// 叉乘，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="right"></param>
        /// <returns></returns>
        public T CrossToOther<T>(T right) where T : class, IVector4<float>
        {
            T result = New(this as T);
            return result.Cross<T>(right);
        }
        #endregion

        #region 偏移
        /// <summary>
        /// 点通过矩阵偏移，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByMatrix<T>(IMatrix4x4<float> matrix) where T : class, IVector4<float>
        {
            if (matrix == null || matrix.IsIdentity())
            {
                return this as T;
            }
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            var w = this.W;
            this.X = matrix[0, 0] * x + matrix[1, 0] * y + matrix[2, 0] * z + matrix[3, 0] * w;
            this.Y = matrix[0, 1] * x + matrix[1, 1] * y + matrix[2, 1] * z + matrix[3, 1] * w;
            this.Z = matrix[0, 2] * x + matrix[1, 2] * y + matrix[2, 2] * z + matrix[3, 2] * w;
            this.W = matrix[0, 3] * x + matrix[1, 3] * y + matrix[2, 3] * z + matrix[3, 3] * w;
            return this as T;
        }

        /// <summary>
        /// 点通过矩阵偏移，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByMatrixToOther<T>(IMatrix4x4<float> matrix) where T : class, IVector4<float>
        {
            T newPoint = New<T>(this as T);
            return newPoint.MultiplyByMatrix<T>(matrix);
        }

        /// <summary>
        /// 点通过四元数转换,根据qpq-1公式,自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public T MultiplyByQuaternion<T>(IQuaternion<float> quaternion) where T : class, IVector4<float>
        {
            this.Cross(quaternion).Cross(quaternion.Conjugate<Quaternion>());
            return this as T;
        }

        /// <summary>
        /// 点通过四元数转换,根据qpq-1公式,产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public T MultiplyByQuaternionToOther<T>(IQuaternion<float> quaternion) where T : class, IVector4<float>
        {
            T newPoint = New<T>(this as T);
            return newPoint.MultiplyByQuaternion<T>(quaternion);
        }
        #endregion

        #region 角度
        /// <summary>
        /// 计算角度，结果单位为度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float AngleWithDegree<T>(T v2) where T : class, IVector4<float>
        {
            var radians = Angle(v2);
            return radians* 180f/ (float)Math.PI;
        }
        /// <summary>
        /// 计算角度，结果单位为弧度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float Angle<T>(T v2) where T : class, IVector4<float>
        {
            var temp1 = Dot(v2);
            var temp2 = Length()* v2.Length();
            if (temp2== 0f)
                return 0f;
            var radian_angle = temp1/ temp2;
            return (float)Math.Acos(radian_angle);
        }

        #endregion

        #region 转换
        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(X).Concat(BitConverter.GetBytes(Y)).Concat(BitConverter.GetBytes(Z)).Concat(BitConverter.GetBytes(W)).ToArray();
        }
        public float[] ToArray()
        {
            return new[] { X, Y, Z, W };
        }

        public List<float> ToList()
        {
            return new List<float> { X, Y, Z, W };
        }

        public float Max()
        {
            return Math.Max(Math.Max(Math.Max(X, Y), Z), W);
        }

        public float Min()
        {
            return Math.Min(Math.Min(Math.Min(X, Y), Z), W);
        }
        #endregion

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }
        public string GetUniqueCode(float errorMeter)
        {
            string hashCode = (Math.Round(X / errorMeter, 0)).ToString() + ",";
            hashCode += Math.Round(Y / errorMeter, 0) + ",";
            hashCode += Math.Round(Z / errorMeter, 0);
            hashCode += Math.Round(W / errorMeter, 0);
            return hashCode;
        }
        public override string ToString()
        {
            return ToString(",");
        }
        public string ToString(string p)
        {
            return X + p + Y + p + Z + p + W;
        }

        public bool Equals(IVector4<float> other)
        {
            return (X.Equals(other.X) || Math.Abs(X - other.X) < FLOAD_DELTA) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < FLOAD_DELTA) && (Z.Equals(other.Z) || Math.Abs(Z - other.Z) < FLOAD_DELTA);
        }

        public bool Equals<T>(T other) where T : class, IVector4<float>
        {
            return (X.Equals(other.X) || Math.Abs(X - other.X) < FLOAD_DELTA) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < FLOAD_DELTA) && (Z.Equals(other.Z) || Math.Abs(Z - other.Z) < FLOAD_DELTA);
        }

        public bool Equals<T>(T other, float buffer) where T : class, IVector4<float>
        {
            return Math.Abs(this.X - other.X) < buffer && Math.Abs(this.Y - other.Y) < buffer && Math.Abs(this.Z - other.Z) < buffer && Math.Abs(this.W - other.W) < buffer;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is IVector4<float> && Equals((IVector4<float>)obj);
        }

        #region 重写操作符
        public static Vector4 operator +(Vector4 left, Vector4 right)
        {
            return left.AddToOther(right);
        }

        public static Vector4 operator -(Vector4 u)
        {
            return u.NegateToOther<Vector4>();
        }

        public static Vector4 operator -(Vector4 left, Vector4 right)
        {
            return left.MinusToOther(right);
        }

        public static Vector4 operator *(float p, Vector4 u)
        {
            return u.MultiplyToOther<Vector4>(p);
        }

        public static Vector4 operator *(Vector4 u, float p)
        {
            return u.MultiplyToOther<Vector4>(p);
        }

        public static Vector4 operator *(Vector4 left, Vector4 right)
        {
            return left.MultiplyToOther(right);
        }

        public static Vector4 operator /(Vector4 left, Vector4 right)
        {
            return left.DivideToOther(right);
        }

        public static Vector4 operator /(Vector4 u, float a)
        {
            return u.DivideToOther<Vector4>(a);
        }

        public static Vector4 operator /(float a, Vector4 u)
        {
            return new Vector4(a / u.X, a / u.Y, a / u.Z, a / u.W);
        }
        #endregion
    }

    public class Vector4d : IVector4d
    {
        private const double DOUBLE_DELTA = 1E-15;
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }

        public static readonly Vector4d Zero = new Vector4d(0, 0, 0, 0);
        public static readonly Vector4d One = new Vector4d(1, 1, 1, 1);
        #region 构造函数
        public Vector4d() : this(0, 0, 0, 0) { }

        public Vector4d(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        public Vector4d(Vector4d other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
            W = other.W;
        }

        public static T New<T>() where T : class, IVector4<double>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IVector4d)|| typeT == typeof(IVector4<double>))
            {
                model = new Vector4d() as T;
            }
            else if (typeT == typeof(IQuaternionD)|typeT == typeof(IQuaternion<double>))
            {
                model = new QuaternionD() as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            return model;
        }

        public static T New<T>(double x, double y, double z, double w) where T : class, IVector4<double>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IVector4d) || typeT == typeof(IVector4<double>))
            {
                model = new Vector4d() as T;
            }
            else if (typeT == typeof(IQuaternionD) | typeT == typeof(IQuaternion<double>))
            {
                model = new QuaternionD() as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            model.X = x;
            model.Y = y;
            model.Z = z;
            model.W = w;
            return model;
        }

        public static T New<T>(T other) where T : class, IVector4<double>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IVector4d) || typeT == typeof(IVector4<double>))
            {
                model = new Vector4d() as T;
            }
            else if (typeT == typeof(IQuaternionD) | typeT == typeof(IQuaternion<double>))
            {
                model = new QuaternionD() as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            model.X = other.X;
            model.Y = other.Y;
            model.Z = other.Z;
            model.W = other.W;
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
            if (typeT == typeof(IQuaternion) || typeT == typeof(IQuaternion<float>))
            {
                model = new Quaternion();
            }
            else if (typeT == typeof(IQuaternionD) || typeT == typeof(IQuaternion<double>))
            {
                model = new QuaternionD();
            }
            else if (typeT == typeof(IVector4) || typeT == typeof(IVector4<float>))
            {
                model = new Vector4();
            }
            else if (typeT == typeof(IVector4d) || typeT == typeof(IVector4<double>))
            {
                model = new Vector4d();
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            var newModelType = model.X.GetType();
            var IsSame = this.X.GetType() == newModelType;
            if (IsSame)
            {
                model.X = X; model.Y = Y; model.Z = Z; model.W = W;
            }
            else
            {
                if (newModelType == typeof(double))
                {
                    model.X = X; model.Y = Y; model.Z = Z; model.W = W;
                }
                else
                {
                    model.X = (float)X; model.Y = (float)Y; model.Z = (float)Z; model.W = (float)W;
                }
            }
            return model as T;
        }
        #endregion

        public double this[int index]
        {
            get
            {
                if (index == 0)
                    return X;
                else if (index == 1)
                    return Y;
                else if (index == 2)
                    return Z;
                else
                    return W;
            }
            set
            {
                if (index == 0)
                    X = value;
                else if (index == 1)
                    Y = value;
                else if (index == 2)
                    Z = value;
                else
                    W = value;
            }
        }

        #region 距离长度方向
        /// <summary>
        /// 长度
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W);
        }
        /// <summary>
        /// 归一化，自身改变
        /// </summary>
        /// <returns></returns>
        public T Normalize<T>() where T : class, IVector4<double>
        {
            var len = Length();
            if (len == 0f) return this as T;
            this.X /= len;
            this.Y /= len;
            this.Z /= len;
            this.W /= len;
            return this as T;
        }

        /// <summary>
        /// 归一化，产生新的内存
        /// </summary>
        /// <returns></returns>
        public T NormalizeToOther<T>() where T : class, IVector4<double>
        {
            T result = New(this as T);
            return result.Normalize<T>();
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double DistanceTo<T>(T v2) where T : class, IVector4<double>
        {
            return Math.Sqrt(Math.Pow(this.X - v2.X, 2) + Math.Pow(this.Y - v2.Y, 2) + Math.Pow(this.Z - v2.Z, 2) + Math.Pow(this.W - v2.W, 2));
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pZ"></param>
        /// <param name="pW"></param>
        /// <returns></returns>
        public double DistanceTo(double pX, double pY, double pZ, double pW)
        {
            return Math.Sqrt(Math.Pow(this.X - pX, 2) + Math.Pow(this.Y - pY, 2) + Math.Pow(this.Z - pZ, 2) + Math.Pow(this.W - pW, 2));
        }
        #endregion

        #region 加减乘除
        /// <summary>
        /// 向量加法，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Add<T>(T v2) where T : class, IVector4<double>
        {
            this.X += v2.X;
            this.Y += v2.Y;
            this.Z += v2.Z;
            this.W += v2.W;
            return this as T;
        }
        /// <summary>
        /// 向量加法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T AddToOther<T>(T v2) where T : class, IVector4<double>
        {
            T result = New(this as T);
            return result.Add(v2);
        }
        /// <summary>
        /// 向量数加，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Add<T>(double k) where T : class, IVector4<double>
        {
            this.X += k;
            this.Y += k;
            this.Z += k;
            this.W += k;
            return this as T;
        }
        /// <summary>
        /// 向量数加,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T AddToOther<T>(double k) where T : class, IVector4<double>
        {
            T result = New(this as T);
            return result.Add<T>(k);
        }
        /// <summary>
        /// 向量减法，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Minus<T>(T v2) where T : class, IVector4<double>
        {
            this.X -= v2.X;
            this.Y -= v2.Y;
            this.Z -= v2.Z;
            this.W -= v2.W;
            return this as T;
        }
        /// <summary>
        /// 向量减法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MinusToOther<T>(T v2) where T : class, IVector4<double>
        {
            T result = New(this as T);
            return result.Minus(v2);
        }
        /// <summary>
        /// 向量数减，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Minus<T>(double k) where T : class, IVector4<double>
        {
            this.X -= k;
            this.Y -= k;
            this.Z -= k;
            this.W -= k;
            return this as T;
        }
        /// <summary>
        /// 向量数减,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MinusToOther<T>(double k) where T : class, IVector4<double>
        {
            T result = New(this as T);
            return result.Minus<T>(k);
        }
        /// <summary>
        /// 向量乘法，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Multiply<T>(T v2) where T : class, IVector4<double>
        {
            this.X *= v2.X;
            this.Y *= v2.Y;
            this.Z *= v2.Z;
            this.W *= v2.W;
            return this as T;
        }
        /// <summary>
        /// 向量乘法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MultiplyToOther<T>(T v2) where T : class, IVector4<double>
        {
            T result = New(this as T);
            return result.Multiply<T>(v2);
        }
        /// <summary>
        /// 向量数乘，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T Multiply<T>(double k) where T : class, IVector4<double>
        {
            this.X *= k;
            this.Y *= k;
            this.Z *= k;
            this.W *= k;
            return this as T;
        }
        /// <summary>
        /// 向量数乘,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T MultiplyToOther<T>(double k) where T : class, IVector4<double>
        {
            T result = New(this as T);
            return result.Multiply<T>(k);
        }
        /// <summary>
        /// 向量除法，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Divide<T>(T v2) where T : class, IVector4<double>
        {
            this.X /= v2.X;
            this.Y /= v2.Y;
            this.Z /= v2.Z;
            this.W /= v2.W;
            return this as T;
        }
        /// <summary>
        /// 向量除法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T DivideToOther<T>(T v2) where T : class, IVector4<double>
        {
            T result = New(this as T);
            return result.Divide<T>(v2);
        }
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T Divide<T>(double k) where T : class, IVector4<double>
        {
            this.X /= k;
            this.Y /= k;
            this.Z /= k;
            this.W /= k;
            return this as T;
        }
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixT</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T DivideToOther<T>(double k) where T : class, IVector4<double>
        {
            T result = New(this as T);
            return result.Divide<T>(k);
        }
        /// <summary>
        /// 取反，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Negate<T>() where T : class, IVector4<double>
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;
            this.W = -this.W;
            return this as T;
        }
        /// <summary>
        /// 取反,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NegateToOther<T>() where T : class, IVector4<double>
        {
            T result = New(this as T);
            return result.Negate<T>();
        }
        #endregion

        #region 点乘和叉乘
        /// <summary>
        /// 点乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Vector4"></param>
        /// <returns></returns>
        public double Dot<T>(T v2) where T : class, IVector4<double>
        {
            return X * v2.X + Y * v2.Y + Z * v2.Z + W * v2.W;
        }

        /// <summary>
        /// 叉乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector4"></param>
        /// <returns></returns>
        public T Cross<T>(T right) where T : class, IVector4<double>
        {
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            var w = this.W;
            this.X = w * right.X + x * right.W + y * right.Z - z * right.Y;
            this.Y = w * right.Y - x * right.Z + y * right.W + z * right.X;
            this.Z = w * right.Z + x * right.Y - y * right.X + z * right.W;
            this.W = w * right.W - x * right.X - y * right.Y - z * right.Z;
            return this as T;
        }

        /// <summary>
        /// 叉乘，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="right"></param>
        /// <returns></returns>
        public T CrossToOther<T>(T right) where T : class, IVector4<double>
        {
            T result = New(this as T);
            return result.Cross<T>(right);
        }
        #endregion

        #region 偏移
        /// <summary>
        /// 点通过矩阵偏移，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByMatrix<T>(IMatrix4x4<double> matrix) where T : class, IVector4<double>
        {
            if (matrix == null || matrix.IsIdentity())
            {
                return this as T;
            }
            var x = this.X;
            var y = this.Y;
            var z = this.Z;
            var w = this.W;
            this.X = matrix[0, 0] * x + matrix[1, 0] * y + matrix[2, 0] * z + matrix[3, 0] * w;
            this.Y = matrix[0, 1] * x + matrix[1, 1] * y + matrix[2, 1] * z + matrix[3, 1] * w;
            this.Z = matrix[0, 2] * x + matrix[1, 2] * y + matrix[2, 2] * z + matrix[3, 2] * w;
            this.W = matrix[0, 3] * x + matrix[1, 3] * y + matrix[2, 3] * z + matrix[3, 3] * w;
            return this as T;
        }

        /// <summary>
        /// 点通过矩阵偏移，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public T MultiplyByMatrixToOther<T>(IMatrix4x4<double> matrix) where T : class, IVector4<double>
        {
            T newPoint = New<T>(this as T);
            return newPoint.MultiplyByMatrix<T>(matrix);
        }

        /// <summary>
        /// 点通过四元数转换,根据qpq-1公式,自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public T MultiplyByQuaternion<T>(IQuaternion<double> quaternion) where T : class, IVector4<double>
        {
            this.Cross(quaternion).Cross(quaternion.Conjugate<QuaternionD>());
            return this as T;
        }

        /// <summary>
        /// 点通过四元数转换,根据qpq-1公式,产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public T MultiplyByQuaternionToOther<T>(IQuaternion<double> quaternion) where T : class, IVector4<double>
        {
            T newPoint = New<T>(this as T);
            return newPoint.MultiplyByQuaternion<T>(quaternion);
        }
        #endregion

        #region 角度
        /// <summary>
        /// 计算角度，结果单位为度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double AngleWithDegree<T>(T v2) where T : class, IVector4<double>
        {
            var radians = Angle(v2);
            return radians * 180f / Math.PI;
        }
        /// <summary>
        /// 计算角度，结果单位为弧度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double Angle<T>(T v2) where T : class, IVector4<double>
        {
            var temp1 = Dot(v2);
            var temp2 = Length() * v2.Length();
            if (temp2 == 0f)
                return 0f;
            var radian_angle = temp1 / temp2;
            return Math.Acos(radian_angle);
        }

        #endregion

        #region 转换
        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(X).Concat(BitConverter.GetBytes(Y)).Concat(BitConverter.GetBytes(Z)).Concat(BitConverter.GetBytes(W)).ToArray();
        }
        public double[] ToArray()
        {
            return new[] { X, Y, Z, W };
        }

        public List<double> ToList()
        {
            return new List<double> { X, Y, Z, W };
        }

        public double Max()
        {
            return Math.Max(Math.Max(Math.Max(X, Y), Z), W);
        }

        public double Min()
        {
            return Math.Min(Math.Min(Math.Min(X, Y), Z), W);
        }
        #endregion

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }
        public string GetUniqueCode(double errorMeter)
        {
            string hashCode = (Math.Round(X / errorMeter, 0)).ToString() + ",";
            hashCode += Math.Round(Y / errorMeter, 0) + ",";
            hashCode += Math.Round(Z / errorMeter, 0);
            hashCode += Math.Round(W / errorMeter, 0);
            return hashCode;
        }
        public override string ToString()
        {
            return ToString(",");
        }
        public string ToString(string p)
        {
            return X + p + Y + p + Z + p + W;
        }

        public bool Equals(IVector4<double> other)
        {
            return (X.Equals(other.X) || Math.Abs(X - other.X) < DOUBLE_DELTA) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < DOUBLE_DELTA) && (Z.Equals(other.Z) || Math.Abs(Z - other.Z) < DOUBLE_DELTA);
        }

        public bool Equals<T>(T other) where T : class, IVector4<double>
        {
            return (X.Equals(other.X) || Math.Abs(X - other.X) < DOUBLE_DELTA) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < DOUBLE_DELTA) && (Z.Equals(other.Z) || Math.Abs(Z - other.Z) < DOUBLE_DELTA);
        }

        public bool Equals<T>(T other, double buffer) where T : class, IVector4<double>
        {
            return Math.Abs(this.X - other.X) < buffer && Math.Abs(this.Y - other.Y) < buffer && Math.Abs(this.Z - other.Z) < buffer && Math.Abs(this.W - other.W) < buffer;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is IVector4<double> && Equals((IVector4<double>)obj);
        }

        #region 重写操作符
        public static Vector4d operator +(Vector4d left, Vector4d right)
        {
            return left.AddToOther(right);
        }

        public static Vector4d operator -(Vector4d u)
        {
            return u.NegateToOther<Vector4d>();
        }

        public static Vector4d operator -(Vector4d left, Vector4d right)
        {
            return left.MinusToOther(right);
        }

        public static Vector4d operator *(double p, Vector4d u)
        {
            return u.MultiplyToOther<Vector4d>(p);
        }

        public static Vector4d operator *(Vector4d u, double p)
        {
            return u.MultiplyToOther<Vector4d>(p);
        }

        public static Vector4d operator *(Vector4d left, Vector4d right)
        {
            return left.MultiplyToOther(right);
        }

        public static Vector4d operator /(Vector4d left, Vector4d right)
        {
            return left.DivideToOther(right);
        }

        public static Vector4d operator /(Vector4d u, double a)
        {
            return u.DivideToOther<Vector4d>(a);
        }

        public static Vector4d operator /(double a, Vector4d u)
        {
            return new Vector4d(a / u.X, a / u.Y, a / u.Z, a / u.W);
        }
        #endregion
    }
}


