using Newtonsoft.Json;
namespace AgDataHandle.Maths.Numerics
{
    public class BoundingVolume
    {
        /// <summary>
        /// use region (west, south, east, north, minHeight, maxHeight)
        /// </summary>
        [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
        public double[] Region { get; set; }
        /// <summary>
        /// bounding box, 12
        /// </summary>
        [JsonProperty("box", NullValueHandling = NullValueHandling.Ignore)]
        public double[] Box { get; set; }
        /// <summary>
        /// bounding sphere, 4
        /// </summary>
        [JsonProperty("sphere", NullValueHandling = NullValueHandling.Ignore)]
        public double[] Sphere { get; set; }
    }
}