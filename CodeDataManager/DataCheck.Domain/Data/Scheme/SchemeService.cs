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
    /// 方案表
    /// </summary>
    public class SchemeService : PDRepositoryBaseCustom<SqliteDbContext, SchemeEntity>, IApplicationService
    {
        public SchemeService(SqliteDbContext ctx) : base(ctx)
        { }

        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <param name="queryParameters">参数</param>
        /// <returns></returns>
        public async Task<List<SchemeEntity>> GetDataAsync(DbQueryParameter queryParameters)
        {
            try
            {
                SqlMapStatement sqlmap = Context.QuerySqlMap("SchemeService", "GetData", queryParameters.Items);
                var data = await sqlmap.ExecuteListAsync<SchemeEntity>();
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
        public async Task<PageDataResult<List<SchemeEntity>>> GetPageDataAsync(DbQueryParameter queryParameters)
        {
            try
            {
                SqlMapStatement sqlmap = Context.QuerySqlMap("SchemeService", "GetPageData", queryParameters.Items);
                var data = await sqlmap.ExecutePageListAsync<SchemeEntity>(queryParameters.PageIndex, queryParameters.PageSize);
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
            return PageDataResult<List<SchemeEntity>>.Empty(queryParameters.PageIndex, queryParameters.PageSize);
        }
    }
}