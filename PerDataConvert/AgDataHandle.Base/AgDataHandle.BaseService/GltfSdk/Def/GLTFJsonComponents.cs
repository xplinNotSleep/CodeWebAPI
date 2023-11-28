using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    #region gltf同名的文件
    public class GLTFExtensionInfo
    {
        public string gltfsorce = "RvtConvert";
        public GLTFJsonObjects objects { get; set; } = new GLTFJsonObjects();
        public void Add(GLTFAttribute attribute)
        {
            objects.components.Add(attribute);
        }
        public void SetBuild(BuildingJsonObject buildingjsonobject)
        {
            objects.building = buildingjsonobject;
        }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Count
        {
            get { return objects.components.Count; }
        }
    }

    public class GLTFJsonObjects
    {
        public string source { get; set; } = "RvtConvert";
        public List<GLTFAttribute> components { get; set; } = new List<GLTFAttribute>();
        public BuildingJsonObject building { get; set; } = new BuildingJsonObject();
    }


    public class BuildingJsonObject
    {
        public string uuid { get; set; } = "0";
        //public string Boundingbox { get; set; }
        public dynamic Baselines { get; set; } /*= new List<BaseLine>();*/
        public JObject location { get; set; }
        public double Height { get; set; }
    }
    public class BaseLine
    {
        public Dictionary<string, double> Direction { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> Start { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> End { get; set; } = new Dictionary<string, double>();
        public string Length { get; set; }
    }

    public class GLTFAttribute
    {
        [JsonProperty(Required = Required.DisallowNull)]
        public string uuid { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string matname { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string objectid { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string level { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string familyName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string typename { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string revitId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string typec { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string buildNumber { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string category { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        public Dictionary<string, string> userData { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        public Dictionary<string, object> DBInfo { get; set; } = new Dictionary<string, object>();
    }
    #endregion

    /// <summary>
    /// 对应材质表中字段textruesource所需要的信息
    /// </summary>
    public class RVTTextrueSourceAttribute
    {
        /// <summary>
        /// 透明度
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double Transparency { get; set; }

        /// <summary>
        /// 高光
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Shininess { get; set; }

        /// <summary>
        /// 平滑度
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Smoothness { get; set; }

        /// <summary>
        /// 贴图
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Texturing { get; set; }

        /// <summary>
        /// 着色模式下的颜色
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TintingColor { get; set; }

    }


}
