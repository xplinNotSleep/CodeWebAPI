using AGSpatialDataCheck.GISUtils.CheckParams;
using AGSpatialDataCheck.GISUtils.CheckUtils;
using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    [Route("api/line")]
    public class LineApiController : ApiControllerBase
    {
        LineCheckUtils CheckUtils { get; set; }
        public LineApiController()
        {
            CheckUtils = new LineCheckUtils();
        }

        /// <summary>
        /// 线自相交检查,包括封闭图形
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        [HttpPost("IsSelfIntersected")]
        public async Task<BaseMessage> IsSelfIntersected([FromBody] CheckLayer layer)
        {
            return CheckUtils.IsSelfIntersected(layer);
        }

        /// <summary>
        /// 同一要素类中线要素相交检查
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        [HttpPost("IsIntersected")]
        public async Task<BaseMessage> IsIntersected([FromBody] CheckLayer layer)
        {
            return CheckUtils.IsIntersected(layer);
        }

        /// <summary>
        /// 同一要素类中线要素重叠检查，包括重叠、包含与被包含等
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        [HttpPost("IsOverlap")]
        public async Task<BaseMessage> IsOverlap([FromBody] CheckLayer layer)
        {
            return CheckUtils.IsOverlap(layer);
        }

        /// <summary>
        /// 同一要素类中线要素是否为孤立线
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns>有悬挂端点的要素</returns>
        [HttpPost("IsHang")]
        public async Task<BaseMessage> IsHang([FromBody] CheckLayer layer)
        {
            return CheckUtils.IsHang(layer);
        }

        /// <summary>
        /// 检查线要素是否具有多部件
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns>有悬挂端点的要素</returns>
        [HttpPost("IsMultipart")]
        public async Task<BaseMessage> IsMultipart([FromBody] CheckLayer layer)
        {
            return CheckUtils.IsMultipart(layer);
        }
    }
}

