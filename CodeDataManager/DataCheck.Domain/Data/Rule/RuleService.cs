using Pure.Data;
using Pure.Data.SqlMap;
using ServiceCenter.Core;
using ServiceCenter.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace AGSpatialDataCheck.Domain
{
    /// <summary>
    /// 规则表
    /// </summary>
    public class RuleService : PDRepositoryBaseCustom<SqliteDbContext, RuleEntity>, IApplicationService
    {
        public RuleService(SqliteDbContext ctx) : base(ctx)
        { }

        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <param name="queryParameters">参数</param>
        /// <returns></returns>
        public async Task<List<RuleEntity>> GetDataAsync(DbQueryParameter queryParameters)
        {
            try
            {
                SqlMapStatement sqlmap = Context.QuerySqlMap("RuleService", "GetData", queryParameters.Items);
                var data = await sqlmap.ExecuteListAsync<RuleEntity>();
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
        public async Task<PageDataResult<List<RuleEntity>>> GetPageDataAsync(DbQueryParameter queryParameters)
        {
            try
            {
                SqlMapStatement sqlmap = Context.QuerySqlMap("RuleService", "GetPageData", queryParameters.Items);
                var data = await sqlmap.ExecutePageListAsync<RuleEntity>(queryParameters.PageIndex, queryParameters.PageSize);
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
            return PageDataResult<List<RuleEntity>>.Empty(queryParameters.PageIndex, queryParameters.PageSize);
        }
    }
}