using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public class GLTFBufferConvertor : GLTFBaseConvertor<GLTFBuffer>
    {
        public override GLTFBuffer ReadJson(JsonReader reader, Type objectType, GLTFBuffer existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // existingValue 就是当前 GLTFBuffer 对象，对它进行定制化赋值
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, GLTFBuffer value, JsonSerializer serializer)
        {
            //添加针对GLB初始化
            writer.WriteStartObject();
            WriteProperty(writer, "name", value.Name);
            WriteProperty(writer, "byteLength", value.ByteLength);
            writer.WriteEndObject();
        }
    }
}
