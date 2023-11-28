using AgDataHandle.Maths.Numerics;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService
{
    /// <summary>
    /// tile data model
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// geo transform
        /// </summary>
        [JsonProperty("transform", NullValueHandling = NullValueHandling.Ignore)]
        public double[] Transform { get; set; }

        /// <summary>
        /// bounding
        /// </summary>
        [JsonProperty("boundingVolume")]
        public BoundingVolume BoundingVolume { get; set; }
        /// <summary>
        /// geo error
        /// </summary>
        [JsonProperty("geometricError")]
        public double GeometricError { get; set; }
        /// <summary>
        /// ADD or REPLACE
        /// </summary>
        [JsonProperty("refine")]
        public string Refine { get; set; } = "REPLACE"; // "ADD"
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "";
        /// <summary>
        /// visibility
        /// </summary>
        [JsonProperty("visibility")]
        public bool visibility { get; set; } = true;

        /// <summary>
        /// reuseTextures
        /// </summary>
        [JsonProperty("reuseTextures")]
        public List<ReuseTexture> ReuseTexture { get; set; }

        /// <summary>
        /// tileset.json or b3dm file
        /// </summary>
        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public TileContent Content { get; set; }
        /// <summary>
        /// children tiles
        /// </summary>
        [JsonProperty("children")]
        public List<Tile> Children { get; set; }
        [JsonIgnore]
        public string Path { get; set; }
        [JsonIgnore]
        public List<string> ModelPaths { get; set; }
        //[JsonIgnore]
        /// <summary>
        /// LOD层级
        /// </summary>
        [JsonProperty("level")]
        public int Level { get; set; }
        [JsonIgnore]
        public object Extion { get; set; }

        public Tile()
        {
            Children = new List<Tile>();
            ModelPaths = new List<string>();
        }

        /// <summary>
        /// 浅克隆
        /// </summary>
        /// <returns></returns>
        public Tile Clone()
        {
            var tile = new Tile();
            tile.Transform = Transform;
            tile.BoundingVolume = BoundingVolume;

            tile.GeometricError = GeometricError;
            tile.Refine = Refine;
            tile.Name = Name;
            tile.visibility = visibility;

            tile.ReuseTexture = ReuseTexture;
            tile.Content = Content;
            tile.Children = Children;
            tile.ModelPaths = ModelPaths;
            tile.Path = Path;
            tile.Level = Level;
            tile.Extion = Extion;
            return tile;
        }

        public void UpdateBoundingVolume(BoundingVolume boundingVolume)
        {
            BoundingBox boundingBox = new BoundingBox(BoundingVolume);
            var newboundingVolume = new BoundingBox(boundingVolume);
            boundingBox.Update(newboundingVolume);
            BoundingVolume = boundingBox.ToBoundingVolume();

        }
    }
}
