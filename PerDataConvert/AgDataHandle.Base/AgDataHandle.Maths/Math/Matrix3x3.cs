using MathNet.Numerics;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// Cesium
    public class Matrix3x3 : MatrixNxN, IMatrix3x3
    {
        public Matrix3x3() : base(3) { }
        public Matrix3x3(float[] arr) : base(arr,3) { }
        public Matrix3x3(Matrix3x3 other) : base(other) { }

        public Matrix3x3(float column0Row0, float column1Row0, float column2Row0,
                           float column0Row1, float column1Row1, float column2Row1,
                           float column0Row2, float column1Row2, float column2Row2) : base(3)
        {
            Mat[0, 0] = column0Row0;
            Mat[1, 0] = column0Row1;
            Mat[2, 0] = column0Row2;
            Mat[0, 1] = column1Row0;
            Mat[1, 1] = column1Row1;
            Mat[2, 1] = column1Row2;
            Mat[0, 2] = column2Row0;
            Mat[1, 2] = column2Row1;
            Mat[2, 2] = column2Row2;
        }
        public static Matrix3x3 Identity
        {
            get
            {
                return new Matrix3x3(
                    1f, 0f, 0f,
                    0f, 1f, 0f,
                    0f, 0f, 1f
                    );
            }
        }

        /// <summary>
        /// 矩阵求逆（伴随矩阵法），自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override T Inverse<T>()
        {
            //直接计算
            var m11 = this[0,0];
            var m12 = this[0,1];
            var m13 = this[0,2];
            var m21 = this[1,0];
            var m22 = this[1,1];
            var m23 = this[1,2];
            var m31 = this[2,0];
            var m32 = this[2,1];
            var m33 = this[2,2];

            var determinant = m11 * (m22 * m33 - m32 * m23) + m21 * (m32 * m13 - m12 * m33) + m31 * (m12 * m23 - m22 * m13);

            if (determinant == 0.0f)
            {
                Exception myException = new Exception("没有逆矩阵");
                throw myException;
            }

            this[0, 0] = m22 * m33 - m23 * m32;
            this[1, 0] = m23 * m31 - m21 * m33;
            this[2, 0] = m21 * m32 - m22 * m31;
            this[0, 1] = m13 * m32 - m12 * m33;
            this[1, 1] = m11 * m33 - m13 * m31;
            this[2, 1] = m12 * m31 - m11 * m32;
            this[0, 2] = m12 * m23 - m13 * m22;
            this[1, 2] = m13 * m21 - m11 * m23;
            this[2, 2] = m11 * m22 - m12 * m21;
            return this.Divide<T>(determinant);
        }

        /// <summary>
        /// 矩阵求逆（伴随矩阵法），自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override bool TryInverseToOther<T>(out T result)
        {
            result = New(this as T);
            try
            {
                result.Inverse<T>();
            }
            catch
            {
                result = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 通过向量缩放，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T ScaleByVector<T>(IVector3<float> vector) where T : class, IMatrix3x3<float>
        {
            this[0,0]*=vector.X; 
            this[0,1]*=vector.X;
            this[0,2]*=vector.X;

            this[1, 0] *= vector.Y;
            this[1, 1] *= vector.Y;
            this[1, 2] *= vector.Y;

            this[2, 0] *= vector.Z;
            this[2, 1] *= vector.Z;
            this[2, 2] *= vector.Z;
            return this as T;
        }

        /// <summary>
        /// 通过向量缩放，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T ScaleByVectorToOther<T>(IVector3<float> vector) where T : class, IMatrix3x3<float>
        {
            T Mc = New(this as T);
            return Mc.ScaleByVector<T>(vector);
        }

        /// <summary>
        /// 转成欧拉角
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToEulerAngle<T>() where T : class, IEulerAngle<float>
        {
            IEulerAngle result = new EulerAngle();
            //通过m32计算sin(pitch)
            float sp = -this[2, 1];

            //检查万向锁
            if (Math.Abs(sp)> 9.99999f)
            {
                //向正上方或正下方看
                result.Pitch =(float) Math.PI*sp;
                //back置零，计算heading
                result.Heading = (float)Math.Atan2(-this[1, 2],this[0, 0]);
                result.Bank = 0f;
            }
            else
            {
                result.Heading = (float)Math.Atan2(this[2, 0],this[2, 2]);
                result.Pitch = (float)Math.Asin(sp);
                result.Bank = (float)Math.Atan2(this[0, 1], this[1, 1]);
            }
            return result as T;
        }

        public T ToQuaternionByCesium<T>() where T : class, IQuaternion<float>
        {
            #region 抄Cesium源码的

            Quaternion result = new Quaternion();

            double root;
            double x;
            double y;
            double z;
            double w;

            var m00 = this[0, 0];
            var m11 = this[1, 1];
            var m22 = this[2, 2];
            var trace = m00 + m11 + m22;

            if (trace > 0.0)
            {
                // |w| > 1/2, may as well choose w > 1/2
                root = Math.Sqrt(trace + 1.0); // 2w
                w = 0.5 * root;
                root = 0.5 / root; // 1/(4w)

                x = (this[2, 1] - this[1, 2]) * root;
                y = (this[0, 2] - this[2, 0]) * root;
                z = (this[1, 0] - this[0, 1]) * root;
            }
            else
            {
                // |w| <= 1/2
                int[] fromRotationMatrixNext = { 1, 2, 0 };
                var next = fromRotationMatrixNext;

                var i = 0;
                if (m11 > m00)
                {
                    i = 1;
                }
                if (m22 > m00 && m22 > m11)
                {
                    i = 2;
                }
                var j = next[i];
                var k = next[j];

                root = Math.Sqrt(
                  this[GetElementIndex(i, i)] -
                    this[GetElementIndex(j, j)] -
                    this[GetElementIndex(k, k)] +
                    1.0
                );

                double[] fromRotationMatrixQuat = new double[3];
                var quat = fromRotationMatrixQuat;
                quat[i] = 0.5 * root;
                root = 0.5 / root;
                w =
                  (this[GetElementIndex(k, j)] -
                    this[GetElementIndex(j, k)]) *
                  root;
                quat[j] =
                  (this[GetElementIndex(j, i)] +
                    this[GetElementIndex(i, j)]) *
                  root;
                quat[k] =
                  (this[GetElementIndex(k, i)] +
                    this[GetElementIndex(i, k)]) *
                  root;

                x = -quat[0];
                y = -quat[1];
                z = -quat[2];
            }

            result.X = (float)x;
            result.Y = (float)y;
            result.Z = (float)z;
            result.W = (float)w;
            return result as T;

            #endregion
        }

        /// <summary>
        /// 转成四元数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToQuaternion<T>() where T : class, IQuaternion<float>
        {
            #region 原来的写法
            
            int[] fromRotationMatrixNext = new int[3] { 1, 2, 0 };
            float[] fromRotationMatrixQuat = new float[3];

            float root;
            float x;
            float y;
            float z;
            float w;

            float m00 = this[0, 0];
            float m11 = this[1, 1];
            float m22 = this[2, 2];
            float trace = m00 + m11 + m22;

            if (trace > 0.0)
            {
                // |w| > 1/2, may as well choose w > 1/2
                root = (float)Math.Sqrt(trace + 1.0f); // 2w
                w = 0.5f * root;
                root = 0.5f / root; // 1/(4w)
                x = (this[2, 1] - this[1, 2]) * root;
                y = (this[0, 2] - this[2, 0]) * root;
                z = (this[1, 0] - this[0, 1]) * root;
            }
            else
            {
                // |w| <= 1/2
                int[] next = fromRotationMatrixNext;
                int i = 0;
                if (m11 > m00)
                {
                    i = 1;
                }
                if (m22 > m00 && m22 > m11)
                {
                    i = 2;
                }
                int j = next[i];
                int k = next[j];
                root = (float)Math.Sqrt(this[i, i] - this[j, j] - this[k, k] + 1.0);

                var quat = fromRotationMatrixQuat;
                quat[i] = 0.5f * root;
                root = 0.5f / root;
                w = (this[k, j] - this[j, k]) * root;
                quat[j] = (this[j, i] + this[i, j]) * root;
                quat[k] = (this[k, i] + this[i, k]) * root;

                x = -quat[0];
                y = -quat[1];
                z = -quat[2];
            }
            return new Quaternion(x, y, z, w) as T;
            
            #endregion
            #region 三维书的写法,暂时保留不删除
            /*
            float[] t1s = new float[4];
            //探测w,x,z中的最大绝对值
            t1s[3] = this[0, 0] + this[1, 1] + this[2, 2];//W
            t1s[0] = this[0, 0] - this[1, 1] - this[2, 2];//X
            t1s[1] = this[1, 1] - this[0, 0] - this[2, 2];//Y
            t1s[2] = this[2, 2] - this[0, 0] - this[1, 1];//Z
            int biggestIndex = 3;
            float biggestSquareVal = t1s[3];
            for (int i = 0; i < 3; i++)
            {
                if (t1s[i] > biggestSquareVal)
                {
                    biggestIndex = i;
                    biggestSquareVal = t1s[i];
                }
            }
            float biggestVal = (float)Math.Sqrt(biggestSquareVal + 1.0f) * 0.5f;
            float mult = 0.25f / biggestVal;
            
            switch (biggestIndex)
            {
                case 3:
                    t1s[3] = biggestVal;
                    t1s[0] = (this[1, 2] - this[2, 1]) * mult;
                    t1s[1] = (this[2, 0] - this[0, 2]) * mult;
                    t1s[2] = (this[0, 1] - this[1, 0]) * mult;
                    if (t1s[0].AlmostEqual(0) && t1s[1].AlmostEqual(0) && t1s[2].AlmostEqual(0))
                    {
                        goto case 0;
                    }
                    break;
                case 0:
                    t1s[0] = biggestVal;
                    t1s[3] = (this[1, 2] - this[2, 1]) * mult;
                    t1s[1] = (this[0, 1] + this[1, 0]) * mult;
                    t1s[2] = (this[2, 0] + this[0, 2]) * mult;
                    if (t1s[3].AlmostEqual(0))
                    {
                        goto case 1;
                    }
                    break;
                case 1:
                    t1s[1] = biggestVal;
                    t1s[3] = (this[2, 0] - this[0, 2]) * mult;
                    t1s[0] = (this[0, 1] + this[1, 0]) * mult;
                    t1s[2] = (this[1, 2] + this[2, 1]) * mult;
                    if (t1s[3].AlmostEqual(0))
                    {
                        goto case 2;
                    }
                    break;
                case 2:
                    t1s[2] = biggestVal;
                    t1s[3] = (this[0, 1] - this[1, 0]) * mult;
                    t1s[0] = (this[2, 0] + this[0, 2]) * mult;
                    t1s[1] = (this[1, 2] + this[2, 1]) * mult;
                    break;
            }
            */
            #endregion
            #region 对三行一的矩阵转换并不是很正确
            /*
            if (t1s[3].AlmostEqual(0))
            {
                if(Math.Abs(this[0, 0]).AlmostEqual(1))
                {
                    if(Math.Abs(this[1, 2]).AlmostEqual(1) && Math.Abs(this[2, 1]).AlmostEqual(1))
                    {
                        return new Quaternion((float)Math.Sin(Math.PI / 4), 0,0, (float)Math.Cos(Math.PI / 4)) as T;
                    }
                }
                if (Math.Abs(this[1, 0]).AlmostEqual(1))
                {
                    if (Math.Abs(this[1, 2]).AlmostEqual(1) && Math.Abs(this[2, 1]).AlmostEqual(1))
                    {
                        return new Quaternion((float)Math.Sin(Math.PI / 4), 0, 0, (float)Math.Cos(Math.PI / 4)) as T;
                    }
                }
                if (Math.Abs(this[1, 1]).AlmostEqual(1))
                {
                    if (Math.Abs(this[0, 2]).AlmostEqual(1) && Math.Abs(this[2, 0]).AlmostEqual(1))
                    {
                        return new Quaternion(0, (float)Math.Sin(Math.PI / 4),  0, (float)Math.Cos(Math.PI / 4)) as T;
                    }
                }
                if (Math.Abs(this[2, 2]).AlmostEqual(1))
                {
                    if (Math.Abs(this[1, 0]).AlmostEqual(1) && Math.Abs(this[0, 1]).AlmostEqual(1))
                    {
                        return new Quaternion((float)Math.Sin(Math.PI / 4), 0, 0, (float)Math.Cos(Math.PI / 4)) as T;
                    }
                }
            }
            return new Quaternion(t1s) as T;
            */
            #endregion           
        }

        private int GetElementIndex(int column, int row)
        {
            return column * 3 + row;
        }

        /// <summary>
        /// Frobenius范数，矩阵中的元素的平方和再开方
        /// </summary>
        /// <returns></returns>
        private float ComputeFrobeniusNorm()
        {
            return (float)Math.Sqrt(this.ToArray().Select(c => c * c).Sum());
        }

        private float OffDiagonalFrobeniusNorm()
        {
            var norm = 0.0;
            for (var i = 0; i < 3; ++i)
            {
                var temp = this[rowVal[i], colVal[i]];
                norm += 2.0 * temp * temp;
            }

            return (float)Math.Sqrt(norm);
        }

        //col和row与Cesium相反，作了相应调换
        static int[] colVal = { 1, 0, 0 };
        static int[] rowVal = { 2, 2, 1 };

        /// <summary>
        /// This routine was created based upon Matrix Computations, 3rd ed., by Golub and Van Loan,
        // section 8.4.2 The 2by2 Symmetric Schur Decomposition.
        ///
        /// The routine takes a matrix, which is assumed to be symmetric, and
        /// finds the largest off-diagonal term, and then creates
        // a matrix (result) which can be used to help reduce it
        /// </summary>
        /// <returns></returns>
        private T ShurDecomposition<T>() where T : class, IMatrix3x3<float>
        {
            float tolerance = 1e-15f;
            float maxDiagonal = 0.0f;
            int rotAxis = 1;

            // find pivot (rotAxis) based on max diagonal of matrix
            for (int i = 0; i < 3; ++i)
            {
                float temp = Math.Abs(this[rowVal[i], colVal[i]]);
                if (temp > maxDiagonal)
                {
                    rotAxis = i;
                    maxDiagonal = temp;
                }
            }
            float c = 1.0f;
            float s = 0.0f;

            int p = rowVal[rotAxis];
            int q = colVal[rotAxis];

            if (Math.Abs(this[p,q]) > tolerance)
            {
                float qq = this[q, q];
                float pp = this[p, p];
                float qp = this[p, q];

                float tau = (qq - pp) / 2.0f / qp;
                float t = 0.0f;
                if (tau < 0)
                {
                    t = -1.0f / (-tau + (float)Math.Sqrt(1.0f + tau * tau));
                }
                else
                {
                    t = 1.0f / (tau + (float)Math.Sqrt(1.0f + tau * tau));
                }

                c = 1.0f / (float)Math.Sqrt(1.0f + t * t);
                s = t * c;
            }

            //Matrix3x3 result = new Matrix3x3();  /*Matrix3x3.Identity.Clone();*/
            var result = New<T>();
            result[p, p] = result[q, q] = c;
            result[p, q] = s;
            result[q, p] = -s;
            return result;
        }
        /// <summary>
        /// 行优先，与Cesium对应方法相反
        /// </summary>
        /// <returns></returns>

        //private int GetElementIndex(int coloum, int row)
        //{
        //    return row + coloum * 3;
        //}

        /// <summary>
        /// Computes the eigenvectors and eigenvalues of a symmetric matrix.
        /// </summary>
        /// <returns>{Item1: unitaryMatrix, Item2: diagMatrix}</returns>
        public Tuple<T, T> ComputeEigenDecomposition<T>() where T : class, IMatrix3x3<float>
        {
            float tolerance = 1e-20f;
            int maxSweeps = 10;

            int count = 0;
            int sweep = 0;

            float epsilon = tolerance* ComputeFrobeniusNorm();

            var unitaryMatrix = New<T>();
            Matrix3x3 diagMatrix = new Matrix3x3(this);

            while (sweep < maxSweeps && diagMatrix.OffDiagonalFrobeniusNorm()> epsilon)
            {
                var jMatrix = diagMatrix.ShurDecomposition<T>();
                var jTransposepose = jMatrix.Transpose<T>();
                diagMatrix = diagMatrix.Multiply(jMatrix).To<Matrix3x3>();
                diagMatrix = jTransposepose.Multiply(diagMatrix);
                unitaryMatrix = unitaryMatrix.Multiply(jMatrix);

                if (++count > 2)
                {
                    ++sweep;
                    count = 0;
                }
            }

            return new Tuple<T, T>(unitaryMatrix as T, diagMatrix as T);
        }

        /// <summary>
        /// 创造一个绕Z轴旋转的4X4矩阵
        /// </summary>
        /// <param name = "radians" ></ param >
        /// < returns ></ returns >
        public static T CreateRotationZ<T>(float radians) where T : class, IMatrix3x3<float>
        {
            float num =(float) Math.Cos(radians);
            float num2 = (float)Math.Sin(radians);
            var result = New<T>();
            result[0,0] = num;
            result[0,1] = num2;
            result[1,0] = -num2;
            result[1,1] = num;
            return result;
        }
    }

    /// <summary>
    /// Cesium
    public class Matrix3x3D : MatrixNxND, IMatrix3x3D
    {
        public Matrix3x3D() : base(3) { }
        public Matrix3x3D(double[] arr) : base(arr, 3) { }
        public Matrix3x3D(Matrix3x3D other) : base(other) { }

        public Matrix3x3D(double column0Row0, double column1Row0, double column2Row0,
                           double column0Row1, double column1Row1, double column2Row1,
                           double column0Row2, double column1Row2, double column2Row2) : base(3)
        {
            Mat[0, 0] = column0Row0;
            Mat[1, 0] = column0Row1;
            Mat[2, 0] = column0Row2;
            Mat[0, 1] = column1Row0;
            Mat[1, 1] = column1Row1;
            Mat[2, 1] = column1Row2;
            Mat[0, 2] = column2Row0;
            Mat[1, 2] = column2Row1;
            Mat[2, 2] = column2Row2;
        }
       
        public static Matrix3x3D Identity
        {
            get
            {
                return new Matrix3x3D(
                    1, 0, 0,
                    0, 1, 0,
                    0, 0, 1
                    );
            }
        }

        /// <summary>
        /// 矩阵求逆（伴随矩阵法），自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override T Inverse<T>()
        {
            //直接计算
            var m11 = this[0, 0];
            var m12 = this[0, 1];
            var m13 = this[0, 2];
            var m21 = this[1, 0];
            var m22 = this[1, 1];
            var m23 = this[1, 2];
            var m31 = this[2, 0];
            var m32 = this[2, 1];
            var m33 = this[2, 2];

            var determinant = m11 * (m22 * m33 - m32 * m23) + m21 * (m32 * m13 - m12 * m33) + m31 * (m12 * m23 - m22 * m13);

            if (determinant == 0.0f)
            {
                Exception myException = new Exception("没有逆矩阵");
                throw myException;
            }

            this[0, 0] = m22 * m33 - m23 * m32;
            this[1, 0] = m23 * m31 - m21 * m33;
            this[2, 0] = m21 * m32 - m22 * m31;
            this[0, 1] = m13 * m32 - m12 * m33;
            this[1, 1] = m11 * m33 - m13 * m31;
            this[2, 1] = m12 * m31 - m11 * m32;
            this[0, 2] = m12 * m23 - m13 * m22;
            this[1, 2] = m13 * m21 - m11 * m23;
            this[2, 2] = m11 * m22 - m12 * m21;
            return this.Divide<T>(determinant);
        }

        /// <summary>
        /// 矩阵求逆（伴随矩阵法），自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override bool TryInverseToOther<T>(out T result)
        {
            result = New(this as T);
            try
            {
                result.Inverse<T>();
            }
            catch
            {
                result = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 通过向量缩放，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T ScaleByVector<T>(IVector3<double> vector) where T : class, IMatrix3x3<double>
        {
            this[0, 0] *= vector.X;
            this[0, 1] *= vector.X;
            this[0, 2] *= vector.X;

            this[1, 0] *= vector.Y;
            this[1, 1] *= vector.Y;
            this[1, 2] *= vector.Y;

            this[2, 0] *= vector.Z;
            this[2, 1] *= vector.Z;
            this[2, 2] *= vector.Z;
            return this as T;
        }

        /// <summary>
        /// 通过向量缩放，产生新的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T ScaleByVectorToOther<T>(IVector3<double> vector) where T : class, IMatrix3x3<double>
        {
            T Mc = New(this as T);
            return Mc.ScaleByVector<T>(vector);
        }

        /// <summary>
        /// 转成欧拉角
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToEulerAngle<T>() where T : class, IEulerAngle<double>
        {
            IEulerAngle<double> result = new EulerAngleD();
            //通过m32计算sin(pitch)
            double sp = -this[2, 1];

            //检查万向锁
            if (Math.Abs(sp) > 9.99999f)
            {
                //向正上方或正下方看
                result.Pitch = (double)Math.PI * sp;
                //back置零，计算heading
                result.Heading = (double)Math.Atan2(-this[1, 2], this[0, 0]);
                result.Bank = 0f;
            }
            else
            {
                result.Heading = (double)Math.Atan2(this[2, 0], this[2, 2]);
                result.Pitch = (double)Math.Asin(sp);
                result.Bank = (double)Math.Atan2(this[0, 1], this[1, 1]);
            }
            return result as T;
        }

        public T ToQuaternionByCesium<T>() where T : class, IQuaternion<double>
        {
            #region 抄Cesium源码的

            Quaternion result = new Quaternion();

            double root;
            double x;
            double y;
            double z;
            double w;

            var m00 = this[0, 0];
            var m11 = this[1, 1];
            var m22 = this[2, 2];
            var trace = m00 + m11 + m22;

            if (trace > 0.0)
            {
                // |w| > 1/2, may as well choose w > 1/2
                root = Math.Sqrt(trace + 1.0); // 2w
                w = 0.5 * root;
                root = 0.5 / root; // 1/(4w)

                x = (this[2, 1] - this[1, 2]) * root;
                y = (this[0, 2] - this[2, 0]) * root;
                z = (this[1, 0] - this[0, 1]) * root;
            }
            else
            {
                // |w| <= 1/2
                int[] fromRotationMatrixNext = { 1, 2, 0 };
                var next = fromRotationMatrixNext;

                var i = 0;
                if (m11 > m00)
                {
                    i = 1;
                }
                if (m22 > m00 && m22 > m11)
                {
                    i = 2;
                }
                var j = next[i];
                var k = next[j];

                root = Math.Sqrt(
                  this[GetElementIndex(i, i)] -
                    this[GetElementIndex(j, j)] -
                    this[GetElementIndex(k, k)] +
                    1.0
                );

                double[] fromRotationMatrixQuat = new double[3];
                var quat = fromRotationMatrixQuat;
                quat[i] = 0.5 * root;
                root = 0.5 / root;
                w =
                  (this[GetElementIndex(k, j)] -
                    this[GetElementIndex(j, k)]) *
                  root;
                quat[j] =
                  (this[GetElementIndex(j, i)] +
                    this[GetElementIndex(i, j)]) *
                  root;
                quat[k] =
                  (this[GetElementIndex(k, i)] +
                    this[GetElementIndex(i, k)]) *
                  root;

                x = -quat[0];
                y = -quat[1];
                z = -quat[2];
            }

            result.X = (float)x;
            result.Y = (float)y;
            result.Z = (float)z;
            result.W = (float)w;
            return result as T;

            #endregion
        }

        /// <summary>
        /// 转成四元数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToQuaternion<T>() where T : class, IQuaternion<double>
        {
            #region 原来的
                        
            int[] fromRotationMatrixNext = new int[3] { 1, 2, 0 };
            double[] fromRotationMatrixQuat = new double[3];

            double root;
            double x;
            double y;
            double z;
            double w;

            double m00 = this[0,0];
            double m11 = this[1,1];
            double m22 = this[2,2];
            double trace = m00 + m11 + m22;

            if (trace > 0.0)
            {
                // |w| > 1/2, may as well choose w > 1/2
                root = (double)Math.Sqrt(trace + 1.0f); // 2w
                w = 0.5f * root;
                root = 0.5f / root; // 1/(4w)

                x = (this[2, 1] - this[1, 2]) * root;
                y = (this[0, 2] - this[2, 0]) * root;
                z = (this[1, 0] - this[0, 1]) * root;
            }
            else
            {
                // |w| <= 1/2
                int[] next = fromRotationMatrixNext;
                int i = 0;
                if (m11 > m00)
                {
                    i = 1;
                }
                if (m22 > m00 && m22 > m11)
                {
                    i = 2;
                }
                int j = next[i];
                int k = next[j];
                root = (double)Math.Sqrt(this[i, i] - this[j, j] - this[k, k] + 1.0);

                var quat = fromRotationMatrixQuat;
                quat[i] = 0.5f * root;
                root = 0.5f / root;
                w = (this[k, j] - this[j, k]) * root;
                quat[j] = (this[j, i] + this[i, j]) * root;
                quat[k] = (this[k, i] + this[i, k]) * root;

                x = -quat[0];
                y = -quat[1];
                z = -quat[2];
            }
            return new QuaternionD(x, y, z, w) as T;
            
            #endregion
        }

        private int GetElementIndex(int column, int row)
        {
            return column * 3 + row;
        }


        /// <summary>
        /// Frobenius范数，矩阵中的元素的平方和再开方
        /// </summary>
        /// <returns></returns>
        private double ComputeFrobeniusNorm()
        {
            return (double)Math.Sqrt(this.ToArray().Select(c => c * c).Sum());
        }

        private double OffDiagonalFrobeniusNorm()
        {
            var norm = 0.0;
            for (var i = 0; i < 3; ++i)
            {
                var temp = this[rowVal[i],colVal[i]];
                norm += 2.0 * temp * temp;
            }

            return (double)Math.Sqrt(norm);
        }

        //col和row与Cesium相反，作了相应调换
        static int[] colVal = { 1, 0, 0 };
        static int[] rowVal = { 2, 2, 1 };

        /// <summary>
        /// This routine was created based upon Matrix Computations, 3rd ed., by Golub and Van Loan,
        // section 8.4.2 The 2by2 Symmetric Schur Decomposition.
        ///
        /// The routine takes a matrix, which is assumed to be symmetric, and
        /// finds the largest off-diagonal term, and then creates
        // a matrix (result) which can be used to help reduce it
        /// </summary>
        /// <returns></returns>
        private T ShurDecomposition<T>() where T : class, IMatrix3x3<double>
        {
            double tolerance = 1e-15f;
            double maxDiagonal = 0.0f;
            int rotAxis = 1;

            // find pivot (rotAxis) based on max diagonal of matrix
            for (int i = 0; i < 3; ++i)
            {
                double temp = Math.Abs(this[rowVal[i],colVal[i]]);
                if (temp > maxDiagonal)
                {
                    rotAxis = i;
                    maxDiagonal = temp;
                }
            }
            double c = 1.0f;
            double s = 0.0f;

            int p = rowVal[rotAxis];
            int q = colVal[rotAxis];

            if (Math.Abs(this[p, q]) > tolerance)
            {
                double qq = this[q, q];
                double pp = this[p, p];
                double qp = this[p,q];

                double tau = (qq - pp) / 2.0f / qp;
                double t = 0.0f;
                if (tau < 0)
                {
                    t = -1.0f / (-tau + (double)Math.Sqrt(1.0f + tau * tau));
                }
                else
                {
                    t = 1.0f / (tau + (double)Math.Sqrt(1.0f + tau * tau));
                }

                c = 1.0f / (double)Math.Sqrt(1.0f + t * t);
                s = t * c;
            }

            //Matrix3x3 result = new Matrix3x3();  /*Matrix3x3.Identity.Clone();*/
            var result = New<T>();
            result[p, p] = result[q,q] = c;
            result[p,q] = s;
            result[q,p] = -s;
            return result;
        }

        /// <summary>
        /// Computes the eigenvectors and eigenvalues of a symmetric matrix.
        /// </summary>
        /// <returns>{Item1: unitaryMatrix, Item2: diagMatrix}</returns>
        public Tuple<T, T> ComputeEigenDecomposition<T>() where T : class, IMatrix3x3<double>
        {
            double tolerance = 1e-20f;
            int maxSweeps = 10;

            int count = 0;
            int sweep = 0;

            double epsilon = tolerance * ComputeFrobeniusNorm();

            var unitaryMatrix = New<T>();
            var diagMatrix = new Matrix3x3D(this);

            while (sweep < maxSweeps && diagMatrix.OffDiagonalFrobeniusNorm() > epsilon)
            {
                var jMatrix = diagMatrix.ShurDecomposition<T>();
                var jTransposepose = jMatrix.Transpose<T>();
                diagMatrix = diagMatrix.Multiply(jMatrix).To<Matrix3x3D>();
                diagMatrix = jTransposepose.Multiply(diagMatrix);
                unitaryMatrix = unitaryMatrix.Multiply(jMatrix);

                if (++count > 2)
                {
                    ++sweep;
                    count = 0;
                }
            }

            return new Tuple<T, T>(unitaryMatrix as T, diagMatrix as T);
        }

        /// <summary>
        /// 创造一个绕Z轴旋转的4X4矩阵
        /// </summary>
        /// <param name = "radians" ></ param >
        /// < returns ></ returns >
        public static T CreateRotationZ<T>(double radians) where T : class, IMatrix3x3<double>
        {
            double num = (double)Math.Cos(radians);
            double num2 = (double)Math.Sin(radians);
            var result = New<T>();
            result[0,0] = num;
            result[0,1] = num2;
            result[1,0] = -num2;
            result[1,1] = num;
            return result;
        }
    }
}
