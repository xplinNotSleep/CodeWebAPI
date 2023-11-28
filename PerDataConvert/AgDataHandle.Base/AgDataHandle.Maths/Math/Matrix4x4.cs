using System;

namespace AgDataHandle.Maths
{    
    /// <summary>
     /// 当前矩阵默认为4x4齐次矩阵
     /// </summary>
    public class Matrix4x4 : MatrixNxN, IMatrix4x4
    {
        #region 构造函数
        /// <summary>
        /// 单位矩阵
        /// </summary>
        public static readonly Matrix4x4 Identity = new Matrix4x4(
            1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, 1f, 0f,
            0f, 0f, 0f, 1f
            );

        /// <summary>
        /// 默认矩阵
        /// </summary>
        public Matrix4x4() : base(4)
        {
        }
        /// <summary>
        /// Matrix is column major ordered
        /// </summary>
        public Matrix4x4(float column0Row0, float column1Row0, float column2Row0, float column3Row0,
            float column0Row1, float column1Row1, float column2Row1, float column3Row1,
            float column0Row2, float column1Row2, float column2Row2, float column3Row2,
            float column0Row3, float column1Row3, float column2Row3, float column3Row3):base(4)
        {
            Mat[0, 0] = column0Row0;
            Mat[1, 0] = column0Row1;
            Mat[2, 0] = column0Row2;
            Mat[3, 0] = column0Row3;
            Mat[0, 1] = column1Row0;
            Mat[1, 1] = column1Row1;
            Mat[2, 1] = column1Row2;
            Mat[3, 1] = column1Row3;
            Mat[0, 2] = column2Row0;
            Mat[1, 2] = column2Row1;
            Mat[2, 2] = column2Row2;
            Mat[3, 2] = column2Row3;
            Mat[0, 3] = column3Row0;
            Mat[1, 3] = column3Row1;
            Mat[2, 3] = column3Row2;
            Mat[3, 3] = column3Row3;
        }

        /// <summary>
        /// 复制其他
        /// </summary>
        /// <param name="other"></param>
        public Matrix4x4(Matrix4x4 other) : base(other)
        {
        }

        /// <summary>
        /// 输入数组获得矩阵
        /// </summary>
        /// <param name="a"></param>
        public Matrix4x4(float[] a) : base(a,4)
        {
        }

        /// <summary>
        /// 构造函数复制其他
        /// </summary>
        /// <param name="other"></param>
        public Matrix4x4(float[,] other) : base(other,4)
        {
        }
        #endregion

        #region 解剖4X4齐次矩阵的两个变量
        /// <summary>
        /// 4X4齐次矩阵的旋转矩阵
        /// </summary>
        public IMatrix3x3 RotateMatrix
        {
            get { return this.GetCofactor<IMatrix3x3>(3, 3); }
        }

        /// <summary>
        /// 4X4齐次矩阵的偏移量
        /// </summary>
        public Vector3 Translation
        {
            get { return new Vector3((float)this[3, 0], (float)this[3, 1], (float)this[3, 2]); }
        }
        #endregion

        #region 矩阵X矩阵
        /// <summary>
        /// TODO:要删除的，先齐次，然后在4X4的矩阵相乘
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Matrix4x4 MultiplyByMatrix3(Matrix4x4 source, Matrix3x3 rotation)
        {
            var result = new Matrix4x4();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < 3; k++)
                        result[i, j] += source[i, k] * rotation[k, j];
                }
            return result;
        }
        #endregion

        public Matrix4x4 Clone()
        {
            var newItem = new Matrix4x4();
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    newItem.Mat[i, j] = this[i, j];
                }
            }
            return newItem;
        }

        /// <summary>
        /// 通过向量缩放，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T ScaleByVector<T>(IVector3<float> vector) where T : class, IMatrix4x4<float>
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
        public T ScaleByVectorToOther<T>(IVector3<float> vector) where T : class, IMatrix4x4<float>
        {
            T Mc = New(this as T);
            return Mc.ScaleByVector<T>(vector);
        }

        /// <summary>
        /// 通过向量平移,自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T TranslationByVector<T>(IVector3<float> vector) where T : class, IMatrix4x4<float>
        {
            this[3,0]+=vector.X;
            this[3, 1] += vector.Y;
            this[3, 2] += vector.Z;
            return this as T;
        }
        /// <summary>
        /// 通过向量平移，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T TranslationByVectorToOther<T>(IVector3<float> vector) where T : class, IMatrix4x4<float>
        {
            T Mc = New(this as T);
            return Mc.TranslationByVector<T>(vector);
        }

        /// <summary>
        /// 4X4的导致求逆默认当是齐次矩阵
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T HomogeneousInverse<T>() where T : class, IMatrix4x4<float>
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Swap(i, j, j, i);
                }
            this[3, 0] = -this[3, 0];
            this[3, 1] = -this[3, 1];
            this[3, 2] = -this[3, 2];
            return this as T;
        }
        #region 静态方法
        /// <summary>
        /// 移动、旋转、缩放转换为4X4矩阵
        /// </summary>
        /// <param name = "translation" ></ param >
        /// < param name="rotation"></param>
        /// <param name = "scale" ></ param >
        /// <reference>https://www.cnblogs.com/bbsno1/archive/2013/08/18/3266744.html 来源或许是这个</reference>>
        /// < returns ></ returns >
        public static T FromTranslationQuaternionRotationScale<T>(Vector3 translation, IQuaternion rotation, Vector3 scale) where T : class, IMatrix4x4<float>
        {
            var scaleX = scale.X;
            var scaleY = scale.Y;
            var scaleZ = scale.Z;
            var x2 = rotation.X * rotation.X;
            var xy = rotation.X * rotation.Y;
            var xz = rotation.X * rotation.Z;
            var xw = rotation.X * rotation.W;
            var y2 = rotation.Y * rotation.Y;
            var yz = rotation.Y * rotation.Z;
            var yw = rotation.Y * rotation.W;
            var z2 = rotation.Z * rotation.Z;
            var zw = rotation.Z * rotation.W;
            var w2 = rotation.W * rotation.W;

            var m00 = x2 - y2 - z2 + w2;
            var m01 = 2.0f * (xy - zw);
            var m02 = 2.0f * (xz + yw);

            var m10 = 2.0f * (xy + zw);
            var m11 = -x2 + y2 - z2 + w2;
            var m12 = 2.0f * (yz - xw);

            var m20 = 2.0f * (xz - yw);
            var m21 = 2.0f * (yz + xw);
            var m22 = -x2 - y2 + z2 + w2;

            var result = New<T>();

            result[0, 0] = m00 * scaleX;
            result[0, 1] = m10 * scaleX;
            result[0, 2] = m20 * scaleX;

            result[1, 0] = m01 * scaleY;
            result[1, 1] = m11 * scaleY;
            result[1, 2] = m21 * scaleY;

            result[2, 0] = m02 * scaleZ;
            result[2, 1] = m12 * scaleZ;
            result[2, 2] = m22 * scaleZ;

            result[3, 0] = translation.X;
            result[3, 1] = translation.Y;
            result[3, 2] = translation.Z;

            result[3, 3] = 1f;
            return result;
            #region 新写法，有待验证，先保留
            //var scaleX = scale.X;
            //var scaleY = scale.Y;
            //var scaleZ = scale.Z;
            //var m = rotation.ToMatrix<Matrix3x3>();
            //var result = New<T>();
            //result[0, 0] = m[0, 0] * scaleX;
            //result[0, 1] = m[0, 1] * scaleX;
            //result[0, 2] = m[0, 2] * scaleX;

            //result[1, 0] = m[1, 0] * scaleY;
            //result[1, 1] = m[1, 1] * scaleY;
            //result[1, 2] = m[1, 2] * scaleY;

            //result[2, 0] = m[2, 0] * scaleZ;
            //result[2, 1] = m[2, 1] * scaleZ;
            //result[2, 2] = m[2, 2] * scaleZ;

            //result[3, 0] = translation.X;
            //result[3, 1] = translation.Y;
            //result[3, 2] = translation.Z;

            //result[3, 3] = 1f;


            //return result;
            #endregion
        }
        /// <summary>
        /// 通过两个三角网计算平移矩阵
        /// </summary>
        /// <param name="vecArr1">第一个三角网</param>
        /// <param name="vecArr2">第二个三角网</param>
        /// <param name="IsZUpAxis">是否Z轴向上</param>
        /// <returns></returns>
        public static T ComputeMatrix<T>(Vector3[] vecArr1, Vector3[] vecArr2, bool IsZUpAxis = true) where T : class, IMatrix4x4<float>
        {
            ///平移第一个点
            var moveVec = vecArr2[0].MinusToOther(vecArr1[0]);
            if (!vecArr1[1].Equals(vecArr2[1]))
            {
                float Radian = vecArr1[1].Angle(vecArr2[1]);
                Vector3 Ro = vecArr1[1].CrossToOther(vecArr2[1]);
                //vect1和vect2向量共线时,即旋转180度法向量为0，旋转轴无法求出,引用第三个点求
                if (Ro.X== 0 && Ro.Y==0 &&Ro.Z== 0)
                {
                    Ro = vecArr1[2];
                }
                var Unit = Ro.Normalize<Vector3>();
                var newPoint = vecArr1[0].RotateToOther(Unit, Radian);
                moveVec = vecArr2[0].MinusToOther(newPoint);
                float x = (float)Math.Round(-Unit.X);
                float y = IsZUpAxis ? (float)Math.Round(-Unit.Z) : (float)Math.Round(Unit.Y);
                float z = IsZUpAxis ? (float)Math.Round(Unit.Y) : (float)Math.Round(-Unit.Z);
                float C = (float)Math.Cos(Radian);
                float S = (float)Math.Sin(Radian);

                float xx = x* x;
                float xy = x* y;
                float xz = x* z;
                float yy = y* y;
                float yz = y* z;
                float zz = z* z;
                float xS = x* S;
                float yS = y* S;
                float zS = z* S;
                var OneMinusC = 1f- C;
                var result = New<T>();
                result[0,0] = xx* OneMinusC+C;
                result[0,1] = xy* OneMinusC- zS;
                result[0,2] = xz* OneMinusC+ yS;
                result[0,3] = 0;
                result[1,0] = xy* OneMinusC+ zS;
                result[1,1] = yy* OneMinusC+ C;
                result[1,2] = yz* OneMinusC- xS;
                result[1,3] = 0;
                result[2,0] = xz* OneMinusC- yS;
                result[2,1] = yz* OneMinusC+ xS;
                result[2,2] = zz* OneMinusC+ C;
                result[2,3] = 0;
                result[3,0] = -moveVec.X;
                result[3,1] = moveVec.Z;
                result[3,2] = moveVec.Y;
                result[3,3] = 0;
                return result;
            }
            else
            {
                var result = New<T>();
                result[3, 0] = IsZUpAxis ? -moveVec.X : moveVec.X;
                result[3, 1] = IsZUpAxis ? moveVec.Z : moveVec.Y;
                result[3, 2] = IsZUpAxis ? moveVec.Y : moveVec.Z;
                return result;
            }
        }


        /// <summary>
        /// 创造一个绕X轴旋转的4X4矩阵
        /// </summary>
        /// <param name = "radians" ></ param >
        /// < returns ></ returns >
        public static T CreateRotationX<T>(float radians) where T : class, IMatrix4x4<float>
        {
            float num = (float)Math.Cos(radians);
            float num2 = (float)Math.Sin(radians);
            var result = New<T>();
            result[1,1] = num;
            result[1,2] = num2;
            result[2,1] =-num2;
            result[2,2] = num;
            return result;
        }

        /// <summary>
        /// 创造一个绕Y轴旋转的4X4矩阵
        /// </summary>
        /// <param name = "radians" ></ param >
        /// < returns ></ returns >
        public static T CreateRotationY<T>(float radians) where T : class, IMatrix4x4<float>
        {
            float num = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            var result = New<T>();
            result[0,0] = num;
            result[0,2] = -num2;
            result[2,0] = num2;
            result[2,2] = num;
            return result;
        }

        /// <summary>
        /// 创造一个绕Z轴旋转的4X4矩阵
        /// </summary>
        /// <param name = "radians" ></ param >
        /// < returns ></ returns >
        public static T CreateRotationZ<T>(float radians) where T : class, IMatrix4x4<float>
        {
            float num = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            var result = New<T>();
            result[0,0] = num;
            result[0,1] = num2;
            result[1,0] = -num2;
            result[1,1] = num;
            return result;
        }


        /// <summary>
        /// 创造一个绕任意轴旋转的4X4矩阵
        /// </summary>
        /// <param name = "axis" ></ param >
        /// < param name="angle"></param>
        /// <returns></returns>
        public static T CreateFromAxisAngle<T>(Vector3 axis, float angle) where T : class, IMatrix4x4<float>
        {
            float x = axis.X;
            float y = axis.Y;
            float z = axis.Z;
            float num = (float)Math.Sin((double)angle);
            float num2 = (float)Math.Cos((double)angle);
            float num3 = x*x;
            float num4 = y*y;
            float num5 = z*z;
            float num6 = x*y;
            float num7 = x*z;
            float num8 = y*z;
            var result = New<T>();
            result[0,0] = num3+ num2*( 1- num3);
            result[0,1] = num6 - num2 * num6 + num * z;
            result[0,2] = num7 - num2 * num7 - num * y;
            result[1,0] = num6 - num2 * num6 - num * z;
            result[1,1] = num4 + num2 * (1f - num4);
            result[1,2] = num8 - num2 * num8 + num * x;
            result[2,0] = num7 - num2 * num7 + num * y;
            result[2,1] = num8 - num2 * num8 - num * x;
            result[2,2] = num5 + num2 * (1f - num5);
            return result;
        }

        /// <summary>
        /// 根据z轴方向创建变换矩阵
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static T CreateTransByZAxisDir<T>(Vector3 direction, Vector3 center) where T : class, IMatrix4x4<float>
        {
            var zAxis = direction;
            zAxis.Normalize<Vector3>();
            Vector3 xAxis = null;
            Vector3 yAxis = null;
            if (Math.Abs(direction.Z) >= 0.00001f)
            {
                if(direction.Z>0)
                {
                    xAxis = new Vector3(1, 0, 0);
                    yAxis = zAxis.CrossToOther(xAxis).Normalize<Vector3>();
                    xAxis = yAxis.CrossToOther(zAxis).Normalize<Vector3>();
                }
                else
                {
                    xAxis = new Vector3(-1, 0, 0);
                    yAxis = zAxis.CrossToOther(xAxis).Normalize<Vector3>();
                    xAxis = yAxis.CrossToOther(zAxis).Normalize<Vector3>();
                }
            }
            else if(Math.Abs(direction.X) ==1)
            {
                xAxis = new Vector3(0, 0, -1);
                yAxis = zAxis.CrossToOther(xAxis).Normalize<Vector3>();
                xAxis = yAxis.CrossToOther(zAxis).Normalize<Vector3>();
            }
            else
            {
                xAxis = new Vector3(1, 0, 0);
                yAxis = zAxis.CrossToOther(xAxis).Normalize<Vector3>();
                xAxis = yAxis.CrossToOther(zAxis).Normalize<Vector3>();
            }

            var matrix = New<T>();
            matrix[0, 0] = xAxis.X;
            matrix[0, 1] = xAxis.Y;
            matrix[0, 2] = xAxis.Z;
            matrix[1, 0] = yAxis.X;
            matrix[1, 1] = yAxis.Y;
            matrix[1, 2] = yAxis.Z;
            matrix[2, 0] = zAxis.X;
            matrix[2, 1] = zAxis.Y;
            matrix[2, 2] = zAxis.Z;

            matrix[3, 0] = center.X;
            matrix[3, 1] = center.Y;
            matrix[3, 2] = center.Z;
            return matrix;

        }
        public static T CreateTransByAxisDir<T>(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, Vector3 center) where T : class, IMatrix4x4<float>
        {
            var matrix = New<T>();
            matrix[0, 0] = xAxis.X;
            matrix[0, 1] = xAxis.Y;
            matrix[0, 2] = xAxis.Z;
            matrix[1, 0] = yAxis.X;
            matrix[1, 1] = yAxis.Y;
            matrix[1, 2] = yAxis.Z;
            matrix[2, 0] = zAxis.X;
            matrix[2, 1] = zAxis.Y;
            matrix[2, 2] = zAxis.Z;

            matrix[3, 0] = center.X;
            matrix[3, 1] = center.Y;
            matrix[3, 2] = center.Z;
            return matrix;

        }
        #endregion
    }

    /// <summary>
    /// 当前矩阵默认为4x4齐次矩阵
    /// </summary>
    public class Matrix4x4D : MatrixNxND, IMatrix4x4D
    {
        #region 构造函数
        /// <summary>
        /// 单位矩阵
        /// </summary>
        public static readonly Matrix4x4D Identity = new Matrix4x4D(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
            );

        /// <summary>
        /// 默认矩阵
        /// </summary>
        public Matrix4x4D() : base(4)
        {
        }
        /// <summary>
        /// Matrix is column major ordered
        /// </summary>
        public Matrix4x4D(double column0Row0, double column1Row0, double column2Row0, double column3Row0,
            double column0Row1, double column1Row1, double column2Row1, double column3Row1,
            double column0Row2, double column1Row2, double column2Row2, double column3Row2,
            double column0Row3, double column1Row3, double column2Row3, double column3Row3):base(4)
        {
            Mat[0, 0] = column0Row0;
            Mat[1, 0] = column0Row1;
            Mat[2, 0] = column0Row2;
            Mat[3, 0] = column0Row3;
            Mat[0, 1] = column1Row0;
            Mat[1, 1] = column1Row1;
            Mat[2, 1] = column1Row2;
            Mat[3, 1] = column1Row3;
            Mat[0, 2] = column2Row0;
            Mat[1, 2] = column2Row1;
            Mat[2, 2] = column2Row2;
            Mat[3, 2] = column2Row3;
            Mat[0, 3] = column3Row0;
            Mat[1, 3] = column3Row1;
            Mat[2, 3] = column3Row2;
            Mat[3, 3] = column3Row3;
        }

        /// <summary>
        /// 复制其他
        /// </summary>
        /// <param name="other"></param>
        public Matrix4x4D(Matrix4x4D other) : base(other)
        {
        }

        /// <summary>
        /// 输入数组获得矩阵
        /// </summary>
        /// <param name="a"></param>
        public Matrix4x4D(double[] a) : base(a,4)
        {
        }

        /// <summary>
        /// 构造函数复制其他
        /// </summary>
        /// <param name="other"></param>
        public Matrix4x4D(double[,] other) : base(other,4)
        {
        }
        #endregion

        #region 解剖4X4齐次矩阵的两个变量
        /// <summary>
        /// 4X4齐次矩阵的旋转矩阵
        /// </summary>
        public IMatrix3x3D RotateMatrix
        {
            get { return this.GetCofactor<IMatrix3x3D>(3, 3); }
        }

        /// <summary>
        /// 4X4齐次矩阵的偏移量
        /// </summary>
        public Vector3d Translation
        {
            get { return new Vector3d(this[3, 0], this[3, 1], this[3, 2]); }
        }
        #endregion

        #region 矩阵X矩阵
        /// <summary>
        /// TODO:要删除的，先齐次，然后在4X4的矩阵相乘
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Matrix4x4D MultiplyByMatrix3(Matrix4x4D source, Matrix3x3D rotation)
        {
            var result = new Matrix4x4D();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < 3; k++)
                        result[i, j] += source[i, k] * rotation[k, j];
                }
            return result;
        }
        #endregion

        public Matrix4x4D Clone()
        {
            var newItem = new Matrix4x4D();
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    newItem.Mat[i, j] = this[i, j];
                }
            }
            return newItem;
        }

        /// <summary>
        /// 通过向量缩放，自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T ScaleByVector<T>(IVector3<double> vector) where T : class, IMatrix4x4<double>
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
        public T ScaleByVectorToOther<T>(IVector3<double> vector) where T : class, IMatrix4x4<double>
        {
            T Mc = New(this as T);
            return Mc.ScaleByVector<T>(vector);
        }


        /// <summary>
        /// 通过向量平移,自身改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T TranslationByVector<T>(IVector3<double> vector) where T : class, IMatrix4x4<double>
        {
            this[3, 0] += vector.X;
            this[3, 1] += vector.Y;
            this[3, 2] += vector.Z;
            return this as T;
        }
        /// <summary>
        /// 通过向量平移，产生新内存保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public T TranslationByVectorToOther<T>(IVector3<double> vector) where T : class, IMatrix4x4<double>
        {
            T Mc = New(this as T);
            return Mc.TranslationByVector<T>(vector);
        }

        /// <summary>
        /// 4X4的导致求逆默认当是齐次矩阵
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T HomogeneousInverse<T>() where T : class, IMatrix4x4<double>
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Swap(i, j, j, i);
                }
            this[3, 0] = -this[3, 0];
            this[3, 1] = -this[3, 1];
            this[3, 2] = -this[3, 2];
            return this as T;
        }
        #region 静态方法
        /// <summary>
        /// 移动、旋转、缩放转换为4X4矩阵
        /// </summary>
        /// <param name = "translation" ></ param >
        /// < param name="rotation"></param>
        /// <param name = "scale" ></ param >
        /// <reference>https://www.cnblogs.com/bbsno1/archive/2013/08/18/3266744.html 来源或许是这个</reference>>
        /// < returns ></ returns >
        public static T FromTranslationQuaternionRotationScale<T>(Vector3d translation, IQuaternionD rotation, Vector3d scale) where T : class, IMatrix4x4<double>
        {
            var scaleX = scale.X;
            var scaleY = scale.Y;
            var scaleZ = scale.Z;
            var x2 = rotation.X * rotation.X;
            var xy = rotation.X * rotation.Y;
            var xz = rotation.X * rotation.Z;
            var xw = rotation.X * rotation.W;
            var y2 = rotation.Y * rotation.Y;
            var yz = rotation.Y * rotation.Z;
            var yw = rotation.Y * rotation.W;
            var z2 = rotation.Z * rotation.Z;
            var zw = rotation.Z * rotation.W;
            var w2 = rotation.W * rotation.W;

            var m00 = x2 - y2 - z2 + w2;
            var m01 = 2.0f * (xy - zw);
            var m02 = 2.0f * (xz + yw);

            var m10 = 2.0f * (xy + zw);
            var m11 = -x2 + y2 - z2 + w2;
            var m12 = 2.0f * (yz - xw);

            var m20 = 2.0f * (xz - yw);
            var m21 = 2.0f * (yz + xw);
            var m22 = -x2 - y2 + z2 + w2;

            var result = New<T>();

            result[0, 0] = m00 * scaleX;
            result[0, 1] = m10 * scaleX;
            result[0, 2] = m20 * scaleX;

            result[1, 0] = m01 * scaleY;
            result[1, 1] = m11 * scaleY;
            result[1, 2] = m21 * scaleY;

            result[2, 0] = m02 * scaleZ;
            result[2, 1] = m12 * scaleZ;
            result[2, 2] = m22 * scaleZ;

            result[3, 0] = translation.X;
            result[3, 1] = translation.Y;
            result[3, 2] = translation.Z;

            return result;
        }
        /// <summary>
        /// 通过两个三角网计算平移矩阵
        /// </summary>
        /// <param name="vecArr1">第一个三角网</param>
        /// <param name="vecArr2">第二个三角网</param>
        /// <param name="IsZUpAxis">是否Z轴向上</param>
        /// <returns></returns>
        public static T ComputeMatrix<T>(Vector3d[] vecArr1, Vector3d[] vecArr2, bool IsZUpAxis = true) where T : class, IMatrix4x4<double>
        {
            ///平移第一个点
            var moveVec = vecArr2[0].MinusToOther(vecArr1[0]);
            if (!vecArr1[1].Equals(vecArr2[1]))
            {
                double Radian = vecArr1[1].Angle(vecArr2[1]);
                Vector3d Ro = vecArr1[1].CrossToOther(vecArr2[1]);
                //vect1和vect2向量共线时,即旋转180度法向量为0，旋转轴无法求出,引用第三个点求
                if (Ro.X == 0 && Ro.Y == 0 && Ro.Z == 0)
                {
                    Ro = vecArr1[2];
                }
                var Unit = Ro.Normalize<Vector3d>();
                var newPoint = vecArr1[0].RotateToOther(Unit, Radian);
                moveVec = vecArr2[0].MinusToOther(newPoint);
                double x = (double)Math.Round(-Unit.X);
                double y = IsZUpAxis ? (double)Math.Round(-Unit.Z) : (double)Math.Round(Unit.Y);
                double z = IsZUpAxis ? (double)Math.Round(Unit.Y) : (double)Math.Round(-Unit.Z);
                double C = (double)Math.Cos(Radian);
                double S = (double)Math.Sin(Radian);

                double xx = x * x;
                double xy = x * y;
                double xz = x * z;
                double yy = y * y;
                double yz = y * z;
                double zz = z * z;
                double xS = x * S;
                double yS = y * S;
                double zS = z * S;
                var OneMinusC = 1f - C;
                var result = New<T>();
                result[0, 0] = xx * OneMinusC + C;
                result[0, 1] = xy * OneMinusC - zS;
                result[0, 2] = xz * OneMinusC + yS;
                result[0, 3] = 0;
                result[1, 0] = xy * OneMinusC + zS;
                result[1, 1] = yy * OneMinusC + C;
                result[1, 2] = yz * OneMinusC - xS;
                result[1, 3] = 0;
                result[2, 0] = xz * OneMinusC - yS;
                result[2, 1] = yz * OneMinusC + xS;
                result[2, 2] = zz * OneMinusC + C;
                result[2, 3] = 0;
                result[3, 0] = -moveVec.X;
                result[3, 1] = moveVec.Z;
                result[3, 2] = moveVec.Y;
                result[3, 3] = 0;
                return result;
            }
            else
            {
                var result = New<T>();
                result[3, 0] = IsZUpAxis ? -moveVec.X : moveVec.X;
                result[3, 1] = IsZUpAxis ? moveVec.Z : moveVec.Y;
                result[3, 2] = IsZUpAxis ? moveVec.Y : moveVec.Z;
                return result;
            }
        }


        /// <summary>
        /// 创造一个绕X轴旋转的4X4矩阵
        /// </summary>
        /// <param name = "radians" ></ param >
        /// < returns ></ returns >
        public static T CreateRotationX<T>(double radians) where T : class, IMatrix4x4<double>
        {
            double num = (double)Math.Cos(radians);
            double num2 = (double)Math.Sin(radians);
            var result = New<T>();
            result[1, 1] = num;
            result[1, 2] = num2;
            result[2, 1] = -num2;
            result[2, 2] = num;
            return result;
        }

        /// <summary>
        /// 创造一个绕Y轴旋转的4X4矩阵
        /// </summary>
        /// <param name = "radians" ></ param >
        /// < returns ></ returns >
        public static T CreateRotationY<T>(double radians) where T : class, IMatrix4x4<double>
        {
            double num = (double)Math.Cos((double)radians);
            double num2 = (double)Math.Sin((double)radians);
            var result = New<T>();
            result[0, 0] = num;
            result[0, 2] = -num2;
            result[2, 0] = num2;
            result[2, 2] = num;
            return result;
        }

        /// <summary>
        /// 创造一个绕Z轴旋转的4X4矩阵
        /// </summary>
        /// <param name = "radians" ></ param >
        /// < returns ></ returns >
        public static T CreateRotationZ<T>(double radians) where T : class, IMatrix4x4<double>
        {
            double num = (double)Math.Cos((double)radians);
            double num2 = (double)Math.Sin((double)radians);
            var result = New<T>();
            result[0, 0] = num;
            result[0, 1] = num2;
            result[1, 0] = -num2;
            result[1, 1] = num;
            return result;
        }


        /// <summary>
        /// 创造一个绕任意轴旋转的4X4矩阵
        /// </summary>
        /// <param name = "axis" ></ param >
        /// < param name="angle"></param>
        /// <returns></returns>
        public static T CreateFromAxisAngle<T>(Vector3d axis, double angle) where T : class, IMatrix4x4<double>
        {
            double x = axis.X;
            double y = axis.Y;
            double z = axis.Z;
            double num = (double)Math.Sin((double)angle);
            double num2 = (double)Math.Cos((double)angle);
            double num3 = x * x;
            double num4 = y * y;
            double num5 = z * z;
            double num6 = x * y;
            double num7 = x * z;
            double num8 = y * z;
            var result = New<T>();
            result[0, 0] = num3 + num2 * (1 - num3);
            result[0, 1] = num6 - num2 * num6 + num * z;
            result[0, 2] = num7 - num2 * num7 - num * y;
            result[1, 0] = num6 - num2 * num6 - num * z;
            result[1, 1] = num4 + num2 * (1f - num4);
            result[1, 2] = num8 - num2 * num8 + num * x;
            result[2, 0] = num7 - num2 * num7 + num * y;
            result[2, 1] = num8 - num2 * num8 - num * x;
            result[2, 2] = num5 + num2 * (1f - num5);
            return result;
        }
        #endregion
    }
}