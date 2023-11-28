using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    public class SZ_2000 : CGCS_2000
    {
        public SZ_2000()
        {
            AliasName = "SZ_2000";
            Name = "CGCS2000 / 3 - degree Gauss - Kruger CM 114E";
            Type = CoordinateType.ProjectedCoordinateSystem;
            EPSG = "";
            CentralLongitude = 114.00f;

            
            //下面两个值不使用
            LatitudeOrigin = double.NaN;
            LongitudeOrigin = double.NaN;


        }
    }
}
