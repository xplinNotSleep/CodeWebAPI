using ServiceCenter.Core;
using ServiceCenter.Core.Data;

namespace AGSpatialDataCheck.Domain
{
    public class ContextConfigLoader : Singleton<ContextConfigLoader>
    {
        private string _path = "";
        public string Path
        {
            get
            {
                if (_path == "")
                {
                    _path =FileHelper.GetPath("SqliteDbContext.xml");
                }

                return _path;
            }
        }
    }

    public class SqliteDbContext : DbContext
    {
        public SqliteDbContext(): base(ContextConfigLoader.Instance.Path, null, null)
        {
        }

    }

  
}
