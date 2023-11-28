using AgDataHandle.Maths;
using System;
using System.Collections.Generic;

namespace AgDataHandle.Maths.Geometry
{
    public class Mesh
    {
        public List<Vertex> Vertexs { get; set; }
        public List<Vector2> UVs { get; set; }

        public List<Vector3> Normals { get; set; }
        public List<Face> Faces { get; set; }
    }
}
