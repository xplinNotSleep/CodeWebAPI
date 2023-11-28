using AGSpatialDataCheck.GISUtils.CheckParams;
using AGSpatialDataCheck.GISUtils.CheckUtils;
using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    [Route("api/PolygonToPolygon")]
    public class PolygonToPolygonApiController : ApiControllerBase
    {
        PolygonToPolygonCheckUtils CheckUtils { get; set; }
        public PolygonToPolygonApiController()
        {
            CheckUtils = new PolygonToPolygonCheckUtils();
        }

        /// <summary>
        /// 判断两个面要素类中是否有相互重叠的要素（包括包含与被包含，不包括接边）
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        [HttpPost("IsOverlap")]
        public async Task<BaseMessage> IsOverlap([FromBody] MultiSdeLayer multiLayer)
        {
            return CheckUtils.IsOverlap(multiLayer);
        }

        /// <summary>
        /// 判断两个面要素类是否具有空隙
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        [HttpPost("HasGap")]
        public async Task<BaseMessage> HasGap([FromBody] MultiSdeLayer multiLayer)
        {
            return CheckUtils.HasGap(multiLayer);
        }

        /// <summary>
        /// 判断两个面要素类是否是相互覆盖的要素
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        [HttpPost("IsCovered")]
        public async Task<BaseMessage> IsCovered([FromBody] MultiSdeLayer multiLayer)
        {
            return CheckUtils.IsCovered(multiLayer);
        }

        /// <summary>
        /// 多个面合并后是否与某个面重合
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        [HttpPost("IsCoincided")]
        public async Task<BaseMessage> IsCoincided([FromBody] MultiSdeLayer multiLayer)
        {
            return CheckUtils.IsCoincided(multiLayer);
        }
    }
}
