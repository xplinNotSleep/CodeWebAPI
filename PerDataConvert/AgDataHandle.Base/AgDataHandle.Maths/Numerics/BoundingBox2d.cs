using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.Numerics
{
    public class BoundingBox2d
    {
        public Vector2d Min { get { return new Vector2d(MinX, MinY); } }

        public Vector2d Max { get { return new Vector2d(MaxX, MaxY); } }
        public double MinX { get; private set; }
        public double MaxX { get; private set; }
        public double MinY { get; private set; }
        public double MaxY { get; private set; }
        public double Width { get { return MaxX - MinX; } }
        public double Height { get { return MaxY - MinY; } }

        public BoundingBox2d()
        {
            MinX = double.MaxValue;
            MinY = double.MaxValue;
            MaxX = double.MinValue;
            MaxY = double.MinValue;
        }
        public BoundingBox2d(double xmin, double xmax, double ymin, double ymax)
        {
            MinX = xmin;
            MaxX = xmax;
            MinY = ymin;
            MaxY = ymax;
        }
        public void Update(Vector2d p)
        {
            MinX = p.X < MinX ? p.X : MinX;
            MaxX = p.X > MaxX ? p.X : MaxX;
            MinY = p.Y < MinY ? p.Y : MinY;
            MaxY = p.Y > MaxY ? p.Y : MaxY;
        }
        public void Update(double x, double y)
        {
            MinX = x < MinX ? x : MinX;
            MaxX = x > MaxX ? x : MaxX;
            MinY = y < MinY ? y : MinY;
            MaxY = y > MaxY ? y : MaxY;
        }
        public Vector2d Center()
        {
            double x = (MinX + MaxX) / 2;
            double y = (MinY + MaxY) / 2;
            return new Vector2d(x, y);
        }
    }
}
