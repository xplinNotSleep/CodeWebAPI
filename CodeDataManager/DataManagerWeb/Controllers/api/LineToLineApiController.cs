using AGSpatialDataCheck.GISUtils.CheckParams;
using AGSpatialDataCheck.GISUtils.CheckUtils;
using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    [Route("api/LineToLine")]
    public class LineToLineApiController : ApiControllerBase
    {
        LineToLineCheckUtils CheckUtils;
        public LineToLineApiController()
        {
            CheckUtils= new LineToLineCheckUtils();
        }

        /// <summary>
        /// 线要素类与其他线要素类中的要素相交检查
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        [HttpPost("IsIntersected")]
        public async Task<BaseMessage> IsIntersected([FromBody] MultiSdeLayer multiLayer)
        {
            return CheckUtils.IsIntersected(multiLayer);
        }

        /// <summary>
        /// 线要素是否被线要素覆盖
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        [HttpPost("IsCovered")]
        public async Task<BaseMessage> IsCovered([FromBody] MultiSdeLayer multiLayer)
        {
            return CheckUtils.IsCovered(multiLayer);
        }
    }
}
