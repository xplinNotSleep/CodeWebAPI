using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    [JsonObject(
        ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore)]
    public abstract class GLTFElement
    {
        [JsonIgnore]
        public virtual GLTF GLTF { get; set; }

        /// <summary>
        /// 从解析到的 JProperty 创建 Extensions
        /// </summary>
        /// <param name="extensions">从 JsonReader 读取到的 JProperty</param>
        /// <param name="extDict">GLTFElement(Node/Mesh/Material等)的扩展项和额外项</param>
        public virtual void LoadExtension(JProperty extensions, Dictionary<string, JObject> extDict)
        {
            var extsJson = extensions.Value as JObject;
            if (extsJson == null) return;
            foreach (var prop in extsJson.Properties())
            {
                extDict.Add(prop.Name, prop.Value as JObject);
            }
        }
    }

}
