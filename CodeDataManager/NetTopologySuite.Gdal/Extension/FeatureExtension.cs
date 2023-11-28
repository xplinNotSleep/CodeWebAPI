using OSGeo.OGR;

namespace NetTopologySuite.GdalEx.Extension
{
    public static class FeatureExtension
    {
        public static int SetGeomFieldEx(this Feature feature, string field_name, Geometry geom)
        {
            return feature.SetGeomField(EncodingUtils.Utf8ToGb2312(field_name), geom);
        }

        public static FieldDefn GetFieldDefnRefEx(this Feature feature, string field_name)
        {
            return feature.GetFieldDefnRef(EncodingUtils.Utf8ToGb2312(field_name));
        }

        public static GeomFieldDefn GetGeomFieldDefnRefEx(this Feature feature, string field_name)
        {
            return feature.GetGeomFieldDefnRef(EncodingUtils.Utf8ToGb2312(field_name));
        }

        public static string GetFieldAsStringEx(this Feature feature, int id)
        {
            return EncodingUtils.Gb2312ToUtf8(feature.GetFieldAsString(id));
        }

        public static string GetFieldAsStringEx(this Feature feature, string field_name)
        {
            return EncodingUtils.Gb2312ToUtf8(feature.GetFieldAsString(EncodingUtils.Utf8ToGb2312(field_name)));
        }

        public static int GetFieldAsIntegerEx(this Feature feature, string field_name)
        {
            return feature.GetFieldAsInteger(EncodingUtils.Utf8ToGb2312(field_name));
        }

        public static long GetFieldAsInteger64Ex(this Feature feature, string field_name)
        {
            return feature.GetFieldAsInteger64(EncodingUtils.Utf8ToGb2312(field_name));
        }

        public static double GetFieldAsDoubleEx(this Feature feature, string field_name)
        {
            return feature.GetFieldAsDouble(EncodingUtils.Utf8ToGb2312(field_name));
        }

        public static void GetFieldAsDateTimeEx(this Feature feature, string field_name, out int pnYear, out int pnMonth, out int pnDay, out int pnHour, out int pnMinute, out float pfSecond, out int pnTZFlag)
        {
            feature.GetFieldAsDateTime(EncodingUtils.Utf8ToGb2312(field_name), out pnYear, out pnMonth, out pnDay, out pnHour, out pnMinute, out pfSecond, out pnTZFlag);
        }

        public  static string GetFieldAsISO8601DateTimeEx(this Feature feature, string field_name, string[] options)
        {
            return feature.GetFieldAsISO8601DateTime(EncodingUtils.Utf8ToGb2312(field_name), options);
        }

        public static int GetFieldIndexEx(this Feature feature, string field_name)
        {
            return feature.GetFieldIndex(EncodingUtils.Utf8ToGb2312(field_name));
        }

        public static int GetGeomFieldIndexEx(this Feature feature, string field_name)
        {
            return feature.GetGeomFieldIndex(EncodingUtils.Utf8ToGb2312(field_name));
        }

        public static void UnsetFieldEx(this Feature feature, string field_name)
        {
            feature.UnsetField(EncodingUtils.Utf8ToGb2312(field_name));
        }

        public static void SetFieldNullEx(this Feature feature, string field_name)
        {
            feature.SetFieldNull(EncodingUtils.Utf8ToGb2312(field_name));
        }

        public static void SetFieldEx(this Feature feature, string field_name, string value)
        {
            feature.SetField(EncodingUtils.Utf8ToGb2312(field_name), value);
        }

        public static void SetFieldEx(this Feature feature, string field_name, int value)
        {
            feature.SetField(EncodingUtils.Utf8ToGb2312(field_name), value);
        }

        public static void SetFieldEx(this Feature feature, string field_name, double value)
        {
            feature.SetField(EncodingUtils.Utf8ToGb2312(field_name), value);
        }

        public static void SetFieldEx(this Feature feature, string field_name, int year, int month, int day, int hour, int minute, float second, int tzflag)
        {
            feature.SetField(EncodingUtils.Utf8ToGb2312(field_name), year, month, day, hour, minute, second, tzflag);
        }

        public static void SetFieldBinaryFromHexStringEx(this Feature feature, string field_name, string pszValue)
        {
            feature.SetFieldBinaryFromHexString(EncodingUtils.Utf8ToGb2312(field_name), pszValue);
        }

        public static string GetStyleStringEx(this Feature feature)
        {
            return EncodingUtils.Gb2312ToUtf8(feature.GetStyleString());
        }

        public static void SetStyleStringEx(this Feature feature, string the_string)
        {
            feature.SetStyleString(EncodingUtils.Utf8ToGb2312(the_string));
        }

        public static FieldType GetFieldTypeEx(this Feature feature, string field_name)
        {
            return feature.GetFieldType(EncodingUtils.Utf8ToGb2312(field_name));
        }

        public static string GetNativeDataEx(this Feature feature)
        {
            return EncodingUtils.Gb2312ToUtf8(feature.GetNativeData());
        }

        public static string GetNativeMediaTypeEx(this Feature feature)
        {
            return EncodingUtils.Gb2312ToUtf8(feature.GetNativeMediaType());
        }

        public static void SetNativeDataEx(this Feature feature, string nativeData)
        {
            feature.SetNativeData(EncodingUtils.Utf8ToGb2312(nativeData));
        }

        public static void SetNativeMediaTypeEx(this Feature feature, string nativeMediaType)
        {
            feature.SetNativeMediaType(EncodingUtils.Utf8ToGb2312(nativeMediaType));
        }
    }
}
