using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public class GLTFImageConvertor : GLTFBaseConvertor<GLTFImage>
    {
        public override GLTFImage ReadJson(JsonReader reader, Type objectType, GLTFImage existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value as GLTFImage;
        }

        public override void WriteJson(JsonWriter writer, GLTFImage value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            WriteProperty(writer, "name", value.Name);
            if (value.BufferViewIndex != -1)
            {
                WriteProperty(writer, "bufferView", value.BufferViewIndex);
            }
            WriteProperty(writer, "uri", value.Uri);
            WriteProperty(writer, "mimeType", value.MimeType);
            writer.WriteEndObject();
        }
    }
}
