using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public class GLTFBufferViewConvertor : GLTFBaseConvertor<GLTFBufferView>
    {
        public override GLTFBufferView ReadJson(JsonReader reader, Type objectType, GLTFBufferView existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value as GLTFBufferView;
        }

        public override void WriteJson(JsonWriter writer, GLTFBufferView value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (value.ByteLength < 1)
                throw new Exception("[GLTFBufferView Schema Error] byteLength 必须大于1");
            if (value.BufferIndex < 0)
                throw new Exception("[GLTFBufferView Schema Error] buffer 必须大于0");

            WriteProperty(writer, "buffer", value.BufferIndex);
            WriteProperty(writer, "byteLength", value.ByteLength);
            if (value.ByteOffset > 0)
                WriteProperty(writer, "byteOffset", value.ByteOffset);
            //if (value.ByteStride < 4 || value.ByteStride % 4 != 0 || value.ByteStride > 252)
            //    throw new Exception("[GLTFBufferView Schema Error] byteStride 必须大于4、小于252、是4的倍数");

            int stride = value.ByteStride;
            if (stride != 0 && stride % 4 == 0 && stride >= 4 && stride <= 252) // 0 是默认程序值，用于表示“不写入”; 其正常值必须在 [4, 252] 且必须是4的倍数
                WriteProperty(writer, "byteStride", value.ByteStride);
            if (value.Target == 34962 || value.Target == 34963)
                WriteProperty(writer, "target", value.Target);
            writer.WriteEndObject();
        }
    }
}
