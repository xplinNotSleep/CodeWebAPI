using AGSpatialDataCheck.GISUtils.CheckParams;
using AGSpatialDataCheck.GISUtils.CheckUtils;
using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    [Route("api/Point")]
    public class PointApiController : ApiControllerBase
    {
        PointCheckUtils CheckUtils { get; set; }
        public PointApiController()
        {
            CheckUtils = new PointCheckUtils();
        }

        /// <summary>
        /// 点重叠检查
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        [HttpPost("IsOverlap")]
        public async Task<BaseMessage> IsOverlap([FromBody] CheckLayer layer)
        {
            return CheckUtils.IsOverlap(layer);
        }
    }
}
