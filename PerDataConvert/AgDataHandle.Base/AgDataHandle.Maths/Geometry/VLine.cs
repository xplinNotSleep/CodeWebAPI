using AgDataHandle.Maths;

namespace AgDataHandle.Maths.Geometry
{
    public class LineV
    {
        public Vector3 P1;
        public Vector3 P2;
        //p1到p2的向量
        public Vector3 P1_P2;

        public LineV(Vector3 fromPos, Vector3 endPos)
        {
            this.P1 = fromPos;
            this.P2 = endPos;

            ComputeDirection();
        }

        private void ComputeDirection()
        {
            P1_P2 = P1-P2;
        }

        public Vector3 GetLinePoint(double p)
        {
            return new Vector3((float)(P1_P2.X * p + P1.X), (float)(P1_P2.Y * p + P1.Y), (float)(P1_P2.Z * p + P1.Z));
        }
    }
}
