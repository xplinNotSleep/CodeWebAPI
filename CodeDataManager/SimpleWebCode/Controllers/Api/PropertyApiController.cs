using AGSpatialDataCheck.GISUtils.CheckParams;
using AGSpatialDataCheck.GISUtils.CheckUtils;
using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AGSpatialDataCheck.Web.Controllers.Api
{
    [Route("api/Property")]
    public class PropertyApiController : ApiControllerBase
    {
        PropertyCheckUtils CheckUtils { get; set; }
        public PropertyApiController()
        {
            CheckUtils = new PropertyCheckUtils();
        }

        /// <summary>
        /// 检查标识字段值是否存在空值
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsUniqueValueExist")]
        public async Task<BaseMessage> IsUniqueValueExist([FromBody] CheckField field)
        {
            return CheckUtils.IsUniqueValueExist(field);
        }

        /// <summary>
        /// 检查标识字段值是否唯一
        /// </summary>
        /// <param name="field">核查字段</param>
        /// <returns></returns>
        [HttpPost("IsGuidUnique")]
        public async Task<BaseMessage> IsGuidUnique([FromBody] CheckField field)
        {
            return CheckUtils.IsGuidUnique(field);
        }

        /// <summary>
        /// 检查字段值是否唯一
        /// </summary>
        /// <param name="field">核查字段</param>
        /// <returns></returns>
        [HttpPost("IsValueUnique")]
        public async Task<BaseMessage> IsValueUnique([FromBody] CheckField field)
        {
            return CheckUtils.IsValueUnique(field);
        }

        /// <summary>
        /// 检查字段值是否为空
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsValueExist")]
        public async Task<BaseMessage> IsValueExist([FromBody] CheckField field)
        {
            return CheckUtils.IsValueExist(field);
        }

        /// <summary>
        /// 检查字段值是否不为空（字段值必须为空，不为空则为错误）
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsValueNotExist")]
        public async Task<BaseMessage> IsValueNotExist([FromBody] CheckField field)
        {
            return CheckUtils.IsValueNotExist(field);
        }

        /// <summary>
        /// 检查属性值是否符合规则
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsEffectiveCoding")]
        public async Task<BaseMessage> IsEffectiveCoding([FromBody] CheckField field)
        {
            return CheckUtils.IsEffectiveCoding(field);
        }

        /// <summary>
        /// 检查数值型属性值是否在区间范围内
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsNumValueInRange")]
        public async Task<BaseMessage> IsNumValueInRange([FromBody] CheckField field)
        {
            return CheckUtils.IsNumValueInRange(field);
        }

        /// <summary>
        /// 检查字符型属性值是否在区间范围内
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsStrValueInRange")]
        public async Task<BaseMessage> IsStrValueInRange([FromBody] CheckField field)
        {
            return CheckUtils.IsStrValueInRange(field);
        }

        /// <summary>
        /// 数据库表中字符型日期字段(年月日)是否为合法的日期（如2019-01-01)
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsValidDate")]
        public async Task<BaseMessage> IsValidDate([FromBody] CheckField field)
        {
            return CheckUtils.IsValidDate(field);
        }

        /// <summary>
        /// 是否为合法的年份（如2019),暂无年份范围
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsValidYear")]
        public async Task<BaseMessage> IsValidYear([FromBody] CheckField field)
        {
            return CheckUtils.IsValidYear(field);
        }

        /// <summary>
        /// 与其他日期比较后，判断逻辑上是否为合法的年份（如2019),可为逗号分隔的多个年份
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsValidYearCompared")]
        public async Task<BaseMessage> IsValidYearCompared([FromBody] CheckField field)
        {
            return CheckUtils.IsValidYearCompared(field);
        }

        /// <summary>
        /// 字符型年月是否为合法的年月
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsValidYearMonth")]
        public async Task<BaseMessage> IsValidYearMonth([FromBody] CheckField field)
        {
            return CheckUtils.IsValidYearMonth(field);
        }

        /// <summary>
        /// 日期(年月日)字段值与其他日期比较后，逻辑上是否为合法的日期（如桥梁建成日期不能超过调查时间),还未完全实现
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsValidDateCompared")]
        public async Task<BaseMessage> IsValidDateCompared([FromBody] CheckField field)
        {
            return CheckUtils.IsValidDateCompared(field);
        }

        /// <summary>
        /// 检查身份证是否有效，返回不合法的身份证记录信息
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsEffectiveIDCard")]
        public async Task<BaseMessage> IsEffectiveIDCard([FromBody] CheckField field)
        {
            return CheckUtils.IsEffectiveIDCard(field);
        }

        /// <summary>
        /// 检查是否为有效的字段值,返回的是与所给值（这些值实际是不合符要求的）相同的记录信息
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        [HttpPost("IsEffectiveValue")]
        public async Task<BaseMessage> IsEffectiveValue([FromBody] CheckField field)
        {
            return CheckUtils.IsEffectiveValue(field);
        }
    }
}
