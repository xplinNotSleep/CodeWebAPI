using System;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 欧拉角，该类用于表示heading-pitch-bank
    /// </summary>
    public class EulerAngle : IEulerAngle
    {
        public float Heading { get; set; }
        public float Pitch { get; set; }
        public float Bank { get; set; }

        public static Vector3 UNIT_X = new Vector3(1, 0, 0);
        public static Vector3 UNIT_Y = new Vector3(0, 1, 0);
        public static Vector3 UNIT_Z = new Vector3(0, 0, 1);

        public EulerAngle() { }
        /// <summary>
        /// 欧拉角，默认左手坐标系
        /// </summary>
        /// <param name="heading">绕物体Y轴的旋转量，左手左边系，+X为右 yaw</param>
        /// <param name="pitch">绕物体X轴的旋转量，+Y为上 </param>
        /// <param name="bank">绕物体Z轴的旋转量，+Z向前 roll</param>
        public EulerAngle(float heading, float pitch, float bank)
        {
            this.Heading = heading;
            this.Pitch = pitch;
            this.Bank = bank;
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
            if (typeT == typeof(IEulerAngle)||typeT==typeof(IEulerAngle<float>))
            {
                model = new EulerAngle();
            }
            else if (typeT == typeof(IEulerAngleD) || typeT == typeof(IEulerAngle<double>))
            {
                model = new EulerAngleD();
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            var newModelType = model.Heading.GetType();
            var IsSame = this.Heading.GetType() == newModelType;
            if (IsSame)
            {
                model.Heading = Heading; model.Pitch = Pitch; model.Bank = Bank;
            }
            else
            {
                if (newModelType == typeof(double))
                {
                    model.Heading = Heading; model.Pitch = Pitch; model.Bank = Bank;
                }
                else
                {
                    model.Heading = (float)Heading; model.Pitch = (float)Pitch; model.Bank = (float)Bank;
                }
            }
            return model as T;
        }

        /// <summary>
        /// 置0
        /// </summary>
        public void ToIdentity()
        {
            this.Heading = 0f;
            this.Pitch = 0f;
            this.Bank = 0f;
        }

        /// <summary>
        /// 变换为“限制集”欧拉角
        /// </summary>
        public void Canonize()
        {
            //首先，将pitch变换到-pi到pi之间
            Pitch = (float)MathUtils.WrapPi((double)Pitch);
            //现在将pitch变换到-pi/2到pi/2之间
            if (Pitch < -Math.PI / 2)
            {
                Pitch = -(float)Math.PI - Pitch;
                Heading += Heading + (float)Math.PI;
                Bank += (float)Math.PI;
            }
            else if (Pitch > Math.PI / 2)
            {
                Pitch = (float)Math.PI - Pitch;
                Heading += Heading + (float)Math.PI;
                Bank += (float)Math.PI;
            }
            //现在检查万向锁的情况，允许存在一定的误差
            if (Math.Abs(Pitch) > Math.PI / 2 - 1e-4f)
            {
                //在万向锁中，将所有饶垂直轴的旋转付给heading
                Heading += Bank;
                Bank = 0f;
            }
            else
            {
                //非万向锁，将bank转换到限制集中
                Bank = (float)MathUtils.WrapPi((double)Bank);
            }
            //将heading转换到限制集中
            Heading = (float)MathUtils.WrapPi((double)Heading);
        }

        /// <summary>
        /// 转成四元数，构造执行物体-惯性旋转的四元数
        /// </summary>
        /// <returns></returns>
        public T ToQuaternion<T>() where T : class, IQuaternion<float>
        {
            /* 可彪抄书上的
            float sp = (float)Math.Sin(Pitch);
            float sb = (float)Math.Sin(Bank);
            float sh = (float)Math.Sin(Heading);
            float cp = (float)Math.Cos(Pitch);
            float cb = (float)Math.Cos(Bank);
            float ch = (float)Math.Cos(Heading);
            return new Quaternion(ch * sp * cb + sh * cp * sb,
               -ch * sp * sb + sh * cp * cb,
               -sh * sp * cb + ch * cp * sb,
                ch * cp * cb + sh * sp * sb) as T;
            */

            /* 慧云抄网上的
            // Abbreviations for the various angular functions
            double cy = Math.Cos(Heading * 0.5);
            double sy = Math.Sin(Heading * 0.5);
            double cp = Math.Cos(Pitch * 0.5);
            double sp = Math.Sin(Pitch * 0.5);
            double cr = Math.Cos(Bank * 0.5);
            double sr = Math.Sin(Bank * 0.5);

            Quaternion q = new Quaternion();
            q.W = (float)(cy * cp * cr + sy * sp * sr);
            q.X = (float)(cy * cp * sr - sy * sp * cr);
            q.Y = (float)(sy * cp * sr + cy * sp * cr);
            q.Z = (float)(sy * cp * cr - cy * sp * sr);

            return q as T;
            */

            // 抄cesium源码的
            var scratchHeadingQuaternion = new Quaternion();
            var scratchPitchQuaternion = new Quaternion();
            var scratchRollQuaternion = new Quaternion();

            Quaternion result = new Quaternion();

            scratchRollQuaternion = Quaternion.FromAxisAngle<Quaternion>(UNIT_X, this.Bank);
            scratchPitchQuaternion = Quaternion.FromAxisAngle<Quaternion>(UNIT_Y, -this.Pitch);

            result = Quaternion.Multiply(scratchPitchQuaternion,scratchRollQuaternion);

            scratchHeadingQuaternion = Quaternion.FromAxisAngle<Quaternion>(UNIT_Z, -this.Heading);

            return Quaternion.Multiply(scratchHeadingQuaternion, result) as T;
        }

        /// <summary>
        /// 转成旋转矩阵
        /// </summary>
        /// <returns></returns>
        public T ToMatrix<T>() where T : class, IMatrix3x3<float>
        {
            float sp = (float)Math.Sin(Pitch);
            float sb = (float)Math.Sin(Bank);
            float sh = (float)Math.Sin(Heading);
            float cp = (float)Math.Cos(Pitch);
            float cb = (float)Math.Cos(Bank);
            float ch = (float)Math.Cos(Heading);
            IMatrix3x3<float> matrix = new Matrix3x3();
            matrix[0, 0] = ch * cb + sh * sp * sb;
            matrix[0, 1] = -ch * cb + sh * sp * cb;
            matrix[0, 2] = sh * cp;
            matrix[1, 0] = sb * cp;
            matrix[1, 1] = cb * cp;
            matrix[1, 2] = -sp;
            matrix[2, 0] = -sh * cb + ch * sp * sb;
            matrix[2, 1] = sb * sh + ch * sp * cb;
            matrix[2, 2] = ch * cp;
            return matrix as T;
        }

        public bool Equals(IEulerAngle<float> other)
        {
            return Heading == other.Heading && Pitch == other.Pitch && Bank == other.Bank;
        }
    }

    /// <summary>
    /// 欧拉角，该类用于表示heading-pitch-bank
    /// </summary>
    public class EulerAngleD : IEulerAngleD
    {
        public double Heading { get; set; }
        public double Pitch { get; set; }
        public double Bank { get; set; }

        public EulerAngleD() { }
        /// <summary>
        /// 欧拉角，默认左手坐标系
        /// </summary>
        /// <param name="heading">绕物体Y轴的旋转量，左手左边系，+X为右</param>
        /// <param name="pitch">绕物体X轴的旋转量，+Y为上</param>
        /// <param name="bank">绕物体Z轴的旋转量，+Z向前</param>
        public EulerAngleD(double heading, double pitch, double bank)
        {
            this.Heading = heading;
            this.Pitch = pitch;
            this.Bank = bank;
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
            if (typeT == typeof(IEulerAngle) || typeT == typeof(IEulerAngle<float>))
            {
                model = new EulerAngle();
            }
            else if (typeT == typeof(IEulerAngleD) || typeT == typeof(IEulerAngle<double>))
            {
                model = new EulerAngleD();
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            var newModelType = model.Heading.GetType();
            var IsSame = this.Heading.GetType() == newModelType;
            if (IsSame)
            {
                model.Heading = Heading; model.Pitch = Pitch; model.Bank = Bank;
            }
            else
            {
                if (newModelType == typeof(double))
                {
                    model.Heading = Heading; model.Pitch = Pitch; model.Bank = Bank;
                }
                else
                {
                    model.Heading = (float)Heading; model.Pitch = (float)Pitch; model.Bank = (float)Bank;
                }
            }
            return model as T;
        }

        /// <summary>
        /// 置0
        /// </summary>
        public void ToIdentity()
        {
            this.Heading = 0;
            this.Pitch = 0;
            this.Bank = 0;
        }

        /// <summary>
        /// 变换为“限制集”欧拉角
        /// </summary>
        public void Canonize()
        {
            //首先，将pitch变换到-pi到pi之间
            Pitch = MathUtils.WrapPi(Pitch);
            //现在将pitch变换到-pi/2到pi/2之间
            if (Pitch < -Math.PI / 2)
            {
                Pitch = -Math.PI - Pitch;
                Heading += Heading + (double)Math.PI;
                Bank += Math.PI;
            }
            else if (Pitch > Math.PI / 2)
            {
                Pitch = Math.PI - Pitch;
                Heading += Heading + Math.PI;
                Bank += Math.PI;
            }
            //现在检查万向锁的情况，允许存在一定的误差
            if (Math.Abs(Pitch) > Math.PI / 2 - 1e-4f)
            {
                //在万向锁中，将所有饶垂直轴的旋转付给heading
                Heading += Bank;
                Bank = 0f;
            }
            else
            {
                //非万向锁，将bank转换到限制集中
                Bank = MathUtils.WrapPi(Bank);
            }
            //将heading转换到限制集中
            Heading = MathUtils.WrapPi(Heading);
        }

        /// <summary>
        /// 转成四元数，构造执行物体-惯性旋转的四元数
        /// </summary>
        /// <returns></returns>
        public T ToQuaternion<T>() where T : class, IQuaternion<double>
        {
            double sp = Math.Sin(Pitch);
            double sb = Math.Sin(Bank);
            double sh = Math.Sin(Heading);
            double cp = Math.Cos(Pitch);
            double cb = Math.Cos(Bank);
            double ch = Math.Cos(Heading);
            return new QuaternionD(ch * sp * cb + sh * cp * sb,
               -ch * sp * sb + sh * cp * cb,
               -sh * sp * cb + ch * cp * sb,
                ch * cp * cb + sh * sp * sb) as T;
        }

        /// <summary>
        /// 转成旋转矩阵
        /// </summary>
        /// <returns></returns>
        public T ToMatrix<T>() where T : class, IMatrix3x3<double>
        {
            double sp = Math.Sin(Pitch);
            double sb = Math.Sin(Bank);
            double sh = Math.Sin(Heading);
            double cp = Math.Cos(Pitch);
            double cb = Math.Cos(Bank);
            double ch = Math.Cos(Heading);
            IMatrix3x3<double> matrix = new Matrix3x3D();
            matrix[0, 0] = ch * cb + sh * sp * sb;
            matrix[0, 1] = -ch * cb + sh * sp * cb;
            matrix[0, 2] = sh * cp;
            matrix[1, 0] = sb * cp;
            matrix[1, 1] = cb * cp;
            matrix[1, 2] = -sp;
            matrix[2, 0] = -sh * cb + ch * sp * sb;
            matrix[2, 1] = sb * sh + ch * sp * cb;
            matrix[2, 2] = ch * cp;
            return matrix as T;
        }

        public bool Equals(IEulerAngle<double> other)
        {
            return Heading == other.Heading && Pitch == other.Pitch && Bank == other.Bank;
        }
    }
}
