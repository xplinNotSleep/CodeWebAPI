using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgDataHandle.Maths
{
    public class Hull
    {
        private static List<HullNode> unused_nodes = new List<HullNode>();
        private static List<HullLine> hull_edges = new List<HullLine>();
        private static List<HullLine> hull_concave_edges = new List<HullLine>();

        /// <summary>
        /// 获取外包多边形的点，不共用点
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="concavity"></param>
        /// <returns></returns>
        public static List<Vector2> GetConcaveHullPoints(List<Vector2> sourcePoints, int scaleFactor, double concavity)
        {
            unused_nodes.Clear();
            hull_edges.Clear();
            hull_concave_edges.Clear();
            List<HullNode> dot_list = new List<HullNode>();
            for (int i = 0; i < sourcePoints.Count; i++)
            {
                dot_list.Add(new HullNode(sourcePoints[i].X, sourcePoints[i].Y, i));
            }
            setConvexHull(dot_list);
            var lines = setConcaveHull(concavity, scaleFactor);
            var rePoints = new List<Vector2>();
            rePoints.Add(new Vector2((float)lines[0].nodes[0].x, (float)lines[0].nodes[0].y));
            var startLine = lines[0];
            lines.RemoveAt(0);
            var flag = true;
            while(flag)
            {
                var nextLine = lines.Find(p=>p.nodes[0].SamePositionNode(startLine.nodes[1]));
                if (nextLine != null)
                {
                    rePoints.Add(new Vector2((float)nextLine.nodes[0].x, (float)nextLine.nodes[0].y));
                    startLine = nextLine;
                    lines.Remove(nextLine);
                    continue;
                }

                nextLine = lines.Find(p => p.nodes[1].SamePositionNode(startLine.nodes[1]));
                if (nextLine != null)
                {
                    rePoints.Add(new Vector2((float)nextLine.nodes[1].x, (float)nextLine.nodes[1].y));
                    startLine = new HullLine(nextLine.nodes[1], nextLine.nodes[0]); 
                    lines.Remove(nextLine);
                    continue;
                }
                flag = false;
            }
            return rePoints;
        }

        private static List<HullLine> getHull(List<HullNode> nodes)
        {
            List<HullNode> convexH = new List<HullNode>();
            List<HullLine> exitLines = new List<HullLine>();

            convexH = new List<HullNode>();
            convexH.AddRange(GrahamScan.convexHull(nodes));
            for (int i = 0; i < convexH.Count - 1; i++)
            {
                exitLines.Add(new HullLine(convexH[i], convexH[i + 1]));
            }
            exitLines.Add(new HullLine(convexH[0], convexH[convexH.Count - 1]));
            return exitLines;
        }

        private static void setConvexHull(List<HullNode> nodes)
        {
            unused_nodes.AddRange(nodes);
            hull_edges.AddRange(getHull(nodes));
            foreach (HullLine line in hull_edges)
            {
                foreach (HullNode node in line.nodes)
                {
                    unused_nodes.RemoveAll(a => a.id == node.id);
                }
            }
        }

        private static List<HullLine> setConcaveHull(double concavity, int scaleFactor)
        {
            /* Run setConvHull before! 
             * Concavity is a value used to restrict the concave angles 
             * It can go from -1 (no concavity) to 1 (extreme concavity) 
             * Avoid concavity == 1 if you don't want 0º angles
             * */
            bool aLineWasDividedInTheIteration;
            hull_concave_edges.AddRange(hull_edges);
            do
            {
                aLineWasDividedInTheIteration = false;
                for (int linePositionInHull = 0; linePositionInHull < hull_concave_edges.Count && !aLineWasDividedInTheIteration; linePositionInHull++)
                {
                    HullLine line = hull_concave_edges[linePositionInHull];
                    List<HullNode> nearbyPoints = HullFunctions.getNearbyPoints(line, unused_nodes, scaleFactor);
                    List<HullLine> dividedLine = HullFunctions.getDividedLine(line, nearbyPoints, hull_concave_edges, concavity);
                    if (dividedLine.Count > 0)
                    { // Line divided!
                        aLineWasDividedInTheIteration = true;
                        unused_nodes.Remove(unused_nodes.Where(n => n.id == dividedLine[0].nodes[1].id).FirstOrDefault()); // Middlepoint no longer free
                        hull_concave_edges.AddRange(dividedLine);
                        hull_concave_edges.RemoveAt(linePositionInHull); // Divided line no longer exists
                    }
                }

                hull_concave_edges = hull_concave_edges.OrderByDescending(a => HullLine.getLength(a.nodes[0], a.nodes[1])).ToList();
            } while (aLineWasDividedInTheIteration);

            return hull_concave_edges;
        }
    }

    public class HullNode
    {
        public int id;
        public double x;
        public double y;
        public double cos; // Used for middlepoint calculations
        public HullNode(double x, double y, int id)
        {
            this.x = x;
            this.y = y;
            this.id = id;
        }

        public bool SamePositionNode(HullNode hullNode)
        {
            return x == hullNode.x && y == hullNode.y;
        }
    }

    public class HullLine
    {
        public HullNode[] nodes = new HullNode[2];
        public HullLine(HullNode n1, HullNode n2)
        {
            nodes[0] = n1;
            nodes[1] = n2;
        }
        public static double getLength(HullNode node1, HullNode node2)
        {
            /* It actually calculates relative length */
            double length;
            length = Math.Pow(node1.y - node2.y, 2) + Math.Pow(node1.x - node2.x, 2);
            //length = Math.sqrt(Math.Pow(node1.y - node2.y, 2) + Math.Pow(node1.x - node2.x, 2));
            return length;
        }

        public HullLine FindNext(List<HullLine> hullLines)
        {
            return hullLines.Find(p=>p.nodes[0].SamePositionNode(nodes[1]));
        }
    }

    public static class HullFunctions
    {
        public static List<HullLine> getDividedLine(HullLine line, List<HullNode> nearbyPoints, List<HullLine> concave_hull, double concavity)
        {
            // returns two lines if a valid middlePoint is found
            // returns empty list if the line can't be divided
            List<HullLine> dividedLine = new List<HullLine>();
            List<HullNode> okMiddlePoints = new List<HullNode>();
            foreach (HullNode middlePoint in nearbyPoints)
            {
                double _cos = getCos(line.nodes[0], line.nodes[1], middlePoint);
                if (_cos < concavity)
                {
                    HullLine newLineA = new HullLine(line.nodes[0], middlePoint);
                    HullLine newLineB = new HullLine(middlePoint, line.nodes[1]);
                    if (!lineCollidesWithHull(newLineA, concave_hull) && !lineCollidesWithHull(newLineB, concave_hull))
                    {
                        middlePoint.cos = _cos;
                        okMiddlePoints.Add(middlePoint);
                    }
                }
            }
            if (okMiddlePoints.Count > 0)
            {
                // We want the middlepoint to be the one with the widest angle (smallest cosine)
                okMiddlePoints = okMiddlePoints.OrderBy(p => p.cos).ToList();
                dividedLine.Add(new HullLine(line.nodes[0], okMiddlePoints[0]));
                dividedLine.Add(new HullLine(okMiddlePoints[0], line.nodes[1]));
            }
            return dividedLine;
        }

        public static bool lineCollidesWithHull(HullLine line, List<HullLine> concave_hull)
        {
            foreach (HullLine hullLine in concave_hull)
            {
                // We don't want to check a collision with this point that forms the hull AND the line
                if (line.nodes[0].id != hullLine.nodes[0].id && line.nodes[0].id != hullLine.nodes[1].id
                    && line.nodes[1].id != hullLine.nodes[0].id && line.nodes[1].id != hullLine.nodes[1].id)
                {
                    // Avoid line interesections with the rest of the hull
                    if (LineIntersectionFunctions.doIntersect(line.nodes[0], line.nodes[1], hullLine.nodes[0], hullLine.nodes[1]))
                        return true;
                }
            }
            return false;
        }

        private static double getCos(HullNode A, HullNode B, HullNode O)
        {
            /* Law of cosines */
            double aPow2 = Math.Pow(A.x - O.x, 2) + Math.Pow(A.y - O.y, 2);
            double bPow2 = Math.Pow(B.x - O.x, 2) + Math.Pow(B.y - O.y, 2);
            double cPow2 = Math.Pow(A.x - B.x, 2) + Math.Pow(A.y - B.y, 2);
            double cos = (aPow2 + bPow2 - cPow2) / (2 * Math.Sqrt(aPow2 * bPow2));
            return Math.Round(cos, 4);
        }

        public static List<HullNode> getNearbyPoints(HullLine line, List<HullNode> nodeList, int scaleFactor)
        {
            /* The bigger the scaleFactor the more points it will return
             * Inspired by this precious algorithm:
             * http://www.it.uu.se/edu/course/homepage/projektTDB/ht13/project10/Project-10-report.pdf
             * Be carefull: if it's too small it will return very little points (or non!), 
             * if it's too big it will add points that will not be used and will consume time
             * */
            List<HullNode> nearbyPoints = new List<HullNode>();
            double[] boundary;
            int tries = 0;
            double node_x_rel_pos;
            double node_y_rel_pos;

            while (tries < 2 && nearbyPoints.Count == 0)
            {
                boundary = getBoundary(line, scaleFactor);
                foreach (HullNode node in nodeList)
                {
                    //Not part of the line
                    if (!(node.x == line.nodes[0].x && node.y == line.nodes[0].y ||
                        node.x == line.nodes[1].x && node.y == line.nodes[1].y))
                    {
                        node_x_rel_pos = Math.Floor(node.x / scaleFactor);
                        node_y_rel_pos = Math.Floor(node.y / scaleFactor);
                        //Inside the boundary
                        if (node_x_rel_pos >= boundary[0] && node_x_rel_pos <= boundary[2] &&
                            node_y_rel_pos >= boundary[1] && node_y_rel_pos <= boundary[3])
                        {
                            nearbyPoints.Add(node);
                        }
                    }
                }
                //if no points are found we increase the area
                scaleFactor = scaleFactor * 4 / 3;
                tries++;
            }
            return nearbyPoints;
        }

        private static double[] getBoundary(HullLine line, int scaleFactor)
        {
            /* Giving a scaleFactor it returns an area around the line 
             * where we will search for nearby points 
             * */
            double[] boundary = new double[4];
            HullNode aNode = line.nodes[0];
            HullNode bNode = line.nodes[1];
            double min_x_position = Math.Floor(Math.Min(aNode.x, bNode.x) / scaleFactor);
            double min_y_position = Math.Floor(Math.Min(aNode.y, bNode.y) / scaleFactor);
            double max_x_position = Math.Floor(Math.Max(aNode.x, bNode.x) / scaleFactor);
            double max_y_position = Math.Floor(Math.Max(aNode.y, bNode.y) / scaleFactor);

            boundary[0] = min_x_position;
            boundary[1] = min_y_position;
            boundary[2] = max_x_position;
            boundary[3] = max_y_position;

            return boundary;
        }
    }

    public static class LineIntersectionFunctions
    {
        // The main function that returns true if line segment 'p1q1' 
        // and 'p2q2' intersect. 
        public static Boolean doIntersect(HullNode p1, HullNode q1, HullNode p2, HullNode q2)
        {
            // Find the four orientations needed for general and 
            // special cases 
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case 
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases 
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
            if (o2 == 0 && onSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
            if (o3 == 0 && onSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
            if (o4 == 0 && onSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases 
        }

        // Given three colinear points p, q, r, the function checks if 
        // point q lies on line segment 'pr' 
        private static bool onSegment(HullNode p, HullNode q, HullNode r)
        {
            if (q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
                q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y))
                return true;

            return false;
        }

        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are colinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        private static int orientation(HullNode p, HullNode q, HullNode r)
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
            // for details of below formula. 
            double val = (q.y - p.y) * (r.x - q.x) -
                    (q.x - p.x) * (r.y - q.y);

            if (val == 0) return 0; // colinear 

            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }
    }
}
