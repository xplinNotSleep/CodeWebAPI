using Newtonsoft.Json;

namespace AgDataHandle.BaseService
{
    /// <summary>
    /// represents a tileset.json file
    /// </summary>
    public class Tileset
    {
        /// <summary>
        ///  asset
        /// </summary>
        [JsonProperty("asset")]
        public TilesetAsset Asset { get; set; } = new TilesetAsset();
        /// <summary>
        /// additional properties
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> Properties { get; set; }
        /// <summary>
        /// geometric errors
        /// </summary>
        [JsonIgnore]
        public double GeometricError { get; set; }
        /// <summary>
        /// tile root (tileset.json or b3dm file)
        /// </summary>
        [JsonProperty("root")]
        public Tile Root { get; set; }

        [JsonIgnore]
        public string Path { get; set; }
    }

    /// <summary>
    /// tile data
    /// </summary>
    public class TileContent
    {
        public TileContent() { }
        public TileContent(string url)
        {
            Url = url;
        }
        /// <summary>
        /// tileset.json or b3dm file
        /// </summary>
        [JsonProperty("uri")]
        public string Url { get; set; }

        [JsonProperty("url")]
        private string Url2 { set { Url = value; } }
    }
    /// <summary>
    /// asset definition
    /// </summary>
    public class TilesetAsset
    {
        /// <summary>
        /// asset version
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; } = "1.0";
        /// <summary>
        /// TilesetVersion
        /// </summary>
        [JsonProperty("tilesetVersion")]
        //public string TilesetVersion { get; set; } = "1.0.0-AGExport";
        public string TilesetVersion
        {
            get
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Version.txt");
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
                else
                {
                    return "NoVersionInfo";
                }
            }
        }
        /// <summary>
        /// Up axis, Y or Z
        /// </summary>
        [JsonProperty("gltfUpAxis")]
        public string GltfUpAxis { get; set; } = "Z";


        [JsonProperty("datasource", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 暂时只写入文件名和格式
        /// </summary>
        public string DataSource { get; set; }

        [JsonProperty("propertyDB", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 属性文件挂接
        /// </summary>
        public PropertyDB propertyDB { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("createTime")]
        public string CreateTime
        {
            get
            {
                return DateTime.Now.ToLongDateString();
            }
        }

        /// <summary>
        /// 样式名，供前端根据Style字段搜索已定义的CustomShader样式。暂时仅用于白模顶部影像贴图
        /// </summary>
        [JsonProperty("style")]
        public Style Style { get; set; }
    }

    /// <summary>
    /// 共享材质
    /// </summary>
    public class ReuseTexture
    {
        /// <summary>
        /// 是否翻转
        /// </summary>
        [JsonProperty("flipY")]
        public bool FlipY { get; set; } = false;

        [JsonProperty("magFilter", Required = Required.DisallowNull)]
        public int MagFilter { get; set; } = 9729;

        [JsonProperty("minFilter", Required = Required.DisallowNull)]
        public int MinFilter { get; set; } = 9986;

        [JsonProperty("wrapS", Required = Required.DisallowNull)]
        public int WrapS { get; set; } = 10497;

        [JsonProperty("wrapT", Required = Required.DisallowNull)]
        public int WrapT { get; set; } = 10497;

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class PropertyDB
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        [JsonProperty("type", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]

        public string type { get; set; } = "DB";

        /// <summary>
        /// db文件不包含后缀的名称
        /// </summary>

        [JsonProperty("hostDB", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string hostDB { get; set; }

        /// <summary>
        /// 对应构件表名称
        /// </summary>
        [JsonProperty("tableName", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string tableName { get; set; }

        /// <summary>
        /// 日照分析表
        /// </summary>
        [JsonProperty("sunshineTable", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string sunshineTable { get; set; }

        /// <summary>
        /// 数据库关联瓦片的字段
        /// </summary>

        [JsonProperty("primaryField", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string primaryField { get; set; }

        /// <summary>
        /// 瓦片中关联数据库字段
        /// </summary>
        [JsonProperty("tilePrimaryField", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string tilePrimaryField { get; set; }

    }

    public class Style
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// 图片格式
        /// </summary>
        [JsonProperty("format")]
        public string Format { get; set; }
    }
}
