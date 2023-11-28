using OSGeo.OGR;

namespace NetTopologySuite.GdalEx
{
    public class FieldAttribute
    {
        public string Name { get; set; }
        public FieldType Type { get; set; }
        public int Width { get; set; }
        public int Precision { get; set; }
    }
}
