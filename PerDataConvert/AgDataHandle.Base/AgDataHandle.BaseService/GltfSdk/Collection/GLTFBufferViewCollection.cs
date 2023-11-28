using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Collection
{
    [JsonArray(false)]
    public class GLTFBufferViewCollection : GLTFCollection<GLTFBufferView>
    {
        [JsonIgnore]
        private GLTF gltf = null;
        [JsonIgnore]
        public GLTF GLTF
        {
            get
            {
                return gltf;
            }
            set
            {
                gltf = value;
                foreach (var el in ls)
                {
                    el.GLTF = value;
                }
            }
        }

        internal static GLTFBufferViewCollection Deserialize(JsonReader reader, GLTF model)
        {
            var col = new GLTFBufferViewCollection();
            reader.Read();
            while (reader.TokenType != JsonToken.EndArray)
            {
                GLTFBufferView bv = GLTFBufferView.Deserialize(reader, model);
                col.Add(bv);
            }
            col.GLTF = model;
            return col;
        }
    }
}
