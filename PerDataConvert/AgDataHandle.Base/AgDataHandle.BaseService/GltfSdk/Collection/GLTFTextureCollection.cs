using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Collection
{
    [JsonArray(true)] // means count of this jarray's items can be 0
    public class GLTFTextureCollection : GLTFCollection<GLTFTexture>
    {
        [JsonIgnore]
        private GLTF model = null;
        [JsonIgnore]
        public GLTF GLTF
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
                foreach (var el in ls)
                {
                    el.GLTF = value;
                }
            }
        }

        internal static GLTFTextureCollection Deserialize(JsonReader reader, GLTF model)
        {
            var col = new GLTFTextureCollection();
            reader.Read(); // token -> startArray
            while (reader.TokenType != JsonToken.EndArray)
            {
                GLTFTexture tex = GLTFTexture.Deserialize(reader, model);
                if (tex == null)
                {
                    continue;
                }
                col.Add(tex);
            }
            col.GLTF = model;
            return col;
        }
    }
}
