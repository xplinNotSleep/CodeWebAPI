using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AgDataHandle.Maths.SpatialStructure
{
    /// <summary>
    /// 塔式起重机轨迹预测
    /// </summary>
    public class TowerCraneTrajectoryPrediction
    {
        /// <summary>
        /// 历史点
        /// </summary>
        private List<Vector3> m_historicalPoints;
        /// <summary>
        /// 步长时间
        /// </summary>
        private float m_stepTime;
        private float m_r;
        /// <summary>
        /// 塔式起重机主体中心点
        /// </summary>
        public Vector2 Center { get; set; }
        private Vector2 m_xDir = new Vector2(1, 0);
        public TowerCraneTrajectoryPrediction(List<Vector3> points,float stepTime)
        {
            m_historicalPoints = points;
            m_stepTime = stepTime;
            Center = Vector2.Zero;
        }
      
        /// <summary>
        /// 预测未来几秒的轨迹
        /// </summary>
        /// <param name="time">预测的时间长度</param>
        /// <returns></returns>
        public List<Vector3> ForecastPath(float time)
        {
            var change = m_historicalPoints[m_historicalPoints.Count - 1] - m_historicalPoints[0];
            var verticalSpeed = change.Z / m_stepTime;
            var horizontalSpeed = 0f;
            
            if(change.X!=0)
            {
                var sp = new Vector2(m_historicalPoints[0].X, m_historicalPoints[0].Y);
                m_r = (sp - Center).Length();
                var p0 =new Vector2(m_historicalPoints[m_historicalPoints.Count - 2].X, m_historicalPoints[m_historicalPoints.Count - 2].Y)-Center;
                var p1 = new Vector2(m_historicalPoints[m_historicalPoints.Count - 1].X, m_historicalPoints[m_historicalPoints.Count - 1].Y)-Center;
                float angle0 = p0.AngleWithDegree(m_xDir);
                if (p0.Y < 0)
                    angle0 = 360-angle0;
                float angle1 = p1.AngleWithDegree(m_xDir);
                if (p1.Y < 0)
                    angle1 = 360-angle1;
                horizontalSpeed = (angle1 - angle0) / m_stepTime;

            }
            var ps = ForecastPath(time,verticalSpeed*m_stepTime,horizontalSpeed * m_stepTime);
            DebugPath(m_historicalPoints, @"D:\模型数据\OBJ\轨迹测试\source.obj");
            DebugPath(ps, @"D:\模型数据\OBJ\轨迹测试\re.obj");
            return ps;
        }
        private List<Vector3> ForecastPath(float time,float verticalSpeed,float horizontalSpeed)
        {
            var list = new List<Vector3>();
            var num = time / m_stepTime;
            var lastPoint = m_historicalPoints[m_historicalPoints.Count - 1];
            var p = new Vector2(lastPoint.X, lastPoint.Y) - Center;
            float angle = p.AngleWithDegree(m_xDir);
            if (p.Y < 0)
                angle = 360 - angle;
            for (int i = 1; i <= num; i++)
            {
                var z = lastPoint.Z+i * verticalSpeed;
                var reAngle =angle+ i * horizontalSpeed;
                var x = m_r*Math.Cos(reAngle / 180*Math.PI)+Center.X;
                var y = m_r * Math.Sin(reAngle / 180 * Math.PI) + Center.Y;
                list.Add(new Vector3(x, y, z));
            }
            return list;
        }


        public static List<Vector3> CreateTestPoints(float r)
        {
            var list = new List<Vector3>();
            var p = new Vector3(r,0,20);
            list.Add(p);
            list.Add(MathAlgorithm.RoateZ(p,3f));
            list.Add(MathAlgorithm.RoateZ(list[list.Count-1], 3f));
            list.Add(MathAlgorithm.RoateZ(list[list.Count-1], 3f));
            list.Add(MathAlgorithm.RoateZ(list[list.Count-1], 3f));
            list.Add(MathAlgorithm.RoateZ(list[list.Count-1], 3f));
            list.Add(MathAlgorithm.RoateZ(list[list.Count-1], 3f));
            list.Add(MathAlgorithm.RoateZ(list[list.Count-1], 3f));
            list.Add(MathAlgorithm.RoateZ(list[list.Count-1], 3f));
            list.Add(MathAlgorithm.RoateZ(list[list.Count-1], 3f));
            return list;
        }
        private void DebugPath(List<Vector3> points,string savePath)
        {
            var sb = new StringBuilder();
            foreach (var item in points)
            {
                sb.AppendLine($"v {item.X} {item.Y} {item.Z}");
            }
            sb.Append("l");
            for (int i = 0; i < points.Count; i++)
            {
                sb.Append($" {i+1}");
            }
            File.WriteAllText(savePath, sb.ToString());
        }
    }
}
