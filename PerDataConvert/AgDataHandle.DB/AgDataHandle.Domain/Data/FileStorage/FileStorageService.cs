using Pure.Data;
using Pure.Data.SqlMap;
using ServiceCenter.Core;
using ServiceCenter.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace AgDataHandle.Domain
{
    /// <summary>
    /// 文件管理表
    /// </summary>
    public class FileStorageService : PDRepositoryBaseCustom<FILEDbContext, FileStorageEntity>, IApplicationService
    {
        public FileStorageService(FILEDbContext ctx) : base(ctx)
        { }

        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <param name="queryParameters">参数</param>
        /// <returns></returns>
        public async Task<List<FileStorageEntity>> GetDataAsync(DbQueryParameter queryParameters)
        {
            try
            {
                SqlMapStatement sqlmap = Context.QuerySqlMap("FileStorageService", "GetData", queryParameters.Items);
                var data = await sqlmap.ExecuteListAsync<FileStorageEntity>();
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
        public async Task<PageDataResult<List<FileStorageEntity>>> GetPageDataAsync(DbQueryParameter queryParameters)
        {
            try
            {
                SqlMapStatement sqlmap = Context.QuerySqlMap("FileStorageService", "GetPageData", queryParameters.Items);
                var data = await sqlmap.ExecutePageListAsync<FileStorageEntity>(queryParameters.PageIndex, queryParameters.PageSize);
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
            return PageDataResult<List<FileStorageEntity>>.Empty(queryParameters.PageIndex, queryParameters.PageSize);
        }
    }
}