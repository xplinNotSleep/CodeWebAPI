//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace AgCIM.Tools.Maths
//{
//    public class Vector3d
//    {
//        public double X { get; set; }
//        public double Y { get; set; }
//        public double Z { get; set; }
//        public double Length()
//        {
//            return Math.Sqrt(X * X + Y * Y + Z * Z);
//        }
//        public Vector3d()
//        {
//            this.X = 0;
//            this.Y = 0;
//            this.Z = 0;
//        }

//        public Vector3d(double x, double y, double z)
//        {
//            this.X = x;
//            this.Y = y;
//            this.Z = z;
//        }
//        public Vector3d(Vector2d point)
//        {
//            this.X = point.X;
//            this.Y = point.Y;
//            this.Z = 0;
//        }

//        #region 重载操作符
//        public static Vector3d operator +(Vector3d a, Vector3d b)
//        {
//            return new Vector3d(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
//        }
//        public static Vector3d operator -(Vector3d a, Vector3d b)
//        {
//            return new Vector3d(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
//        }
//        public static Vector3d operator /(Vector3d a, double div)
//        {
//            return new Vector3d(a.X / div, a.Y / div, a.Z / div);
//        }
//        public static Vector3d operator *(Vector3d a, double multi)
//        {
//            return new Vector3d(a.X * multi, a.Y * multi, a.Z * multi);
//        }
//        public static Vector3d operator *(double multi, Vector3d a)
//        {
//            return new Vector3d(a.X * multi, a.Y * multi, a.Z * multi);
//        }
//        #endregion

//        public Vector3d Normalize()
//        {
//            double len = Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
//            if (len == 0) return this;
//            this.X /= len;
//            this.Y /= len;
//            this.Z /= len;
//            return this;
//        }
//        public Vector3d MultiplyBy(double val)
//        {
//            return new Vector3d(X * val, Y * val, Z * val);
//        }
//        public static Vector3d Cross(Vector3d v1, Vector3d v2)
//        {
//            double x = v1.Y * v2.Z - v1.Z * v2.Y;
//            double y = v1.Z * v2.X - v1.X * v2.Z;
//            double z = v1.X * v2.Y - v1.Y * v2.X;
//            return new Vector3d(x, y, z);
//        }
//        public static void Cross(ref Vector3d lhs, ref Vector3d rhs, out Vector3d result)
//        {
//            result = new Vector3d(lhs.Y * rhs.Z - lhs.Z * rhs.Y, lhs.Z * rhs.X - lhs.X * rhs.Z, lhs.X * rhs.Y - lhs.Y * rhs.X);
//        }
//        public static double Dot(Vector3d v1, Vector3d v2)
//        {
//            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
//        }
//        public bool Equals(Vector3d other)
//        {
//            return (X.Equals(other.X) || Math.Abs(X - other.X) < 1e-8) && (Y.Equals(other.Y) || Math.Abs(Y - other.Y) < 1e-8) && (Z.Equals(other.Z) || Math.Abs(Z - other.Z) < 1e-8);
//        }

//        public override bool Equals(object obj)
//        {
//            if (ReferenceEquals(null, obj)) return false;
//            return obj is Vector3d && Equals((Vector3d)obj);
//        }

//        public override int GetHashCode()
//        {
//            unchecked
//            {
//                var hashCode = X.GetHashCode();
//                hashCode = (hashCode * 397) ^ Y.GetHashCode();
//                hashCode = (hashCode * 397) ^ Z.GetHashCode();
//                return hashCode;
//            }
//        }
//    }
//}
