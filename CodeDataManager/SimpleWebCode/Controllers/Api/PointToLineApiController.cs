using AGSpatialDataCheck.GISUtils.CheckParams;
using AGSpatialDataCheck.GISUtils.CheckUtils;
using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    [Route("api/PointToLine")]
    public class PointToLineApiController : ApiControllerBase
    {
        PointToLineCheckUtils CheckUtils { get; set; }
        public PointToLineApiController()
        {
            CheckUtils= new PointToLineCheckUtils();
        }
        
        /// <summary>
        /// 线要素是否与点要素有关联(孤立管线检测)
        /// </summary>
        /// <param name="multiLayer"></param>
        /// <returns></returns>
        [HttpPost("IsSingleLine")]
        public async Task<BaseMessage> IsSingleLine([FromBody] MultiSdeLayer multiLayer)
        {
            return CheckUtils.IsSingleLine(multiLayer);
        }

        /// <summary>
        /// 点要素是否与线要素有关联(孤立管点检测)
        /// </summary>
        /// <param name="multiLayer"></param>
        /// <returns></returns>
        [HttpPost("IsSinglePoint")]
        public async Task<BaseMessage> IsSinglePoint([FromBody] MultiSdeLayer multiLayer)
        {
            return CheckUtils.IsSinglePoint(multiLayer);
        }

        /// <summary>
        /// 点要素是否被线要素覆盖
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        [HttpPost("IsCovered")]
        public async Task<BaseMessage> IsCovered([FromBody] MultiSdeLayer multiLayer)
        {
            return CheckUtils.IsCovered(multiLayer);
        }

        /// <summary>
        /// 点要素不能在线上检查
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        [HttpPost("IsOnLine")]
        public async Task<BaseMessage> IsOnLine([FromBody] MultiSdeLayer multiLayer)
        {
            return CheckUtils.IsOnLine(multiLayer);
        }
    }
}
