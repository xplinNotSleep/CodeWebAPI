//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;

//namespace AgCIM.Tools.Maths
//{
//    public class Vector4d : IEquatable<Vector4d>
//    {
//		public double X { get; set; }
//		public double Y { get; set; }
//		public double Z { get; set; }
//		public double W { get; set; }

//		public Vector4d(double x, double y, double z, double w)
//		{
//			X = x;
//			Y = y;
//			Z = z;
//			W = w;
//		}

//		public Vector4d(Vector4d other)
//		{
//			X = other.X;
//			Y = other.Y;
//			Z = other.Z;
//			W = other.W;
//		}


//		public bool Equals(Vector4d other)
//		{
//			if (ReferenceEquals(null, other)) return false;
//			return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
//		}

//		public override bool Equals(object obj)
//		{
//			if (ReferenceEquals(null, obj)) return false;
//			return obj is Vector4 && Equals((Vector4)obj);
//		}

//		public override int GetHashCode()
//		{
//			unchecked
//			{
//				var hashCode = X.GetHashCode();
//				hashCode = (hashCode * 397) ^ Y.GetHashCode();
//				hashCode = (hashCode * 397) ^ Z.GetHashCode();
//				hashCode = (hashCode * 397) ^ W.GetHashCode();
//				return hashCode;
//			}
//		}

//		public static bool operator ==(Vector4d left, Vector4d right)
//		{
//			if (ReferenceEquals(null, left)) return false;
//			return left.Equals(right);
//		}

//		public static bool operator !=(Vector4d left, Vector4d right)
//		{
//			return !left.Equals(right);
//		}

//		public static Vector4d operator *(Vector4d u, double p)
//		{
//			return u.Scale(p);
//		}
//		public static Vector4d operator +(Vector4d l, Vector4d r)
//		{
//			return l.Add(r);
//		}
//		public Vector4d Scale(double p)
//		{
//			return new Vector4d(this.X * p, this.Y * p, this.Z * p, this.W * p);
//		}
//		public Vector4d Add(Vector4d p)
//		{
//			return new Vector4d(this.X + p.X, this.Y + p.Y, this.Z + p.Z, this.W + p.W);
//		}
//		public Vector4d Clone()
//		{
//			return new Vector4d(X, Y, Z, W);
//		}

//		public byte[] ToFloatBytes()
//		{
//			return BitConverter.GetBytes(X).Concat(BitConverter.GetBytes(Y)).Concat(BitConverter.GetBytes(Z)).Concat(BitConverter.GetBytes(W)).ToArray();
//		}
//	}
//}
