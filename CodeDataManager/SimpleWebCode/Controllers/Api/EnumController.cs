using AGSpatialDataCheck.GISUtils;
using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumController : ApiControllerBase
    {
        [HttpGet("ENUM_RULE_TYPE")]
        public async Task<BaseMessage<dynamic>> GetRuleType()
        {
            try
            {
                var dict = typeof(ENUM_RULE_TYPE).ToEnumItemInfoList();
                return Success<dynamic>(dict);
            }
            catch (Exception ex)
            {
                return Fail<dynamic>(null, ex.Message, ex);
            }
        }
    }
}
