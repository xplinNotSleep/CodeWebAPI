using System;

namespace AgDataHandle.Maths
{
    public class MatrixNxN : Matrix, IMatrixNxN
    {
        public MatrixNxN():base() { }
        /// <summary>
        /// 默认单位矩阵
        /// </summary>
        /// <param name="n"></param>
        public MatrixNxN(int row) : base(row,row)
        {
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < row; j++)
                {
                    if (i == j)
                    {
                        Mat[i, j] = 1;
                    }
                    else
                    {
                        Mat[i, j] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 对称矩阵
        /// </summary>
        /// <param name="n"></param>
        public MatrixNxN(float[] a, int row) : base(a,row, row)
        {
            if (a.Length != row * row)
            {
                throw new Exception("输入的数组不满足对称矩阵");
            }
        }

        /// <summary>
        /// 对称矩阵
        /// </summary>
        /// <param name="n"></param>
        public MatrixNxN(float[,] a, int row) : base(a, row, row)
        {
        }

        /// <summary>
        /// 复制别的矩阵
        /// </summary>
        /// <param name="other"></param>
        public MatrixNxN(MatrixNxN other) : this(other.row)
        {
            Array.Copy(other.Mat, 0, Mat, 0, other.row * other.row);
        }

        /// <summary>
        /// 是否正交矩阵
        /// </summary>
        /// <returns></returns>
        public virtual bool IsOrthogonal()
        {
            for (var i = 0; i < row; i++)
            {
                float store = 0;
                for (var j = 0; j < column; j++)
                {
                    store +=(float)Math.Pow(this[i, j],2);
                }
                if (store!=1)
                {
                    return false;
                }
            }
            //对于每一行向量
            for (var i = 0; i < row - 1; i++)
            {
                float pointMuti = 0;
                //相对于i行外的向量
                for (var j = i + 1; j < row; j++)
                {
                    //每一列对应位置的乘积，其实就是i行向量乘以j行变量的积
                    for (var k = 0; k < column; k++)
                    {
                        pointMuti +=this[i, k]* this[j, k];
                    }
                }
                //如果它们的积不为0,也就是不垂直，则判断矩阵非正交
                if (pointMuti!=0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否可转换为正交矩阵
        /// </summary>
        /// <returns></returns>
        public virtual bool IsCanOrthogonal()
        {
            float lastStore = default(float);
            for (var i = 0; i < row; i++)
            {
                float store = default(float);
                for (var j = 0; j < column; j++)
                {
                    store += (float)Math.Pow(this[i, j], 2);
                    if (i != 0 && store!= lastStore)
                    {
                        return false;
                    }
                    lastStore = store;
                }
            }
            //对于每一行向量
            for (var i = 0; i < row - 1; i++)
            {
                float pointMuti = default(float);
                //相对于i行外的向量
                for (var j = i + 1; j < row; j++)
                {
                    //每一列对应位置的乘积，其实就是i行向量乘以j行变量的积
                    for (var k = 0; k < column; k++)
                    {
                        pointMuti += this[i, k] * this[j, k];
                    }
                }
                //如果它们的积不为0,也就是不垂直，则判断矩阵非正交
                if (pointMuti != 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否单位矩阵
        /// </summary>
        /// <returns></returns>
        public bool IsIdentity()
        {
            for (var i = 0; i < row; i++)
                for (var j = 0; j < column; j++)
                {
                    if (i == j && this[i, j]!=1)
                    {
                        return false;
                    }
                    if (i != j && this[i, j]!=0)
                    {
                        return false;
                    }
                }
            return true;
        }

        /// <summary>
        /// 矩阵除法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T Divide<T>(T Mb) where T : class, IMatrixNxN<float>
        {
            return Multiply<T>(Mb.Inverse<T>());
        }

        /// <summary>
        /// 矩阵除法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T DivideToOther<T>(T Mb) where T : class, IMatrixNxN<float>
        {
            return MultiplyToOther<T>(Mb.InverseToOther<T>());
        }

        /// <summary>
        /// 矩阵求逆（伴随矩阵法）
        /// </summary>
        /// <returns></returns>
        public virtual T Inverse<T>() where T : class, IMatrixNxN<float>
        {
            float d = GetDeterminant<T>();
            if (d== 0)
            {
                Exception myException = new Exception("没有逆矩阵");
                throw myException;
            }
            T Ax = GetAdjoint<T>();
            T At = Ax.Transpose<T>();
            T An = At.Divide<T>(d);
            return An;
        }
        public virtual bool TryInverseToOther<T>(out T result) where T : class, IMatrixNxN<float>
        {
            float d = GetDeterminant<T>();
            if (d == 0)
            {
                result = null;
                return false;
            }
            T Ax = GetAdjoint<T>();
            T At = Ax.Transpose<T>();
            T An = At.Divide<T>(d);
            result = An;
            return true;
        }

        /// <summary>
        /// 矩阵求逆（伴随矩阵法）
        /// </summary>
        /// <returns></returns>
        public virtual T InverseToOther<T>() where T : class, IMatrixNxN<float>
        {
            T Mc = New(this as T);
            return Mc.Inverse<T>();
        }


        /// <summary>
        /// 矩阵的行列式,矩阵必须是方阵
        /// </summary>
        /// <typeparam name="T">class, IMatrix</typeparam>
        /// <returns></returns>
        public virtual float GetDeterminant<T>() where T : class, IMatrixNxN<float>
        {
            float[,] a = Mat;
            if (row == 1) return a[0, 0];

            float D = 0;
            for (int i = 0; i < row; i++)
            {
                D += a[i, 1]* GetCofactor<MatrixNxN>(i, 1).GetDeterminant<MatrixNxN>();
            }
            return D;
        }

        /// <summary>
        /// 矩阵的伴随矩阵
        /// </summary>
        /// <returns></returns>
        public virtual T GetAdjoint<T>() where T : class, IMatrixNxN<float>
        {
            T Mc = New<T>(row, column);
            float[,] c = Mc.Mat;
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    c[i, j] = GetCofactor<T>(i, j).GetDeterminant<T>();
            return Mc;
        }

        /// <summary>
        /// 对应行列式的代数余子式矩阵
        /// </summary>
        /// <param name="ai"></param>
        /// <param name="aj"></param>
        /// <returns></returns>
        public virtual T GetCofactor<T>(int ai, int aj) where T : class, IMatrixNxN<float>
        {
            int n2 = row - 1;
            T Mc = New<T>(n2);
            float[,] a = Mat;
            float[,] b = Mc.Mat;

            //左上
            for (int i = 0; i < ai; i++)
                for (int j = 0; j < aj; j++)
                {
                    b[i, j] = a[i, j];
                }
            //右下
            for (int i = ai; i < n2; i++)
                for (int j = aj; j < n2; j++)
                {
                    b[i, j] = a[i + 1, j + 1];
                }
            //右上
            for (int i = 0; i < ai; i++)
                for (int j = aj; j < n2; j++)
                {
                    b[i, j] = a[i, j + 1];
                }
            //左下
            for (int i = ai; i < n2; i++)
                for (int j = 0; j < aj; j++)
                {
                    b[i, j] = a[i + 1, j];
                }
            //符号位
            if ((ai + aj) % 2 != 0)
            {
                for (int i = 0; i < n2; i++)
                    b[i, 0] = -b[i, 0];

            }
            return Mc;

        }
    }

    public class MatrixNxND : MatrixD, IMatrixNxND
    {
        public MatrixNxND() : base() { }
        /// <summary>
        /// 默认单位矩阵
        /// </summary>
        /// <param name="n"></param>
        public MatrixNxND(int row) : base(row, row)
        {
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < row; j++)
                {
                    if (i == j)
                    {
                        Mat[i, j] = 1;
                    }
                    else
                    {
                        Mat[i, j] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 对称矩阵
        /// </summary>
        /// <param name="n"></param>
        public MatrixNxND(double[] a, int row) : base(a, row,row)
        {
            if (a.Length != row * row)
            {
                throw new Exception("输入的数组不满足对称矩阵");
            }
        }

        /// <summary>
        /// 对称矩阵
        /// </summary>
        /// <param name="n"></param>
        public MatrixNxND(double[,] a, int row) : base(a, row, row)
        {
        }

        /// <summary>
        /// 复制别的矩阵
        /// </summary>
        /// <param name="other"></param>
        public MatrixNxND(MatrixNxND other) : this(other.row)
        {
            Array.Copy(other.Mat, 0, Mat, 0, other.row * other.row);
        }

        /// <summary>
        /// 是否正交矩阵
        /// </summary>
        /// <returns></returns>
        public virtual bool IsOrthogonal()
        {
            for (var i = 0; i < row; i++)
            {
                double store = 0;
                for (var j = 0; j < column; j++)
                {
                    store += (double)Math.Pow(this[i, j], 2);
                }
                if (store != 1)
                {
                    return false;
                }
            }
            //对于每一行向量
            for (var i = 0; i < row - 1; i++)
            {
                double pointMuti = 0;
                //相对于i行外的向量
                for (var j = i + 1; j < row; j++)
                {
                    //每一列对应位置的乘积，其实就是i行向量乘以j行变量的积
                    for (var k = 0; k < column; k++)
                    {
                        pointMuti += this[i, k] * this[j, k];
                    }
                }
                //如果它们的积不为0,也就是不垂直，则判断矩阵非正交
                if (pointMuti != 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否可转换为正交矩阵
        /// </summary>
        /// <returns></returns>
        public virtual bool IsCanOrthogonal()
        {
            double lastStore = default(double);
            for (var i = 0; i < row; i++)
            {
                double store = default(double);
                for (var j = 0; j < column; j++)
                {
                    store += (double)Math.Pow(this[i, j], 2);
                    if (i != 0 && store != lastStore)
                    {
                        return false;
                    }
                    lastStore = store;
                }
            }
            //对于每一行向量
            for (var i = 0; i < row - 1; i++)
            {
                double pointMuti = default(double);
                //相对于i行外的向量
                for (var j = i + 1; j < row; j++)
                {
                    //每一列对应位置的乘积，其实就是i行向量乘以j行变量的积
                    for (var k = 0; k < column; k++)
                    {
                        pointMuti += this[i, k] * this[j, k];
                    }
                }
                //如果它们的积不为0,也就是不垂直，则判断矩阵非正交
                if (pointMuti != 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否单位矩阵
        /// </summary>
        /// <returns></returns>
        public bool IsIdentity()
        {
            for (var i = 0; i < row; i++)
                for (var j = 0; j < column; j++)
                {
                    if (i == j && this[i, j] != 1)
                    {
                        return false;
                    }
                    if (i != j && this[i, j] != 0)
                    {
                        return false;
                    }
                }
            return true;
        }

        /// <summary>
        /// 矩阵除法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T Divide<T>(T Mb) where T : class, IMatrixNxN<double>
        {
            return Multiply<T>(Mb.Inverse<T>());
        }

        /// <summary>
        /// 矩阵除法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T DivideToOther<T>(T Mb) where T : class, IMatrixNxN<double>
        {
            return MultiplyToOther<T>(Mb.InverseToOther<T>());
        }

        /// <summary>
        /// 矩阵求逆（伴随矩阵法）
        /// </summary>
        /// <returns></returns>
        public virtual T Inverse<T>() where T : class, IMatrixNxN<double>
        {
            double d = GetDeterminant<T>();
            if (d == 0)
            {
                Exception myException = new Exception("没有逆矩阵");
                throw myException;
            }
            T Ax = GetAdjoint<T>();
            T At = Ax.Transpose<T>();
            T An = At.Divide<T>(d);
            return An;
        }
        public virtual bool TryInverseToOther<T>(out T result) where T : class, IMatrixNxN<double>
        {
            double d = GetDeterminant<T>();
            if (d == 0)
            {
                result = null;
                return false;
            }
            T Ax = GetAdjoint<T>();
            T At = Ax.Transpose<T>();
            T An = At.Divide<T>(d);
            result = An;
            return true;
        }

        /// <summary>
        /// 矩阵求逆（伴随矩阵法）
        /// </summary>
        /// <returns></returns>
        public virtual T InverseToOther<T>() where T : class, IMatrixNxN<double>
        {
            T Mc = New(this as T);
            return Mc.Inverse<T>();
        }


        /// <summary>
        /// 矩阵的行列式,矩阵必须是方阵
        /// </summary>
        /// <typeparam name="T">class, IMatrix</typeparam>
        /// <returns></returns>
        public virtual double GetDeterminant<T>() where T : class, IMatrixNxN<double>
        {
            double[,] a = Mat;
            if (row == 1) return a[0, 0];

            double D = 0;
            for (int i = 0; i < row; i++)
            {
                D += a[i, 1] * GetCofactor<T>(i, 1).GetDeterminant<T>();
            }
            return D;
        }

        /// <summary>
        /// 矩阵的伴随矩阵
        /// </summary>
        /// <returns></returns>
        public virtual T GetAdjoint<T>() where T : class, IMatrixNxN<double>
        {
            T Mc = New<T>(row, column);
            double[,] c = Mc.Mat;
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    c[i, j] = GetCofactor<T>(i, j).GetDeterminant<T>();
            return Mc;
        }

        /// <summary>
        /// 对应行列式的代数余子式矩阵
        /// </summary>
        /// <param name="ai"></param>
        /// <param name="aj"></param>
        /// <returns></returns>
        public virtual T GetCofactor<T>(int ai, int aj) where T : class, IMatrixNxN<double>
        {
            int n2 = row - 1;
            T Mc = New<T>(n2, n2);
            double[,] a = Mat;
            double[,] b = Mc.Mat;

            //左上
            for (int i = 0; i < ai; i++)
                for (int j = 0; j < aj; j++)
                {
                    b[i, j] = a[i, j];
                }
            //右下
            for (int i = ai; i < n2; i++)
                for (int j = aj; j < n2; j++)
                {
                    b[i, j] = a[i + 1, j + 1];
                }
            //右上
            for (int i = 0; i < ai; i++)
                for (int j = aj; j < n2; j++)
                {
                    b[i, j] = a[i, j + 1];
                }
            //左下
            for (int i = ai; i < n2; i++)
                for (int j = 0; j < aj; j++)
                {
                    b[i, j] = a[i + 1, j];
                }
            //符号位
            if ((ai + aj) % 2 != 0)
            {
                for (int i = 0; i < n2; i++)
                    b[i, 0] = -b[i, 0];

            }
            return Mc;

        }
    }
}
