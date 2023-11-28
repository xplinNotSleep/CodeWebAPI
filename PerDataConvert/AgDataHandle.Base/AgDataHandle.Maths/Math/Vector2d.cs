//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AgCIM.Tools.Maths
//{
//    public class Vector2d
//    {
//        public double X { get; set; }
//        public double Y { get; set; }

//        public Vector2d()
//        {
//            X = 0;
//            Y = 0;
//        }

//        public Vector2d(double x, double y)
//        {
//            X = x;
//            Y = y;
//        }

//        public static Vector2d operator +(Vector2d a, Vector2d b)
//        {
//            return new Vector2d(a.X + b.X, a.Y + b.Y);
//        }
//        public static Vector2d operator -(Vector2d a, Vector2d b)
//        {
//            return new Vector2d(a.X - b.X, a.Y - b.Y);
//        }
//        public static Vector2d operator *(Vector2d a, double multi)
//        {
//            return new Vector2d(a.X * multi, a.Y * multi);
//        }
//        public static Vector2d operator /(Vector2d a, double div)
//        {
//            return new Vector2d(a.X / div, a.Y / div);
//        }
//        public static double Dot(Vector2d left, Vector2d right)
//        {
//            return left.X * right.X + left.Y * right.Y;
//        }
//        public static double Cross(Vector2d left, Vector2d right)
//        {
//            return left.X * right.Y - left.Y * right.X;
//        }
//        /// <summary>
//        /// 余弦定理计算向量夹角，范围为0-360°，角度正方向为逆时针方向，结果表示为弧度
//        /// </summary>
//        /// <returns></returns>
//        public static double AngleBetween(Vector2d left, Vector2d right)
//        {
//            double temp1 = Dot(left, right);
//            double temp2 = left.Length() * right.Length();
//            if (temp2 == 0)
//                return 0;
//            //出现过由于双精度发生的略大于1或略小于-1的cos值，故需要截取至单精度
//            float angleCos = (float)(temp1 / temp2);
//            return Cross(left, right) > 0 ? Math.Acos(angleCos) : Math.PI * 2 - Math.Acos(angleCos);
//        }
//        public Vector2d Normalize()
//        {
//            return this / this.Length();
//        }
//        public double Length()
//        {
//            return Math.Sqrt(this.X * this.X + this.Y * this.Y);
//        }
//        public static double Distance(Vector2d left, Vector2d right)
//        {
//            return Math.Sqrt((left.X - right.X) * (left.X - right.X) + (left.Y - right.Y) * (left.Y - right.Y));
//        }
//        public static Vector2d Lerp(Vector2d start, Vector2d end, double t)
//        {
//            Vector2d result = end * t;
//            result += start * (1.0 - t);
//            return result;
//        }
//        public static bool Equals(Vector2d left, Vector2d right)
//        {
//            return Math.Abs(left.X - right.X) < 1e-8 && Math.Abs(left.Y - right.Y) < 1e-8;
//        }

//        public bool Equals(Vector2d other)
//        {
//            return (X.Equals(other.X) || Math.Abs(X - other.X) < 1e-8) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < 1e-8);
//        }

//        public override bool Equals(object obj)
//        {
//            if (ReferenceEquals(null, obj)) return false;
//            return obj is Vector2d && Equals((Vector2d)obj);
//        }

//        public override int GetHashCode()
//        {
//            unchecked
//            {
//                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
//            }
//        }
//    }
//}
