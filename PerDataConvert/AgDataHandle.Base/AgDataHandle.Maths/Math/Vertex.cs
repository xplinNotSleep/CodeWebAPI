using System;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 三维Mesh的顶点
    /// </summary>
    public class Vertex : Vector3
    {
        public Vector3 Normal { get; set; }
        public Vector2 UV { get; set; }
        public Color Color { get; set; }
        /// <summary>
        /// 3dtiles的BatchID,用于标记属性
        /// </summary>
        public int BatchID { get; set; } = 0;
        
        #region 构造函数
        public Vertex():this(0,0,0) { }
        public Vertex(float x, float y, float z) : base(x, y, z)
        {
        }
        public Vertex(float x, float y) : base(x, y, 0)
        {
        }
        public Vertex(string x, string y, string z)
        {
            X = float.Parse(x);
            Y = float.Parse(y);
            Z = float.Parse(z);
        }
        public Vertex(Vector3 v) {
            X = Convert.ToSingle(v.X);
            Y = Convert.ToSingle(v.Y);
            Z = Convert.ToSingle(v.Z);
        }
        #endregion

        #region 克隆
        public Vertex CloneAsVertex()
        {
            var result = new Vertex(X, Y, Z);
            result.Normal = Normal;
            result.Color = Color;
            result.UV = UV;
            result.BatchID = BatchID;
            return result;
        }
        #endregion

        #region 重写操作符
        public static Vertex operator +(Vertex left, Vertex right)
        {
            return left.AddToOther(right);
        }

        public static Vertex operator -(Vertex u)
        {
            return u.Negate<Vertex>();
        }

        public static Vertex operator -(Vertex left, Vertex right)
        {
            return left.MinusToOther(right);
        }

        public static Vertex operator *(float p, Vertex u)
        {
            return u.MultiplyToOther<Vertex>(p);
        }

        public static Vertex operator *(Vertex u, float p)
        {
            return u.MultiplyToOther<Vertex>(p);
        }

        public static Vertex operator *(Vertex left, Vertex right)
        {
            return left.MultiplyToOther(right);
        }

        public static Vertex operator /(Vertex left, Vertex right)
        {
            return left.DivideToOther(right);
        }

        public static Vertex operator /(Vertex u, float a)
        {
            return u.DivideToOther<Vertex>(a);
        }

        public static Vertex operator /(float a, Vertex u)
        {
            return new Vertex(a / u.X, a / u.Y, a / u.Z);
        }
        #endregion
    }

}
