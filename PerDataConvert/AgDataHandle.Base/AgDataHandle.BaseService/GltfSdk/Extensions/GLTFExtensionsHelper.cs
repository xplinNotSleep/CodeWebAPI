using Newtonsoft.Json.Linq;

public static class GLTFExtensionsHelper
{
    public static void AddGLTFValue(this Dictionary<string, JObject> extensions, string key, dynamic value)
    {
        if (!extensions.ContainsKey(key))
        {
            extensions.Add(key, new JObject { { "Value", value } });
        }
    }

    public static string GetGLTFValue(this Dictionary<string, JObject> extensions, string key)
    {
        if (!extensions.ContainsKey(key))
        {
            key = extensions.Keys.FirstOrDefault(p => p.ToLower() == key.ToLower());
            if (string.IsNullOrEmpty(key))
                return "";
        }
        return extensions[key].Value<string>("Value");
    }

    public static void AddValue(this Dictionary<string, JObject> extensions, string key, string JKey, object JValue)
    {
        extensions.Add(key, new JObject(new JProperty(JKey, JValue)));
    }

    public static object GetValue(this Dictionary<string, JObject> extensions, string key, string JKey)
    {
        if (!extensions.ContainsKey(key))
        {
            key = extensions.Keys.FirstOrDefault(p => p.ToLower() == key.ToLower());
            if (string.IsNullOrEmpty(key))
                return null;
        }
        return extensions[key].Value<string>(JKey);
    }

    public static void Merge(this Dictionary<string, JObject> extensions, Dictionary<string, JObject> other)
    {
        if (extensions == null)
            throw new Exception("源数据不能为null");
        if (other == null) return;
        foreach (var key in other.Keys)
        {
            if (!extensions.ContainsKey(key))
            {
                extensions.Add(key, other[key]);
            }
            else
            {
                extensions[key].Merge(other[key], new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Merge, MergeNullValueHandling = MergeNullValueHandling.Ignore, PropertyNameComparison = StringComparison.CurrentCultureIgnoreCase });
            }
        }
    }
}
