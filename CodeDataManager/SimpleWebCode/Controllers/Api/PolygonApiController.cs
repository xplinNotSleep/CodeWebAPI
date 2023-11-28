using AGSpatialDataCheck.GISUtils.CheckParams;
using AGSpatialDataCheck.GISUtils.CheckUtils;
using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    [Route("api/Polygon")]
    public class PolygonApiController : ApiControllerBase
    {
        PolygonCheckUtils CheckUtils { get; set; }
        public PolygonApiController()
        {
            CheckUtils = new PolygonCheckUtils();
        }

        /// <summary>
        /// 同一要素类中面要素相交检查
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        [HttpPost("IsIntersected")]
        public async Task<BaseMessage> IsIntersected([FromBody] CheckLayer layer)
        {
            return CheckUtils.IsIntersected(layer);
        }
    }
}
