using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shp.Readers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Readers
{

    /// <summary>
    /// Generic base class for reading a shapefile.
    /// </summary>
    public abstract class ShapefileReader<T> : ShapefileReader where T : Geometry
    {
        private protected readonly ShpReader<T> ShpReader;

        /// <inheritdoc/>
        public override ShapeType ShapeType => ShpReader.ShapeType;

        /// <inheritdoc/>
        public override Envelope BoundingBox => ShpReader.BoundingBox;

        /// <summary>
        /// Shape geometry.
        /// </summary>
        public T Shape => ShpReader.Shape;

        /// <inheritdoc/>
        public override Geometry Geometry => ShpReader.Shape;

        /// <inheritdoc/>
        public override string Projection { get; } = null;


        /// <summary>
        /// Initializes a new instance of the reader class.
        /// </summary>
        /// <param name="shpStream">SHP file stream.</param>
        /// <param name="dbfStream">DBF file stream.</param>
        /// <param name="options">Reader options.</param>
        public ShapefileReader(Stream shpStream, Stream dbfStream, ShapefileReaderOptions options, string backFieldNames=null, string whereClause=null)
            : base(new DbfReader(dbfStream, options?.Encoding),backFieldNames, whereClause)
        {
            try
            {
                options = options ?? ShapefileReaderOptions.Default;
                options.DbfRecordCount = DbfReader.RecordCount;
                ShpReader = CreateShpReader(shpStream, options);
            }
            catch
            {
                DisposeManagedResources();
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the reader class.
        /// </summary>
        /// <param name="shpPath">Path to SHP file.</param>
        /// <param name="options">Reader options.</param>
        public ShapefileReader(string shpPath, ShapefileReaderOptions options, string backFieldNames=null, string whereClause = null)
            : base(new DbfReader(Path.ChangeExtension(shpPath, ".dbf"), options?.Encoding), backFieldNames, whereClause)
        {
            try
            {
                options = options ?? ShapefileReaderOptions.Default;
                options.DbfRecordCount = DbfReader.RecordCount;
                var shpStream = OpenManagedFileStream(shpPath, ".shp", FileMode.Open);
                ShpReader = CreateShpReader(shpStream, options);

                var prjFile = Path.ChangeExtension(shpPath, ".prj");
                if (File.Exists(prjFile))
                    Projection = File.ReadAllText(prjFile);
            }
            catch
            {
                DisposeManagedResources();
                throw;
            }
        }

        public ShapefileReader(string shpPath, string backFieldNames, string whereClause, Encoding Encoding)
    : base(new DbfReader(Path.ChangeExtension(shpPath, ".dbf"), Encoding), backFieldNames, whereClause)
        {
            try
            {
               var options = ShapefileReaderOptions.Default;
                options.DbfRecordCount = DbfReader.RecordCount;
                var shpStream = OpenManagedFileStream(shpPath, ".shp", FileMode.Open);
                ShpReader = CreateShpReader(shpStream, options);

                var prjFile = Path.ChangeExtension(shpPath, ".prj");
                if (File.Exists(prjFile))
                    Projection = File.ReadAllText(prjFile);
            }
            catch
            {
                DisposeManagedResources();
                throw;
            }
        }



        /// <summary>
        /// Reads feature geometry and attributes from underlying SHP and DBF files. 
        /// </summary>
        /// <param name="deleted">Indicates if the record was marked as deleted.</param>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next record;
        /// false if the enumerator has passed the end of the table.
        /// </returns>
        public override bool Read(out bool deleted)
        {
            var readShpSucceed = ShpReader.Read(out var skippedCount);
            for (int i = 0; i < skippedCount; i++)
            {
                if (!DbfReader.Read(out _))
                {
                    ThrowCorruptedShapefileDataException();
                }
            }
            var readDbfSucceed = DbfReader.Read(out deleted);

            if (readDbfSucceed != readShpSucceed)
            {
                ThrowCorruptedShapefileDataException();
            }
            return readDbfSucceed;
        }

        /// <inheritdoc/>
        public override bool Read(out bool deleted, out Feature feature)
        {
            var readSucceed = Read(out deleted);
            if (!readSucceed)
            {
                feature = null;
                return false;
            }
            string[] arrBackFieldNames = null;
            IEnumerable<KeyValuePair<string, object>> values =null;
            if (!string.IsNullOrEmpty(BackFieldNames))
            {
                arrBackFieldNames = BackFieldNames.ToLower().Split(new char[] { ',' });
                values = Fields.Where(p => arrBackFieldNames.Contains(p.Name)).Select(p=>new KeyValuePair<string,object>(p.Name,p.Value));
            }
            else
            {
                values = Fields.ToDictionary();
            }
            var attributes = new AttributesTable(values);
            feature = new Feature(Shape, attributes);
            feature.BoundingBox = Shape.EnvelopeInternal;
            return true;
        }

        /// <inheritdoc/>
        public override void Restart()
        {
            DbfReader.Restart();
            ShpReader.Restart();
        }

        /// <inheritdoc/>
        protected override void DisposeManagedResources()
        {
            ShpReader?.Dispose();
            base.DisposeManagedResources(); 
        }

        internal abstract ShpReader<T> CreateShpReader(Stream shpStream, ShapefileReaderOptions options);

        private static void ThrowCorruptedShapefileDataException()
        {
            throw new ShapefileException("Corrupted shapefile data. "
                    + "The dBASE table must contain feature attributes with one record per feature. "
                    + "There must be one-to-one relationship between geometry and attributes.");
        }
    }



}
