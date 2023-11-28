using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths
{
    /// <summary>
    /// 光栅化信息
    /// </summary>
    public class RasterizationInfo
    {
        public int TotalPixelNum { get; set; }
        public int VisiblePixelNum { get; set; }
        public float VisiblePixelRatio { get; set; }
    }
}
