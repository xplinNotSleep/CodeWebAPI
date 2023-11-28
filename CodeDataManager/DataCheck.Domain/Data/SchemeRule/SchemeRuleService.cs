using AGSpatialDataCheck.Domain.Dto;
using Pure.Data;
using Pure.Data.SqlMap;
using ServiceCenter.Core;
using ServiceCenter.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AGSpatialDataCheck.Domain
{
    /// <summary>
    /// 方案规则表
    /// </summary>
    public class SchemeRuleService : PDRepositoryBaseCustom<SqliteDbContext, SchemeRuleEntity>, IApplicationService
    {
        public SchemeRuleService(SqliteDbContext ctx) : base(ctx)
        { }

        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <param name="queryParameters">参数</param>
        /// <returns></returns>
        public async Task<List<SchemeRuleEntity>> GetDataAsync(DbQueryParameter queryParameters)
        {
            try
            {
                SqlMapStatement sqlmap = Context.QuerySqlMap("SchemeRuleService", "GetData", queryParameters.Items);
                var data = await sqlmap.ExecuteListAsync<SchemeRuleEntity>();
                return data;
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog(ex.Message, ex);
            }
            finally
            {
                Context.Close();
            }
            return null;
        }

        /// <summary>
        /// 根据方案id获取方案的规则参数
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<List<TaskDto>> GetRulesBySchemeAsync(string Id)
        {
            try
            {
                SqlMapStatement sqlmap = Context.QuerySqlMap("SchemeRuleService", "GetRulesByScheme", new { SchemeId = Id});
                var data = await sqlmap.ExecuteListAsync<TaskDto>();
                return data;
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog(ex.Message, ex);
            }
            finally
            {
                Context.Close();
            }
            return null;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public async Task<PageDataResult<List<SchemeRuleEntity>>> GetPageDataAsync(DbQueryParameter queryParameters)
        {
            try
            {
                SqlMapStatement sqlmap = Context.QuerySqlMap("SchemeRuleService", "GetPageData", queryParameters.Items);
                var data = await sqlmap.ExecutePageListAsync<SchemeRuleEntity>(queryParameters.PageIndex, queryParameters.PageSize);
                return data;
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog(ex.Message, ex);
            }
            finally
            {
                Context.Close();
            }
            return PageDataResult<List<SchemeRuleEntity>>.Empty(queryParameters.PageIndex, queryParameters.PageSize);
        }
    }
}