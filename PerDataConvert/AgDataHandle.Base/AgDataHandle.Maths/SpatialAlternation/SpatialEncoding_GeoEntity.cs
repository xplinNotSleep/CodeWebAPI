using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    public partial class SpatialEncoding
    {
        public string GetGeoCode(double[] center, string srs)
        {
            var cs = AgDataHandle.Maths.SpatialAlternation.Coordinate.FindCoordinateSystem(CoordinateSystemType.CGCS_2000);
            var actualCoordinates = new double[2] { center[0], center[1] };
            if (srs.Contains("cgcs2000")||srs.Contains("CGCS2000"))
            {
                cs.CentralLongitude = float.Parse(srs.Split(new string[] { "::" }, StringSplitOptions.None)[1]);
                var deg = cs.UnProject(new Vector2d(center[1], center[0]));
                actualCoordinates = new double[2] { deg[1], deg[0] };
            }          
            //var deg = XYConvertDegreeByCGCS2000.xyTolatlon(center[1], center[0], 113.18);
            StringBuilder sb = new StringBuilder("NE");
            judgeFirstLod(ref actualCoordinates, new double[2] { 90, 44 }, sb);
            judgeSecondeLod(ref actualCoordinates, new double[2] { 6, 4 }, sb);
            judgeFirstLod(ref actualCoordinates, new double[2] { 3, 2 }, sb);
            JudgeFourthLod(ref actualCoordinates, new double[2] { 1, 1 }, sb);
            actualCoordinates = new double[2] { (actualCoordinates[0] - (int)actualCoordinates[0]) * 60, (actualCoordinates[1] - (int)actualCoordinates[1]) * 60 };
            judgeFirstLod(ref actualCoordinates, new double[2] { 30, 30 }, sb);//第五级         
            JudgeSixLod(ref actualCoordinates, new double[2] { 15, 10 }, sb);
            JudgeFourthLod(ref actualCoordinates, new double[2] { 5, 5 }, sb);//第七级
            JudgeEightLod(ref actualCoordinates, new double[2] { 1, 1 }, sb);//第八级
            actualCoordinates = new double[2] { (actualCoordinates[0] - (int)actualCoordinates[0]) * 60, (actualCoordinates[1] - (int)actualCoordinates[1]) * 60 };
            JudgeEightLod(ref actualCoordinates, new double[2] { 12, 12 }, sb);
            JudgeTenLod(ref actualCoordinates, new double[2] { 4, 4 }, sb);
            judgeFirstLod(ref actualCoordinates, new double[2] { 2, 2 }, sb);//第十一级同第一
            judgeFirstLod(ref actualCoordinates, new double[2] { 1, 1 }, sb);
            judgeThirteenLod(ref actualCoordinates, new double[2] { 0.25, 0.25 }, sb);
            judgeFourteenLod(ref actualCoordinates, new double[2] { 0.03125, 0.03125 }, sb);
            judgeFourteenLod(ref actualCoordinates, new double[2] { 0.0039625, 0.0039625 }, sb);
            judgeFourteenLod(ref actualCoordinates, new double[2] { 0.00041528, 0.00041528 }, sb);
            return sb.ToString();
        }

        private void judgeFourteenLod(ref double[] deg, double[] vs, StringBuilder sb)
        {
            sb.Append((int)(deg[0] / vs[0]));
            sb.Append((int)(deg[1] / vs[1]));
            deg = new double[2] { deg[0] - vs[0] * (int)(deg[0] / vs[0]), deg[1] - vs[1] * (int)(deg[1] / vs[1]) };
        }

        private void judgeThirteenLod(ref double[] deg, double[] vs, StringBuilder sb)
        {
            sb.Append((int)(deg[0] / vs[0]));
            sb.Append((int)(deg[1] / vs[1]));
            deg = new double[2] { deg[0] - vs[0] * (int)(deg[0] / vs[0]), deg[1] - vs[1] * (int)(deg[1] / vs[1]) };
        }

        private void JudgeTenLod(ref double[] deg, double[] vs, StringBuilder sb)
        {
            switch ((int)(deg[0] / vs[0]))
            {
                case 0:
                    if ((int)(deg[1] / vs[1]) == 0)
                    {
                        sb.Append("0");
                        deg = new double[2] { deg[0], deg[1] };
                    }
                    else if ((int)(deg[1] / vs[1]) == 1)
                    {
                        sb.Append("3");
                        deg = new double[2] { deg[0], deg[1] - vs[1] * 1 };
                    }
                    else if ((int)(deg[1] / vs[1]) == 2)
                    {
                        sb.Append("6");
                        deg = new double[2] { deg[0], deg[1] - vs[1] * 2 };
                    }
                    break;
                case 1:
                    if ((int)(deg[1] / vs[1]) == 0)
                    {
                        sb.Append("1");
                        deg = new double[2] { deg[0] - vs[0], deg[1] };
                    }
                    else if ((int)(deg[1] / vs[1]) == 1)
                    {
                        sb.Append("4");
                        deg = new double[2] { deg[0] - vs[0], deg[1] - vs[1] * 1 };
                    }
                    else if ((int)(deg[1] / vs[1]) == 2)
                    {
                        sb.Append("7");
                        deg = new double[2] { deg[0] - vs[0], deg[1] - vs[1] * 2 };
                    }
                    break;
                case 2:
                    if ((int)(deg[1] / vs[1]) == 0)
                    {
                        sb.Append("2");
                        deg = new double[2] { deg[0] - vs[0] * 2, deg[1] };
                    }
                    else if ((int)(deg[1] / vs[1]) == 1)
                    {
                        sb.Append("5");
                        deg = new double[2] { deg[0] - vs[0] * 2, deg[1] - vs[1] * 1 };
                    }
                    else if ((int)(deg[1] / vs[1]) == 2)
                    {
                        sb.Append("8");
                        deg = new double[2] { deg[0] - vs[0] * 2, deg[1] - vs[1] * 2 };
                    }
                    break;

            }
        }

        private void judgeFirstLod(ref double[] actualCoordinates, double[] vs, StringBuilder sb)
        {
            switch ((int)(actualCoordinates[0] / vs[0]))
            {
                case 0:
                    if ((int)(actualCoordinates[1] / vs[1]) == 0) sb.Append("0"); else sb.Append("2");
                    break;
                case 1:
                    if ((int)(actualCoordinates[1] / vs[1]) == 0) sb.Append("1"); else sb.Append("3");
                    break;
            }
            actualCoordinates = new double[] { actualCoordinates[0] - vs[0] <= 0 ? actualCoordinates[0] : actualCoordinates[0] - vs[0], actualCoordinates[1] - vs[1] <= 0 ? actualCoordinates[1] : actualCoordinates[1] - vs[1] };
        }

        private void judgeSecondeLod(ref double[] deg, double[] vs, StringBuilder sb)
        {
            string[] zimu = new string[11] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };
            sb.Append((int)(deg[0] / vs[0]));
            sb.Append(zimu[(int)(deg[1] / vs[1])]);
            var x = deg[0] - (int)(deg[0] / vs[0]) * vs[0];
            var y = deg[1] - (int)(deg[1] / vs[1]) * vs[1];
            deg = new double[2] { x, y };
        }

        private void JudgeFourthLod(ref double[] deg, double[] vs, StringBuilder sb)
        {
            switch ((int)(deg[0] / vs[0]))
            {
                case 0:
                    if ((int)(deg[1] / vs[1]) == 0) sb.Append("0"); else sb.Append("3"); deg = new double[2] { deg[0] - vs[0] * 0, deg[1] - vs[1] <= 0 ? deg[1] : deg[1] - vs[1] };
                    break;
                case 1:
                    if ((int)(deg[1] / vs[1]) == 0) sb.Append("1"); else sb.Append("4"); deg = new double[2] { deg[0] - vs[0] * 1, deg[1] - vs[1] <= 0 ? deg[1] : deg[1] - vs[1] };
                    break;
                case 2:
                    if ((int)(deg[1] / vs[1]) == 0) sb.Append("2"); else sb.Append("5"); deg = new double[2] { deg[0] - vs[0] * 2, deg[1] - vs[1] <= 0 ? deg[1] : deg[1] - vs[1] };
                    break;
            }
        }

        private void JudgeSixLod(ref double[] deg, double[] vs, StringBuilder sb)
        {

            switch ((int)(deg[1] / vs[1]))
            {
                case 0:
                    if ((int)(deg[0] / vs[0]) == 0) sb.Append("0"); else sb.Append("1"); deg = new double[2] { deg[0] - vs[0] <= 0 ? deg[0] : deg[0] - vs[0], deg[1] - vs[1] * 0 };
                    break;
                case 1:
                    if ((int)(deg[0] / vs[0]) == 0) sb.Append("2"); else sb.Append("3"); deg = new double[2] { deg[0] - vs[0] <= 0 ? deg[0] : deg[0] - vs[0], deg[1] - vs[1] * 1 };
                    break;
                case 2:
                    if ((int)(deg[0] / vs[0]) == 0) sb.Append("4"); else sb.Append("5"); deg = new double[2] { deg[0] - vs[0] <= 0 ? deg[0] : deg[0] - vs[0], deg[1] - vs[1] * 2 };
                    break;
            }
        }

        private void JudgeEightLod(ref double[] deg, double[] vs, StringBuilder sb)
        {
            sb.Append((int)(deg[0] / vs[0]));
            sb.Append((int)(deg[1] / vs[1]));
            deg = new double[] { deg[0] - (int)(deg[0] / vs[0]) * vs[0], deg[1] - vs[1] * (int)(deg[1] / vs[1]) };
        }
    }
}
