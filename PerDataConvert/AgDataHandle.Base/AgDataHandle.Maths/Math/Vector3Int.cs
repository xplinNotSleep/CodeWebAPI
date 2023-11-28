//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace AgCIM.Tools.Maths
//{
//    public class Vector3Int
//    {
//        public static readonly Vector3Int Zero = new Vector3Int(0, 0, 0);
//        public static readonly Vector3Int One = new Vector3Int(1, 1, 1);

//        public int X { get; set; }
//        public int Y { get; set; }
//        public int Z { get; set; }

//        public Vector3Int() : this(0, 0, 0) { }

//        public Vector3Int(int x, int y, int z)
//        {
//            X = x;
//            Y = y;
//            Z = z;
//        }

//        public int this[int index]
//        {
//            get
//            {
//                if (index == 0)
//                    return X;
//                else if (index == 1)
//                    return Y;
//                else
//                    return Z;
//            }
//            set
//            {
//                if (index == 0)
//                    X = value;
//                else if (index == 1)
//                    Y = value;
//                else
//                    Z = value;
//            }
//        }

//        public bool Equals(Vector3Int other)
//        {
//            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
//        }

//        public override bool Equals(object obj)
//        {
//            if (ReferenceEquals(null, obj)) return false;
//            return obj is Vector3Int && Equals((Vector3Int)obj);
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

//        public override string ToString()
//        {
//            return ToString(",");
//        }
//        public string ToString(string p)
//        {
//            return X + p + Y + p + Z;
//        }

//        public Vector3Int Clone()
//        {
//            return new Vector3Int(X, Y, Z);
//        }

//        public int[] ToArray()
//        {
//            return new[] { X, Y, Z };
//        }

//        public List<int> ToList()
//        {
//            return new List<int> { X, Y, Z };
//        }

//        public int Max()
//        {
//            return Math.Max( Math.Max(X,Y),Z);
//        }

//        public int Min()
//        {
//            return Math.Min(Math.Min(X, Y), Z);
//        }

//        /// <summary>
//        /// 只有两个索引相同的三角面
//        /// </summary>
//        /// <param name="other"></param>
//        /// <returns></returns>
//        public bool HasTwoSame(Vector3Int other)
//        {
//            if (X == other.X)
//            {
//                if (Y == other.Y)
//                {
//                    return true;
//                }
//                if (Z == other.Z)
//                {
//                    return true;
//                }
//                if (Y == other.Z)
//                {
//                    return true;
//                }
//                if (Z == other.Y)
//                {
//                    return true;
//                }
//            }
//            if (Y == other.Y)
//            {
//                if (Z == other.Z)
//                {
//                    return true;
//                }
//                if (X == other.Z)
//                {
//                    return true;
//                }
//                if (Z == other.X)
//                {
//                    return true;
//                }
//            }
//            if (Z == other.Z)
//            {
//                if (X == other.Y)
//                {
//                    return true;
//                }
//                if (Y == other.X)
//                {
//                    return true;
//                }
//            }
//            if(X== other.Y)
//            {
//                if (Y == other.Z)
//                {
//                    return true;
//                }
//                if (Y == other.X)
//                {
//                    return true;
//                }
//                if (Z == other.X)
//                {
//                    return true;
//                }
//            }
//            if (X == other.Z)
//            {
//                if (Y == other.X)
//                {
//                    return true;
//                }
//                if (Z == other.X)
//                {
//                    return true;
//                }
//                if (Z == other.Y)
//                {
//                    return true;
//                }
//            }
//            if (Y == other.Z)
//            {
//                if (Z == other.Y)
//                {
//                    return true;
//                }
//                if (Z == other.X)
//                {
//                    return true;
//                }
//            }
//            if (Y == other.X)
//            {
//                if (Z == other.Y)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        public static Vector3Int operator +(Vector3Int left, Vector3Int right)
//        {
//            return new Vector3Int(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
//        }

//        public static Vector3Int operator -(Vector3Int u)
//        {
//            return new Vector3Int(-u.X, -u.Y, -u.Z);
//        }

//        public static Vector3Int operator -(Vector3Int left, Vector3Int right)
//        {
//            return new Vector3Int(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
//        }

//        public static Vector3Int operator *(int p, Vector3Int u)
//        {
//            return new Vector3Int(u.X * p, u.Y * p, u.Z * p);
//        }

//        public static Vector3Int operator *(Vector3Int u, int p)
//        {
//            return new Vector3Int(u.X * p, u.Y * p, u.Z * p);
//        }

//        public static Vector3Int operator *(Vector3Int left, Vector3Int right)
//        {
//            return new Vector3Int(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
//        }

//        public static Vector3Int operator /(Vector3Int left, Vector3Int right)
//        {
//            return new Vector3Int(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
//        }

//        public static Vector3Int operator /(Vector3Int u, int a)
//        {
//            return new Vector3Int(u.X / a, u.Y / a, u.Z / a);
//        }
//    }
//}
