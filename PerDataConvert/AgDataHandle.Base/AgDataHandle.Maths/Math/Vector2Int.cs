//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace AgCIM.Tools.Maths
//{
//    public class Vector2Int
//    {
//        public static readonly Vector2Int Zero = new Vector2Int(0, 0);
//        public static readonly Vector2Int One = new Vector2Int(1, 1);

//        public int X { get; set; }
//        public int Y { get; set; }

//        public Vector2Int() : this(0, 0) { }

//        public Vector2Int(int x, int y)
//        {
//            X = x;
//            Y = y;
//        }

//        public int this[int index]
//        {
//            get
//            {
//                if (index == 0)
//                    return X;
//                else
//                    return Y;
//            }
//            set
//            {
//                if (index == 0)
//                    X = value;
//                else 
//                    Y = value;
//            }
//        }

//        public bool Equals(Vector2Int other)
//        {
//            return X.Equals(other.X) && Y.Equals(other.Y);
//        }

//        public override bool Equals(object obj)
//        {
//            if (ReferenceEquals(null, obj)) return false;
//            return obj is Vector2Int && Equals((Vector2Int)obj);
//        }

//        public override int GetHashCode()
//        {
//            unchecked
//            {
//                var hashCode = X.GetHashCode();
//                hashCode = (hashCode * 397) ^ Y.GetHashCode();
//                return hashCode;
//            }
//        }

//        public override string ToString()
//        {
//            return ToString(",");
//        }
//        public string ToString(string p)
//        {
//            return X + p + Y;
//        }

//        public Vector2Int Clone()
//        {
//            return new Vector2Int(X, Y);
//        }

//        public int[] ToArray()
//        {
//            return new[] { X, Y};
//        }

//        public int Max()
//        {
//            return Math.Max(X, Y);
//        }

//        public int Min()
//        {
//            return Math.Min(X, Y);
//        }

//        public static Vector2Int operator +(Vector2Int left, Vector2Int right)
//        {
//            return new Vector2Int(left.X + right.X, left.Y + right.Y);
//        }

//        public static Vector2Int operator -(Vector2Int u)
//        {
//            return new Vector2Int(-u.X, -u.Y);
//        }

//        public static Vector2Int operator -(Vector2Int left, Vector2Int right)
//        {
//            return new Vector2Int(left.X - right.X, left.Y - right.Y);
//        }

//        public static Vector2Int operator *(int p, Vector2Int u)
//        {
//            return new Vector2Int(u.X*p, u.Y*p);
//        }

//        public static Vector2Int operator *(Vector2Int u, int p)
//        {
//            return new Vector2Int(u.X * p, u.Y * p);
//        }

//        public static Vector2Int operator *(Vector2Int left, Vector2Int right)
//        {
//            return new Vector2Int(left.X * right.X, left.Y * right.Y);
//        }

//        public static Vector2Int operator /(Vector2Int left, Vector2Int right)
//        {
//            return new Vector2Int(left.X / right.X, left.Y / right.Y);
//        }

//        public static Vector2Int operator /(Vector2Int u, int a)
//        {
//            return new Vector2Int(u.X / a, u.Y / a);
//        }
//    }
//}
