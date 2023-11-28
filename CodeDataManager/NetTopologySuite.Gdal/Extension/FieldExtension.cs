using OSGeo.OGR;

namespace NetTopologySuite.GdalEx.Extension
{
    public static class FieldExtension
    {
        public static string GetNameEx(this FieldDefn field)
        {
            return EncodingUtils.Gb2312ToUtf8(field.GetName());
        }

        public static string GetNameRefEx(this FieldDefn field)
        {
            return EncodingUtils.Gb2312ToUtf8(field.GetNameRef());
        }

        public static void SetNameEx(this FieldDefn field,string name)
        {
            field.SetName(EncodingUtils.Utf8ToGb2312(name));
        }

        public static string GetAlternativeNameEx(this FieldDefn field)
        {
            return EncodingUtils.Gb2312ToUtf8(field.GetAlternativeName());
        }

        public static string GetAlternativeNameRefEx(this FieldDefn field)
        {
            return EncodingUtils.Gb2312ToUtf8(field.GetAlternativeNameRef());
        }

        public static void SetAlternativeNameEx(this FieldDefn field, string alternativeName)
        {
            field.SetAlternativeName(EncodingUtils.Utf8ToGb2312(alternativeName));
        }

        public static string GetTypeNameEx(this FieldDefn field)
        {
            return EncodingUtils.Gb2312ToUtf8(field.GetTypeName());
        }

        public static string GetFieldTypeNameEx(this FieldDefn field, FieldType type)
        {
            return EncodingUtils.Gb2312ToUtf8(field.GetFieldTypeName(type));
        }

        public static string GetDefaultEx(this FieldDefn field)
        {
            return EncodingUtils.Gb2312ToUtf8(field.GetDefault());
        }

        public static void SetDefaultEx(this FieldDefn field, string pszValue)
        {
            field.SetDefault(EncodingUtils.Utf8ToGb2312(pszValue));
        }

        public static string GetDomainNameEx(this FieldDefn field)
        {
            return EncodingUtils.Gb2312ToUtf8(field.GetDomainName());
        }

        public static void SetDomainNameEx(this FieldDefn field, string name)
        {
            field.SetDomainName(EncodingUtils.Utf8ToGb2312(name));
        }

        public static string GetCommentEx(this FieldDefn field)
        {
            return EncodingUtils.Gb2312ToUtf8(field.GetComment());
        }

        public static void SetCommentEx(this FieldDefn field, string comment)
        {
            field.SetComment(EncodingUtils.Utf8ToGb2312(comment));
        }
    }
}
