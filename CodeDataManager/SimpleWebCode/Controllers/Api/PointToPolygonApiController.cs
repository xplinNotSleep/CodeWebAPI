using AGSpatialDataCheck.GISUtils.CheckParams;
using AGSpatialDataCheck.GISUtils.CheckUtils;
using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    [Route("api/PointToPolygon")]
    public class PointToPolygonApiController : ApiControllerBase
    {
        PointToPolygonCheckUtils CheckUtils { get; set; }
        public PointToPolygonApiController()
        {
            CheckUtils = new PointToPolygonCheckUtils();
        }

        /// <summary>
        /// 点要素类中要素是否与其他要素类中要素重叠或相交
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        [HttpPost("IsOverlap")]
        public async Task<BaseMessage> IsOverlap([FromBody] MultiSdeLayer multiLayer)
        {
            return CheckUtils.IsOverlap(multiLayer);
        }
    }
}
