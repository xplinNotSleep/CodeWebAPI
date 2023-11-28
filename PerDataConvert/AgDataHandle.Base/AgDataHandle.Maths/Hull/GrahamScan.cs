using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    public static class GrahamScan
    {
        const int TURN_LEFT = 1;
        const int TURN_RIGHT = -1;
        const int TURN_NONE = 0;
        public static int turn(HullNode p, HullNode q, HullNode r)
        {
            return ((q.x - p.x) * (r.y - p.y) - (r.x - p.x) * (q.y - p.y)).CompareTo(0);
        }

        public static void keepLeft(List<HullNode> hull, HullNode r)
        {
            while (hull.Count > 1 && turn(hull[hull.Count - 2], hull[hull.Count - 1], r) != TURN_LEFT)
            {
                hull.RemoveAt(hull.Count - 1);
            }
            if (hull.Count == 0 || hull[hull.Count - 1] != r)
            {
                hull.Add(r);
            }
        }

        public static double getAngle(HullNode p1, HullNode p2)
        {
            double xDiff = p2.x - p1.x;
            double yDiff = p2.y - p1.y;
            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }

        public static List<HullNode> MergeSort(HullNode p0, List<HullNode> arrPoint)
        {
            if (arrPoint.Count == 1)
            {
                return arrPoint;
            }
            List<HullNode> arrSortedInt = new List<HullNode>();
            int middle = (int)arrPoint.Count / 2;
            List<HullNode> leftArray = arrPoint.GetRange(0, middle);
            List<HullNode> rightArray = arrPoint.GetRange(middle, arrPoint.Count - middle);
            leftArray = MergeSort(p0, leftArray);
            rightArray = MergeSort(p0, rightArray);
            int leftptr = 0;
            int rightptr = 0;
            for (int i = 0; i < leftArray.Count + rightArray.Count; i++)
            {
                if (leftptr == leftArray.Count)
                {
                    arrSortedInt.Add(rightArray[rightptr]);
                    rightptr++;
                }
                else if (rightptr == rightArray.Count)
                {
                    arrSortedInt.Add(leftArray[leftptr]);
                    leftptr++;
                }
                else if (getAngle(p0, leftArray[leftptr]) < getAngle(p0, rightArray[rightptr]))
                {
                    arrSortedInt.Add(leftArray[leftptr]);
                    leftptr++;
                }
                else
                {
                    arrSortedInt.Add(rightArray[rightptr]);
                    rightptr++;
                }
            }
            return arrSortedInt;
        }

        public static List<HullNode> convexHull(List<HullNode> points)
        {
            HullNode p0 = null;
            foreach (HullNode value in points)
            {
                if (p0 == null)
                    p0 = value;
                else
                {
                    if (p0.y > value.y)
                        p0 = value;
                }
            }
            List<HullNode> order = new List<HullNode>();
            foreach (HullNode value in points)
            {
                if (p0 != value)
                    order.Add(value);
            }

            order = MergeSort(p0, order);
            List<HullNode> result = new List<HullNode>();
            result.Add(p0);
            result.Add(order[0]);
            result.Add(order[1]);
            order.RemoveAt(0);
            order.RemoveAt(0);
            foreach (HullNode value in order)
            {
                keepLeft(result, value);
            }
            return result;
        }
    }
}
