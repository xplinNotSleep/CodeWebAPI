using AgDataHandle.Maths;
using System.Collections.Generic;

namespace AgDataHandle.Maths.Geometry
{
    public class LineInnerPointInfo
    {
        public string Msg = null;
        public int LineIndex = -1;
        public float LineRadio = -1;
        public Vector3 Point { get; set; }

        public int TriangleIndex = -1;
    }

    public class LineInnerPointInfoCmperByLine : IComparer<LineInnerPointInfo>
    {
        public int Compare(LineInnerPointInfo x, LineInnerPointInfo y)
        {
            if (x.LineIndex != y.LineIndex)
            {
                return x.LineIndex.CompareTo(y.LineIndex);
            }
            return x.LineRadio.CompareTo(y.LineRadio);
        }
    }
}
