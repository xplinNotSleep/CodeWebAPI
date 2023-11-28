using Newtonsoft.Json;

namespace AgDataHandle.BaseService.CommonHelper
{
    class C73
    {
        public static int GetInt32(JsonReader reader)
        {
            var id = reader.ReadAsDouble();
            id = id == null ? -1 : id;
            return (int)id;
        }
    }
}
