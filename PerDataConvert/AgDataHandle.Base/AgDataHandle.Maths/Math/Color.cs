using System;

namespace AgDataHandle.Maths
{
	public class Color : IEquatable<Color>
	{
		public static Color Black { get { return new Color(0f, 0f, 0f, 1f); } }
		public static Color White { get { return new Color(1f, 1f, 1f, 1f); } }
		public static Color Red { get { return new Color(1f, 0f, 0f, 1f); } }

		public float R { get; set; }
		public float G { get; set; }
		public float B { get; set; }
		public float A { get; set; }
		public Color() : this(0f, 0f, 0f, 1f) { }
		public Color(float r, float g, float b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = 1.0f;
		}

		public Color(float r, float g, float b, float a)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}

		public Color(Color other)
		{
			R = other.R;
			G = other.G;
			B = other.B;
			A = other.A;
		}

		public bool Equals(Color other)
		{
			return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B) && A.Equals(other.A);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Color && Equals((Color)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = R.GetHashCode();
				hashCode = (hashCode * 397) ^ G.GetHashCode();
				hashCode = (hashCode * 397) ^ B.GetHashCode();
				hashCode = (hashCode * 397) ^ A.GetHashCode();
				return hashCode;
			}
		}

		/// <summary>
		/// 创建随机颜色,范围0-1
		/// </summary>
		/// <returns></returns>
		public static Color CreateRandColor()
		{
			var rand = new Random(Guid.NewGuid().GetHashCode());
			var r = (float)rand.NextDouble();
			var g = (float)rand.NextDouble();
			var b = (float)rand.NextDouble();
			return new Color(r, g, b);
		}
		/// <summary>
		/// 创建随机颜色，范围0-255
		/// </summary>
		/// <returns></returns>
		public static System.Drawing.Color CreateRandBitmapColor()
		{
			var rand = new Random(Guid.NewGuid().GetHashCode());
			var r = rand.NextDouble()*255;
			var g = rand.NextDouble()*255;
			var b = rand.NextDouble()*255;
			return System.Drawing.Color.FromArgb((int)r, (int)g, (int)b);
		}
	}
}
