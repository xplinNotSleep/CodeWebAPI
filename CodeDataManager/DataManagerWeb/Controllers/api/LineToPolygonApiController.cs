using AGSpatialDataCheck.GISUtils.CheckParams;
using AGSpatialDataCheck.GISUtils.CheckUtils;
using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    [Route("api/LineToPolygon")]
    public class LineToPolygonApiController : ApiControllerBase
    {
        LineToPolygonCheckUtils CheckUtils { get; set; }
        public LineToPolygonApiController()
        {
            CheckUtils = new LineToPolygonCheckUtils();
        }

        /// <summary>
        /// 线要素类与面要素类中的要素相交检查
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        [HttpPost("IsIntersected")]
        public async Task<BaseMessage> IsIntersected([FromBody] MultiSdeLayer multiLayer)
        {
            return CheckUtils.IsIntersected(multiLayer);
        }
    }
}
