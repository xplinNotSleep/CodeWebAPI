using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgDataHandle.Maths.Geometry
{
    public class TriangleList
    {
        public List<Triangle> Items = new List<Triangle>();

        public int VerticalCount
        {
            get
            {
                return Items.Count * 3;
            }
        }
    }
}
