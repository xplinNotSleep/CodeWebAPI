using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;
using System.Collections;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public class GLTFAccessorConvertor : GLTFBaseConvertor<GLTFAccessor>
    {
        public override GLTFAccessor ReadJson(JsonReader reader, Type objectType, GLTFAccessor existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value as GLTFAccessor;
        }

        public override void WriteArray(JsonWriter writer, string name, IEnumerable arr)
        {
            int arrCount = 0;
            if (arr is List<float>)
                arrCount = (arr as List<float>).Count;
            if (arr is List<int>)
                arrCount = (arr as List<int>).Count;

            if (arr != null && arrCount != 0)
            {
                writer.WritePropertyName(name);
                writer.WriteStartArray();
                foreach (var item in arr)
                {
                    writer.WriteValue(item);
                }
                writer.WriteEndArray();
            }
        }

        private List<int> FloatToInt(List<float> arr)
        {
            List<int> ls = new List<int>(arr.Count);

            for (int i = 0; i < arr.Count; i++)
            {
                ls.Add((int)arr[i]);
            }
            return ls;
        }

        public override void WriteJson(JsonWriter writer, GLTFAccessor value, JsonSerializer serializer)
        {
            if (value.Count < 1)
                throw new Exception("[GLTFAccessor Schema Error] count 必须大于1");
            writer.WriteStartObject();
            WriteProperty(writer, "count", value.Count);
            if (!(value.BufferViewIndex < 0))
                WriteProperty(writer, "bufferView", value.BufferViewIndex);
            WriteProperty(writer, "componentType", value.ComponentType);
            WriteProperty(writer, "type", value.Type);
            WriteProperty(writer, "name", value.Name);
            WriteProperty(writer, "byteOffset", value.ByteOffset);
            if (value.ComponentType == 5123)
            {
                if (value.Max != null)
                    WriteArray(writer, "max", FloatToInt(value.Max));
                if (value.Min != null)
                    WriteArray(writer, "min", FloatToInt(value.Min));
            }
            else
            {
                WriteArray(writer, "max", value.Max);
                WriteArray(writer, "min", value.Min);
            }
            writer.WriteEndObject();
        }
    }
}
