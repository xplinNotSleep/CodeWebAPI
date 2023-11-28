using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shp.Readers;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Readers
{


    /// <summary>
    /// Point shapefile reader.
    /// </summary>
    public class ShapefilePointReader : ShapefileReader<Point>
    {
        /// <inheritdoc/>
        public ShapefilePointReader(Stream shpStream, Stream dbfStream, ShapefileReaderOptions options = null)
            : base(shpStream, dbfStream, options)
        { }

        /// <inheritdoc/>
        public ShapefilePointReader(string shpPath, ShapefileReaderOptions options = null)
            : base(shpPath, options)
        { }

        public ShapefilePointReader(string shpPath,string backFieldNames, string whereClause, Encoding Encoding)
    : base(shpPath, backFieldNames, whereClause,Encoding)
        { }


        internal override ShpReader<Point> CreateShpReader(Stream shpStream, ShapefileReaderOptions options)
        {
            return new ShpPointReader(shpStream, options);
        }
    }


}
