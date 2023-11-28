using AgDataHandle.Maths;
using System.Collections.Generic;

namespace AgDataHandle.Maths.Geometry
{
    public class TriangleItor
    {
        public List<Triangle> triangles;

        public TriangleItor(List<Triangle> triangles1)
        {
            this.triangles = triangles1;
        }

        public Vector3 this[int index]
        {
            get
            {
                return triangles[index / 3].GetPoint(index % 3);
            }
        }

        public int Count
        {
            get { return triangles.Count; }
        }
    }
}
