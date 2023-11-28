using Newtonsoft.Json;
using System;

namespace AgDataHandle.Maths.SpatialAlternation
{
    public class GisPosition
    {
        public GisPosition() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lonDegree">Longitude in degrees</param>
        /// <param name="latDegree">Latitude in degrees</param>
        /// <param name="transHeight">Tile origin's height in meters.</param>
        public GisPosition(double lonDegree, double latDegree, double transHeight, double rotate)
        {
            Longitude = Math.PI * lonDegree / 180.0f;
            Latitude = Math.PI * latDegree / 180.0f;
            TransHeight = transHeight;
            Rotate = rotate;
        }
        /// <summary>
        /// Tile origin's(models' point (0,0,0)) longitude in radian.
        /// </summary>
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        /// <summary>
        /// Tile origin's latitude in radian.
        /// </summary>
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        /// <summary>
        /// Tile origin's height in meters.
        /// </summary>
        [JsonProperty("transHeight")]
        public double? TransHeight { get; set; }
        /// <summary>
        /// Tile origin's height in meters.
        /// </summary>
        [JsonProperty("rotate")]
        public double? Rotate { get; set; }

        public void CheckLatLonIsDegree()
        {
            if (Math.Abs(Longitude) > Math.PI|| Math.Abs(Latitude) > Math.PI)
            {
                Longitude = Math.PI * Longitude / 180.0f;
                Latitude = Math.PI * Latitude / 180.0f;
            }
        }
    }

}
