using System;

namespace AgDataHandle.Maths
{
    public class Quaternion : Vector4, IQuaternion
	{
		#region 构造函数
		public static readonly Quaternion Identity = new Quaternion(0f, 0f, 0f, 1f);
		public Quaternion() : this(0, 0, 0, 0) { }

		public Quaternion(float[] ps)
		{
			X = ps[0];
			Y = ps[1];
			Z = ps[2];
			W = ps[3];
		}

		public Quaternion(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}
		public Quaternion(Quaternion other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
			W = other.W;
		}
		#endregion

		/// <summary>
		/// 置为单位四元数
		/// </summary>
		public void ToIdentity()
		{
			X = 0f;
			Y = 0f;
			Z = 0f;
			W = 1f;
		}

		public static T SetToRotateAboutX<T>(float theta) where T : class, IQuaternion<float>
		{
			float thetaOver2 = theta*0.5f;
			return New<T>((float)Math.Sin(thetaOver2), 0f, 0f, (float)Math.Cos(thetaOver2));
		}

		public static T SetToRotateAboutY<T>(float theta) where T : class, IQuaternion<float>
		{
			float thetaOver2 = theta * 0.5f;
			return New<T>(0f, (float)Math.Sin(thetaOver2), 0f, (float)Math.Cos(thetaOver2));
		}

		public static T SetToRotateAboutZ<T>(float theta) where T : class, IQuaternion<float>
		{
			float thetaOver2 = theta * 0.5f;
			return New<T>(0f, 0f, (float)Math.Sin(thetaOver2), (float)Math.Cos(thetaOver2));
		}

		/// <summary>
		/// 返回旋转轴
		/// </summary>
		/// <returns></returns>
		public IVector3<float> GetRotationAxis()
		{
			float sinThetaOver2Sq = 1f- (float)Math.Pow(W, 2);
			if (sinThetaOver2Sq== 0f)
			{
				return new Vector3(1f, 0f, 0f);
			}
			float oneOverSinThetaOver2 = 1f/(float)Math.Sqrt(sinThetaOver2Sq);
			var result = new Vector3(X, Y, Z);
			return result.Multiply<Vector3>(oneOverSinThetaOver2);
		}

		/// <summary>
		/// 返回旋转角
		/// </summary>
		/// <returns></returns>
		public float GetRotationAngle()
		{
			float thetaOver2 = (float)MathUtils.SafeAcos(W);
			return thetaOver2* 2;
		}

		/// <summary>
		/// 四元数转欧拉角
		/// </summary>
		/// <returns></returns>
		public T ToEulerAngle<T>() where T : class, IEulerAngle<float>
		{
			/* 参照3d数据基础写的，似乎heading和bank反了
            IEulerAngle<float> eulerAngle = new EulerAngle();
            //计算sin(pitch)
            float sp = -2.0f * (Y * Z - W * X);
            //检查万向锁，允许错在一定误差
            if (Math.Abs(sp) > 0.9999f)
            {
                //向正上方或正下方看
                eulerAngle.Pitch = (float)(Math.PI * 0.5f * sp);// MathUtil.PIOver2 * sp;
                                                                //bank置0，计算heading
                eulerAngle.Heading = (float)Math.Atan2(-X * Z + W * Y, 0.5f - Y * Y - Z * Z);
                eulerAngle.Bank = 0f;
            }
            else
            {
                //计算角度
                eulerAngle.Pitch = (float)Math.Asin(sp);
                eulerAngle.Heading = (float)Math.Atan2(X * Z + W * Y, 0.5f - X * X - Y * Y);
                eulerAngle.Bank = (float)Math.Atan2(X + Y + W * Z, 0.5f - X * X - Z * Z);
            }
            return eulerAngle as T;
			*/

			/* 慧云抄网上的
            IEulerAngle<float> eulerAngle = new EulerAngle();

			// roll (x-axis rotation)
			double sinr_cosp = 2 * (W * X + Y * Z);
			double cosr_cosp = 1 - 2 * (X * X + Y * Y);
			eulerAngle.Bank = (float)Math.Atan2(sinr_cosp, cosr_cosp);

			// pitch (y-axis rotation)
			double sinp = 2 * (W * Y - Z * X);
			if (Math.Abs(sinp) >= 1)
				eulerAngle.Pitch = sinp > 0 ? (float)(Math.PI / 2) : (float)(0 - (Math.PI / 2)); // use 90 degrees if out of range
			else
				eulerAngle.Pitch = (float)Math.Asin(sinp);

			// yaw (z-axis rotation)
			double siny_cosp = 2 * (W * Z + X * Y);
			double cosy_cosp = 1 - 2 * (Y * Y + Z * Z);
			eulerAngle.Heading = (float)Math.Atan2(siny_cosp, cosy_cosp);

			return eulerAngle as T;
			*/

			// 抄Cesium源码的
			EulerAngle result = new EulerAngle();

			var test = 2 * (W * Y - Z * X);
			var denominatorRoll = 1 - 2 * (X * X + Y * Y);
			var numeratorRoll = 2 * (W * X + Y * Z);
			var denominatorHeading = 1 - 2 * (Y * Y + Z * Z);
			var numeratorHeading = 2 * (W * Z + X * Y);
			result.Heading = (float)-Math.Atan2(numeratorHeading, denominatorHeading);
			result.Bank = (float)Math.Atan2(numeratorRoll, denominatorRoll);

			var min = -1;
			var max = 1;
			test = test < min ? min : test > max ? max : test;

			result.Pitch = -(float)Math.Asin(test);

			return result as T;
		}

		/// <summary>
		/// 四元数转3X3旋转矩阵
		/// </summary>
		/// <returns></returns>
		public T ToMatrix<T>() where T : class, IMatrix3x3<float>
		{
			IMatrix3x3<float> matrix = new Matrix3x3();
			matrix[0, 0] = 1.0f - 2.0f * (Y * Y + Z * Z);
			matrix[0, 1] = 2.0f * (X * Y - W * Z);
			matrix[0, 2] = 2.0f * (X * Z + W * Y);
			matrix[1, 0] = 2.0f * (X * Y + W * Z);
			matrix[1, 1] = 1.0f - 2.0f * (X * X + Z * Z);
			matrix[1, 2] = 2.0f * (Y * Z + W * X);
			matrix[2, 0] = 2.0f * (X * Z + W * Y);
			matrix[2, 1] = 2.0f * (Y * Z + W * X);
			matrix[2, 2] = 1.0f - 2.0f * (X * X + Y * Y);
			return matrix as T;
		}

		/// <summary>
		/// 球面线性插值，单位四元数才能用，假如不是就先规则化
		/// </summary>
		/// <param name="q1"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public T Slerp<T>(T q1, float t) where T : class, IQuaternion<float>
		{
			//检查出界的参数，如果出界返回临界点
			if (t <= 0f) return this as T;
			if (t > 1f) return q1;

			//如果点乘为负，用-q1
			//四元数q和-1代表相同的旋转，但可能产生不同的slerp运算，我们要选择正确的一个以便用锐角进行旋转
			float cosOmega = this.Dot(q1);
			float q1w = q1.W;
			float q1x = q1.X;
			float q1y = q1.Y;
			float q1z = q1.Z;
			if (cosOmega < 0f)
			{
				q1w = -q1w;
				q1x = -q1x;
				q1y = -q1y;
				q1z = -q1z;
			}
			float k0;
			float k1;
			if (cosOmega < 0.9999f)
			{
				k0 = 1f - t;
				k1 = t;
			}
			else
			{
				float sinOmega = (float)Math.Sqrt(1f - cosOmega * cosOmega);
				float omega = (float)Math.Atan2(sinOmega, cosOmega);
				float oneOverSionOmega = 1f / sinOmega;

				k0 = (float)Math.Sin((1f - t) * omega * oneOverSionOmega);
				k1 = (float)Math.Sin(t * omega * oneOverSionOmega);
			}
			return New<T>(k0 * X + k1 * q1x, k0 * Y + k1 * q1y, k0 * Z + k1 * q1z, k0 * W + k1 * q1w);
		}

		/// <summary>
		/// 四元数共轭，即与四元数旋转方向相反的四元数
		/// </summary>
		/// <returns></returns>
		public T Conjugate<T>() where T : class, IQuaternion<float>
		{
			return New<T>(-X, -Y, -Z, W);
		}

		/// <summary>
		/// 四元数幂运算
		/// </summary>
		/// <param name="exponent"></param>
		/// <returns></returns>
		public T Pow<T>(float exponent) where T : class, IQuaternion<float>
		{
			//检查单位四元数，防止除零
			if (W> 0.9999f) { return this as T; }
			var alpha = (float)Math.Acos(W);
			var newAlpha = alpha* exponent;
			T result = New<T>();
			result.W = (float)Math.Cos(newAlpha);
			var mult = (float)Math.Sin(newAlpha)/(float)Math.Sin(alpha);
			result.X = X*mult;
			result.Y = Y*mult;
			result.Z = Z*mult;
			return result;
		}

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

		/// <summary>
		/// 根据旋转轴，旋转角度计算四元数
		/// </summary>
		/// <param name="axis">旋转轴</param>
		/// <param name="angle">旋转角度(弧度)</param>
		/// <returns></returns>
		public static T FromAxisAngle<T>(Vector3 axis, float angle) where T : class, IQuaternion<float>
		{
			var halfAngle = angle / 2f;
			var s = (float)Math.Sin(halfAngle);
			Vector3 fromAxisAngleScratch = axis.Normalize<Vector3>();
			var x = fromAxisAngleScratch.X* s;
			var y = fromAxisAngleScratch.Y* s;
			var z = fromAxisAngleScratch.Z* s;
			var w = (float)Math.Cos(halfAngle);

			return New<T>(x, y, z, w);
		}

		public static Quaternion Multiply(Quaternion left, Quaternion right)
        {
			Quaternion result = new Quaternion();
			var leftX = left.X;
			var leftY = left.Y;
			var leftZ = left.Z;
			var leftW = left.W;

			var rightX = right.X;
			var rightY = right.Y;
			var rightZ = right.Z;
			var rightW = right.W;

			var x = leftW * rightX + leftX * rightW + leftY * rightZ - leftZ * rightY;
			var y = leftW * rightY - leftX * rightZ + leftY * rightW + leftZ * rightX;
			var z = leftW * rightZ + leftX * rightY - leftY * rightX + leftZ * rightW;
			var w = leftW * rightW - leftX * rightX - leftY * rightY - leftZ * rightZ;

			result.X = x;
			result.Y = y;
			result.Z = z;
			result.W = w;
			return result;
		}

		#region 重写操作符
		public static Quaternion operator +(Quaternion left, Quaternion right)
		{
			return left.AddToOther(right);
		}

		public static Quaternion operator -(Quaternion u)
		{
			return u.Negate<Quaternion>();
		}

		public static Quaternion operator -(Quaternion left, Quaternion right)
		{
			return left.MinusToOther(right);
		}

		public static Quaternion operator *(float p, Quaternion u)
		{
			return u.MultiplyToOther<Quaternion>(p);
		}

		public static Quaternion operator *(Quaternion u, float p)
		{
			return u.MultiplyToOther<Quaternion>(p);
		}

		public static Quaternion operator *(Quaternion left, Quaternion right)
		{
			return left.MultiplyToOther(right);
		}

		public static Quaternion operator /(Quaternion left, Quaternion right)
		{
			return left.DivideToOther(right);
		}

		public static Quaternion operator /(Quaternion u, float a)
		{
			return u.DivideToOther<Quaternion>(a);
		}

		public static Quaternion operator /(float a, Quaternion u)
		{
			return new Quaternion(a / u.X, a / u.Y, a / u.Z, a / u.W);
		}
		#endregion
	}

	public class QuaternionD : Vector4d, IQuaternionD
	{
		#region 构造函数
		public static readonly QuaternionD Identity = new QuaternionD(0, 0, 0, 1);
		public QuaternionD() : this(0, 0, 0, 0) { }

		public QuaternionD(double x, double y, double z, double w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}
		public QuaternionD(QuaternionD other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
			W = other.W;
		}
		#endregion

		/// <summary>
		/// 置为单位四元数
		/// </summary>
		public void ToIdentity()
		{
			X = 0f;
			Y = 0f;
			Z = 0f;
			W = 1f;
		}

		public static T SetToRotateAboutX<T>(double theta) where T : class, IQuaternion<double>
		{
			double thetaOver2 = theta * 0.5f;
			return New<T>(Math.Sin(thetaOver2), 0f, 0f, Math.Cos(thetaOver2));
		}

		public static T SetToRotateAboutY<T>(double theta) where T : class, IQuaternion<double>
		{
			double thetaOver2 = theta * 0.5f;
			return New<T>(0f, Math.Sin(thetaOver2), 0f, Math.Cos(thetaOver2));
		}

		public static T SetToRotateAboutZ<T>(double theta) where T : class, IQuaternion<double>
		{
			double thetaOver2 = theta * 0.5f;
			return New<T>(0f, 0f, Math.Sin(thetaOver2), Math.Cos(thetaOver2));
		}

		/// <summary>
		/// 返回旋转轴
		/// </summary>
		/// <returns></returns>
		public IVector3<double> GetRotationAxis()
		{
			double sinThetaOver2Sq = 1f - Math.Pow(W, 2);
			if (sinThetaOver2Sq == 0f)
			{
				return new Vector3d(1f, 0f, 0f);
			}
			double oneOverSinThetaOver2 = 1f / Math.Sqrt(sinThetaOver2Sq);
			var result = new Vector3d(X, Y, Z);
			return result.Multiply<Vector3d>(oneOverSinThetaOver2);
		}

		/// <summary>
		/// 返回旋转角
		/// </summary>
		/// <returns></returns>
		public double GetRotationAngle()
		{
			double thetaOver2 = MathUtils.SafeAcos(W);
			return thetaOver2 * 2;
		}

		/// <summary>
		/// 四元数转欧拉角
		/// </summary>
		/// <returns></returns>
		public T ToEulerAngle<T>() where T : class, IEulerAngle<double>
		{
			IEulerAngle<double> eulerAngle = new EulerAngleD();
			//计算sin(pitch)
			double sp = -2.0f * (Y * Z - W * X);
			//检查万向锁，允许错在一定误差
			if (Math.Abs(sp) > 0.9999f)
			{
				//向正上方或正下方看
				eulerAngle.Pitch = (Math.PI * 0.5f * sp);// MathUtil.PIOver2 * sp;
																//bank置0，计算heading
				eulerAngle.Heading = Math.Atan2(-X * Z + W * Y, 0.5f - Y * Y - Z * Z);
				eulerAngle.Bank = 0f;
			}
			else
			{
				//计算角度
				eulerAngle.Pitch = Math.Asin(sp);
				eulerAngle.Heading = Math.Atan2(X * Z + W * Y, 0.5f - X * X - Y * Y);
				eulerAngle.Bank = Math.Atan2(X + Y + W * Z, 0.5f - X * X - Z * Z);
			}
			return eulerAngle as T;
		}

		/// <summary>
		/// 四元数转3X3旋转矩阵
		/// </summary>
		/// <returns></returns>
		public T ToMatrix<T>() where T:class, IMatrix3x3<double>
		{
			IMatrix3x3<double> matrix = new Matrix3x3D();
			matrix[0, 0] = 1.0f - 2.0f * (Y * Y + Z * Z);
			matrix[0, 1] = 2.0f * (X * Y - W * Z);
			matrix[0, 2] = 2.0f * (X * Z + W * Y);
			matrix[1, 0] = 2.0f * (X * Y + W * Z);
			matrix[1, 1] = 1.0f - 2.0f * (X * X + Z * Z);
			matrix[1, 2] = 2.0f * (Y * Z + W * X);
			matrix[2, 0] = 2.0f * (X * Z + W * Y);
			matrix[2, 1] = 2.0f * (Y * Z + W * X);
			matrix[2, 2] = 1.0f - 2.0f * (X * X + Y * Y);
			return matrix as T;
		}

		/// <summary>
		/// 球面线性插值，单位四元数才能用，假如不是就先规则化
		/// </summary>
		/// <param name="q1"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public T Slerp<T>(T q1, double t) where T : class, IQuaternion<double>
		{
			//检查出界的参数，如果出界返回临界点
			if (t <= 0f) return this as T;
			if (t > 1f) return q1;

			//如果点乘为负，用-q1
			//四元数q和-1代表相同的旋转，但可能产生不同的slerp运算，我们要选择正确的一个以便用锐角进行旋转
			double cosOmega = this.Dot(q1);
			double q1w = q1.W;
			double q1x = q1.X;
			double q1y = q1.Y;
			double q1z = q1.Z;
			if (cosOmega < 0f)
			{
				q1w = -q1w;
				q1x = -q1x;
				q1y = -q1y;
				q1z = -q1z;
			}
			double k0;
			double k1;
			if (cosOmega < 0.9999f)
			{
				k0 = 1f - t;
				k1 = t;
			}
			else
			{
				double sinOmega = Math.Sqrt(1f - cosOmega * cosOmega);
				double omega = Math.Atan2(sinOmega, cosOmega);
				double oneOverSionOmega = 1f / sinOmega;

				k0 = Math.Sin((1f - t) * omega * oneOverSionOmega);
				k1 = Math.Sin(t * omega * oneOverSionOmega);
			}
			return New<T>(k0 * X + k1 * q1x, k0 * Y + k1 * q1y, k0 * Z + k1 * q1z, k0 * W + k1 * q1w);
		}

		/// <summary>
		/// 四元数共轭，即与四元数旋转方向相反的四元数
		/// </summary>
		/// <returns></returns>
		public T Conjugate<T>() where T : class, IQuaternion<double>
		{
			return New<T>(-X, -Y, -Z, W);
		}

		/// <summary>
		/// 四元数幂运算
		/// </summary>
		/// <param name="exponent"></param>
		/// <returns></returns>
		public T Pow<T>(double exponent) where T : class, IQuaternion<double>
		{
			//检查单位四元数，防止除零
			if (W > 0.9999f) { return this as T; }
			var alpha = Math.Acos(W);
			var newAlpha = alpha * exponent;
			T result = New<T>();
			result.W = Math.Cos(newAlpha);
			var mult = Math.Sin(newAlpha) / Math.Sin(alpha);
			result.X = X * mult;
			result.Y = Y * mult;
			result.Z = Z * mult;
			return result;
		}

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

		/// <summary>
		/// 根据旋转轴，旋转角度计算四元数
		/// </summary>
		/// <param name="axis">旋转轴</param>
		/// <param name="angle">旋转角度(弧度)</param>
		/// <returns></returns>
		public static T FromAxisAngle<T>(IVector3d axis, double angle) where T : class, IQuaternion<double>
		{
			var halfAngle = angle / 2f;
			var s = Math.Sin(halfAngle);
			Vector3d fromAxisAngleScratch = axis.Normalize<Vector3d>();
			var x = fromAxisAngleScratch.X * s;
			var y = fromAxisAngleScratch.Y * s;
			var z = fromAxisAngleScratch.Z * s;
			var w = Math.Cos(halfAngle);

			return New<T>(x, y, z, w);
		}

		#region 重写操作符
		public static QuaternionD operator +(QuaternionD left, QuaternionD right)
		{
			return left.AddToOther(right);
		}

		public static QuaternionD operator -(QuaternionD u)
		{
			return u.Negate<QuaternionD>();
		}

		public static QuaternionD operator -(QuaternionD left, QuaternionD right)
		{
			return left.MinusToOther(right);
		}

		public static QuaternionD operator *(double p, QuaternionD u)
		{
			return u.MultiplyToOther<QuaternionD>(p);
		}

		public static QuaternionD operator *(QuaternionD u, double p)
		{
			return u.MultiplyToOther<QuaternionD>(p);
		}

		public static QuaternionD operator *(QuaternionD left, QuaternionD right)
		{
			return left.MultiplyToOther(right);
		}

		public static QuaternionD operator /(QuaternionD left, QuaternionD right)
		{
			return left.DivideToOther(right);
		}

		public static QuaternionD operator /(QuaternionD u, double a)
		{
			return u.DivideToOther<QuaternionD>(a);
		}

		public static QuaternionD operator /(double a, QuaternionD u)
		{
			return new QuaternionD(a / u.X, a / u.Y, a / u.Z, a / u.W);
		}
		#endregion
	}
}