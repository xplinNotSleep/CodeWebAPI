using AgDataHandle.Maths.SpatialAlternation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgDataHandle.Maths.Numerics
{
    public class BoundingBox
    {
        public Vector3 Min { get { return new Vector3((float)MinX, (float)MinY, (float)MinZ); } }
        public Vector3 Max { get { return new Vector3((float)MaxX, (float)MaxY, (float)MaxZ); } }

        public double MaxX = double.MinValue;
        public double MaxY = double.MinValue;
        public double MaxZ = double.MinValue;
        public double MinX = double.MaxValue;
        public double MinY = double.MaxValue;
        public double MinZ = double.MaxValue;

        public double Width { get { return MaxX - MinX; } }
        public double Height { get { return MaxY - MinY; } }
        public double ZWidth { get { return MaxZ - MinZ; } }
        #region 构造函数

        public BoundingBox(Vector3 min, Vector3 max)
        {
            MaxX = max.X;
            MaxY = max.Y;
            MaxZ = max.Z;
            MinX = min.X;
            MinY = min.Y;
            MinZ = min.Z;
        }
        public BoundingBox(double minx, double maxx, double miny, double maxy, double minz = double.MaxValue, double maxz = double.MinValue)
        {
            MinX = minx;
            MinY = miny;
            MinZ = minz;
            MaxX = maxx;
            MaxY = maxy;
            MaxZ = maxz;
        }

        public BoundingBox()
        {
        }

        public BoundingBox(BoundingVolume boundingVolume)
        {
            MinX = boundingVolume.Box[0] - Math.Abs(boundingVolume.Box[3]);
            MaxX = boundingVolume.Box[0] + Math.Abs(boundingVolume.Box[3]);
            MinY = boundingVolume.Box[1] - Math.Abs(boundingVolume.Box[7]);
            MaxY = boundingVolume.Box[1] + Math.Abs(boundingVolume.Box[7]);
            MinZ = boundingVolume.Box[2] - Math.Abs(boundingVolume.Box[11]);
            MaxZ = boundingVolume.Box[2] + Math.Abs(boundingVolume.Box[11]);
        }
        public BoundingBox(BoundingBox other)
        {
            MinX = other.MinX;
            MinY = other.MinY;
            MinZ = other.MinZ;
            MaxX = other.MaxX;
            MaxY = other.MaxY;
            MaxZ = other.MaxZ;
        }
        public BoundingBox(IEnumerable<Vector2d> points)
        {
            this.Update(points.ToList());
        }
        #endregion

        #region 扩张
        public void Update(Vector2 v)
        {
            MaxX = MaxX < v.X ? v.X : MaxX;
            MaxY = MaxY < v.Y ? v.Y : MaxY;
            MinX = MinX > v.X ? v.X : MinX;
            MinY = MinY > v.Y ? v.Y : MinY;
        }
        public void Update(Vector2d v)
        {
            MinX = MinX < v.X ? MinX : (double)v.X;
            MinY = MinY < v.Y ? MinY : (double)v.Y;

            MaxX = MaxX > v.X ? MaxX : (double)v.X;
            MaxY = MaxY > v.Y ? MaxY : (double)v.Y;
        }

        public void Update(Vector3 v)
        {
            MaxX = MaxX < v.X ? v.X : MaxX;
            MaxY = MaxY < v.Y ? v.Y : MaxY;
            MaxZ = MaxZ < v.Z ? v.Z : MaxZ;

            MinX = MinX > v.X ? v.X : MinX;
            MinY = MinY > v.Y ? v.Y : MinY;
            MinZ = MinZ > v.Z ? v.Z : MinZ;
        }
        public void Update(Vector3d v)
        {
            MaxX = MaxX < v.X ? v.X : MaxX;
            MaxY = MaxY < v.Y ? v.Y : MaxY;
            MaxZ = MaxZ < v.Z ? v.Z : MaxZ;

            MinX = MinX > v.X ? v.X : MinX;
            MinY = MinY > v.Y ? v.Y : MinY;
            MinZ = MinZ > v.Z ? v.Z : MinZ;
        }

        public void Update(double x, double y, double z)
        {
            MaxX = MaxX < x ? x : MaxX;
            MaxY = MaxY < y ? y : MaxY;
            MaxZ = MaxZ < z ? z : MaxZ;

            MinX = MinX > x ? x : MinX;
            MinY = MinY > y ? y : MinY;
            MinZ = MinZ > z ? z : MinZ;
        }

        public void Update(BoundingBox boundingBox)
        {
            MaxX = MaxX < boundingBox.MaxX ? boundingBox.MaxX : MaxX;
            MaxY = MaxY < boundingBox.MaxY ? boundingBox.MaxY : MaxY;
            MaxZ = MaxZ < boundingBox.MaxZ ? boundingBox.MaxZ : MaxZ;

            MinX = MinX > boundingBox.MinX ? boundingBox.MinX : MinX;
            MinY = MinY > boundingBox.MinY ? boundingBox.MinY : MinY;
            MinZ = MinZ > boundingBox.MinZ ? boundingBox.MinZ : MinZ;
        }

        public void Update(List<Vector2d> vertices)
        {
            foreach (var item in vertices)
            {
                Update(item);
            }
        }

        public void Update(List<Vector3> vertices)
        {
            foreach (var item in vertices)
            {
                Update(item.X, item.Y, item.Z);
            }
        }

        public void Reset()
        {
            MinX = double.MaxValue;
            MinY = double.MaxValue;
            MaxX = double.MinValue;
            MaxY = double.MinValue;
        }

        public void Offset(Vector3 v)
        {
            this.MinX += v.X;
            this.MaxX += v.X;
            this.MinY += v.Y;
            this.MaxY += v.Y;
            this.MinZ += v.Z;
            this.MaxZ += v.Z;
        }

        /// <summary>
        /// 缩放包围盒
        /// </summary>
        /// <param name="value"></param>
        public void Scale(double value = 1)
        {
            var xDis = MaxX- MinX;
            var xScaleDis = xDis*(value-1)/2;
            MaxX+=xScaleDis;
            MinX-=xScaleDis;
            var yDis = MaxY- MinY;
            var yScaleDis = yDis*(value-1)/2;
            MaxY+=yScaleDis;
            MaxY-=yScaleDis;
        }

        /// <summary>
        /// 设置最低包围盒高度
        /// </summary>
        public void SetZMinValue(double zValue)
        {
            var zDis = MaxZ- MinZ;
            MaxZ = zDis > zValue ? MaxZ : (MinZ + zValue);
            //MaxZ=MaxZ>zValue ? MaxZ : zValue;
        }
        #endregion

        #region 计算关系
        /// <summary>
        /// 判断包含关系 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Contains(double x, double y)
        {
            return MinX <= x
                && x <= MaxX
                && MinY <= y
                && y <= MaxY;
        }
        public bool Contains(Vector2d v)
        {
            return MinX <= v.X
                && v.X <= MaxX
                && MinY <= v.Y
                && v.Y <= MaxY;
        }
        public bool Contains(Vector3 box, double buffer = 0)
        {
            if (MinX - buffer > box.X) return false;
            if (MinY - buffer > box.Y) return false;
            if (MinZ - buffer > box.Z) return false;
            if (MaxX + buffer < box.X) return false;
            if (MaxY + buffer < box.Y) return false;
            if (MaxZ + buffer < box.Z) return false;
            return true;
        }

        public bool ContainsIngoreZ(Vector3 box, double buffer = 0)
        {
            if (MinX - buffer > box.X) return false;
            if (MinY - buffer > box.Y) return false;
            if (MaxX + buffer < box.X) return false;
            if (MaxY + buffer < box.Y) return false;
            return true;
        }
        public bool ContainsIngoreZ(List<Vector3> list,double buffer=0)
        {
            var flag = true;
            foreach (var item in list)
            {
                if (!ContainsIngoreZ(item,buffer))
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }
        public bool ContainsIngoreZ(BoundingBox bb)
        {
            if(!ContainsIngoreZ(bb.Min))
                return false;
            if (!ContainsIngoreZ(bb.Max))
                return false;
            return true;
        }
        public bool Contains(Vector3 v)
        {
            return (v.X >= MinX && v.X <= MaxX) &&
                   (v.Y >= MinY && v.Y <= MaxY) &&
                   (v.Z >= MinZ && v.Z <= MaxZ);
        }
        public bool IntersectXY(BoundingBox bb)
        {
            if (MinX > bb.MaxX) return false;
            if (MaxX < bb.MinX) return false;
            if (MinY > bb.MaxY) return false;
            if (MaxY < bb.MinY) return false;
            return true;
        }

        public bool Intersects(BoundingBox bb)
        {
            if (bb == null)
                return false;
            var box2Min = bb.Min;
            var box2Max = bb.Max;
            if (Min[0] > box2Max[0]) return false;
            if (Max[0] < box2Min[0]) return false;
            if (Min[1] > box2Max[1]) return false;
            if (Max[1] < box2Min[1]) return false;
            if (Min[2] > box2Max[2]) return false;
            if (Max[2] < box2Min[2]) return false;

            return true;
        }

        public bool Contains(BoundingBox bb)
        {
            var flag = false;
            for (int i = 0; i < 8; i++)
            {
                var point = bb.Corner(i);
                if (this.Contains(point))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public bool Equals(BoundingBox bb)
        {
            return this.Min.Equals(bb.Min) && this.Max.Equals(bb.Max);
        }

        public bool IsContains(BoundingBox box)
        {
            if (MinX > box.MinX) return false;
            if (MinY > box.MinY) return false;
            if (MaxX < box.MaxX) return false;
            if (MaxY < box.MaxY) return false;
            return true;
        }
        /// <summary>
        /// 计算两个包围盒相交面积，前提是先判断是否相交
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double ComputeIntersectAreaLatLon(BoundingBox other)
        {
            if (!IntersectXY(other)) return 0;
            List<double> XArr = new List<double>() { MinX, MaxX, other.MinX, other.MaxX };
            List<double> YArr = new List<double>() { MinY, MaxY, other.MinY, other.MaxY };
            XArr.Remove(XArr.Min());
            XArr.Remove(XArr.Max());
            YArr.Remove(YArr.Min());
            YArr.Remove(YArr.Max());
            double xDif = Math.Abs(XArr[0] - XArr[1]);
            double yDif = Math.Abs(YArr[0] - YArr[1]);
            //这里的经纬度转米的方法暂时不修改，一是两个包围盒不好计算出一个合适的中心点，二是只是计算包围盒相交面积，受精度影响不大
            double xDifMeter = LatLonHelper.LongitudeToMeters(xDif, LatLonHelper.Degree2rad((YArr[0] - YArr[1]) / 2));
            double yDifMeter = LatLonHelper.LatitudeToMeters(yDif);
            return xDifMeter * yDifMeter;
        }
        public double ComputeIntersectArea(BoundingBox other, bool checkBoundingBoxIntersectFirst = false)
        {
            //避免重复判断包围盒是否相交，加入控制开关
            if (checkBoundingBoxIntersectFirst)
            {
                if (!IntersectXY(other)) 
                    return 0;
            }
            List<double> XArr = new List<double>() { MinX, MaxX, other.MinX, other.MaxX };
            List<double> YArr = new List<double>() { MinY, MaxY, other.MinY, other.MaxY };
            XArr.Remove(XArr.Min());
            XArr.Remove(XArr.Max());
            YArr.Remove(YArr.Min());
            YArr.Remove(YArr.Max());
            double xDif = Math.Abs(XArr[0] - XArr[1]);
            double yDif = Math.Abs(YArr[0] - YArr[1]);
            return xDif * yDif;
        }
        #endregion

        #region 计算位置

        public Vector3 Corner(int pos)
        {
            Vector3 result = null;
            switch (pos)
            {
                case 0:
                    result = new Vector3((float)MinX, (float)MinY, (float)MinZ);
                    break;
                case 1:
                    result = new Vector3((float)MinX, (float)MaxY, (float)MinZ);
                    break;
                case 2:
                    result = new Vector3((float)MaxX, (float)MaxY, (float)MinZ);
                    break;
                case 3:
                    result = new Vector3((float)MaxX, (float)MinY, (float)MinZ);
                    break;
                case 4:
                    result = new Vector3((float)MaxX, (float)MinY, (float)MaxZ);
                    break;
                case 5:
                    result = new Vector3((float)MinX, (float)MinY, (float)MaxZ);
                    break;
                case 6:
                    result = new Vector3((float)MinX, (float)MaxY, (float)MaxZ);
                    break;
                case 7:
                    result = new Vector3((float)MaxX, (float)MaxY, (float)MaxZ);
                    break;
                default:
                    break;
            }
            return result;
        }

        public Vector3 Center()
        {
            double x = (MinX + MaxX) / 2;
            double y = (MinY + MaxY) / 2;
            double z = (MinZ + MaxZ) / 2;
            return new Vector3((float)x, (float)y, (float)z);
        }

        public Vector2d CenterXY()
        {
            return new Vector2d((MinX + MaxX) / 2, (MinY + MaxY) / 2);
        }

        public Vector3 HalfLength()
        {
            var x = (Max.X - Min.X) / 2;
            var y = (Max.Y - Min.Y) / 2;
            var z = (Max.Z - Min.Z) / 2;
            return new Vector3((float)x, (float)y, (float)z);
        }

        public void Combine(BoundingBox box2)
        {
            this.MinX = this.MinX > box2.Min.X ? box2.Min.X : this.MinX;
            this.MinY = this.MinY > box2.Min.Y ? box2.Min.Y : this.MinY;
            this.MinZ = this.MinZ > box2.Min.Z ? box2.Min.Z : this.MinZ;

            this.MaxX = this.MaxX < box2.Max.X ? box2.Max.X : this.MaxX;
            this.MaxY = this.MaxY < box2.Max.X ? box2.Max.X : this.MaxY;
            this.MaxZ = this.MaxZ < box2.Max.X ? box2.Max.X : this.MaxZ;

        }

        public BoundingVolume ToBoundingVolume()
        {
            var bound = new BoundingVolume();
            var position = new Vector3((float)(MinX + MaxX) / 2, (float)(MinY + MaxY) / 2, (float)(MinZ + MaxZ) / 2);
            var l = (MaxX - MinX) / 2;
            var w = (MaxY - MinY) / 2;
            var h = (MaxZ - MinZ) / 2;
            double[] Box = {
            position.X,position.Y,position.Z,l,
            0,0,0,w,
            0,0,0,h};
            bound.Box = Box;
            return bound;
        }

        public string ToArrayString()
        {
            return "[" + MinX + "," + MinY + "," + MaxX + "," + MaxY + "]";
        }

        public double[] ToArray()
        {
            return new double[] { MinX, MinY, MaxX, MaxY };
        }

        public Vector3 BinaryCorner(int pos)
        {
            Vector3 result = null;
            double x = (MinX + MaxX) / 2;
            double y = (MinY + MaxY) / 2;
            double z = (MinZ + MaxZ) / 2;
            switch (pos)
            {
                case 0:
                    result = new Vector3((float)x, (float)MinY, (float)MinZ);
                    break;
                case 1:
                    result = new Vector3((float)x, (float)MaxY, (float)MinZ);
                    break;
                case 2:
                    result = new Vector3((float)x, (float)MaxY, (float)MaxZ);
                    break;
                case 3:
                    result = new Vector3((float)x, (float)MinY, (float)MaxZ);
                    break;
                case 4:
                    result = new Vector3((float)MaxX, (float)y, (float)MinZ);
                    break;
                case 5:
                    result = new Vector3((float)MaxX, (float)y, (float)MaxZ);
                    break;
                case 6:
                    result = new Vector3((float)MinX, (float)y, (float)MaxZ);
                    break;
                case 7:
                    result = new Vector3((float)MinX, (float)y, (float)MinZ);
                    break;
                case 8:
                    result = new Vector3((float)MaxX, (float)MaxY, (float)z);
                    break;
                case 9:
                    result = new Vector3((float)MaxX, (float)MinY, (float)z);
                    break;
                case 10:
                    result = new Vector3((float)MinX, (float)MaxY, (float)z);
                    break;
                case 11:
                    result = new Vector3((float)MinX, (float)MinY, (float)z);
                    break;
                default:
                    break;
            }
            return result;
        }
        #endregion

        #region 多边形合并工具方法
        public double MinDistanceTo(BoundingBox other)
        {
            //b | a
            if (this.MinX > other.MaxX)
            {
                //a/b
                var dx = this.MinX - other.MaxX;
                if (this.MinY > other.MaxY)
                {
                    var dy = this.MinY - other.MaxY;
                    return Math.Sqrt((dx * dx) + dy * dy);
                }
                if (this.MaxY < other.MinY)
                {
                    var dy = this.MaxY - other.MinY;
                    return Math.Sqrt((dx * dx) + dy * dy);
                }
                return dx;
            }
            if (this.MaxX < other.MinX)
            {
                //a/b
                var dx = this.MinX - other.MaxX;
                if (this.MinY > other.MaxY)
                {
                    var dy = this.MinY - other.MaxY;
                    return Math.Sqrt((dx * dx) + dy * dy);
                }
                if (this.MaxY < other.MinY)
                {
                    var dy = this.MaxY - other.MinY;
                    return Math.Sqrt((dx * dx) + dy * dy);
                }
                return dx;
            }
            if (this.MinY > other.MaxY)
            {
                //a/b
                var dy = this.MinY - other.MaxY;
                if (this.MinX > other.MaxX)
                {
                    var dx = this.MinX - other.MaxX;
                    return Math.Sqrt((dx * dx) + dy * dy);
                }
                if (this.MaxX < other.MinX)
                {
                    var dx = this.MaxX - other.MinX;
                    return Math.Sqrt((dx * dx) + dy * dy);
                }
                return dy;
            }
            if (this.MaxY < other.MinY)
            {
                //a/b
                var dy = other.MinY - this.MaxY;
                if (this.MinX > other.MaxX)
                {
                    var dx = this.MinX - other.MaxX;
                    return Math.Sqrt((dx * dx) + dy * dy);
                }
                if (this.MaxX < other.MinX)
                {
                    var dx = other.MinX - this.MaxX;
                    return Math.Sqrt((dx * dx) + dy * dy);
                }
                return dy;
            }
            return 0;
        }
        public double ContainsRadio(BoundingBox bbi)
        {
            if (this.Contains(bbi.MinX, bbi.MinY) && this.Contains(bbi.MaxX, bbi.MaxY))
            {
                return 1;
            }
            double size = (bbi.MaxX - bbi.MinX) * (bbi.MaxY - bbi.MinY);
            if (this.Contains(bbi.MinX, bbi.MinY))
            {
                double dx = this.MaxX - bbi.MinX;
                double dy = this.MaxY - bbi.MinY;
                return (dx * dy) / size;
            }
            if (this.Contains(bbi.MaxX, bbi.MinY))
            {
                double dx = bbi.MaxX - this.MinX;
                double dy = this.MaxY - bbi.MinY;
                return (dx * dy) / size;
            }
            if (this.Contains(bbi.MinX, bbi.MaxY))
            {
                double dx = this.MaxX - bbi.MinX;
                double dy = bbi.MaxY - this.MinY;
                return (dx * dy) / size;
            }
            if (this.Contains(bbi.MaxX, bbi.MaxY))
            {
                double dx = bbi.MaxX - this.MinX;
                double dy = bbi.MaxY - this.MinY;
                return (dx * dy) / size;
            }

            //以下都暂不考虑bbi包含this的情况
            if (bbi.MaxX > this.MaxX && bbi.MinX < this.MinX)
            {
                double dx = this.MaxX - this.MinX;
                if (bbi.MinY > this.MinY && bbi.MaxY < this.MaxY)//中间
                {
                    double dy = bbi.MaxY - bbi.MinY;
                    return (dx * dy) / size;
                }
                if (bbi.MaxY > this.MaxY && bbi.MinY < this.MaxY && bbi.MinY > this.MinY)
                {
                    double dy = this.MaxY - bbi.MinY;
                    return (dx * dy) / size;
                }
                if (bbi.MinY < this.MinY && bbi.MaxY > this.MinY && bbi.MaxY < this.MaxY)
                {
                    double dy = bbi.MaxY - this.MinY;
                    return (dx * dy) / size;
                }
            }
            if (bbi.MaxY > this.MaxY && bbi.MinY < this.MinY)
            {
                double dy = this.MaxY - this.MinY;
                if (bbi.MinX > this.MinX && bbi.MaxX < this.MaxX)//中间
                {
                    double dx = bbi.MaxX - bbi.MinX;
                    return (dx * dy) / size;
                }
                if (bbi.MinX < this.MinX && bbi.MaxX > this.MinX && bbi.MaxX < this.MaxX)//左边凸出 且 右边不超出
                {
                    double dx = bbi.MaxX - this.MinX;
                    return (dx * dy) / size;
                }
                if (bbi.MaxX > this.MaxX && bbi.MinX > this.MinX && bbi.MinX < this.MaxX)//右边凸出 且 左边不超出
                {
                    double dx = this.MaxX - bbi.MinX;
                    return (dx * dy) / size;
                }
            }
            return 0;
        }
        public double GetArea()
        {
            return (MaxX - MinX) * (MaxY - MinY);
        }
        //计算经纬度包围盒的以米为单位的面积
        public double GetAreaLatLon()
        {
            Matrix4x4D matrix = GisUtilDouble.Wgs84Transform(LatLonHelper.Degree2rad((this.MinX + this.MaxX) / 2), LatLonHelper.Degree2rad((this.MinY + this.MaxY) / 2), 0).Inverse<Matrix4x4D>();
            Vector3d pointMin = GisUtilDouble.CesiumFromRadians(LatLonHelper.Degree2rad(this.MinX), LatLonHelper.Degree2rad(this.MinY), 0).MultiplyByMatrix<Vector3d>(matrix);
            Vector3d pointMax = GisUtilDouble.CesiumFromRadians(LatLonHelper.Degree2rad(this.MaxX), LatLonHelper.Degree2rad(this.MaxY), 0).MultiplyByMatrix<Vector3d>(matrix);
            return (pointMax.X - pointMin.X) * (pointMax.Y - pointMin.Y);
        }
        /// <summary>
        /// 获取包围盒体积，对于长宽高，当小于 阈值时，值为1
        /// </summary>
        /// <param name="threshold "></param>
        /// <returns></returns>
        public double GetVolume(float threshold = 0.001f)
        {
            var l = (MaxX - MinX) < threshold ? 1 : (MaxX - MinX);
            var w = (MaxY - MinY) < threshold ? 1 : (MaxY - MinY);
            var h = (MaxZ - MinZ) < threshold ? 1 : (MaxZ - MinZ);
            return l * w * h;
        }
        #endregion

        #region 变换
        public void Transform(IMatrix4x4 matrix4X4)
        {
            var re = new Vector4((float)MaxX, (float)MaxY, (float)MaxZ, 1).MultiplyByMatrix<Vector4>(matrix4X4);
            Update(re.X, re.Y, re.Z);
            re = new Vector4((float)MinX, (float)MinY, (float)MinZ, 1).MultiplyByMatrix<Vector4>(matrix4X4);
            Update(re.X, re.Y, re.Z);
        }

        public void Transform2(IMatrix4x4 matrix4X4)
        {
            var re = new Vector3((float)MaxX, (float)MaxY, (float)MaxZ).MultiplyByMatrix<Vector3>(matrix4X4);
            var re2 = new Vector3((float)MinX, (float)MinY, (float)MinZ).MultiplyByMatrix<Vector3>(matrix4X4);
            if(re.X>re2.X)
            {
                MinX=re2.X;
                MaxX=re.X;
            }
            else
            {
                MinX=re.X;
                MaxX=re2.X;
            }
            if (re.Y>re2.Y)
            {
                MinY=re2.Y;
                MaxY=re.Y;
            }
            else
            {
                MinY=re.Y;
                MaxY=re2.Y;
            }
            if (re.Z>re2.Z)
            {
                MinZ=re2.Z;
                MaxZ=re.Z;
            }
            else
            {
                MinZ=re.Z;
                MaxZ=re2.Z;
            }
        }

        public BoundingBox TransformToNew(IMatrix4x4 matrix4X4)
        {
            BoundingBox boundingBox = new BoundingBox();
            var re = new Vector3((float)MaxX, (float)MaxY, (float)MaxZ).MultiplyByMatrix<Vector3>(matrix4X4);
            var re2 = new Vector3((float)MinX, (float)MinY, (float)MinZ).MultiplyByMatrix<Vector3>(matrix4X4);
            if (re.X > re2.X)
            {
                boundingBox.MinX = re2.X;
                boundingBox.MaxX = re.X;
            }
            else
            {
                boundingBox.MinX = re.X;
                boundingBox.MaxX = re2.X;
            }
            if (re.Y > re2.Y)
            {
                boundingBox.MinY = re2.Y;
                boundingBox.MaxY = re.Y;
            }
            else
            {
                boundingBox.MinY = re.Y;
                boundingBox.MaxY = re2.Y;
            }
            if (re.Z > re2.Z)
            {
                boundingBox.MinZ = re2.Z;
                boundingBox.MaxZ = re.Z;
            }
            else
            {
                boundingBox.MinZ = re.Z;
                boundingBox.MaxZ = re2.Z;
            }
            return boundingBox;
        }
        #endregion

        #region 分裂
        /// <summary>
        /// 以四叉树的形式分裂包围盒
        /// </summary>
        /// <returns></returns>
        public List<BoundingBox> SplitByQuadtree()
        {
            Vector3 center = Center();
            Vector3 buttomCenter = center - new Vector3(0, 0, HalfLength().Z);
            List<BoundingBox> boxList = new List<BoundingBox>();
            BoundingBox box0 = new BoundingBox();
            box0.Update(buttomCenter);
            box0.Update(new Vector3(MinX, MinY, MaxZ));
            boxList.Add(box0);
            BoundingBox box1 = new BoundingBox();
            box1.Update(buttomCenter);
            box1.Update(new Vector3(MaxX, MinY, MaxZ));
            boxList.Add(box1);
            BoundingBox box2 = new BoundingBox();
            box2.Update(buttomCenter);
            box2.Update(new Vector3(MinX, MaxY, MaxZ));
            boxList.Add(box2);
            BoundingBox box3 = new BoundingBox();
            box3.Update(buttomCenter);
            box3.Update(new Vector3(MaxX, MaxY, MaxZ));
            boxList.Add(box3);
            return boxList;
        }

        /// <summary>
        /// 以四叉树的形式分裂包围盒
        /// </summary>
        /// <returns></returns>
        public List<BoundingBox> SplitByOctree()
        {
            var center = Center();
            var boxList = new List<BoundingBox>();
            for (int i = 0; i < 8; i++)
            {
                var box = new BoundingBox();
                box.Update(center);
                box.Update(Corner(i));
                boxList.Add(box);
            }

            return boxList;
        }

        /// <summary>
        /// 一分为二分裂包围盒
        /// </summary>
        /// <returns></returns>
        public List<BoundingBox> SplitByBinary()
        {
            var boxList = new List<BoundingBox>();
            var box = new BoundingBox();
            if ( this.ZWidth> this.Height && this.ZWidth>this.Width)
            {
                box.Update(Min);
                for (int i = 8; i < 12; i++)
                {
                    box.Update(BinaryCorner(i));
                }
                boxList.Add(box);
                var box1 = new BoundingBox();
                box1.Update(Max);
                for (int i = 8; i < 12; i++)
                {
                    box1.Update(BinaryCorner(i));
                }
                boxList.Add(box1);
            }
            else
            {
                if (this.Width > this.Height)
                {
                    box.Update(Min);
                    for (int i = 0; i < 4; i++)
                    {
                        box.Update(BinaryCorner(i));
                    }
                    boxList.Add(box);
                    var box1 = new BoundingBox();
                    box1.Update(Max);
                    for (int i = 0; i < 4; i++)
                    {
                        box1.Update(BinaryCorner(i));
                    }
                    boxList.Add(box1);
                }
                else
                {
                    box.Update(Min);
                    for (int i = 4; i < 8; i++)
                    {
                        box.Update(BinaryCorner(i));
                    }
                    boxList.Add(box);
                    var box1 = new BoundingBox();
                    box1.Update(Max);
                    for (int i = 4; i < 8; i++)
                    {
                        box1.Update(BinaryCorner(i));
                    }
                    boxList.Add(box1);
                }
            }
            return boxList;
        }
        /// <summary>
        /// 使用固定大小均匀划分包围盒
        /// </summary>
        /// <param name="unit">划分的包围盒单元大小</param>
        /// <returns></returns>
        public BoundingBox[,,] SplitByGrid(float unit)
        {
            BoundingBox bounds = this;
            float hunit = unit * 0.5f;
            Vector3 start = bounds.Min + new Vector3(hunit, hunit, hunit);

            int width = (int)Math.Ceiling(bounds.Width / unit);
            int height = (int)Math.Ceiling(bounds.Height / unit);
            int depth = (int)Math.Ceiling(bounds.ZWidth / unit);

            BoundingBox[,,] boxes = new BoundingBox[width, height, depth];
            Vector3 voxelSize = Vector3.One * hunit;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        Vector3 p = new Vector3(x, y, z) * unit + start;
                        BoundingBox aabb = new BoundingBox();
                        aabb.Update(p - voxelSize);
                        aabb.Update(p + voxelSize);
                        boxes[x, y, z] = aabb;
                    }
                }
            }
            return boxes;
        }
        public BoundingBox[,] SplitByGridIgnoreZ(float unit)
        {
            BoundingBox bounds = this;

            int width = (int)Math.Ceiling(bounds.Width / unit);
            int height = (int)Math.Ceiling(bounds.Height / unit);

            BoundingBox[,] boxes = new BoundingBox[width, height];
            Vector3 start = bounds.Min;
            Vector3 pizelSize = new Vector3(1, 1, 0) * unit;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 p = new Vector3(x, y, 0) * unit + start;
                    BoundingBox aabb = new BoundingBox();
                    aabb.Update(p);
                    aabb.Update(p + pizelSize);
                    boxes[x, y] = aabb;
                }
            }
            return boxes;
        }
        #endregion

    }
}
