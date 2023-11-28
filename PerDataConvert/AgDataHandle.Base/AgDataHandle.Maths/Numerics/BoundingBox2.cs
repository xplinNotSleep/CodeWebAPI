using AgDataHandle.Maths;
using System.Collections.Generic;

namespace AgDataHandle.Maths.Numerics
{
    public class BoundingBox2
    {
        public Vector2 Min { get; set; }

        public Vector2 Max { get; set; }
        public float xMin { get; private set; }
        public float xMax { get; private set; }
        public float yMin { get; private set; }
        public float yMax { get; private set; }
        public float Width { get { return xMax - xMin; } }
        public float Height { get { return yMax - yMin; } }
        public bool IsValid()
        {
            return Min.X < Max.X && Min.Y < Max.Y;
        }

        public bool IsIn(Vector2 p)
        {
            return p.X > Min.X && p.X < Max.X && p.Y > Min.Y && p.Y < Max.Y;
        }

        public BoundingBox2()
        {
            xMin = float.MaxValue;
            yMin = float.MaxValue;
            xMax = float.MinValue;
            yMax = float.MinValue;
        }
        public BoundingBox2(float xmin, float xmax, float ymin, float ymax)
        {
            xMin = xmin;
            xMax = xmax;
            yMin = ymin;
            yMax = ymax;
            Min = new Vector2(xMin, yMin);
            Max = new Vector2(xMax, yMax);
        }
        public static BoundingBox2 New()
        {
            return new BoundingBox2
            {
                Min = new Vector2(float.MaxValue, float.MaxValue),
                Max = new Vector2(float.MinValue, float.MinValue)
            };

        }

        public BoundingBox2(List<Vector2> vector2List)
        {
            xMin = vector2List[0].X;
            xMax = vector2List[0].X;
            yMin = vector2List[0].Y;
            yMax = vector2List[0].Y;
            for (var i = 1; i < vector2List.Count; i++)
            {
                xMin = vector2List[i].X < xMin ? vector2List[i].X : xMin;
                xMax = vector2List[i].X > xMax ? vector2List[i].X : xMax;
                yMin = vector2List[i].Y < yMin ? vector2List[i].Y : yMin;
                yMax = vector2List[i].Y > yMax ? vector2List[i].Y : yMax;
            }
            Min = new Vector2(xMin,yMin);
            Max = new Vector2(xMax, yMax);
        }

        #region 更新包围盒
        public void Update(List<Vector2> vector2List)
        {
            for (var i = 0; i < vector2List.Count; i++)
            {
                xMin = vector2List[i].X < xMin ? vector2List[i].X : xMin;
                xMax = vector2List[i].X > xMax ? vector2List[i].X : xMax;
                yMin = vector2List[i].Y < yMin ? vector2List[i].Y : yMin;
                yMax = vector2List[i].Y > yMax ? vector2List[i].Y : yMax;
            }
            Min = new Vector2(xMin, yMin);
            Max = new Vector2(xMax, yMax);
        }
        public void Update(Vector2 p)
        {
            xMin = p.X < xMin ? p.X : xMin;
            xMax = p.X > xMax ? p.X : xMax;
            yMin = p.Y < yMin ? p.Y : yMin;
            yMax = p.Y > yMax ? p.Y : yMax;
            Min = new Vector2(xMin, yMin);
            Max = new Vector2(xMax, yMax);
        }
        public void Update(float x,float y)
        {
            xMin = x < xMin ? x : xMin;
            xMax = x > xMax ? x : xMax;
            yMin = y < yMin ? y : yMin;
            yMax = y > yMax ? y : yMax;
            Min = new Vector2(xMin, yMin);
            Max = new Vector2(xMax, yMax);
        }
        public void Update(BoundingBox2 boundingBox)
        {
            xMax = xMax < boundingBox.xMax ? boundingBox.xMax : xMax;
            yMax = yMax < boundingBox.yMax ? boundingBox.yMax : yMax;

            xMin = xMin > boundingBox.xMin ? boundingBox.xMin : xMin;
            yMin = yMin > boundingBox.yMin ? boundingBox.yMin : yMin;
        }
        #endregion

        public Vector2 Center()
        {
            double x = (xMin + xMax) / 2;
            double y = (yMin + yMax) / 2;
            return new Vector2(x, y);
        }
        public double GetArea()
        {
            return (xMax - xMin) * (yMax - yMin);
        }


        #region 包含判断
        public bool ContainsPoint(Vector2 p, float offset = 0.01f)
        {
            return p.X >= xMin - offset && p.X <= xMax + offset && p.Y >= yMin - offset && p.Y <= yMax + offset;
        }
        public bool ContainsPoints(List<Vector2> points, float offset = 0.01f)
        {
            var index = points.FindIndex(p => !ContainsPoint(p));
            return index == -1;
        }
        public bool Contains(BoundingBox2 box)
        {
            return box.xMin>=xMin&&box.xMax<=xMax&&box.yMin>=yMin&&box.yMax<=yMax;
        }

        #endregion

        public bool Intersect(BoundingBox2 box)
        {
            if (xMin > box.xMax || box.xMin > xMin) return false;
            if (yMin > box.yMax || box.yMin > yMin) return false;
            return true;
        }

        public BoundingBox2 Clone()
        {
            return new BoundingBox2(this.xMin,this.xMax,yMin,yMax);
        }


        #region 包围盒分裂
        /// <summary>
        /// /分裂包围盒成四个包围盒
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static List<BoundingBox2> SplitBoxToFour(BoundingBox2 box)
        {
            var list = new List<BoundingBox2>();
            var center = (box.Max + box.Min) / 2;
            var box0 = new BoundingBox2();
            box0.Update(center);
            box0.Update(box.Min);
            list.Add(box0);

            var box1 = new BoundingBox2();
            box1.Update(center);
            box1.Update(box.xMax,box.yMin);
            list.Add(box1);

            var box2 = new BoundingBox2();
            box2.Update(center);
            box2.Update(box.xMin, box.yMax);
            list.Add(box2);

            var box3 = new BoundingBox2();
            box3.Update(center);
            box3.Update(box.Max);
            list.Add(box3);
            return list;
        }
        /// <summary>
        /// 分裂盒子为上下两个
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static List<BoundingBox2> SplitBoxToTopBottom(BoundingBox2 box)
        {
            var list = new List<BoundingBox2>();
            var center = (box.Max + box.Min) / 2;
            var box0 = new BoundingBox2();
            box0.Update(box.Min);
            box0.Update(center.X+box.Width/2,center.Y);
            list.Add(box0);

            var box1 = new BoundingBox2();
            box1.Update(center.X - box.Width / 2, center.Y);
            box1.Update(box.Max);
            list.Add(box1);

            return list;
        }
        #endregion
    }
}
