using AgDataHandle.Maths;

namespace AgDataHandle.BaseService.Utils
{
    public class MatrixHelper
    {
        public static Vector3 GetNormalUp(IMatrix4x4 matrix)
        {
            return new Vector3(matrix[1, 0], matrix[1, 1], matrix[1, 2]);// Y轴
        }

        public static Vector3 GetNormalRight(IMatrix4x4 matrix)
        {
            return new Vector3(matrix[0, 0], matrix[0, 1], matrix[0, 2]);// X轴
        }

        public static Vector3 GetNormalUp(IMatrix4x4D matrix)
        {
            return new Vector3(matrix[1, 0], matrix[1, 1], matrix[1, 2]);// Y轴
        }

        public static Vector3 GetNormalRight(IMatrix4x4D matrix)
        {
            return new Vector3(matrix[0, 0], matrix[0, 1], matrix[0, 2]);// X轴
        }

        public static Vector3 GetPosition(IMatrix4x4 matrix)
        {
            return new Vector3((float)matrix[3, 0], (float)matrix[3, 1], (float)matrix[3, 2]);
        }

        public static Vector3 GetScale(IMatrix4x4 matrix)
        {
            Vector3 scale = new Vector3();
            scale.X = (float)Math.Sqrt(matrix[0, 0] * matrix[0, 0] + matrix[1, 0] * matrix[1, 0] + matrix[2, 0] * matrix[2, 0]);
            scale.Y = (float)Math.Sqrt(matrix[0, 1] * matrix[0, 1] + matrix[1, 1] * matrix[1, 1] + matrix[2, 1] * matrix[2, 1]);
            scale.Z = (float)Math.Sqrt(matrix[0, 2] * matrix[0, 2] + matrix[1, 2] * matrix[1, 2] + matrix[2, 2] * matrix[2, 2]);

            return GetEffectiveScale(scale, matrix);
        }

        private static Vector3 GetEffectiveScale(Vector3 scale, IMatrix4x4 m)
        {
            var t = new Vector3((float)m[3, 0], (float)m[3, 1], (float)m[3, 2]);
            float[] ratio = new float[3] { 1, 1, 1 };
            int symbol = -1;
            for (int i = 0; i < 2; i++)
            {
                ratio[0] *= symbol;
                for (int j = 0; j < 2; j++)
                {
                    ratio[1] *= symbol;
                    for (int w = 0; w < 2; w++)
                    {
                        ratio[2] *= symbol;
                        var newScale = new Vector3(ratio[0] * scale.X, ratio[1] * scale.Y, ratio[2] * scale.Z);
                        Matrix3x3 rotation = GetRotation(newScale, m);
                        var r = rotation.ToQuaternion<Quaternion>();
                        if (IsValidScale(newScale, m, r, t, ratio))
                        {
                            return newScale;
                        }
                    }
                }
            }

            throw new Exception("could not find valid scale");
        }

        private static Matrix3x3 GetRotation(Vector3 scale, IMatrix4x4 m)
        {
            Vector3 unit = new Vector3(1, 1, 1);
            Vector3 inverseScale = new Vector3(unit.X / scale.X, unit.Y / scale.Y, unit.Z / scale.Z);

            float[] res = new float[9];
            res[0] = m[0, 0] * inverseScale.X; // 0,0
            res[1] = m[0, 1] * inverseScale.Y; // 1,0
            res[2] = m[0, 2] * inverseScale.Z; // 2,0

            res[3] = m[1, 0] * inverseScale.X; // 0,1
            res[4] = m[1, 1] * inverseScale.Y; // 1,1
            res[5] = m[1, 2] * inverseScale.Z; // 2,1

            res[6] = m[2, 0] * inverseScale.X; // 0,2                       
            res[7] = m[2, 1] * inverseScale.Y; // 1,2
            res[8] = m[2, 2] * inverseScale.Z;// 2,2

            return new Matrix3x3(res).Transpose<Matrix3x3>();
        }

        private static bool IsValidScale(Vector3 newScale, IMatrix4x4 m, IQuaternion r, Vector3 t, float[] ratio)
        {
            IMatrix4x4 m1 = Matrix4x4.FromTranslationQuaternionRotationScale<Matrix4x4>(t, r, newScale);
            for (int row = 0; row < m1.row; row++)
            {
                for (int col = 0; col < m1.column; col++)
                {
                    if (!(Math.Abs(m1[row, col] - m[row, col]) < 0.001))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
