using AgDataHandle.Maths;
using System.Collections.Generic;

namespace AgDataHandle.Web.Base
{
    public class RequestParam
    {
        /// <summary>
        /// 输入文件全路径，暂时支持sdb
        /// </summary>
        public string inputfile
        {
            get; set;
        }
        /// <summary>
        /// 输出文件夹路径
        /// </summary>
        public string outputpath
        {
            get; set;
        }
        /// <summary>
        /// 附加参数，是个json字符串
        /// </summary>
        public object exportSetting
        {
            get; set;
        }
    }

    public class RequestParam3
    {
        /// <summary>
        /// 输入文件全路径，暂时支持sdb
        /// </summary>
        public string inputfile
        {
            get; set;
        }
        /// <summary>
        /// 附加参数，是个json字符串
        /// </summary>
        public object exportSetting
        {
            get; set;
        }
    }

    public class RequestParam2
    {
        /// <summary>
        /// 输入文件全路径，暂时支持sdb
        /// </summary>
        public string inputfile
        {
            get; set;
        }
        /// <summary>
        /// 输出文件
        /// </summary>
        public string outputfile
        {
            get; set;
        }
        /// <summary>
        /// 附加参数，是个json字符串
        /// </summary>
        public object exportSetting
        {
            get; set;
        }
    }

    public class ProjectParam:Vector2d
    {
        /// <summary>
        /// 中央经线
        /// </summary>
        public double CentralLongitude { get; set; } = 120;
    }

    public class ProjectParam2 : Vector2d
    {
        /// <summary>
        /// 投影字符串,支持Esri、Proj4、ESPGCode、其他Code写法
        /// </summary>
        public string Proj{ get; set; }
        /// <summary>
        /// 投影字符串,支持Esri、Proj4、ESPGCode、其他Code写法,默认ESPG:4490
        /// </summary>
        public string TargetProj { get; set; }
    }

    public class ProjectParam3 
    {
        /// <summary>
        /// 投影字符串,支持Esri、Proj4、ESPGCode、其他Code写法
        /// </summary>
        public string Proj { get; set; }
        /// <summary>
        /// 投影字符串,支持Esri、Proj4、ESPGCode、其他Code写法,默认ESPG:4490
        /// </summary>
        public string TargetProj { get; set; }
        /// <summary>
        /// 所有点
        /// </summary>
        public List<Vector2d> Points { get; set; }
    }
}
