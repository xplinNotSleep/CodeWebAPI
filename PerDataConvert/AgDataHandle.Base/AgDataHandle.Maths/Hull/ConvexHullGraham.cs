using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgDataHandle.Maths
{
    public class ConvexHullGraham 
    {
        private List<Vector2> m_sourcePoints;
        public ConvexHullGraham(List<Vector2> points) 
        {
            m_sourcePoints = points.Distinct().ToList();
        }
        
        public List<Vector2> Calculate() 
        {
            if (m_sourcePoints.Count < 3)
                return m_sourcePoints;

            List<Vector2> resultPoints = new List<Vector2>();

            m_sourcePoints.Sort((a, b) => 
            {
                if (a.Y == b.Y)
                    return a.X.CompareTo(b.X);
                else
                    return a.Y.CompareTo(b.Y);
            });

            resultPoints.Add(m_sourcePoints[0]);

            List<Vector2> sourcePoints2 = m_sourcePoints.GetRange(1, m_sourcePoints.Count - 1);
            sourcePoints2.Sort((a, b) => 
            {
                double a_tan = Math.Atan2(a.Y - m_sourcePoints[0].Y, a.X - m_sourcePoints[0].X);
                double b_tan = Math.Atan2(b.Y - m_sourcePoints[0].Y, b.X - m_sourcePoints[0].X);

                if (a_tan != b_tan)
                    return a_tan.CompareTo(b_tan);
                else
                    return a.X.CompareTo(b.X);
            });

            //将sourcePoints数组替换为两次排序后的新数组
            m_sourcePoints = new List<Vector2>();
            m_sourcePoints.Add(resultPoints[0]);
            m_sourcePoints.AddRange(sourcePoints2);

            //List<double> tans = new List<double>();
            //foreach (var a in sourcePoints2)
            //{
            //    double a_tan = Math.Atan2(a.Y - m_sourcePoints[0].Y, a.X - m_sourcePoints[0].X);
            //    tans.Add(a_tan);
            //}

            resultPoints.Add(m_sourcePoints[1]);//这里加入的应该是第二次排序后的结果

            for (int i = 2; i < m_sourcePoints.Count;) 
            {
                //这里要做几个事情？
                //1.扫描下一个点的时候，过滤筛选下一个点的范围
                //2.

                // 重复校验当前点在结果集中是否凹点，对凹点往后序列点依次回退出栈（测试逻辑，待修正）
                int index = resultPoints.Count - 2;
                while (resultPoints.Count > 2)
                {
                    if (index <= 2) break;
                    if (MathAlgorithm.Cross(resultPoints[index], resultPoints[index + 1], m_sourcePoints[i]) < 0)
                    {
                        int count = resultPoints.Count - index - 1;
                        for (int j = 0; j < count; j++)
                        {
                            resultPoints.RemoveAt(resultPoints.Count - 1);
                        }
                    }
                    index--;
                }

                //判断在最终数组里，若小于等于两个点时，则无论如何都要加入新点，否则去除末尾点会导致后续超出索引
                if (resultPoints.Count <= 2 || MathAlgorithm.Cross(resultPoints[resultPoints.Count - 2], resultPoints[resultPoints.Count - 1], m_sourcePoints[i]) > 0) 
                {

                    resultPoints.Add(m_sourcePoints[i]);
                    i++;
                }
                else 
                {
                    resultPoints.RemoveAt(resultPoints.Count - 1);
                }
            }

            return resultPoints;
        }
       
        private bool IsInDireaction(string direaction, double angle)
        {
            switch (direaction) 
            {
                case "RightUp":
                    if (angle >= 0 && angle < Math.PI / 2)
                        return true;
                    return false;
                case "LeftUp":
                    if (angle >= Math.PI / 2 && angle <= Math.PI)
                        return true;
                    return false;
                case "LeftDown":
                    if (angle >= -Math.PI && angle < -Math.PI / 2)
                        return true;
                    return false;
                case "RightDown":
                    if (angle >= -Math.PI / 2 && angle < 0)
                        return true;
                    return false;
                default:
                    return false;
            }
        }
    }
}
