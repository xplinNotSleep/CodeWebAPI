using AgDataHandle.Maths;

namespace AgDataHandle.Maths.Geometry
{
    public class PlanarAxis
    {
        public Vector3 Center { get; set; }

        public Vector3 Axis1 { get; set; }

        public Vector3 Axis2 { get; set; }

        /// <summary>
        /// 一个点投影到这来
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Vector2 Project2D(Vector3 p)
        {
            var v = p-this.Center;
            var x = Axis1.Dot(v);
            var y = Axis2.Dot(v);

            return new Vector2(x, y);
        }
    }
}
