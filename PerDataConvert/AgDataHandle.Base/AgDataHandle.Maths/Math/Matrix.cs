using System;
using System.Collections.Generic;

namespace AgDataHandle.Maths
{
    public class Matrix :IMatrix
    {
        public Matrix() { }
        #region 矩阵基础信息
        /// <summary>
        /// [行,列]
        /// </summary>
        public float[,] Mat { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        //public static NumberBase<TNum> Num => NumberBase<TNum>.Num;
        #endregion

        public Matrix(int row, int column)
        {
            this.column = column;
            this.row = row;
            Mat = new float[row, column];
        }

        public Matrix(float[,] a, int row, int column)
        {
            this.column = column;
            this.row = row;
            Mat = new float[row, column];
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    Mat[i, j] = a[i, j];
                }
            }
        }

        public Matrix(float[] a, int row, int column)
        {
            this.column = column;
            this.row = row;
            Mat = new float[row, column];
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    Mat[i, j] = a[i * column + j];
                }
            }
        }

        public Matrix(Matrix other) : this(other.row, other.column)
        {
            Array.Copy(other.Mat, 0, Mat, 0, other.column * other.row);
        }

        /// <summary>
        /// 根据行列新建矩阵
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static T New<T>(int row = 0, int column = 0) where T : class, IMatrix<float>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IMatrix4x4))
            {
                model = new Matrix4x4() as T;
            }
            else if (typeT == typeof(IMatrix3x3))
            {
                model = new Matrix3x3() as T;
            }
            else if (typeT == typeof(IMatrixNxN))
            {
                model = new MatrixNxN(row) as T;
            }
            else if (typeT == typeof(IMatrix))
            {
                model = new Matrix(row,column) as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            if (row != 0)
            {
                model.row = row;
                if (column == 0)
                {
                    column = row;
                }
                model.column = column;
                model.Mat = new float[row, column];
            }
            return model;
        }

        /// <summary>
        /// 根据原有的其他矩阵新建相同行列的矩阵
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="other"></param>
        /// <returns></returns>
        public static T New<T>(T other) where T : class, IMatrix<float>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IMatrix4x4))
            {
                model = new Matrix4x4() as T;
            }
            else if (typeT == typeof(IMatrix3x3))
            {
                model = new Matrix3x3() as T;
            }
            else if (typeT == typeof(IMatrixNxN))
            {
                model = new MatrixNxN(other.row) as T;
            }
            else if (typeT == typeof(IMatrix))
            {
                model = new Matrix(other.row,other.column) as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            Array.Copy(other.Mat, 0, model.Mat, 0, other.column * other.row);
            model.row = other.row;
            model.column = other.column;
            return model;
        }
        #region 数据转换
        /// <summary>
        /// 强转，可转任意矩阵，如若异常会在实现的时候报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T To<T>() where T : class
        {
            var typeT = typeof(T);
            if (typeof(T) == GetType()) return (T)Convert.ChangeType(this, typeof(T));
            dynamic model = null;

            if (typeT == typeof(IMatrix4x4))
            {
                model = new Matrix4x4();
            }
            else if (typeT == typeof(IMatrix4x4D))
            {
                model = new Matrix4x4D();
            }
            else if (typeT == typeof(IMatrix3x3))
            {
                model = new Matrix3x3();
            }
            else if (typeT == typeof(IMatrix3x3D))
            {
                model = new Matrix3x3D();
            }
            else if (typeT == typeof(IMatrixNxN))
            {
                model = new MatrixNxN(row);
            }
            else if (typeT == typeof(IMatrixNxND))
            {
                model = new MatrixNxND(row);
            }
            else if (typeT == typeof(IMatrix))
            {
                model = new Matrix(row,column);
            }
            else if (typeT == typeof(IMatrixD))
            {
                model = new MatrixD(row, column);
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            var newModelType = model[0].GetType();
            var IsSame = this[0,0].GetType() == newModelType;
            if (IsSame)
            {
                Array.Copy(Mat, 0, model.Mat, 0, column * row);
            }
            else
            {
                if (newModelType == typeof(double))
                {
                    for (int i = 0; i < row; i++)
                        for (int j = 0; j < column; j++)
                        {
                            model[i, j] = this[i, j];
                        }
                }
                else
                {
                    for (int i = 0; i < row; i++)
                        for (int j = 0; j < column; j++)
                        {
                            model[i, j] = (float)this[i, j];
                        }
                }
            }
            return model as T;
        }

        /// <summary>
        /// 转成数组
        /// </summary>
        /// <returns></returns>
        public virtual float[] ToArray()
        {
            float[] array = new float[column * row];
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    array[i * column + j] = Mat[i, j];
                }
            }
            return array;
        }

        /// <summary>
        /// 转成列表
        /// </summary>
        /// <returns></returns>
        public virtual List<float> ToList()
        {
            List<float> list = new List<float>();
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    list.Add(Mat[i, j]);
                }
            }
            return list;
        }

        /// <summary>
        /// 转成浮点数组
        /// </summary>
        /// <returns></returns>
        public virtual float[] ToFloatArray()
        {
            float[] a = new float[column * row];
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    a[i * column + j] = Mat[i, j];
                }
            }
            return a;
        }

        /// <summary>
        /// 转成浮点列表
        /// </summary>
        /// <returns></returns>
        public virtual List<float> ToFloatList()
        {
            List<float> list = new List<float>();
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    list.Add(Mat[i, j]);
                }
            }
            return list;
        }

        /// <summary>
        /// 转成双精度数组
        /// </summary>
        /// <returns></returns>
        public virtual double[] ToDoubleArray()
        {
            double[] a = new double[column * row];
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    a[i * column + j] = Mat[i, j];
                }
            }
            return a;
        }

        /// <summary>
        /// 转成双精度列表
        /// </summary>
        /// <returns></returns>
        public virtual List<double> ToDoubleList()
        {
            List<double> list = new List<double>();
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    list.Add(Mat[i, j]);
                }
            }
            return list;
        }
        #endregion

        #region 下标获取信息
        /// <summary>
        /// 通过行列获取信息
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public virtual float this[int row, int column]
        {
            get
            {
                return this.Mat[row, column];
            }
            set
            {
                this.Mat[row, column] = value;
            }
        }

        /// <summary>
        /// 通过单下标获取信息，先行后列
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual float this[int index]
        {
            get { return Mat[index % row, index / row]; }
            set { this.Mat[index % row, index / row] = value; }
        }
        #endregion

        #region 加减乘除
        /// <summary>
        /// 矩阵加法
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T Add<T>(T Mb) where T : class, IMatrix<float>
        {
            if (Mb == null)
                return this as T;
            if ((column != Mb.column) || (row != Mb.row))
            {
                Exception myException = new Exception("数组维数不匹配");
                throw myException;
            }
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                {
                    this[i, j] += Mb[i, j];
                }
            return this as T;
        }

        /// <summary>
        /// 矩阵加法
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T AddToOther<T>(T Mb) where T : class, IMatrix<float>
        {
            T Mc = New(this as T);
            return Mc.Add<T>(Mb);
        }

        #region 矩阵+常数
        /// <summary>
        /// 矩阵数加
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T Add<T>(float k) where T : class, IMatrix<float>
        {
            for (int i = 0; i < column; i++)
                for (int j = 0; j < row; j++)
                    this[i, j] += k;
            return this as T;
        }

        /// <summary>
        /// 矩阵数加
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T AddToOther<T>(float k) where T : class, IMatrix<float>
        {
            T Mc = New(this as T);
            return Mc.Add<T>(k);
        }

        /// <summary>
        /// 矩阵数减，自身改变
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T Minus<T>(float k) where T : class, IMatrix<float>
        {
            for (int i = 0; i < column; i++)
                for (int j = 0; j < row; j++)
                    this[i, j] -= k;
            return this as T;
        }

        /// <summary>
        /// 矩阵数减,自身不变，产生新内存保存
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T MinusToOther<T>(float k) where T : class, IMatrix<float>
        {
            T Mc = New(this as T);
            return Mc.Minus<T>(k);
        }

        /// <summary>
        /// 矩阵数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T Divide<T>(float k) where T : class, IMatrix<float>
        {
            for (int i = 0; i < column; i++)
                for (int j = 0; j < row; j++)
                    this[i, j] /= k;
            return this as T;
        }

        /// <summary>
        /// 矩阵数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T DivideToOther<T>(float k) where T : class, IMatrix<float>
        {
            T Mc = New(this as T);
            return Mc.Divide<T>(k);
        }
        #endregion

        /// <summary>
        /// 矩阵减法
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T Minus<T>(T Mb) where T : class, IMatrix<float>
        {
            if (Mb == null)
                return this as T;
            if ((column != Mb.column) || (row != Mb.row))
            {
                Exception myException = new Exception("数组维数不匹配");
                throw myException;
            }
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    this[i, j] -= Mb[i, j];
            return this as T;
        }

        /// <summary>
        /// 矩阵减法
        /// </summary>
        /// <typeparam name="T">class, Matrix</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T MinusToOther<T>(T Mb) where T : class, IMatrix<float>
        {
            T Mc = New(this as T);
            return Mc.Minus<T>(Mb);
        }

        #region 矩阵X常数
        /// <summary>
        /// 矩阵数乘
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T Multiply<T>(float k) where T : class, IMatrix<float>
        {
            for (int i = 0; i < column; i++)
                for (int j = 0; j < row; j++)
                    this[i, j] *= k;
            //this[i, j] *= k;
            return this as T;
        }

        /// <summary>
        /// 矩阵数乘
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T MultiplyToOther<T>(float k) where T : class, IMatrix<float>
        {
            T Mc = New(this as T);
            return Mc.Multiply<T>(k);
        }
        public void MultiplyTest(Matrix3x3 B)
        {
            var t1 = Mat[0, 0] * B.Mat[0, 1] + Mat[0, 1] * B.Mat[1, 1] + Mat[0, 2] * B.Mat[2, 1];
            var t2 = Mat[1, 0] * B.Mat[0, 1] + Mat[1, 1] * B.Mat[1, 1] + Mat[1, 2] * B.Mat[2, 1];
            var t3 = Mat[2, 0] * B.Mat[0, 1] + Mat[2, 1] * B.Mat[1, 1] + Mat[2, 2] * B.Mat[2, 1];
        }
        public Vector3 Multiply(Vector3 p, Vector3 c)
        {
            var x = p.X - c.X;
            var y = p.Y - c.Y;
            var z = p.Z - c.Z;

            var dx = Mat[0, 0] * x + Mat[0, 1] * y + Mat[0, 2] * z;
            var dy = Mat[1, 0] * x + Mat[1, 1] * y + Mat[1, 2] * z;
            var dz = Mat[2, 0] * x + Mat[2, 1] * y + Mat[2, 2] * z;
            return new Vector3(dx, dy, dz);
        }
        /// <summary>
        /// 矩阵数乘
        /// </summary>
        /// <returns></returns>
        public virtual T Negate<T>() where T : class, IMatrix<float>
        {
            for (int i = 0; i < column; i++)
                for (int j = 0; j < row; j++)
                    this[i, j] = -this[i, j];
            return this as T;
        }

        /// <summary>
        /// 矩阵数乘
        /// </summary>
        /// <returns></returns>
        public virtual T NegateToOther<T>() where T : class, IMatrix<float>
        {
            T Mc = New(this as T);
            return Mc.Negate<T>();
        }
        #endregion

        #region 矩阵X矩阵
        /// <summary>
        /// 矩阵乘法
        /// </summary>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T Multiply<T>(T Mb) where T : class, IMatrix<float>
        {
            if (Mb == null)
                return this as T;
            int m2 = Mb.column;
            if (column != Mb.row)
            {
                Exception myException = new Exception("数组维数不匹配");
                throw myException;
            }
            T Mc = New<T>(row, m2);
            float[,] c = Mc.Mat;
            float[,] a = Mat;
            float[,] b = Mb.Mat;
            for (int i = 0; i < row; i++)
                for (int j = 0; j < m2; j++)
                {
                    c[i, j] = 0;
                    for (int k = 0; k < column; k++)
                        c[i, j] += a[i, k] * b[k, j];
                }
            return Mc;
        }

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T MultiplyToOther<T>(T Mb) where T : class, IMatrix<float>
        {
            T Mc = New(this as T);
            return Mc.Multiply<T>(Mb);
        }
        #endregion
        #endregion

        #region 转置
        /// <summary>
        /// 矩阵转置，自身改变
        /// </summary>
        /// <returns></returns>
        public virtual T Transpose<T>() where T : class, IMatrix<float>
        {
            if (row != column)
            {
                int tmp = row;
                row = column;
                column = tmp;
            }
            for (int i = 0; i < row - 1; i++)
                for (int j = i + 1; j < column; j++)
                    Swap(i, j, j, i);
            return this as T;
        }

        /// <summary>
        /// 矩阵转置，生成新的内存
        /// </summary>
        /// <returns></returns>
        public virtual T TransposeToOther<T>() where T : class, IMatrix<float>
        {
            T Mc = New<T>(this as T);
            return Mc.Transpose<T>();
        }


        /// <summary>
        /// 调换矩阵的下标值
        /// </summary>
        /// <param name="row1"></param>
        /// <param name="column1"></param>
        /// <param name="row2"></param>
        /// <param name="column2"></param>
        protected void Swap(int row1, int column1, int row2, int column2)
        {
            float tmp = Mat[row1, column1];
            Mat[row1, column1] = Mat[row2, column2];
            Mat[row2, column2] = tmp;
        }
        #endregion

        #region 转字符串与相等
        /// <summary>
        /// 转成字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            string result = string.Empty;
            foreach (var o in this.ToArray())
            {
                result += o + ",";
            }
            return result.TrimEnd(',');
        }


        /// <summary>
        /// 判断句很是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(IMatrix<float> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    if (!this[i, j].Equals(other[i, j]))
                        return false;
                }
            }
            return true;
        }
        #endregion

        #region 符号重载
        public static Matrix operator +(Matrix value1, Matrix value2)
        {
            Matrix Mc = New(value1);
            return Mc.Add(value2);
        }

        public static Matrix operator +(Matrix value1, float value2)
        {
            Matrix Mc = New(value1);
            return Mc.Add<Matrix>(value2);
        }

        public static Matrix operator +(float value1, Matrix value2)
        {
            Matrix Mc = New(value2);
            return Mc.Add<Matrix>(value1);
        }

        public static Matrix operator -(Matrix value1, Matrix value2)
        {
            Matrix Mc = New(value1);
            return Mc.Minus(value2);
        }

        public static Matrix operator -(Matrix value1, float value2)
        {
            Matrix Mc = New(value1);
            return Mc.Minus<Matrix>(value2);
        }

        public static Matrix operator -(float value1, Matrix value2)
        {
            Matrix Mc = New(value2);
            return Mc.Negate<Matrix>().Add<Matrix>(value1);
        }

        public static Matrix operator *(Matrix value1, Matrix value2)
        {
            Matrix Mc = New(value1);
            return Mc.Multiply(value2);
        }

        public static Matrix operator *(Matrix value1, float value2)
        {
            Matrix Mc = New(value1);
            return Mc.Multiply<Matrix>(value2);
        }

        public static Matrix operator *(float value1, Matrix value2)
        {
            Matrix Mc = New(value2);
            return Mc.Multiply<Matrix>(value1);
        }

        public static Matrix operator /(Matrix value1, float value2)
        {
            Matrix Mc = New(value1);
            return Mc.Divide<Matrix>(value2);
        }
        #endregion
    }

    public class MatrixD :IMatrixD
    {
        #region 矩阵基础信息
        /// <summary>
        /// [行,列]
        /// </summary>
        public double[,] Mat { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        #endregion

        public MatrixD() { }
        public MatrixD(int row, int column)
        {
            this.column = column;
            this.row = row;
            Mat = new double[row, column];
        }

        public MatrixD(double[,] a, int row, int column)
        {
            this.column = column;
            this.row = row;
            Mat = new double[row, column];
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    Mat[i, j] = a[i, j];
                }
            }
        }

        public MatrixD(double[] a, int row, int column)
        {
            this.column = column;
            this.row = row;
            Mat = new double[row, column];
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    Mat[i, j] = a[i * column + j];
                }
            }
        }

        public MatrixD(MatrixD other) : this(other.row, other.column)
        {
            Array.Copy(other.Mat, 0, Mat, 0, other.column * other.row);
        }

        /// <summary>
        /// 根据行列新建矩阵
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static T New<T>(int row = 0, int column = 0) where T : class, IMatrix<double>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IMatrix4x4D))
            {
                model = new Matrix4x4D() as T;
            }
            else if (typeT == typeof(IMatrix3x3D))
            {
                model = new Matrix3x3D() as T;
            }
            else if (typeT == typeof(IMatrixNxND))
            {
                model = new MatrixNxND(row) as T;
            }
            else if (typeT == typeof(IMatrixD))
            {
                model = new MatrixD(row,column) as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            if (row != 0)
            {
                model.row = row;
                if (column == 0)
                {
                    column = row;
                }
                model.column = column;
                model.Mat = new double[row, column];
            }
            return model;
        }

        /// <summary>
        /// 根据原有的其他矩阵新建相同行列的矩阵
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="other"></param>
        /// <returns></returns>
        public static T New<T>(T other) where T : class, IMatrix<double>
        {
            var typeT = typeof(T);
            T model = null;
            if (typeT == typeof(IMatrix4x4D))
            {
                model = new Matrix4x4D() as T;
            }
            else if (typeT == typeof(IMatrix3x3D))
            {
                model = new Matrix3x3D() as T;
            }
            else if (typeT == typeof(IMatrixNxND))
            {
                model = new MatrixNxND(other.row) as T;
            }
            else if (typeT == typeof(IMatrixD))
            {
                model = new MatrixD(other.row,other.column) as T;
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            Array.Copy(other.Mat, 0, model.Mat, 0, other.column * other.row);
            model.row = other.row;
            model.column = other.column;
            return model;
        }
        
        #region 数据转换
        /// <summary>
        /// 强转，可转任意矩阵，如若异常会在实现的时候报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T To<T>() where T : class
        {
            var typeT = typeof(T);
            if (typeof(T) == GetType()) return (T)Convert.ChangeType(this, typeof(T));
            dynamic model = null;
            if (typeT == typeof(IMatrix4x4))
            {
                model = new Matrix4x4();
            }
            else if (typeT == typeof(IMatrix4x4D))
            {
                model = new Matrix4x4D();
            }
            else if (typeT == typeof(IMatrix3x3))
            {
                model = new Matrix3x3();
            }
            else if (typeT == typeof(IMatrix3x3D))
            {
                model = new Matrix3x3D();
            }
            else if (typeT == typeof(IMatrixNxN))
            {
                model = new MatrixNxN(row);
            }
            else if (typeT == typeof(IMatrixNxND))
            {
                model = new MatrixNxND(row);
            }
            else if (typeT == typeof(IMatrixD))
            {
                model = new MatrixD(row,column);
            }
            else if (typeT == typeof(IMatrix))
            {
                model = new Matrix(row,column);
            }
            if (model == null)
            {
                model = Activator.CreateInstance<T>();
            }
            var newModelType = model[0].GetType();
            var IsSame = this[0,0].GetType() == newModelType;
            if (IsSame)
            {
                Array.Copy(Mat, 0, model.Mat, 0, column * row);
            }
            else
            {
                if (newModelType == typeof(double))
                {
                    for (int i = 0; i < row; i++)
                        for (int j = 0; j < column; j++)
                        {
                            model[i, j] = this[i, j];
                        }
                }
                else
                {
                    for (int i = 0; i < row; i++)
                        for (int j = 0; j < column; j++)
                        {
                            model[i, j] = (float)this[i, j];
                        }
                }
            }
            return model as T;
        }

        /// <summary>
        /// 转成数组
        /// </summary>
        /// <returns></returns>
        public virtual double[] ToArray()
        {
            double[] array = new double[column * row];
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    array[i * column + j] = Mat[i, j];
                }
            }
            return array;
        }

        /// <summary>
        /// 转成列表
        /// </summary>
        /// <returns></returns>
        public virtual List<double> ToList()
        {
            List<double> list = new List<double>();
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    list.Add(Mat[i, j]);
                }
            }
            return list;
        }

        /// <summary>
        /// 转成浮点数组
        /// </summary>
        /// <returns></returns>
        public virtual float[] ToFloatArray()
        {
            float[] a = new float[column * row];
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    a[i * column + j] = (float)Mat[i, j];
                }
            }
            return a;
        }

        /// <summary>
        /// 转成浮点列表
        /// </summary>
        /// <returns></returns>
        public virtual List<float> ToFloatList()
        {
            List<float> list = new List<float>();
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    list.Add((float)Mat[i, j]);
                }
            }
            return list;
        }

        /// <summary>
        /// 转成双精度数组
        /// </summary>
        /// <returns></returns>
        public virtual double[] ToDoubleArray()
        {
            double[] a = new double[column * row];
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    a[i * column + j] = Mat[i, j];
                }
            }
            return a;
        }

        /// <summary>
        /// 转成双精度列表
        /// </summary>
        /// <returns></returns>
        public virtual List<double> ToDoubleList()
        {
            List<double> list = new List<double>();
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    list.Add(Mat[i, j]);
                }
            }
            return list;
        }
        #endregion

        #region 下标获取信息
        /// <summary>
        /// 通过行列获取信息
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public virtual double this[int row, int column]
        {
            get
            {
                return this.Mat[row, column];
            }
            set
            {
                this.Mat[row, column] = value;
            }
        }

        /// <summary>
        /// 通过单下标获取信息，先行后列
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual double this[int index]
        {
            get { return Mat[index / column, index % column]; }
            set { this.Mat[index / column, index % column] = value; }
        }
        #endregion

        #region 加减乘除
        /// <summary>
        /// 矩阵加法
        /// </summary>
        /// <typeparam name="T">class, MatrixD</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T Add<T>(T Mb) where T : class, IMatrix<double>
        {
            if (Mb == null)
                return this as T;
            if ((column != Mb.column) || (row != Mb.row))
            {
                Exception myException = new Exception("数组维数不匹配");
                throw myException;
            }
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                {
                    this[i, j] += Mb[i, j];
                }
            return this as T;
        }

        /// <summary>
        /// 矩阵加法
        /// </summary>
        /// <typeparam name="T">class, MatrixD</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T AddToOther<T>(T Mb) where T : class, IMatrix<double>
        {
            T Mc = New(this as T);
            return Mc.Add<T>(Mb);
        }

        #region 矩阵+常数
        /// <summary>
        /// 矩阵数加
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T Add<T>(double k) where T : class, IMatrix<double>
        {
            for (int i = 0; i < column; i++)
                for (int j = 0; j < row; j++)
                    this[i, j] += k;
            return this as T;
        }

        /// <summary>
        /// 矩阵数加
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T AddToOther<T>(double k) where T : class, IMatrix<double>
        {
            T Mc = New(this as T);
            return Mc.Add<T>(k);
        }

        /// <summary>
        /// 矩阵数减，自身改变
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T Minus<T>(double k) where T : class, IMatrix<double>
        {
            for (int i = 0; i < column; i++)
                for (int j = 0; j < row; j++)
                    this[i, j] -= k;
            return this as T;
        }

        /// <summary>
        /// 矩阵数减,自身不变，产生新内存保存
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T MinusToOther<T>(double k) where T : class, IMatrix<double>
        {
            T Mc = New(this as T);
            return Mc.Minus<T>(k);
        }

        /// <summary>
        /// 矩阵数除，自身改变
        /// </summary>
        /// <typeparam name="T">class, MatrixD</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T Divide<T>(double k) where T : class, IMatrix<double>
        {
            for (int i = 0; i < column; i++)
                for (int j = 0; j < row; j++)
                    this[i, j] /= k;
            return this as T;
        }

        /// <summary>
        /// 矩阵数除,自身不变，产生新内存保存
        /// </summary>
        /// <typeparam name="T">class, MatrixD</typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T DivideToOther<T>(double k) where T : class, IMatrix<double>
        {
            T Mc = New(this as T);
            return Mc.Divide<T>(k);
        }
        #endregion

        /// <summary>
        /// 矩阵减法
        /// </summary>
        /// <typeparam name="T">class, MatrixD</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T Minus<T>(T Mb) where T : class, IMatrix<double>
        {
            if (Mb == null)
                return this as T;
            if ((column != Mb.column) || (row != Mb.row))
            {
                Exception myException = new Exception("数组维数不匹配");
                throw myException;
            }
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    this[i, j] -= Mb[i, j];
            return this as T;
        }

        /// <summary>
        /// 矩阵减法
        /// </summary>
        /// <typeparam name="T">class, MatrixD</typeparam>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T MinusToOther<T>(T Mb) where T : class, IMatrix<double>
        {
            T Mc = New(this as T);
            return Mc.Minus<T>(Mb);
        }

        #region 矩阵X常数
        /// <summary>
        /// 矩阵数乘
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T Multiply<T>(double k) where T : class, IMatrix<double>
        {
            for (int i = 0; i < column; i++)
                for (int j = 0; j < row; j++)
                    this[i, j] *= k;
            //this[i, j] *= k;
            return this as T;
        }

        /// <summary>
        /// 矩阵数乘
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public virtual T MultiplyToOther<T>(double k) where T : class, IMatrix<double>
        {
            T Mc = New(this as T);
            return Mc.Multiply<T>(k);
        }

        /// <summary>
        /// 矩阵数乘
        /// </summary>
        /// <returns></returns>
        public virtual T Negate<T>() where T : class, IMatrix<double>
        {
            for (int i = 0; i < column; i++)
                for (int j = 0; j < row; j++)
                    this[i, j] = -this[i, j];
            return this as T;
        }

        /// <summary>
        /// 矩阵数乘
        /// </summary>
        /// <returns></returns>
        public virtual T NegateToOther<T>() where T : class, IMatrix<double>
        {
            T Mc = New(this as T);
            return Mc.Negate<T>();
        }
        #endregion

        #region 矩阵X矩阵
        /// <summary>
        /// 矩阵乘法
        /// </summary>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T Multiply<T>(T Mb) where T : class, IMatrix<double>
        {
            if (Mb == null)
                return this as T;
            int m2 = Mb.column;
            if (column != Mb.row)
            {
                Exception myException = new Exception("数组维数不匹配");
                throw myException;
            }
            T Mc = New<T>(row, m2);
            double[,] c = Mc.Mat;
            double[,] a = Mat;
            double[,] b = Mb.Mat;
            for (int i = 0; i < row; i++)
                for (int j = 0; j < m2; j++)
                {
                    c[i, j] = 0;
                    for (int k = 0; k < column; k++)
                        c[i, j] += a[i, k] * b[k, j];
                }
            return Mc;
        }

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        /// <param name="Mb"></param>
        /// <returns></returns>
        public virtual T MultiplyToOther<T>(T Mb) where T : class, IMatrix<double>
        {
            T Mc = New(this as T);
            return Mc.Multiply<T>(Mb);
        }
        #endregion

        #region 矩阵X向量
        /// <summary>
        /// 矩阵X向量的计算，C为偏移，减掉
        /// </summary>
        /// <param name="p"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public Vector3d Multiply(Vector3d p, Vector3d c)
        {
            var x = p.X - c.X;
            var y = p.Y - c.Y;
            var z = p.Z - c.Z;

            var dx = Mat[0, 0] * x + Mat[0, 1] * y + Mat[0, 2] * z;
            var dy = Mat[1, 0] * x + Mat[1, 1] * y + Mat[1, 2] * z;
            var dz = Mat[2, 0] * x + Mat[2, 1] * y + Mat[2, 2] * z;
            return new Vector3d(dx, dy, dz);
        }
        #endregion
        #endregion

        #region 转置
        /// <summary>
        /// 矩阵转置，自身改变
        /// </summary>
        /// <returns></returns>
        public virtual T Transpose<T>() where T : class, IMatrix<double>
        {
            if (row != column)
            {
                int tmp = row;
                row = column;
                column = tmp;
            }
            for (int i = 0; i < row - 1; i++)
                for (int j = i + 1; j < column; j++)
                    Swap(i, j, j, i);
            return this as T;
        }

        /// <summary>
        /// 矩阵转置，生成新的内存
        /// </summary>
        /// <returns></returns>
        public virtual T TransposeToOther<T>() where T : class, IMatrix<double>
        {
            T Mc = New<T>(column, row);
            for (int i = 0; i < row - 1; i++)
                for (int j = i + 1; j < column; j++)
                    Mc[i, j] = this[j, i];
            return Mc;
        }


        /// <summary>
        /// 调换矩阵的下标值
        /// </summary>
        /// <param name="row1"></param>
        /// <param name="column1"></param>
        /// <param name="row2"></param>
        /// <param name="column2"></param>
        protected void Swap(int row1, int column1, int row2, int column2)
        {
            if(row1== row2 && column1==column2)
            {
                return;
            }
            double tmp = Mat[row1, column1];          
            Mat[row1, column1] = Mat[row2, column2];
            Mat[row2, column2] = tmp;
        }
        #endregion

        #region 转字符串与相等
        /// <summary>
        /// 转成字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            string result = string.Empty;
            foreach (var o in this.ToArray())
            {
                result += o + ",";
            }
            return result.TrimEnd(',');
        }


        /// <summary>
        /// 判断句很是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(IMatrix<double> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    if (!this[i, j].Equals(other[i, j]))
                        return false;
                }
            }
            return true;
        }
        #endregion

        #region 符号重载
        public static MatrixD operator +(MatrixD value1, MatrixD value2)
        {
            MatrixD Mc = New(value1);
            return Mc.Add(value2);
        }

        public static MatrixD operator +(MatrixD value1, double value2)
        {
            MatrixD Mc = New(value1);
            return Mc.Add<MatrixD>(value2);
        }

        public static MatrixD operator +(double value1, MatrixD value2)
        {
            MatrixD Mc = New(value2);
            return Mc.Add<MatrixD>(value1);
        }

        public static MatrixD operator -(MatrixD value1, MatrixD value2)
        {
            MatrixD Mc = New(value1);
            return Mc.Minus(value2);
        }

        public static MatrixD operator -(MatrixD value1, double value2)
        {
            MatrixD Mc = New(value1);
            return Mc.Minus<MatrixD>(value2);
        }

        public static MatrixD operator -(double value1, MatrixD value2)
        {
            MatrixD Mc = New(value2);
            return Mc.Negate<MatrixD>().Add<MatrixD>(value1);
        }

        public static MatrixD operator *(MatrixD value1, MatrixD value2)
        {
            MatrixD Mc = New(value1);
            return Mc.Multiply(value2);
        }

        public static MatrixD operator *(MatrixD value1, double value2)
        {
            MatrixD Mc = New(value1);
            return Mc.Multiply<MatrixD>(value2);
        }

        public static MatrixD operator *(double value1, MatrixD value2)
        {
            MatrixD Mc = New(value2);
            return Mc.Multiply<MatrixD>(value1);
        }

        public static MatrixD operator /(MatrixD value1, double value2)
        {
            MatrixD Mc = New(value1);
            return Mc.Divide<MatrixD>(value2);
        }
        #endregion
    }
}
