using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Collection
{
    public class GLTFSamplerCollection : GLTFCollection<GLTFSampler>
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

        internal static GLTFSamplerCollection Deserialize(JsonReader reader, GLTF model)
        {
            var col = new GLTFSamplerCollection();
            reader.Read(); // token -> startArray
            while (reader.TokenType != JsonToken.EndArray)
            {
                GLTFSampler spl = GLTFSampler.Deserialize(reader, model);
                col.Add(spl);
            }
            col.GLTF = model;
            return col;
        }
    }
}
