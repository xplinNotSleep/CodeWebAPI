using System;
using System.Collections.Generic;
using System.Linq;

namespace AgDataHandle.Maths
{
    public class Vector2 :  IVector2
    {
        private const float FLOAD_DELTA = 1E-09f;
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float this[int index]
        {
            get
            {
                if (index == 0)
                    return X;
                else
                    return Y;
            }
            set
            {
                if (index == 0)
                    X = value;
                else
                    Y = value;
            }
        }

        public static readonly Vector2 Zero = new Vector2(0f, 0f);
        public static readonly Vector2 One = new Vector2(1f, 1f);

        #region 构造函数
        public Vector2() : this(0, 0) { }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Vector2(double x, double y)
        {
            X = (float)x;
            Y = (float)y;
        }
        public Vector2(Vector2 other)
        {
            X = other.X;
            Y = other.Y;
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
            if (typeT == typeof(IVector2) || typeT == typeof(IVector2<float>))
            {
                model = new Vector3();
            }
            else if (typeT == typeof(IVector2d) || typeT == typeof(IVector2<double>))
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
                model.X = X; model.Y = Y;
            }
            else
            {
                if (newModelType == typeof(double))
                {
                    model.X = X; model.Y = Y; 
                }
                else
                {
                    model.X = (float)X; model.Y = (float)Y; 
                }
            }
            return model as T;
        }
        #endregion

        #region 距离长度方向
        /// <summary>
        /// 长度
        /// </summary>
        /// <returns></returns>
        public float Length()
        {
            return (float)Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }
        /// <summary>
        /// 归一化，自身改变
        /// </summary>
        /// <returns></returns>
        public T Normalize<T>() where T : class, IVector2<float>
        {
            var len = Length();
            if (len == 0f) return this as T;
            this.X /= len;
            this.Y /= len;
            return this as T;
        }

        /// <summary>
        /// 归一化，产生新的内存
        /// </summary>
        /// <returns></returns>
        public T NormalizeToOther<T>() where T : class, IVector2<float>
        {
            T result = new Vector2(this) as T;
            return result.Normalize<T>();
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float DistanceTo<T>(T v2) where T : class, IVector2<float>
        {
            return (float)Math.Sqrt((double)(Math.Pow(this.X - v2.X, 2) + Math.Pow(this.Y - v2.Y, 2)));
        }
        /// <summary>
        /// 距离的平方
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float DistanceTo2<T>(T v2) where T : class, IVector2<float>
        {
            return (float)(Math.Pow(this.X - v2.X, 2) + Math.Pow(this.Y - v2.Y, 2));
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pZ"></param>
        /// <returns></returns>
        public float DistanceTo(float pX, float pY)
        {
            return (float)Math.Sqrt((double)(Math.Pow(this.X - pX, 2) + Math.Pow(this.Y - pY, 2)));
        }
        #endregion

        #region 加减乘除
        /// <summary>
        /// 向量加法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Add<T>(T v2) where T : class, IVector2<float>
        {
            this.X += v2.X;
            this.Y += v2.Y;
            return this as T;
        }
        /// <summary>
        /// 向量加法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T AddToOther<T>(T v2) where T : class, IVector2<float>
        {
            T result = new Vector2(this) as T;
            return result.Add(v2);
        }
        /// <summary>
        /// 向量数加，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Add<T>(float k) where T : class, IVector2<float>
        {
            this.X += k;
            this.Y += k;
            return this as T;
        }
        /// <summary>
        /// 向量数加,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T AddToOther<T>(float k) where T : class, IVector2<float>
        {
            T result = new Vector2(this) as T;
            return result.Add<T>(k);
        }
        /// <summary>
        /// 向量减法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Minus<T>(T v2) where T : class, IVector2<float>
        {
            this.X -= v2.X;
            this.Y -= v2.Y;
            return this as T;
        }
        /// <summary>
        /// 向量减法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MinusToOther<T>(T v2) where T : class, IVector2<float>
        {
            T result = new Vector2(this) as T;
            return result.Minus(v2);
        }
        /// <summary>
        /// 向量数减，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Minus<T>(float k) where T : class, IVector2<float>
        {
            this.X -= k;
            this.Y -= k;
            return this as T;
        }
        /// <summary>
        /// 向量数减,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MinusToOther<T>(float k) where T : class, IVector2<float>
        {
            T result = new Vector2(this) as T;
            return result.Minus<T>(k);
        }
        /// <summary>
        /// 向量乘法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Multiply<T>(T v2) where T : class, IVector2<float>
        {
            this.X *= v2.X;
            this.Y *= v2.Y;
            return this as T;
        }
        /// <summary>
        /// 向量乘法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MultiplyToOther<T>(T v2) where T : class, IVector2<float>
        {
            T result = new Vector2(this) as T;
            return result.Multiply<T>(v2);
        }
        /// <summary>
        /// 向量数乘，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T Multiply<T>(float k) where T : class, IVector2<float>
        {
            this.X *= k;
            this.Y *= k;
            return this as T;
        }
        /// <summary>
        /// 向量数乘,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T MultiplyToOther<T>(float k) where T : class, IVector2<float>
        {
            T result = new Vector2(this) as T;
            return result.Multiply<T>(k);
        }
        /// <summary>
        /// 向量除法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Divide<T>(T v2) where T : class, IVector2<float>
        {
            this.X /= v2.X;
            this.Y /= v2.Y;
            return this as T;
        }
        /// <summary>
        /// 向量除法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T DivideToOther<T>(T v2) where T : class, IVector2<float>
        {
            T result = new Vector2(this) as T;
            return result.Divide<T>(v2);
        }
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T Divide<T>(float k) where T : class, IVector2<float>
        {
            this.X /= k;
            this.Y /= k;
            return this as T;
        }
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T DivideToOther<T>(float k) where T : class, IVector2<float>
        {
            T result = new Vector2(this) as T;
            return result.Divide<T>(k);
        }
        /// <summary>
        /// 取反，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Negate<T>() where T : class, IVector2<float>
        {
            this.X = -this.X;
            this.Y = -this.Y;
            return this as T;
        }
        /// <summary>
        /// 取反,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NegateToOther<T>() where T : class, IVector2<float>
        {
            T result = new Vector2(this) as T;
            return result.Negate<T>();
        }
        #endregion

        #region 点乘叉乘
        /// <summary>
        /// 点乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Vector2"></param>
        /// <returns></returns>
        public float Dot<T>(T v2) where T : class, IVector2<float>
        {
            return X * v2.X + Y * v2.Y;
        }

        /// <summary>
        /// 叉乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float Cross<T>(T v2) where T : class, IVector2<float>
        {
            return X * v2.Y - Y * v2.X;
        }
        #endregion

        #region 角度
        /// <summary>
        /// 计算角度，结果单位为度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float AngleWithDegree<T>(T v2) where T : class, IVector2<float>
        {
            var radians = Angle(v2);
            return radians * 180f / (float)Math.PI;
        }
        /// <summary>
        /// 计算角度，结果单位为弧度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float Angle<T>(T v2) where T : class, IVector2<float>
        {
            var temp1 = Dot(v2);
            var temp2 = Length() * v2.Length();
            if (temp2 == 0f)
                return 0f;
            var radian_angle = temp1 / temp2;
            return (float)Math.Acos(radian_angle);
        }
        #endregion

        #region 旋转
        /// <summary>
        /// 旋转一定角度,自身改变
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T Rotate<T>(float angle) where T : class, IVector2<float>
        {
            double x = this.X;
            double y = this.Y;
            double cos=Math.Cos(angle);
            double sin=Math.Sin(angle);
            this.X = (float)(x * cos - y * sin);
            this.Y = (float)(x * sin + y * cos);
            return this as T;
        }
        /// <summary>
        /// 旋转一定角度,产生新的内存
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RotateToOther<T>(float angle) where T : class, IVector2<float>
        {
            T result = new Vector2(this) as T;
            return result.Rotate<T>(angle);
        }
        #endregion

        #region 转换
        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(X).Concat(BitConverter.GetBytes(Y)).ToArray();
        }

        public float[] ToArray()
        {
            return new[] { X, Y };
        }

        public double[] ToDoubleArray()
        {
            return new[] { (double)X, Y };
        }

        public List<float> ToList()
        {
            return new List<float> { X, Y };
        }

        public float Max()
        {
            return Math.Max(X, Y);
        }

        public float Min()
        {
            return Math.Min(X, Y);
        }
        #endregion

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }
        public string GetUniqueCode(float errorMeter=0.000001f)
        {
            string hashCode = ((int)Math.Round(X / errorMeter, 0)).ToString() + ",";
            hashCode += (int)Math.Round(Y / errorMeter, 0) + ",";
            return hashCode;
        }
        public override string ToString()
        {
            return ToString(",");
        }
        public string ToString(string p)
        {
            return X + p + Y;
        }

        #region 相似性
        public bool Equals(IVector2 other)
        {
            return (X.Equals(other.X) || Math.Abs(X - other.X) < FLOAD_DELTA) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < FLOAD_DELTA);
        }

        public bool Equals<T>(T other) where T : class, IVector2
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return (X.Equals(other.X) || Math.Abs(X - other.X) < FLOAD_DELTA) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < FLOAD_DELTA);
        }

        public bool Equals<T>(T other, float buffer) where T : class, IVector2
        {
            if (Math.Abs(this.X - other.X) < buffer && Math.Abs(this.Y - other.Y) < buffer)
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is IVector2 && Equals((IVector2)obj);
        } 
        #endregion

        #region 重写操作符
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return left.AddToOther(right);
        }

        public static Vector2 operator -(Vector2 u)
        {
            return u.NegateToOther<Vector2>();
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return left.MinusToOther(right);
        }

        public static Vector2 operator *(float p, Vector2 u)
        {
            return u.MultiplyToOther<Vector2>(p);
        }

        public static Vector2 operator *(Vector2 u, float p)
        {
            return u.MultiplyToOther<Vector2>(p);
        }

        public static Vector2 operator *(Vector2 left, Vector2 right)
        {
            return left.MultiplyToOther(right);
        }

        public static Vector2 operator /(Vector2 left, Vector2 right)
        {
            return left.DivideToOther(right);
        }

        public static Vector2 operator /(Vector2 u, float a)
        {
            return u.DivideToOther<Vector2>(a);
        }

        public static Vector2 operator /(float a, Vector2 u)
        {
            return new Vector2(a / u.X, a / u.Y);
        }
        public static bool operator ==(Vector2 a, Vector2 u)
        {
            if (ReferenceEquals(null, a) && ReferenceEquals(null, u))
                return true;
            return a.Equals(u);
        }
        public static bool operator !=(Vector2 a, Vector2 u)
        {
            if (a == null && u == null)
                return false;
            return !a.Equals(u);
        }
        #endregion
    }

    public class Vector2d : IVector2d
    {
        private const double DOUBLE_DELTA = 1E-09f;
        public double X { get; set; }
        public double Y { get; set; }
        public double this[int index]
        {
            get
            {
                if (index == 0)
                    return X;
                else
                    return Y;
            }
            set
            {
                if (index == 0)
                    X = value;
                else
                    Y = value;
            }
        }
        public static readonly Vector2d Zero = new Vector2d(0, 0);
        public static readonly Vector2d One = new Vector2d(1, 1);
        #region 构造函数
        public Vector2d() : this(0, 0) { }

        public Vector2d(double x, double y)
        {
            X = x;
            Y = y;
        }
        public Vector2d(Vector2d other)
        {
            X = other.X;
            Y = other.Y;
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
            if (typeT == typeof(IVector2) || typeT == typeof(IVector2<float>))
            {
                model = new Vector3();
            }
            else if (typeT == typeof(IVector2d) || typeT == typeof(IVector2<double>))
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
                model.X = X; model.Y = Y;
            }
            else
            {
                if (newModelType == typeof(double))
                {
                    model.X = X; model.Y = Y;
                }
                else
                {
                    model.X = (float)X; model.Y = (float)Y;
                }
            }
            return model as T;
        }
        #endregion

        #region 距离长度方向
        /// <summary>
        /// 长度
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }
        /// <summary>
        /// 归一化，自身改变
        /// </summary>
        /// <returns></returns>
        public T Normalize<T>() where T : class, IVector2<double>
        {
            double len = Length();
            if (len == 0f) return this as T;
            this.X /= len;
            this.Y /= len;
            return this as T;
        }

        /// <summary>
        /// 归一化，产生新的内存
        /// </summary>
        /// <returns></returns>
        public T NormalizeToOther<T>() where T : class, IVector2<double>
        {
            T result = new Vector2d(this) as T;
            return result.Normalize<T>();
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double DistanceTo<T>(T v2) where T : class, IVector2<double>
        {
            return (double)Math.Sqrt((double)(Math.Pow(this.X - v2.X, 2) + Math.Pow(this.Y - v2.Y, 2)));
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pZ"></param>
        /// <returns></returns>
        public double DistanceTo(double pX, double pY)
        {
            return (double)Math.Sqrt((double)(Math.Pow(this.X - pX, 2) + Math.Pow(this.Y - pY, 2)));
        }
        #endregion

        #region 加减乘除
        /// <summary>
        /// 向量加法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Add<T>(T v2) where T : class, IVector2<double>
        {
            this.X += v2.X;
            this.Y += v2.Y;
            return this as T;
        }
        /// <summary>
        /// 向量加法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T AddToOther<T>(T v2) where T : class, IVector2<double>
        {
            T result = new Vector2d(this) as T;
            return result.Add(v2);
        }
        /// <summary>
        /// 向量数加，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Add<T>(double k) where T : class, IVector2<double>
        {
            this.X += k;
            this.Y += k;
            return this as T;
        }
        /// <summary>
        /// 向量数加,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T AddToOther<T>(double k) where T : class, IVector2<double>
        {
            T result = new Vector2d(this) as T;
            return result.Add<T>(k);
        }
        /// <summary>
        /// 向量减法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Minus<T>(T v2) where T : class, IVector2<double>
        {
            this.X -= v2.X;
            this.Y -= v2.Y;
            return this as T;
        }
        /// <summary>
        /// 向量减法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MinusToOther<T>(T v2) where T : class, IVector2<double>
        {
            T result = new Vector2d(this) as T;
            return result.Minus(v2);
        }
        /// <summary>
        /// 向量数减，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Minus<T>(double k) where T : class, IVector2<double>
        {
            this.X -= k;
            this.Y -= k;
            return this as T;
        }
        /// <summary>
        /// 向量数减,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MinusToOther<T>(double k) where T : class, IVector2<double>
        {
            T result = new Vector2d(this) as T;
            return result.Minus<T>(k);
        }
        /// <summary>
        /// 向量乘法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Multiply<T>(T v2) where T : class, IVector2<double>
        {
            this.X *= v2.X;
            this.Y *= v2.Y;
            return this as T;
        }
        /// <summary>
        /// 向量乘法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T MultiplyToOther<T>(T v2) where T : class, IVector2<double>
        {
            T result = new Vector2d(this) as T;
            return result.Multiply<T>(v2);
        }
        /// <summary>
        /// 向量数乘，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T Multiply<T>(double k) where T : class, IVector2<double>
        {
            this.X *= k;
            this.Y *= k;
            return this as T;
        }
        /// <summary>
        /// 向量数乘,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T MultiplyToOther<T>(double k) where T : class, IVector2<double>
        {
            T result = new Vector2d(this) as T;
            return result.Multiply<T>(k);
        }
        /// <summary>
        /// 向量除法，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T Divide<T>(T v2) where T : class, IVector2<double>
        {
            this.X /= v2.X;
            this.Y /= v2.Y;
            return this as T;
        }
        /// <summary>
        /// 向量除法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public T DivideToOther<T>(T v2) where T : class, IVector2<double>
        {
            T result = new Vector2d(this) as T;
            return result.Divide<T>(v2);
        }
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T Divide<T>(double k) where T : class, IVector2<double>
        {
            this.X /= k;
            this.Y /= k;
            return this as T;
        }
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public T DivideToOther<T>(double k) where T : class, IVector2<double>
        {
            T result = new Vector2d(this) as T;
            return result.Divide<T>(k);
        }
        /// <summary>
        /// 取反，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Negate<T>() where T : class, IVector2<double>
        {
            this.X = -this.X;
            this.Y = -this.Y;
            return this as T;
        }
        /// <summary>
        /// 取反,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NegateToOther<T>() where T : class, IVector2<double>
        {
            T result = new Vector2d(this) as T;
            return result.Negate<T>();
        }
        #endregion

        #region 点乘叉乘
        /// <summary>
        /// 点乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Vector2"></param>
        /// <returns></returns>
        public double Dot<T>(T v2) where T : class, IVector2<double>
        {
            return X * v2.X + Y * v2.Y;
        }

        /// <summary>
        /// 叉乘
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double Cross<T>(T v2) where T : class, IVector2<double>
        {
            return X * v2.Y - Y * v2.X;
        }
        #endregion

        #region 角度
        /// <summary>
        /// 计算角度，结果单位为度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double AngleWithDegree<T>(T v2) where T : class, IVector2<double>
        {
            double radians = Angle(v2);
            return radians * 180.0 / Math.PI;
        }
        /// <summary>
        /// 计算角度，结果单位为弧度
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public double Angle<T>(T v2) where T : class, IVector2<double>
        {
            double temp1 = Dot(v2);
            double temp2 = Length() * v2.Length();
            if (temp2 == 0f)
                return 0f;
            //radian_angle用float类型的目的是截取无效精度部分，避免cos值大于1或者小于-1
            float angleCos = (float)(temp1 / temp2);
            return Math.Acos(angleCos);
        }
        #endregion

        #region 旋转
        /// <summary>
        /// 旋转一定角度,自身改变
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T Rotate<T>(double angle) where T : class, IVector2<double>
        {
            double x = this.X;
            double y = this.Y;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            this.X = x * cos - y * sin;
            this.Y = x * sin + y * cos;
            return this as T;
        }

        /// <summary>
        /// 旋转一定角度,产生新的内存
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns></returns>
        public T RotateToOther<T>(double angle) where T : class, IVector2<double>
        {
            T result = new Vector2d(this) as T;
            return result.Rotate<T>(angle);
        }
        #endregion

        #region 转换
        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(X).Concat(BitConverter.GetBytes(Y)).ToArray();
        }

        public double[] ToArray()
        {
            return new[] { X, Y };
        }

        public List<double> ToList()
        {
            return new List<double> { X, Y };
        }

        public double Max()
        {
            return Math.Max(X, Y);
        }

        public double Min()
        {
            return Math.Min(X, Y);
        }
        #endregion

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }
        public string GetUniqueCode(double errorMeter)
        {
            string hashCode = (Math.Round(X / errorMeter, 0)).ToString() + ",";
            hashCode += Math.Round(Y / errorMeter, 0);
            return hashCode;
        }
        public override string ToString()
        {
            return ToString(",");
        }
        public string ToString(string p)
        {
            return X + p + Y;
        }

        public bool Equals(IVector2d other)
        {
            return (X.Equals(other.X) || Math.Abs(X - other.X) < DOUBLE_DELTA) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < DOUBLE_DELTA);
        }

        public bool Equals<T>(T other) where T : class, IVector2d
        {
            return (X.Equals(other.X) || Math.Abs(X - other.X) < DOUBLE_DELTA) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < DOUBLE_DELTA);
        }

        public bool Equals<T>(T other, double buffer) where T : class, IVector2d
        {
            if (Math.Abs(this.X - other.X) < buffer && Math.Abs(this.Y - other.Y) < buffer)
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is IVector2d && Equals((IVector2d)obj);
        }
        #region 重写操作符
        public static Vector2d operator +(Vector2d left, Vector2d right)
        {
            return left.AddToOther(right);
        }

        public static Vector2d operator -(Vector2d u)
        {
            return u.NegateToOther<Vector2d>();
        }

        public static Vector2d operator -(Vector2d left, Vector2d right)
        {
            return left.MinusToOther(right);
        }

        public static Vector2d operator *(double p, Vector2d u)
        {
            return u.MultiplyToOther<Vector2d>(p);
        }

        public static Vector2d operator *(Vector2d u, double p)
        {
            return u.MultiplyToOther<Vector2d>(p);
        }

        public static Vector2d operator *(Vector2d left, Vector2d right)
        {
            return left.MultiplyToOther(right);
        }

        public static Vector2d operator /(Vector2d left, Vector2d right)
        {
            return left.DivideToOther(right);
        }

        public static Vector2d operator /(Vector2d u, double a)
        {
            return u.DivideToOther<Vector2d>(a);
        }

        public static Vector2d operator /(double a, Vector2d u)
        {
            return new Vector2d(a / u.X, a / u.Y);
        }
        #endregion
    }

    public class Vector2Int
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int this[int index]
        {
            get
            {
                if (index == 0)
                    return X;
                else
                    return Y;
            }
            set
            {
                if (index == 0)
                    X = value;
                else
                    Y = value;
            }
        }
        public static readonly Vector2d Zero = new Vector2d(0, 0);
        public static readonly Vector2d One = new Vector2d(1, 1);
        #region 构造函数
        public Vector2Int() : this(0, 0) { }

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Vector2Int(Vector2Int other)
        {
            X = other.X;
            Y = other.Y;
        }
        #endregion

        #region 距离长度方向
        /// <summary>
        /// 长度
        /// </summary>
        /// <returns></returns>
        public int Length()
        {
            return (int)Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }
        /// <summary>
        /// 归一化，自身改变
        /// </summary>
        /// <returns></returns>
        public Vector2Int Normalize()
        {
            var len = Length();
            if (len == 0f) return this;
            this.X /= len;
            this.Y /= len;
            return this;
        }

        /// <summary>
        /// 归一化，产生新的内存
        /// </summary>
        /// <returns></returns>
        public Vector2Int NormalizeToOther()
        {
            Vector2Int result = new Vector2Int(this);
            return result.Normalize();
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <typeparam name="Vector2Int"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public int DistanceTo(Vector2Int v2)
        {
            return (int)Math.Sqrt((int)(Math.Pow(this.X - v2.X, 2) + Math.Pow(this.Y - v2.Y, 2)));
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pZ"></param>
        /// <returns></returns>
        public int DistanceTo(int pX, int pY)
        {
            return (int)Math.Sqrt((int)(Math.Pow(this.X - pX, 2) + Math.Pow(this.Y - pY, 2)));
        }
        #endregion

        #region 加减乘除
        /// <summary>
        /// 向量加法，自身改变
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2Int Add(Vector2Int v2)
        {
            this.X += v2.X;
            this.Y += v2.Y;
            return this;
        }
        /// <summary>
        /// 向量加法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2Int AddToOther(Vector2Int v2)
        {
            Vector2Int result = new Vector2Int(this);
            return result.Add(v2);
        }
        /// <summary>
        /// 向量数加，自身改变
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2Int Add(int k)
        {
            this.X += k;
            this.Y += k;
            return this;
        }
        /// <summary>
        /// 向量数加,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2Int AddToOther(int k)
        {
            Vector2Int result = new Vector2Int(this);
            return result.Add(k);
        }
        /// <summary>
        /// 向量减法，自身改变
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2Int Minus(Vector2Int v2)
        {
            this.X -= v2.X;
            this.Y -= v2.Y;
            return this;
        }
        /// <summary>
        /// 向量减法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2Int MinusToOther(Vector2Int v2)
        {
            Vector2Int result = new Vector2Int(this);
            return result.Minus(v2);
        }
        /// <summary>
        /// 向量数减，自身改变
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2Int Minus(int k)
        {
            this.X -= k;
            this.Y -= k;
            return this;
        }
        /// <summary>
        /// 向量数减,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2Int MinusToOther(int k)
        {
            Vector2Int result = new Vector2Int(this);
            return result.Minus(k);
        }
        /// <summary>
        /// 向量乘法，自身改变
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2Int Multiply(Vector2Int v2)
        {
            this.X *= v2.X;
            this.Y *= v2.Y;
            return this;
        }
        /// <summary>
        /// 向量乘法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2Int MultiplyToOther(Vector2Int v2)
        {
            Vector2Int result = new Vector2Int(this);
            return result.Multiply(v2);
        }
        /// <summary>
        /// 向量数乘，自身改变
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector2Int Multiply(int k)
        {
            this.X *= k;
            this.Y *= k;
            return this;
        }
        /// <summary>
        /// 向量数乘,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector2Int MultiplyToOther(int k)
        {
            Vector2Int result = new Vector2Int(this);
            return result.Multiply(k);
        }
        /// <summary>
        /// 向量除法，自身改变
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2Int Divide(Vector2Int v2)
        {
            this.X /= v2.X;
            this.Y /= v2.Y;
            return this;
        }
        /// <summary>
        /// 向量除法,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Vector2Int DivideToOther(Vector2Int v2)
        {
            Vector2Int result = new Vector2Int(this);
            return result.Divide(v2);
        }
        /// <summary>
        /// 向量数除，自身改变
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector2Int Divide(int k)
        {
            this.X /= k;
            this.Y /= k;
            return this;
        }
        /// <summary>
        /// 向量数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector2Int">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector2Int DivideToOther(int k)
        {
            Vector2Int result = new Vector2Int(this);
            return result.Divide(k);
        }
        /// <summary>
        /// 取反，自身改变
        /// </summary>
        /// <typeparam name="Vector2Int"></typeparam>
        /// <returns></returns>
        public Vector2Int Negate()
        {
            this.X = -this.X;
            this.Y = -this.Y;
            return this;
        }
        /// <summary>
        /// 取反,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="Vector2Int"></typeparam>
        /// <returns></returns>
        public Vector2Int NegateToOther()
        {
            Vector2Int result = new Vector2Int(this);
            return result.Negate();
        }
        #endregion

        #region 点乘叉乘
        /// <summary>
        /// 点乘
        /// </summary>
        /// <typeparam name="Vector2Int"></typeparam>
        /// <param name="Vector2"></param>
        /// <returns></returns>
        public int Dot(Vector2Int v2)
        {
            return X * v2.X + Y * v2.Y;
        }

        /// <summary>
        /// 叉乘
        /// </summary>
        /// <typeparam name="Vector2Int"></typeparam>
        /// <param name="v2"></param>
        /// <returns></returns>
        public int Cross(Vector2Int v2)
        {
            return X * v2.Y - Y * v2.X;
        }
        #endregion

        #region 转换
        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(X).Concat(BitConverter.GetBytes(Y)).ToArray();
        }

        public int[] ToArray()
        {
            return new[] { X, Y };
        }

        public List<int> ToList()
        {
            return new List<int> { X, Y };
        }

        public int Max()
        {
            return Math.Max(X, Y);
        }

        public int Min()
        {
            return Math.Min(X, Y);
        }
        #endregion

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }
        public override string ToString()
        {
            return ToString(",");
        }
        public string ToString(string p)
        {
            return X + p + Y;
        }

        public bool Equals(IVector2<int> other)
        {
            return X == other.X && Y == other.Y;
        }

        public bool Equals(Vector2Int other)
        {
            return X == other.X && Y == other.Y;
        }

        public string GetUniqueCode(int errorMeter)
        {
            throw new NotImplementedException();
        }

        #region 重写操作符
        public static Vector2Int operator +(Vector2Int left, Vector2Int right)
        {
            return left.AddToOther(right);
        }

        public static Vector2Int operator -(Vector2Int u)
        {
            return u.NegateToOther();
        }

        public static Vector2Int operator -(Vector2Int left, Vector2Int right)
        {
            return left.MinusToOther(right);
        }

        public static Vector2Int operator *(int p, Vector2Int u)
        {
            return u.MultiplyToOther(p);
        }

        public static Vector2Int operator *(Vector2Int u, int p)
        {
            return u.MultiplyToOther(p);
        }

        public static Vector2Int operator *(Vector2Int left, Vector2Int right)
        {
            return left.MultiplyToOther(right);
        }

        public static Vector2Int operator /(Vector2Int left, Vector2Int right)
        {
            return left.DivideToOther(right);
        }

        public static Vector2Int operator /(Vector2Int u, int a)
        {
            return u.DivideToOther(a);
        }

        public static Vector2Int operator /(int a, Vector2Int u)
        {
            return new Vector2Int(a / u.X, a / u.Y);
        }
        #endregion
    }
}
