using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialAlternation
{
    public partial class SpatialEncoding
    {
        private float numbits { get; set; } = 3 * 10;

        public int GeoHashLength {
            get { return (int)(numbits * 2 / 5); }
            set { numbits = (float)value / 2 * 5; }
        }

        public string GetGeoHash(double[] center, string srs)
        {
            List<int> latbits = new List<int>();
            List<int> lonbits = new List<int>();
            if (srs.Contains("cgcs2000") || srs.Contains("CGCS2000"))
            {
                CoordinateSystem cs = Coordinate.FindCoordinateSystem(CoordinateSystemType.CGCS_2000);
                string[] csinfo = srs.Split(new string[] { "::" }, StringSplitOptions.None);
                cs.CentralLongitude = csinfo.Length>1? float.Parse(csinfo[1]):cs.CentralLongitude;//广州的中央子午线
                Vector3d deg = cs.UnProject(new Vector2d(center[1], center[0]));
                 latbits = GetBits(deg[0], -90, 90);
                 lonbits = GetBits(deg[1], -180, 180);
            }
            else
            {
                latbits = GetBits(center[1], -90, 90);
                lonbits = GetBits(center[0], -180, 180);
            }


            List<int> buffer = new List<int>();
            for (int i = 0; i < numbits; i++)
            {
                buffer.Add(lonbits[i]);
                buffer.Add(latbits[i]);
            }
            string code = GetBase32(String.Join("", buffer.ToArray()));
            return code;
        }

        private string GetBase32(string code)
        {
            char[] digits = new char[32] {'0', '1', '2', '3', '4', '5', '6', '7', '8',
           '9', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p',
           'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
            List<char> base32Char = new List<char>();
            for (int i = 0; i < GeoHashLength; i++)
            {
                string result = code.Substring(i * 5, 5);
                int num = Convert.ToInt32(result, 2);
                char cc = digits[num];
                base32Char.Add(cc);
            }
            return String.Join("", base32Char);
        }

        private List<int> GetBits(double degree, double floor, double ceiling)
        {
            List<int> buffer = new List<int>();
            for (int i = 0; i < numbits; i++)
            {
                var mid = (floor + ceiling) / 2;
                if (degree >= mid)
                {
                    buffer.Add(1);
                    floor = mid;
                }
                else
                {
                    buffer.Add(0);
                    ceiling = mid;
                }
            }
            return buffer;
        }
    }
}
