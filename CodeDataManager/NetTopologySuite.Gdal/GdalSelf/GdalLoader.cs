using OSGeo.GDAL;
using OSGeo.OGR;
using System.Text;

namespace NetTopologySuite.GdalEx
{
    public class GdalLoader
    {
        public static void Register()
        {
            GdalConfiguration.ConfigureGdal();
            GdalConfiguration.ConfigureOgr();
            Gdal.AllRegister();
            Ogr.RegisterAll();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}
