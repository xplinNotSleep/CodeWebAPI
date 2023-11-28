using ServiceCenter.Core;
using ServiceCenter.Core.Data;

namespace AgDataHandle.Domain
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
                    _path =FileHelper.GetPath("FILEDbContext.xml");
                }

                return _path;
            }
        }
    }

    public class FILEDbContext : DbContext
    {
        public FILEDbContext(): base(ContextConfigLoader.Instance.Path, null, null)
        {
        }

    }

  
}
