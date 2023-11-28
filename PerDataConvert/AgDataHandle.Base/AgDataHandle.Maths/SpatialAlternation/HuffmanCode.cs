using AgDataHandle.Maths;
using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    public class HuffmanCode
    {
        private int min_unit = 10;
        private int z_unit = 3;

        private int numbits_x;
        private int numbits_y;
        private int numbits_z;


        private double minX;
        private double maxX;

        private double minY;
        private double maxY;

        private double minZ;
        private double maxZ;

        #region 构造函数
        public HuffmanCode(BoundingBox sectionBox):this(sectionBox.Min,sectionBox.Max)
        {
            
        }

        public HuffmanCode(Vector3 minPos, Vector3 maxPos)
        {
            var x = (int)Math.Round((maxPos.X - minPos.X) / min_unit, 0);
            var y = (int)Math.Round((maxPos.Y - minPos.Y) / min_unit, 0);
            var z = (int)Math.Round((maxPos.Z - minPos.Z) / z_unit, 0);

            numbits_x = (int)Math.Log(x, 2);
            numbits_y = (int)Math.Log(y, 2);
            numbits_z = (int)Math.Log(z, 2);

            minX = minPos.X;
            maxX = maxPos.X;
            minY = minPos.Y;
            maxY = maxPos.Y;
            minZ = minPos.Z;
            maxZ = maxPos.Z;
        }
        #endregion

        #region 编码
        public string Encode(BoundingBox sectionBox)
        {
            return Encode(sectionBox.Min, sectionBox.Max);
        }
        public string Encode(Vector3 minPos, Vector3 maxPos)
        {
            string xbits = GetBits(minPos.X, maxPos.X, minX, maxX, numbits_x);
            string ybits = GetBits(minPos.Y, maxPos.Y, minY, maxY, numbits_y);
            string zbits = GetBits(minPos.Z, maxPos.Z, minZ, maxZ, numbits_z);

            return xbits + "_" + ybits+"_"+ zbits;
        }

        private string GetBits(double min, double max, double floor, double ceiling, int number)
        {
            //BitSet buffer = new BitSet(number);
            StringBuilder buffer = new StringBuilder();

            for (int i = 0; i < number; i++)
            {
                double mid = (floor + ceiling) / 2;
                if (min >= mid)
                {
                    buffer.Append("1");
                    floor = mid;
                }
                else if (max < mid)
                {
                    buffer.Append("0");
                    ceiling = mid;
                }
                else
                    break;
            }
            return buffer.ToString();
        } 
        #endregion
    }
}
